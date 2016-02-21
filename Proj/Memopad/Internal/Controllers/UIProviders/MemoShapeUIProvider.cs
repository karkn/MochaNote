/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Figure.Core;
using Mkamo.Editor.Controllers;
using Mkamo.Editor.Forms;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Common.Command;
using Mkamo.Memopad.Properties;
using Mkamo.Figure.Figures;
using System.Drawing;
using Mkamo.Figure.Routers;
using Mkamo.Editor.Tools;
using Mkamo.Editor.Core;
using Mkamo.Model.Core;
using Mkamo.Model.Memo;
using Mkamo.Memopad.Internal.Core;

namespace Mkamo.Memopad.Internal.Controllers.UIProviders {
    internal class MemoShapeUIProvider: AbstractMemoStyledTextUIProvider {
        // ========================================
        // field
        // ========================================
        private MemoShapeController _owner;

        private ToolStripMenuItem _rotate90;
        private ToolStripMenuItem _rotate270;
        private ToolStripMenuItem _flipHorizontal;
        private ToolStripMenuItem _flipVertical;

        // ========================================
        // constructor
        // ========================================
        public MemoShapeUIProvider(MemoShapeController owner): base(owner, true) {
            _owner = owner;
        }

        // ========================================
        // method
        // ========================================
        public override ContextMenuStrip GetContextMenu(MouseEventArgs e) {
            if (_rotate90 == null) {
                InitMenu();
            }

            _ContextMenu.Items.Clear();

            var selectionCount = _owner.Host.Site.SelectionManager.SelectedEditors.Count();

            if (selectionCount == 1) {
                _ContextMenu.Items.Add(_rotate90);
                _ContextMenu.Items.Add(_rotate270);
                _ContextMenu.Items.Add(_flipHorizontal);
                _ContextMenu.Items.Add(_flipVertical);

                _ContextMenu.Items.Add(_Separator1);
            }
            _ContextMenu.Items.Add(_CutInNewMemo);
            
            return _ContextMenu;
        }

        public override void ConfigureDetailForm(DetailSettingsForm detailForm) {
            var fig = _owner.Figure;

            /// background detail page
            var bgPage = new NodeBackgroundDetailPage(new [] { _owner.Host });
            bgPage.Background = fig.IsBackgroundEnabled? fig.Background: null;
            bgPage.IsModified = false;
            detailForm.RegisterPage("背景", bgPage);

            /// border detail page
            var borderPage = new NodeBorderDetailPage(new [] { _owner.Host });
            borderPage.IsBorderEnabled = fig.IsForegroundEnabled;
            borderPage.LineColor = fig.Foreground;
            borderPage.LineWidth = fig.BorderWidth;
            borderPage.LineDashStyle = fig.BorderDashStyle;
            borderPage.IsModified = false;
            detailForm.RegisterPage("枠線", borderPage);
        }

        // ------------------------------
        // private
        // ------------------------------
        private void InitMenu() {
            _rotate90 = new ToolStripMenuItem();
            _rotate90.Text = "右へ90度回転";
            _rotate90.Click += (sender, e) => {
                var fig = _owner.Figure as IRotatable;
                if (fig != null) {
                    var cmd = new DelegatingCommand(
                        () => fig.Rotate(90),
                        () => fig.Rotate(270)
                    );
                    GetExecutor().Execute(cmd);
                }
            };

            _rotate270 = new ToolStripMenuItem();
            _rotate270.Text = "左へ90度回転";
            _rotate270.Click += (sender, e) => {
                var fig = _owner.Figure as IRotatable;
                if (fig != null) {
                    var cmd = new DelegatingCommand(
                        () => fig.Rotate(270),
                        () => fig.Rotate(90)
                    );
                    GetExecutor().Execute(cmd);
                }
            };

            _flipHorizontal = new ToolStripMenuItem();
            _flipHorizontal.Text = "左右反転";
            _flipHorizontal.Click += (sender, e) => {
                var fig = _owner.Figure as IFlippable;
                if (fig != null) {
                    var cmd = new DelegatingCommand(
                        () => fig.FlipHorizontal(),
                        () => fig.FlipHorizontal()
                    );
                    GetExecutor().Execute(cmd);
                }
            };
            _flipVertical = new ToolStripMenuItem();
            _flipVertical.Text = "上下反転";
            _flipVertical.Click += (sender, e) => {
                var fig = _owner.Figure as IFlippable;
                if (fig != null) {
                    var cmd = new DelegatingCommand(
                        () => fig.FlipVertical(),
                        () => fig.FlipVertical()
                    );
                    GetExecutor().Execute(cmd);
                }
            };
        }


        // ------------------------------
        // private
        // ------------------------------
        private Mkamo.Common.Command.ICommandExecutor GetExecutor() {
            return _owner.Host.Site.CommandExecutor;
        }

    }

}
