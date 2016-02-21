/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Editor.Forms;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Editor.Commands;
using Mkamo.Figure.Core;
using System.Drawing;

namespace Mkamo.Memopad.Internal.Controllers.UIProviders {
    internal class MemoTextUIProvider:AbstractMemoStyledTextUIProvider {
        // ========================================
        // field
        // ========================================
        private MemoTextController _owner;

        private ToolStripMenuItem _frame;
        private ToolStripMenuItem _export;

        // ========================================
        // constructor
        // ========================================
        public MemoTextUIProvider(MemoTextController owner): base(owner, true) {
            _owner = owner;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override ContextMenuStrip GetContextMenu(MouseEventArgs e) {
            if (_export == null) {
                InitItems();
            }

            _ContextMenu.Items.Clear();

            var selectionCount = _owner.Host.Site.SelectionManager.SelectedEditors.Count();

            if (selectionCount == 1) {
                _ContextMenu.Items.Add(_frame);
                _ContextMenu.Items.Add(_Separator1);

                _ContextMenu.Items.Add(_export);
                _ContextMenu.Items.Add(_Separator2);
            }
            _ContextMenu.Items.Add(_CutInNewMemo);

            return _ContextMenu;
        }

        public override void ConfigureDetailForm(DetailSettingsForm detailForm) {
            var fig = _owner.Figure;

            /// background detail page
            //var bgPage = new NodeBackgroundDetailPage(new [] { _owner.Host });
            //bgPage.Background = fig.IsBackgroundEnabled? fig.Background: null;
            //bgPage.IsModified = false;
            //detailForm.RegisterPage("背景", bgPage);

            /// border detail page
            var borderPage = new NodeBorderDetailPage(new [] { _owner.Host });
            borderPage.IsBorderEnabled = fig.IsForegroundEnabled;
            borderPage.LineColor = fig.Foreground;
            borderPage.LineWidth = fig.BorderWidth;
            borderPage.LineDashStyle = fig.BorderDashStyle;
            borderPage.IsModified = false;
            detailForm.RegisterPage("枠線", borderPage);
        }

        private void InitItems() {
            _frame = new ToolStripMenuItem("枠線(&F)");

            var noFrame = new ToolStripMenuItem("なし(&N)");
            noFrame.Click += (sender, e) => {
                var node = _owner.Host.Figure as INode;
                if (node != null) {
                    var cmd = new SetNodeBorderCommand(
                        _owner.Host,
                        false,
                        node.Foreground,
                        node.BorderWidth,
                        node.BorderDashStyle
                    );
                    _owner.Host.Site.CommandExecutor.Execute(cmd);
                }
            };

            var solidFrame = new ToolStripMenuItem("実線(&S)");
            solidFrame.Click += (sender, e) => {
                var node = _owner.Host.Figure as INode;
                if (node != null) {
                    var cmd = new SetNodeBorderCommand(
                        _owner.Host,
                        true,
                        Color.DimGray,
                        1,
                        System.Drawing.Drawing2D.DashStyle.Solid
                    );
                    _owner.Host.Site.CommandExecutor.Execute(cmd);
                }
            };

            var dashFrame = new ToolStripMenuItem("点線(&D)");
            dashFrame.Click += (sender, e) => {
                var node = _owner.Host.Figure as INode;
                if (node != null) {
                    var cmd = new SetNodeBorderCommand(
                        _owner.Host,
                        true,
                        Color.DimGray,
                        1,
                        System.Drawing.Drawing2D.DashStyle.Dash
                    );
                    _owner.Host.Site.CommandExecutor.Execute(cmd);
                }
            };

            _frame.DropDownItems.AddRange(
                new[] {
                    noFrame,
                    solidFrame,
                    dashFrame,
                }
            );

            _export = new ToolStripMenuItem("取り出す(&E)");
            var app = MemopadApplication.Instance;

            var exportHtml = new ToolStripMenuItem("HTML(&H)...");
            exportHtml.Click += (sender, e) => {
                var memoFile = _owner.Model;
                var dialog = new SaveFileDialog();
                dialog.RestoreDirectory = true;
                dialog.ShowHelp = true;
                dialog.FileName = "Export.html";
                dialog.Filter = "Html Files(*.html)|*.html";
                if (dialog.ShowDialog() == DialogResult.OK) {
                    var outputPath = dialog.FileName;
                    _owner.Host.RequestExport("Html", outputPath);
                }
            };
            _export.DropDownItems.Add(exportHtml);

            var exportText = new ToolStripMenuItem("テキスト(&T)...");
            exportText.Click += (sender, e) => {
                var memoFile = _owner.Model;
                var dialog = new SaveFileDialog();
                dialog.RestoreDirectory = true;
                dialog.ShowHelp = true;
                dialog.FileName = "Export.txt";
                dialog.Filter = "Text Files(*.txt)|*.txt";
                if (dialog.ShowDialog() == DialogResult.OK) {
                    var outputPath = dialog.FileName;
                    _owner.Host.RequestExport("Text", outputPath);
                }
            };
            _export.DropDownItems.Add(exportText);
        
        }
    }
}
