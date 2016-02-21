/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Properties;

namespace Mkamo.Memopad.Internal.Core {
    internal class MemoDataFolderSync {
        // ========================================
        // field
        // ========================================
        private int _directoryCount;
        private int _currentDirectoryIndex;
        private Action<int> _reportProgress;

        // ========================================
        // constructor
        // ========================================
        public MemoDataFolderSync(Action<int> reportProgress) {
            _reportProgress = reportProgress;
        }

        // ========================================
        // method
        // ========================================
        public static bool ExistsLockFile(string dirPath) {
            var lockFilePath = Path.Combine(dirPath, MemopadConsts.LockFileName);
            return File.Exists(lockFilePath);
        }

        public static int GetAllDirectoryCount(string dirPath) {
            return Directory.GetDirectories(dirPath, "*", SearchOption.AllDirectories).Length;
        }

        /// <summary>
        /// ノート格納フォルダに必要なディレクトリ，ファイルが存在するかどうか。
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static bool ContainsMemo(string dirPath) {
            if (!Directory.Exists(dirPath)) {
                return false;
            }

            return
                Directory.Exists(Path.Combine(dirPath, MemopadConsts.EmbeddedRootName)) &&
                File.Exists(Path.Combine(dirPath, MemopadConsts.MemoSdfFilePath)) &&
                File.Exists(Path.Combine(dirPath, MemopadConsts.ExtendedDataSdfFilePath));
        }

        /// <summary>
        /// ノート格納フォルダには存在しないディレクトリを持つかどうか。
        /// </summary>
        public static bool ContainsNoMemo(string dirPath) {
            if (!Directory.Exists(dirPath)) {
                return false;
            }

            var memoDirs = new [] {
                Path.Combine(dirPath, MemopadConsts.EmbeddedRootName),
            };

            var dirs = Directory.GetDirectories(dirPath);
            foreach (var dir in dirs) {
                if (!memoDirs.Contains(dir, StringComparer.OrdinalIgnoreCase)) {
                    return true;
                }
            }

            if (!ContainsMemo(dirPath)) {
                var files = Directory.GetFiles(dirPath);
                if (files.Length > 0) {
                    return true;
                }

                if (dirs.Length > 0) {
                    return true;
                }
            }

            return false;
        }

        public static void MakeMemoDataFolderIcon(string dirPath) {
            if (!Directory.Exists(dirPath)) {
                throw new ArgumentException("dirPath");
            }

            var iconPath = Path.Combine(dirPath, "data_folder.ico");
            if (!File.Exists(iconPath)) {
                var icon = Resources.memo_data_folder;
                using (var stream = new FileStream(iconPath, FileMode.Create, FileAccess.Write)) {
                    icon.Save(stream);
                    stream.Close();
                }
                File.SetAttributes(iconPath, File.GetAttributes(iconPath) | FileAttributes.Hidden);
            }

            var iniPath = Path.Combine(dirPath, "desktop.ini");
            if (!File.Exists(iniPath)) {
                using (var writer = File.CreateText(iniPath)) {
                    writer.WriteLine("[.Shellclassinfo]");
                    writer.WriteLine("IconFile=data_folder.ico");
                    writer.WriteLine("IconIndex=0");
                    writer.Close();
                }
                File.SetAttributes(iniPath, File.GetAttributes(iniPath) | FileAttributes.Hidden);
            }

            var dir = new DirectoryInfo(dirPath);
            dir.Attributes = dir.Attributes | FileAttributes.System;
        }

        public void SyncMemoDataFoldersTo(string dirPath) {
            if (ExistsLockFile(dirPath)) {
                /// コピー先にlockファイルがあるということは
                /// 起動中のMochaNoteが使用しているディレクトリなのでエラー
                throw new ArgumentException("dirPath");
            }

            ReportProgress(5);

            /// report初期化
            _currentDirectoryIndex = 0;
            _directoryCount = GetAllDirectoryCount(MemopadConsts.MemoRoot);
            ReportProgress(10);

            var sDir = new DirectoryInfo(MemopadConsts.MemoRoot);
            var tDir = new DirectoryInfo(dirPath);
            SyncDirectory(sDir, tDir);

            /// コピーされたlockファイルはいらないので削除
            var lockFilePath = Path.Combine(dirPath, MemopadConsts.LockFileName);
            if (File.Exists(lockFilePath)) {
                File.Delete(lockFilePath);
            }

            ReportProgress(100);
        }

        public void SyncMemoDataFoldersFrom(string dirPath) {
            ReportProgress(5);

            /// report初期化
            _currentDirectoryIndex = 0;
            _directoryCount = GetAllDirectoryCount(dirPath);
            ReportProgress(10);

            var sDir = new DirectoryInfo(dirPath);
            var tDir = new DirectoryInfo(MemopadConsts.MemoRoot);
            SyncDirectory(sDir, tDir);

            ReportProgress(100);
        }



        // ------------------------------
        // private
        // ------------------------------
        private void SyncDirectory(DirectoryInfo sDir, DirectoryInfo tDir) {
            if (!sDir.Exists) {
                throw new ArgumentException("sDir");
            }

            if (!tDir.Exists) {
                tDir.Create();
                tDir.Attributes = sDir.Attributes;
            }

            var sFiles = sDir.GetFiles();
            foreach (var sFile in sFiles) {
                var tFile = new FileInfo(Path.Combine(tDir.FullName, sFile.Name));
                if (
                    !tFile.Exists ||
                    tFile.Length != sFile.Length ||
                    tFile.LastWriteTime != sFile.LastWriteTime
                ) {
                    sFile.CopyTo(tFile.FullName, true);
                }
            }

            DeleteNotExistFilesAndDirs(sDir, tDir);

            var dirs = sDir.GetDirectories();
            foreach (var dir in dirs) {
                SyncDirectory(dir, new DirectoryInfo(Path.Combine(tDir.FullName, dir.Name)));
            }

            ++_currentDirectoryIndex;
            ReportProgress(10 + (80 * _currentDirectoryIndex / _directoryCount));
        }

        private void DeleteNotExistFilesAndDirs(DirectoryInfo sDirectory, DirectoryInfo tDirectory) {
            var tFiles = tDirectory.GetFiles();
            foreach (var tFile in tFiles) {
                var sFile = new FileInfo(Path.Combine(sDirectory.FullName, tFile.Name));
                if (!sFile.Exists) {
                    tFile.IsReadOnly = false;
                    tFile.Delete();
                }
            }

            var tSubDirs = tDirectory.GetDirectories();
            foreach (var tSubDir in tSubDirs) {
                var sDir = new DirectoryInfo(Path.Combine(sDirectory.FullName, tSubDir.Name));
                if (!sDir.Exists) {
                    UnsetReadOnly(tSubDir);
                    tSubDir.Delete(true);
                }
            }
        }

        private void UnsetReadOnly(DirectoryInfo dir) {
            if ((dir.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                dir.Attributes = dir.Attributes & ~FileAttributes.ReadOnly;
            }

            foreach (var file in dir.GetFiles()) {
                file.IsReadOnly = false;
            }

            foreach (var subDirs in dir.GetDirectories()) {
                UnsetReadOnly(subDirs);
            }
        }


        private void ReportProgress(int progress) {
            if (_reportProgress != null) {
                _reportProgress(progress);
            }
        }
    }

}
