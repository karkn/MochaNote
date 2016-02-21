/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Mkamo.Editor.Controllers;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Internal.Forms;

namespace Mkamo.Memopad.Internal.Controllers.UIProviders {
    internal abstract class AbstractMemoStyledTextUIProvider: AbstractMemoContentUIProvider {
        // ========================================
        // field
        // ========================================
        private ToolStripButton _addLineArrowCentral;
        private ToolStripButton _addLineCentral;
        private ToolStripButton _addLineDashArrowCentral;
        private ToolStripButton _addLineDashCentral;

        private ToolStripDropDownButton _addLineOrthogonal;
        private ToolStripDropDownButton _addLineArrowOrthogonal;
        private ToolStripDropDownButton _addLineDashOrthogonal;
        private ToolStripDropDownButton _addLineDashArrowOrthogonal;

        private Point _miniToolBarClickedLocation;


        // ========================================
        // constructor
        // ========================================
        public AbstractMemoStyledTextUIProvider(
            AbstractController owner,
            bool supportDetailForm
        ): base(owner, supportDetailForm) {
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override ToolStripDropDown GetMiniToolBar(MouseEventArgs e) {
            if (_addLineArrowCentral == null) {
                InitMiniToolBar();
            }

            _MiniToolBar.Items.Clear();

            var selectionCount = _Owner.Host.Site.SelectionManager.SelectedEditors.Count();

            if (selectionCount == 1) {
                _miniToolBarClickedLocation = e.Location;

                _MiniToolBar.Items.Add(_addLineCentral);
                _MiniToolBar.Items.Add(_addLineArrowCentral);
                _MiniToolBar.Items.Add(_addLineDashCentral);
                _MiniToolBar.Items.Add(_addLineDashArrowCentral);

                _MiniToolBar.Items.Add(_addLineOrthogonal);
                _MiniToolBar.Items.Add(_addLineArrowOrthogonal);
                _MiniToolBar.Items.Add(_addLineDashOrthogonal);
                _MiniToolBar.Items.Add(_addLineDashArrowOrthogonal);
            }
            
            return _MiniToolBar;
        }

        // ------------------------------
        // private
        // ------------------------------
        private void InitMiniToolBar() {
            var form = (MemopadFormBase) _Owner.Host.Site.EditorCanvas.TransientData[MemopadConsts.FormEditorTransientDataKey];
            var reg = form.ToolRegistry;

            var id = MemopadToolIds.AddLineArrowCentral;
            _addLineArrowCentral = new ToolStripButton(reg.GetAddEdgeToolImage(id));
            _addLineArrowCentral.ToolTipText = reg.GetAddEdgeToolText(id);
            _addLineArrowCentral.Click += (sender, e) => {
                var tool = reg.GetAddEdgeTool(MemopadToolIds.AddLineArrowCentral);
                _Owner.Host.Site.EditorCanvas.Tool = tool;
                tool.SetSource(_miniToolBarClickedLocation);
            };

            id = MemopadToolIds.AddLineCentral;
            _addLineCentral = new ToolStripButton(reg.GetAddEdgeToolImage(id));
            _addLineCentral.ToolTipText = reg.GetAddEdgeToolText(id);
            _addLineCentral.MouseDown += (sender, e) => {
                var tool = reg.GetAddEdgeTool(MemopadToolIds.AddLineCentral);
                _Owner.Host.Site.EditorCanvas.Tool = tool;
                tool.SetSource(_miniToolBarClickedLocation);
            };

            id = MemopadToolIds.AddLineDashArrowCentral;
            _addLineDashArrowCentral = new ToolStripButton(reg.GetAddEdgeToolImage(id));
            _addLineDashArrowCentral.ToolTipText = reg.GetAddEdgeToolText(id);
            _addLineDashArrowCentral.Click += (sender, e) => {
                var tool = reg.GetAddEdgeTool(MemopadToolIds.AddLineDashArrowCentral);
                _Owner.Host.Site.EditorCanvas.Tool = tool;
                tool.SetSource(_miniToolBarClickedLocation);
            };

            id = MemopadToolIds.AddLineDashCentral;
            _addLineDashCentral = new ToolStripButton(reg.GetAddEdgeToolImage(id));
            _addLineDashCentral.ToolTipText = reg.GetAddEdgeToolText(id);
            _addLineDashCentral.MouseDown += (sender, e) => {
                var tool = reg.GetAddEdgeTool(MemopadToolIds.AddLineDashCentral);
                _Owner.Host.Site.EditorCanvas.Tool = tool;
                tool.SetSource(_miniToolBarClickedLocation);
            };

            id = MemopadToolIds.AddLineOrthogonalFreeDirection;
            _addLineOrthogonal = new ToolStripDropDownButton(reg.GetAddEdgeToolImage(id));
            _addLineOrthogonal.ToolTipText = "直線 (経路 直角, 接続位置 四辺の中点)";
            _addLineOrthogonal.DropDownOpening += (se, ev) => {
                if (_addLineOrthogonal.DropDownItems.Count == 0) {
                    PrepareOrth(reg);
                }
            };

            id = MemopadToolIds.AddLineArrowOrthogonalFreeDirection;
            _addLineArrowOrthogonal = new ToolStripDropDownButton(reg.GetAddEdgeToolImage(id));
            _addLineArrowOrthogonal.ToolTipText = "矢印付き直線 (経路 直角, 接続位置 四辺の中点)";
            _addLineArrowOrthogonal.DropDownOpening += (se, ev) => {
                if (_addLineArrowOrthogonal.DropDownItems.Count == 0) {
                    PrepareOrthArrow(reg);
                }
            };

            id = MemopadToolIds.AddLineDashOrthogonalFreeDirection;
            _addLineDashOrthogonal = new ToolStripDropDownButton(reg.GetAddEdgeToolImage(id));
            _addLineDashOrthogonal.ToolTipText = "点線 (経路 直角, 接続位置 四辺の中点)";
            _addLineDashOrthogonal.DropDownOpening += (se, ev) => {
                if (_addLineDashOrthogonal.DropDownItems.Count == 0) {
                    PrepareOrthDash(reg);
                }
            };

            id = MemopadToolIds.AddLineDashArrowOrthogonalFreeDirection;
            _addLineDashArrowOrthogonal = new ToolStripDropDownButton(reg.GetAddEdgeToolImage(id));
            _addLineDashArrowOrthogonal.ToolTipText = "矢印付き点線 (経路 直角, 接続位置 四辺の中点)";
            _addLineDashArrowOrthogonal.DropDownOpening += (se, ev) => {
                if (_addLineDashArrowOrthogonal.DropDownItems.Count == 0) {
                    PrepareOrthDashArrow(reg);
                }
            };

        }

        private void PrepareOrth(IToolRegistry reg) {
            _addLineOrthogonal.DropDownItems.Clear();
            _addLineOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineOrthogonalFreeDirection, reg));
            _addLineOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineOrthogonalUpDirection, reg));
            _addLineOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineOrthogonalDownDirection, reg));
            _addLineOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineOrthogonalLeftDirection, reg));
            _addLineOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineOrthogonalRightDirection, reg));
        }

        private void PrepareOrthDash(IToolRegistry reg) {
            _addLineDashOrthogonal.DropDownItems.Clear();
            _addLineDashOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineDashOrthogonalFreeDirection, reg));
            _addLineDashOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineDashOrthogonalUpDirection, reg));
            _addLineDashOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineDashOrthogonalDownDirection, reg));
            _addLineDashOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineDashOrthogonalLeftDirection, reg));
            _addLineDashOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineDashOrthogonalRightDirection, reg));
        }

        private void PrepareOrthArrow(IToolRegistry reg) {
            _addLineArrowOrthogonal.DropDownItems.Clear();
            _addLineArrowOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineArrowOrthogonalFreeDirection, reg));
            _addLineArrowOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineArrowOrthogonalUpDirection, reg));
            _addLineArrowOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineArrowOrthogonalDownDirection, reg));
            _addLineArrowOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineArrowOrthogonalLeftDirection, reg));
            _addLineArrowOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineArrowOrthogonalRightDirection, reg));
        }

        private void PrepareOrthDashArrow(IToolRegistry reg) {
            _addLineDashArrowOrthogonal.DropDownItems.Clear();
            _addLineDashArrowOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineDashArrowOrthogonalFreeDirection, reg));
            _addLineDashArrowOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineDashArrowOrthogonalUpDirection, reg));
            _addLineDashArrowOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineDashArrowOrthogonalDownDirection, reg));
            _addLineDashArrowOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineDashArrowOrthogonalLeftDirection, reg));
            _addLineDashArrowOrthogonal.DropDownItems.Add(CreateButton(MemopadToolIds.AddLineDashArrowOrthogonalRightDirection, reg));
        }

        private ToolStripMenuItem CreateButton(string id, IToolRegistry reg) {
            var ret = new ToolStripMenuItem(reg.GetAddEdgeToolImage(id));
            ret.Text = reg.GetAddEdgeToolText(id);
            ret.MouseDown += (sender, e) => {
                var tool = reg.GetAddEdgeTool(id);
                _Owner.Host.Site.EditorCanvas.Tool = tool;
                tool.SetSource(_miniToolBarClickedLocation);
            };
            return ret;
        }
    }
}
