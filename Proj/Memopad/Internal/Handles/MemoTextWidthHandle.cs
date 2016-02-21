/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Handles;
using Mkamo.Figure.Core;
using Mkamo.Editor.Utils.CategorizedListBox.Figures;
using System.Drawing;
using Mkamo.Figure.Figures;
using System.Windows.Forms;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Figures;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.DataType;

namespace Mkamo.Memopad.Internal.Handles {
    internal class MemoTextWidthHandle: AbstractAuxiliaryHandle {
        // ========================================
        // field
        // ========================================
        private MemoTextWidthHandleFigure _figure;
        private Size _figureSize;
        private int _rightMargin;

        private Point _dragStartLocation;

        // ========================================
        // constructor
        // ========================================
        public MemoTextWidthHandle() {
            _figure = new MemoTextWidthHandleFigure();
            _figureSize = new Size(10, 8);
            _figure.IsBackgroundEnabled = true;
            _figure.Foreground = Color.DarkGray;
            _figure.Background = new SolidBrushDescription(Color.DarkGray);
            _rightMargin = 4;
            Cursor = Cursors.SizeWE;
        }

        // ========================================
        // property
        // ========================================
        public Size FigureSize {
            get { return _figureSize; }
            set { _figureSize = value; }
        }

        public int RightMargin {
            get { return _rightMargin; }
            set { _rightMargin = value; }
        }

        // ========================================
        // method
        // ========================================
        public override IFigure Figure {
            get { return _figure; }
        }

        public override void Relocate(IFigure hostFigure) {
            var bounds = Host.IsFocused? Host.Focus.Figure.Bounds: hostFigure.Bounds;
            _figure.Bounds = new Rectangle(
                new Point(
                    bounds.Right - _figureSize.Width - _rightMargin,
                    bounds.Top - 7
                ),
                _figureSize
            );
        }

        protected override void OnFigureMouseDown(MouseEventArgs e) {
            base.OnFigureMouseClick(e);

            /// EditorがFocusされていたらCommitしておく
            var focused = Host.Site.FocusManager.FocusedEditor;
            if (focused != null) {
                focused.RequestFocusCommit(true);
            }

            Host.RequestSelect(SelectKind.True, true);
        }

        protected override void OnFigureDragStart(MouseEventArgs e) {
            base.OnFigureDragStart(e);

            _dragStartLocation = e.Location;
        }

        protected override void OnFigureDragMove(MouseEventArgs e) {
            base.OnFigureDragMove(e);

            var req = new ChangeBoundsRequest();
            req.ResizeDirection = Directions.Right;
            req.SizeDelta = (Size) e.Location - (Size) _dragStartLocation;
            Host.ShowFeedback(req);
        }

        protected override void OnFigureDragFinish(MouseEventArgs e) {
            base.OnFigureDragFinish(e);

            Host.RequestResize(new Size(e.X - _dragStartLocation.X, 0), Directions.Right, true);

            var req = new ChangeBoundsRequest();
            Host.HideFeedback(req);
        }

        protected override void OnFigureDragCancel() {
            base.OnFigureDragCancel();

            var req = new ChangeBoundsRequest();
            Host.HideFeedback(req);
        }
    }
}
