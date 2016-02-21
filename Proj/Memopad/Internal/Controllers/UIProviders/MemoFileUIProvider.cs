/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Requests;
using Mkamo.Common.Command;
using Mkamo.Editor.Core;
using System.Windows.Forms;
using Mkamo.Editor.Controllers;
using System.Diagnostics;
using System.IO;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Memopad.Internal.Controls;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Memopad.Internal.Core;

namespace Mkamo.Memopad.Internal.Controllers.UIProviders {
    internal class MemoFileUIProvider: AbstractMemoContentUIProvider {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private MemoFileController _owner;

        private ToolStripMenuItem _open;
        private ToolStripMenuItem _openParentFolder;

        private ToolStripMenuItem _export;

        // ========================================
        // constructor
        // ========================================
        public MemoFileUIProvider(MemoFileController owner): base(owner, true) {
            _owner = owner;
        }


        // ========================================
        // method
        // ========================================
        public override void ConfigureDetailForm(DetailSettingsForm detailForm) {
            /// file detail page
            var memoFile = _owner.Model;
            var filePage = new FileDetailPage(_owner.Host);
            filePage.Title = memoFile.Name;
            filePage.Path = memoFile.IsEmbedded? "": memoFile.Path;
            filePage.IsModified = false;
            detailForm.RegisterPage("ファイル", filePage);
        }
        
        public override ContextMenuStrip GetContextMenu(MouseEventArgs e) {
            if (_open == null || _openParentFolder == null) {
                InitItems();
            }
            UpdateItemEnabled();

            _ContextMenu.Items.Clear();

            var selectionCount = _owner.Host.Site.SelectionManager.SelectedEditors.Count();

            if (selectionCount == 1) {
                _ContextMenu.Items.Add(_open);
                _ContextMenu.Items.Add(_openParentFolder);
                _ContextMenu.Items.Add(_Separator1);

                _ContextMenu.Items.Add(_export);
                _ContextMenu.Items.Add(_Separator2);
            }
            _ContextMenu.Items.Add(_CutInNewMemo);
            
            return _ContextMenu;
        }
    
        private void InitItems() {
            _open = new ToolStripMenuItem();
            _open.Text = "開く(&O)";
            _open.Click += (sender, e) => {
                var memoFile = _owner.Model;
                var path = MemoFileEditorHelper.GetFullPath(memoFile);
                try {
                    Process.Start(path);
                } catch (Exception ex) {
                    Logger.Warn("Cannot open: path=" + path, ex);
                    MessageBox.Show(
                        memoFile.Path + "を開けませんでした",
                        "ファイルオープンエラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            };

            _openParentFolder = new ToolStripMenuItem();
            _openParentFolder.Text = "親フォルダを開く(&P)";
            _openParentFolder.Click += (sender, e) => {
                var memoFile = _owner.Model;
                var path = MemoFileEditorHelper.GetFullPath(memoFile);
                var parent = Path.GetDirectoryName(path);
                try {
                    Process.Start(parent);
                } catch (Exception ex) {
                    Logger.Warn("Cannot open: path=" + path, ex);
                    MessageBox.Show(
                        parent + "を開けませんでした",
                        "ファイルオープンエラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            };

            _export = new ToolStripMenuItem("取り出す(&E)");

            var exportFile = new ToolStripMenuItem("ファイル(&F)...");
            exportFile.Click += (sender, e) => {
                var memoFile = _owner.Model;
                var dialog = new SaveFileDialog();
                dialog.RestoreDirectory = true;
                dialog.ShowHelp = true;
                dialog.FileName = Path.GetFileName(memoFile.Path);
                dialog.Filter = "All Files(*.*)|*.*";
                if (dialog.ShowDialog() == DialogResult.OK) {
                    var outputPath = dialog.FileName;
                    _owner.Host.RequestExport("File", outputPath);
                }
            };
            _export.DropDownItems.Add(exportFile);
        }


        private void UpdateItemEnabled() {
            var memoFile = _owner.Model;
            if (memoFile == null || memoFile.IsEmbedded) {
                _openParentFolder.Enabled = false;
            } else {
                var path = MemoFileEditorHelper.GetFullPath(memoFile);
                var parent = Path.GetDirectoryName(path);
                _openParentFolder.Enabled = parent != null;
                _export.Enabled = File.Exists(path) && !Directory.Exists(path);
            }
        }
    }
}
