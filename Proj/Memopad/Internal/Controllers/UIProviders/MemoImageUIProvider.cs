/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Controllers;
using System.Windows.Forms;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Editor.Forms;
using Mkamo.Editor.Core;
using System.Drawing;

namespace Mkamo.Memopad.Internal.Controllers.UIProviders {
    internal class MemoImageUIProvider: AbstractMemoContentUIProvider {
        // ========================================
        // field
        // ========================================
        private MemoImageController _owner;
        private ToolStripMenuItem _resetSize;

        private ToolStripMenuItem _resize;

        private ToolStripMenuItem _export;

        // ========================================
        // constructor
        // ========================================
        public MemoImageUIProvider(MemoImageController owner): base(owner, true) {
            _owner = owner;
        }

        // ========================================
        // method
        // ========================================
        public override ContextMenuStrip GetContextMenu(MouseEventArgs e) {
            if (_resetSize == null) {
                InitItems();
            }

            _ContextMenu.Items.Clear();

            var selectionCount = _owner.Host.Site.SelectionManager.SelectedEditors.Count();

            if (selectionCount == 1) {
                _ContextMenu.Items.Add(_resetSize);
                _ContextMenu.Items.Add(_resize);
                _ContextMenu.Items.Add(_Separator1);

                _ContextMenu.Items.Add(_export);
                _ContextMenu.Items.Add(_Separator2);
            }
            _ContextMenu.Items.Add(_CutInNewMemo);

            return _ContextMenu;
        }

        public override void ConfigureDetailForm(DetailSettingsForm detailForm) {
            var fig = _owner.Figure;

            /// border detail page
            var borderPage = new NodeBorderDetailPage(new[] { _owner.Host });
            borderPage.IsBorderEnabled = fig.IsForegroundEnabled;
            borderPage.LineColor = fig.Foreground;
            borderPage.LineWidth = fig.BorderWidth;
            borderPage.LineDashStyle = fig.BorderDashStyle;
            borderPage.IsModified = false;
            detailForm.RegisterPage("枠線", borderPage);
        }

        private void InitItems() {
            _resetSize = new ToolStripMenuItem("元のサイズに戻す(&R)");
            _resetSize.Click += (sender, e) => ResizeImage(100);

            _resize = new ToolStripMenuItem("サイズの変更(&S)");

            {
                var resize200 = new ToolStripMenuItem("200%(&0)");
                resize200.Click += (sender, e) => ResizeImage(200);
                _resize.DropDownItems.Add(resize200);

                var resize150 = new ToolStripMenuItem("150%(&1)");
                resize150.Click += (sender, e) => ResizeImage(150);
                _resize.DropDownItems.Add(resize150);

                var resize100 = new ToolStripMenuItem("100%(&2)");
                resize100.Click += (sender, e) => ResizeImage(100);
                _resize.DropDownItems.Add(resize100);

                var resize75 = new ToolStripMenuItem("75%(&3)");
                resize75.Click += (sender, e) => ResizeImage(75);
                _resize.DropDownItems.Add(resize75);

                var resize50 = new ToolStripMenuItem("50%(&4)");
                resize50.Click += (sender, e) => ResizeImage(50);
                _resize.DropDownItems.Add(resize50);

                var resize25 = new ToolStripMenuItem("25%(&5)");
                resize25.Click += (sender, e) => ResizeImage(25);
                _resize.DropDownItems.Add(resize25);

                var resize10 = new ToolStripMenuItem("10%(&6)");
                resize10.Click += (sender, e) => ResizeImage(10);
                _resize.DropDownItems.Add(resize10);
            }
            
            _export = new ToolStripMenuItem("取り出す(&E)");

            var exportPng = new ToolStripMenuItem("PNG(&P)...");
            exportPng.Click += (sender, e) => {
                var memoFile = _owner.Model;
                var dialog = new SaveFileDialog();
                dialog.RestoreDirectory = true;
                dialog.ShowHelp = true;
                dialog.FileName = "Export.png";
                dialog.Filter = "PNG Files(*.png)|*.png";
                if (dialog.ShowDialog() == DialogResult.OK) {
                    var outputPath = dialog.FileName;
                    _owner.Host.RequestExport("PNG", outputPath);
                }
            };
            _export.DropDownItems.Add(exportPng);

            var exportJpeg = new ToolStripMenuItem("JPEG(&J)...");
            exportJpeg.Click += (sender, e) => {
                var memoFile = _owner.Model;
                var dialog = new SaveFileDialog();
                dialog.RestoreDirectory = true;
                dialog.ShowHelp = true;
                dialog.FileName = "Export.jpg";
                dialog.Filter = "JPEG Files(*.jpg)|*.jpg";
                if (dialog.ShowDialog() == DialogResult.OK) {
                    var outputPath = dialog.FileName;
                    _owner.Host.RequestExport("JPEG", outputPath);
                }
            };
            _export.DropDownItems.Add(exportJpeg);
        }

        private void ResizeImage(int percent) {
            var origSize = _owner.Figure.ImageSize;
            var targetSize = new Size(origSize.Width * percent / 100, origSize.Height * percent / 100);
            var currSize = _owner.Figure.Size;
            var delta = targetSize - currSize;
            _owner.Host.RequestResize(delta, Mkamo.Common.DataType.Directions.DownRight, false);
        }
    }
}
