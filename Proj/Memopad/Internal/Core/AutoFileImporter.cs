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
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Model.Memo;
using System.Drawing;
using Mkamo.Editor.Core;
using System.Windows.Forms;

namespace Mkamo.Memopad.Internal.Core {
    internal class AutoFileImporter: IDisposable {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        // ========================================
        // field
        // ========================================
        private FileSystemWatcher _watcher = new FileSystemWatcher();
        private Label _dummy = new Label();

        // ========================================
        // constructor
        // ========================================
        public AutoFileImporter() {
            _watcher.Path = MemopadConsts.AutoFileImportDirPath;
            _watcher.Filter = "";
            _watcher.NotifyFilter = NotifyFilters.FileName;
            _watcher.Created += HandleWatcherCreated;
            _watcher.EnableRaisingEvents = true;

            /// UIスレッドで処理を呼び出すためのダミー
            /// Handleプロパティにアクセスしてウィンドウハンドルを作っておかないとInvoke()で例外が起こる
            var h = _dummy.Handle;
        }

        // ========================================
        // destructor
        // ========================================
        public void Dispose() {
            if (_watcher != null) {
                _watcher.Dispose();
            }
            if (_dummy != null) {
                _dummy.Dispose();
            }
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        private void HandleWatcherCreated(object sender, FileSystemEventArgs e) {
            if (!File.Exists(e.FullPath)) {
                return;
            }

            Logger.Info("File Created: " + e.FullPath);
            var app = MemopadApplication.Instance;
            
            Action act = () => {
                var form = app.MainForm;
                var canvas = default(EditorCanvas);
                var createNote = form.CurrentEditorCanvas == null;
                if (createNote) {
                    var info = app.CreateMemo(Path.GetFileNameWithoutExtension(e.Name) + "のノート");
                    var page = app.MainForm.FindPageContent(info);
                    canvas = page.EditorCanvas;
                } else {
                    canvas = form.CurrentEditorCanvas;
                }
                
                var memo = canvas.RootEditor.Content;
                MemoEditorHelper.AddFileDrops(memo, MemopadConsts.DefaultCaretPosition, new [] { e.FullPath }, true, !createNote, true);
                try {
                    File.Delete(e.FullPath);
                } catch (Exception ex) {
                    Logger.Warn("Can't delete auto imported file: " + e.FullPath, ex);
                }
                app.ShowMainForm();
                app.ActivateMainForm();
            };

            //app.MainForm.Invoke(act);
            _dummy.Invoke(act);
        }

    }
}
