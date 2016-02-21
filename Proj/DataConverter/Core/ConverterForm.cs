/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlServerCe;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.IO;
using Mkamo.Container.Core;
using Mkamo.DataConverter.Properties;

namespace Mkamo.DataConverter.Core {
    public partial class ConvertForm: Form {
        // ========================================
        // static field
        // ========================================
        private const string ConnectionString = @"Data Source={0}; LCID=1033; Mode = Exclusive; Max Database Size = 2048; Max Buffer Size = 1024";

        // ========================================
        // field
        // ========================================
        private SqlServerAccessor _memoAccessor;
        private SqlServerAccessor _exDataAccessor;

        // ========================================
        // constructor
        // ========================================
        public ConvertForm() {
            InitializeComponent();
        }

        // ========================================
        // method
        // ========================================
        private void _convertButton_Click(object sender, EventArgs e) {
            _convertButton.Enabled = false;
            _closeButton.Enabled = false;

            var confirm = MessageBox.Show(
                this,
                "Confidante Ver 1のデータをConfidante Ver 2のデータに変換してもよろしいですか?\r\n" +
                "・既存のConfidante Ver 2のデータがある場合は変換されたデータに上書きされます。\r\n" +
                "・変換処理はデータ数が多い場合数十分以上かかることがあります。",
                "変換の確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (confirm == DialogResult.No) {
                MessageBox.Show(this, "変換を中止しました。", "変換の中止");
                _convertButton.Enabled = true;
                _closeButton.Enabled = true;
                return;
            }

            if (!Directory.Exists(MemopadConstsV1.MemoRoot) || !ContainsMemo(MemopadConstsV1.MemoRoot)) {
                MessageBox.Show(
                    this,
                    "Confidante Ver 1のデータが見つかりませんでした。",
                    "変換エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                _convertButton.Enabled = true;
                _closeButton.Enabled = true;
                return;
            }


            if (File.Exists(MemopadConstsV1.LockFilePath)) {
                MessageBox.Show(
                    this,
                    "Confidante Ver 1のデータは使用中です。\r\n" +
                    "Confidanteを終了してください。",
                    "変換エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                _convertButton.Enabled = true;
                _closeButton.Enabled = true;
                return;
            }
            if (File.Exists(MemopadConstsV2.LockFilePath)) {
                MessageBox.Show(
                    this,
                    "Confidante Ver 2のデータは使用中です。\r\n" +
                    "Confidanteを終了してください。",
                    "変換エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                _convertButton.Enabled = true;
                _closeButton.Enabled = true;
                return;
            }

            if (Directory.Exists(MemopadConstsV2.MemoRoot)) {
                try {
                    Directory.Delete(MemopadConstsV2.MemoRoot, true);
                } catch (Exception) {
                    MessageBox.Show(
                        this,
                        "既存のConfidante Ver 2のデータを削除できませんでした。",
                        "変換エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    _convertButton.Enabled = true;
                    _closeButton.Enabled = true;
                    return;
                }
            }

            var worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += (se, ev) => {
                _progressBar.Value = ev.ProgressPercentage;
            };
            worker.RunWorkerCompleted += (se, ev) => {
                if (ev.Error != null) {
                    MessageBox.Show(
                        this,
                        "変換に失敗しました。\r\n" +
                        ev.Error.Message,
                        "変換エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    Directory.Delete(MemopadConstsV2.MemoRoot, true);
                } else {
                    MessageBox.Show(this, "データの変換が終了しました。", "データ変換の終了");
                }
                _convertButton.Enabled = true;
                _closeButton.Enabled = true;
            };
            worker.DoWork += DoConvert;
            worker.RunWorkerAsync();
                
        }

        private void DoConvert(object sender, DoWorkEventArgs e) {
            var worker = (BackgroundWorker) sender;

            var memoConnection = default(SqlCeConnection);
            var exDataConnection = default(SqlCeConnection);
            try {
                PathUtil.EnsureDirectoryExists(MemopadConstsV2.MemoRoot);
                MakeMemoDataFolderIcon(MemopadConstsV2.MemoRoot);
                worker.ReportProgress(10);

                {
                    var memoConn = string.Format(ConnectionString, MemopadConstsV2.MemoFilePath);
                    var engine = new SqlCeEngine(memoConn);
                    engine.CreateDatabase();

                    memoConnection = new SqlCeConnection(memoConn);
                    memoConnection.Open();
                }
                {
                    var exDataConn = string.Format(ConnectionString, MemopadConstsV2.ExtendedDataFilePath);
                    var engine = new SqlCeEngine(exDataConn);
                    engine.CreateDatabase();

                    exDataConnection = new SqlCeConnection(exDataConn);
                    exDataConnection.Open();
                }
    
    
                _memoAccessor = new SqlServerAccessor(memoConnection);
                _exDataAccessor = new SqlServerAccessor(exDataConnection);
    
                worker.ReportProgress(20);
    
                ConvertData();
                worker.ReportProgress(40);
    
                ConvertModel();
                worker.ReportProgress(70);
    
                CopyData();
                worker.ReportProgress(90);
            
            } finally {
                if (memoConnection != null) {
                    memoConnection.Close();
                }
                if (exDataConnection != null) {
                    exDataConnection.Close();
                }
            }

            worker.ReportProgress(100);
        }

        private void CopyData() {
            /// embedded file
            DirectoryUtil.Copy(MemopadConstsV1.EmbeddedFileRoot, MemopadConstsV2.EmbeddedFileRoot);

            /// settings
            File.Copy(MemopadConstsV1.SettingsFilePath, MemopadConstsV2.SettingsFilePath);

            /// window settings
            var windowSettings = Directory.GetFiles(MemopadConstsV1.MemoRoot, "window.*.xml");
            foreach (var windowSetting in windowSettings) {
                var target = Path.Combine(MemopadConstsV2.MemoRoot, Path.GetFileName(windowSetting));
                File.Copy(windowSetting, target, true);
            }
        }


        private void ConvertData() {
            /// memoinfo
            {
                var file = MemopadConstsV1.MemoInfosFilePath;
                var text = File.ReadAllText(file);
                _memoAccessor.InsertTextData("MemoInfo", text);
            }
            
            /// removed memoinfo
            {
                var file = MemopadConstsV1.RemovedMemoInfosFilePath;
                var text = File.ReadAllText(file);
                _memoAccessor.InsertTextData("RemovedMemoInfo", text);
            }

            /// removed embedded file
            {
                var file = MemopadConstsV1.RemovedEmbeddedFileIdsFilePath;
                var text = File.ReadAllText(file);
                _memoAccessor.InsertTextData("RemovedEmbeddedFileId", text);
            }
            

            /// memento
            var files = Directory.GetFiles(MemopadConstsV1.MementoRoot);
            foreach (var file in files) {
                var id = Path.GetFileNameWithoutExtension(file);
                var bytes = File.ReadAllBytes(file);
                _memoAccessor.InsertMemento(id, bytes);
            }
        }

        private void ConvertModel() {
            var oldModelRoot = MemopadConstsV1.ModelRoot;

            var classDirs = Directory.GetDirectories(oldModelRoot);
            foreach (var classDir in classDirs) {
                var typeName = Path.GetFileName(classDir);

                var entityDirs = Directory.GetDirectories(classDir);
                foreach (var entityDir in entityDirs) {
                    var id = Path.GetFileName(entityDir);

                    var isMemoImage = string.Equals(typeName, "Mkamo.Model.Memo.MemoImage", StringComparison.OrdinalIgnoreCase);
                    var fileImageDesc = default(FileImageDescription);

                    if (isMemoImage) {
                        /// ByteImageDescriptionからFileImageDescriptionに変換
                        using (var stream = new FileStream(Path.Combine(entityDir, "Image.ser"), FileMode.Open, FileAccess.Read)) {
                            var formatter = new BinaryFormatter();
                            var bytesDesc = (BytesImageDescription) formatter.Deserialize(stream);
                            var filepath = GetNewImageFilePath();
                            PathUtil.EnsureDirectoryExists(Path.GetDirectoryName(filepath));
                            File.WriteAllBytes(filepath, bytesDesc.Bytes);
                            fileImageDesc = new FileImageDescription(Path.GetFileName(filepath));
                        }
                    }

                    var files = Directory.GetFiles(entityDir);
                    var hasSerializable = files.Any(
                        file => string.Equals(Path.GetExtension(file), ".ser", StringComparison.OrdinalIgnoreCase)
                    );
                    foreach (var file in files) {
                        var fileName = Path.GetFileName(file);
                        var fileExt = Path.GetExtension(file);

                        if (string.Equals(fileName, "entity.xml", StringComparison.OrdinalIgnoreCase)) {
                            var xml = File.ReadAllText(file);
                            if (hasSerializable) {
                                xml = ReplaceSerralizableProperty(xml);
                            }
                            _memoAccessor.InsertEntity(typeName, id, xml);

                        } else if (string.Equals(fileExt, ".dat", StringComparison.OrdinalIgnoreCase)) {
                            var key = Path.GetFileNameWithoutExtension(file);
                            var text = File.ReadAllText(file);
                            _exDataAccessor.InsertExtendedTextData(typeName, id, key, text);

                        } else if (string.Equals(fileExt, ".obj", StringComparison.OrdinalIgnoreCase)) {
                            var key = Path.GetFileNameWithoutExtension(file);
                            var bytes = File.ReadAllBytes(file);
                            _exDataAccessor.InsertExtendedBlobData(typeName, id, key, bytes);

                        } else if (string.Equals(fileExt, ".ser", StringComparison.OrdinalIgnoreCase)) {
                            if (isMemoImage) {
                                //if (fileImageDesc != null) {
                                var bytes = default(byte[]);
                                using (var stream = new MemoryStream()) {
                                    var formatter = new BinaryFormatter();
                                    formatter.Serialize(stream, fileImageDesc);
                                    bytes = stream.GetBuffer();
                                }
                                _memoAccessor.InsertSerializableProperty(typeName, id, "Image", bytes);
                                //}
                            } else {
                                var key = Path.GetFileNameWithoutExtension(file);
                                var bytes = File.ReadAllBytes(file);
                                _memoAccessor.InsertSerializableProperty(typeName, id, key, bytes);
                            }
                        }
                    }

                }
            }
        }

        private string GetNewImageFilePath() {
            return Path.Combine(MemopadConstsV2.EmbeddedImageRoot, Guid.NewGuid().ToString());
        }

        private string ReplaceSerralizableProperty(string xml) {
            return Regex.Replace(xml, "<Serializable filename=\"(.*)\\.ser\" />", "<Serializable key=\"$1\" />", RegexOptions.IgnoreCase);
        }

        private static bool ContainsMemo(string dirPath) {
            if (!Directory.Exists(dirPath)) {
                return false;
            }

            return
                Directory.Exists(Path.Combine(dirPath, MemopadConstsV1.MementoRootName)) &&
                Directory.Exists(Path.Combine(dirPath, MemopadConstsV1.ModelRootName)) &&
                File.Exists(Path.Combine(dirPath, MemopadConstsV1.MemoInfosFileName)) &&
                File.Exists(Path.Combine(dirPath, MemopadConstsV1.RemovedMemoInfosFileName));
        }

        private static void MakeMemoDataFolderIcon(string dirPath) {
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

        private void _closeButton_Click(object sender, EventArgs e) {
            Close();
        }

        //private string SerializableBytesToXml(byte[] bytes) {
        //    var obj = default(object);
        //    using (var stream = new MemoryStream(bytes)) {
        //        var formatter = new BinaryFormatter();
        //        obj = formatter.Deserialize(stream);
        //    }
            
        //    var ret = new StringBuilder();
        //    var settings = new XmlWriterSettings();
        //    settings.Indent = false;
        //    using (var writer = XmlWriter.Create(ret, settings)) {
        //        var formatter = new NetDataContractSerializer();
        //        formatter.WriteObject(writer, obj);
        //    }

        //    return ret.ToString();
        //}

        // --- db util ---

    }
}
