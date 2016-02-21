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
using Mkamo.Editor.Forms;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Model.Memo;
using Mkamo.Common.Command;
using Mkamo.Memopad.Properties;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Figure.Figures;
using System.Drawing;
using Mkamo.Figure.Routers;
using Mkamo.Figure.Core;
using Mkamo.Editor.Tools;
using Mkamo.Editor.Core;
using Mkamo.Model.Core;

namespace Mkamo.Memopad.Internal.Controllers.UIProviders {
    internal class MemoEdgeUIProvider : AbstractMemoContentUIProvider {
        // ========================================
        // field
        // ========================================
        private MemoEdgeController _owner;

        private ToolStripMenuItem _router;
        private ToolStripMenuItem _central;
        private ToolStripMenuItem _free;
        private ToolStripMenuItem _orthogonal;
        private ToolStripMenuItem _orthogonalMidpoint;

        private ToolStripMenuItem _startCap;
        private ToolStripMenuItem _startCapNormal;
        private ToolStripMenuItem _startCapArrow;

        private ToolStripMenuItem _endCap;
        private ToolStripMenuItem _endCapNormal;
        private ToolStripMenuItem _endCapArrow;

        // ========================================
        // constructor
        // ========================================
        public MemoEdgeUIProvider(MemoEdgeController owner): base(owner, true) {
            _owner = owner;
        }

        // ========================================
        // method
        // ========================================
        public override ContextMenuStrip GetContextMenu(MouseEventArgs e) {
            if (_router == null) {
                InitMenu();
            }

            _ContextMenu.Items.Clear();

            var selectionCount = _owner.Host.Site.SelectionManager.SelectedEditors.Count();

            if (selectionCount == 1) {
                _router.DropDownItems.Add(_central);
                _router.DropDownItems.Add(_free);
                _router.DropDownItems.Add(_orthogonalMidpoint);
                _router.DropDownItems.Add(_orthogonal);
                _ContextMenu.Items.Add(_router);

                _startCap.DropDownItems.Add(_startCapNormal);
                _startCap.DropDownItems.Add(_startCapArrow);
                _ContextMenu.Items.Add(_startCap);

                _endCap.DropDownItems.Add(_endCapNormal);
                _endCap.DropDownItems.Add(_endCapArrow);
                _ContextMenu.Items.Add(_endCap);

                _ContextMenu.Items.Add(_Separator1);
            }
            _ContextMenu.Items.Add(_CutInNewMemo);
            
            return _ContextMenu;
        }

        public override void ConfigureDetailForm(DetailSettingsForm detailForm) {
            var fig = _owner.Figure;

            /// line detail page
            var borderPage = new EdgeLineDetailPage(new [] { _owner.Host });
            borderPage.LineColor = fig.LineColor;
            borderPage.LineWidth = fig.LineWidth;
            borderPage.LineDashStyle = fig.LineDashStyle;
            borderPage.IsModified = false;
            detailForm.RegisterPage("線", borderPage);
        }

        // ------------------------------
        // private
        // ------------------------------
        private void InitMenu() {
            _router = new ToolStripMenuItem("種類(&K)");
            _router.DropDownOpening += (sender, e) => {
                var edge = _owner.Model;
                _central.Checked = edge.Kind == MemoEdgeKind.Central;
                _free.Checked = edge.Kind == MemoEdgeKind.Normal;
                _orthogonalMidpoint.Checked = edge.Kind == MemoEdgeKind.OrthogonalMidpoint;
                _orthogonal.Checked = edge.Kind == MemoEdgeKind.Orthogonal;
            };

            _central = new ToolStripMenuItem("経路 自由，接続位置 中心からの交点(&1)");
            _central.Click += (sender, e) => {
                var edge = _owner.Model;
                var fig = _owner.Figure;
                var oldKind = edge.Kind;
                var oldPoints = fig.EdgePoints.ToArray();
                var cmd = new DelegatingCommand(
                    () => {
                        fig.ClearBendPoints();
                        edge.Kind = MemoEdgeKind.Central;
                    },
                    () => {
                        edge.Kind = oldKind;
                        fig.SetEdgePoints(oldPoints);
                    }
                );
                GetExecutor().Execute(cmd);
            };

            _free = new ToolStripMenuItem("経路 自由，接続位置 自由(&2)");
            _free.Click += (sender, e) => {
                var edge = _owner.Model;
                var fig = _owner.Figure;
                var oldKind = edge.Kind;
                var oldPoints = fig.EdgePoints.ToArray();
                var cmd = new DelegatingCommand(
                    () => {
                        fig.ClearBendPoints();
                        edge.Kind = MemoEdgeKind.Normal;
                    },
                    () => {
                        edge.Kind = oldKind;
                        fig.SetEdgePoints(oldPoints);
                    }
                );
                GetExecutor().Execute(cmd);
            };

            _orthogonalMidpoint = new ToolStripMenuItem("経路 直角，接続位置 四辺の中点(&3)");
            _orthogonalMidpoint.Click += (sender, e) => {
                var edge = _owner.Model;
                var fig = _owner.Figure;
                var oldKind = edge.Kind;
                var oldPoints = fig.EdgePoints.ToArray();
                var cmd = new DelegatingCommand(
                    () => {
                        fig.ClearBendPoints();
                        edge.Kind = MemoEdgeKind.OrthogonalMidpoint;
                    },
                    () => {
                        edge.Kind = oldKind;
                        fig.SetEdgePoints(oldPoints);
                    }
                );
                GetExecutor().Execute(cmd);
            };

            _orthogonal = new ToolStripMenuItem("経路 直角，接続位置 自由(&4)");
            _orthogonal.Click += (sender, e) => {
                var edge = _owner.Model;
                var fig = _owner.Figure;
                var oldKind = edge.Kind;
                var oldPoints = fig.EdgePoints.ToArray();
                var cmd = new DelegatingCommand(
                    () => {
                        fig.ClearBendPoints();
                        edge.Kind = MemoEdgeKind.Orthogonal;
                    },
                    () => {
                        edge.Kind = oldKind;
                        fig.SetEdgePoints(oldPoints);
                    }
                );
                GetExecutor().Execute(cmd);
            };

            _startCap = new ToolStripMenuItem();
            _startCap.Text = "始点(&S)";
            _startCap.DropDownOpening += (sender, e) => {
                var edge = _owner.Model;
                _startCapNormal.Checked = edge.StartCapKind == MemoEdgeCapKind.Normal;
                _startCapArrow.Checked = edge.StartCapKind == MemoEdgeCapKind.Arrow;
            };

            _startCapNormal = new ToolStripMenuItem();
            _startCapNormal.Text = "標準(&N)";
            _startCapNormal.Click += (sender, e) => {
                var edge = _owner.Model;
                var oldKind = edge.StartCapKind;
                var cmd = new DelegatingCommand(
                    () => edge.StartCapKind = MemoEdgeCapKind.Normal,
                    () => edge.StartCapKind = oldKind
                );
                GetExecutor().Execute(cmd);
            };

            _startCapArrow = new ToolStripMenuItem();
            _startCapArrow.Text = "矢印(&A)";
            _startCapArrow.Click += (sender, e) => {
                var edge = _owner.Model;
                var oldKind = edge.StartCapKind;
                var cmd = new DelegatingCommand(
                    () => edge.StartCapKind = MemoEdgeCapKind.Arrow,
                    () => edge.StartCapKind = oldKind
                );
                GetExecutor().Execute(cmd);
            };

            _endCap = new ToolStripMenuItem();
            _endCap.Text = "終点(&E)";
            _endCap.DropDownOpening += (sender, e) => {
                var edge = _owner.Model;
                _endCapNormal.Checked = edge.EndCapKind == MemoEdgeCapKind.Normal;
                _endCapArrow.Checked = edge.EndCapKind == MemoEdgeCapKind.Arrow;
            };

            _endCapNormal = new ToolStripMenuItem();
            _endCapNormal.Text = "標準(&N)";
            _endCapNormal.Click += (sender, e) => {
                var edge = _owner.Model;
                var oldKind = edge.EndCapKind;
                var cmd = new DelegatingCommand(
                    () => edge.EndCapKind = MemoEdgeCapKind.Normal,
                    () => edge.EndCapKind = oldKind
                );
                GetExecutor().Execute(cmd);
            };

            _endCapArrow = new ToolStripMenuItem();
            _endCapArrow.Text = "矢印(&A)";
            _endCapArrow.Click += (sender, e) => {
                var edge = _owner.Model;
                var oldKind = edge.EndCapKind;
                var cmd = new DelegatingCommand(
                    () => edge.EndCapKind = MemoEdgeCapKind.Arrow,
                    () => edge.EndCapKind = oldKind
                );
                GetExecutor().Execute(cmd);
            };
        }

        private Mkamo.Common.Command.ICommandExecutor GetExecutor() {
            return _owner.Host.Site.CommandExecutor;
        }
    }
}
