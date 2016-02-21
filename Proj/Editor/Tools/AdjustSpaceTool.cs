/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Mkamo.Editor.Requests;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Editor.Tools {
    public class AdjustSpaceTool: AbstractTool {
        // ========================================
        // field
        // ========================================
        private ChangeBoundsRequest _moveRequest;
        private AdjustSpaceRequest _spaceRequest;
        private EditorBundle _targets;
        private ITool _toolOnFinished;
        
        private EditorCanvas _canvas;
        private Point _startPoint;

        // ========================================
        // constructor
        // ========================================
        public AdjustSpaceTool(EditorCanvas canvas, ITool toolOnFinished) {
            _canvas = canvas;
            _moveRequest = new ChangeBoundsRequest();
            _spaceRequest = new AdjustSpaceRequest();
            _toolOnFinished = toolOnFinished;
        }

        // ========================================
        // event
        // ========================================

        // ========================================
        // method
        // ========================================
        public override bool HandleMouseDown(MouseEventArgs e) {
            _startPoint = e.Location;
            return true;
        }

        public override bool HandleMouseMove(MouseEventArgs e) {
            return true;
        }

        public override bool HandleMouseUp(MouseEventArgs e) {
            return true;
        }


        public override bool HandleDragStart(MouseEventArgs e) {
            return true;
        }

        public override bool HandleDragMove(MouseEventArgs e) {
            using (_Host.RootEditor.Figure.DirtManager.BeginDirty()) {
                if (_targets != null) {
                    _targets.HideFeedback(_moveRequest);
                }

                var delta = (Size) e.Location - (Size) _startPoint;
                var horizontal = Math.Abs(delta.Width) > Math.Abs(delta.Height);
                horizontal = Math.Abs(delta.Width) < 40 && Math.Abs(delta.Height) < 40 ? false: horizontal;

                var content = _Host.RootEditor.Content;
                if (horizontal) {
                    _moveRequest.MovingEditors = content.Children.Where(edi => edi.Figure.Left > _startPoint.X).ToArray();
                } else {
                    _moveRequest.MovingEditors = content.Children.Where(edi => edi.Figure.Top > _startPoint.Y).ToArray();
                }
                _targets = new EditorBundle(_moveRequest.MovingEditors);

                var moveDelta = horizontal ?
                    new Size(delta.Width, 0) :
                    new Size(0, delta.Height);
                _moveRequest.MoveDelta = moveDelta;
                _targets.ShowFeedback(_moveRequest);

                _spaceRequest.Horizontal = horizontal;
                if (horizontal) {
                    _spaceRequest.Bounds = RectUtil.GetRectangleFromDiagonalPoints(_startPoint, new Point(e.X, _startPoint.Y + 20));
                } else {
                    _spaceRequest.Bounds = RectUtil.GetRectangleFromDiagonalPoints(_startPoint, new Point(_startPoint.X + 20, e.Y));
                }
                _Host.RootEditor.Content.ShowFeedback(_spaceRequest);
            }

            return true;
        }

        public override bool HandleDragFinish(MouseEventArgs e) {
            using (_Host.RootFigure.DirtManager.BeginDirty()) {
                var delta = (Size) e.Location - (Size) _startPoint;
                var horizontal = Math.Abs(delta.Width) > Math.Abs(delta.Height);
 
                var content = _Host.RootEditor.Content;
                if (horizontal) {
                    _moveRequest.MovingEditors = content.Children.Where(edi => edi.Figure.Left > _startPoint.X).ToArray();
                } else {
                    _moveRequest.MovingEditors = content.Children.Where(edi => edi.Figure.Top > _startPoint.Y).ToArray();
                }
                _targets = new EditorBundle(_moveRequest.MovingEditors);

                var moveDelta = horizontal ?
                    new Size(delta.Width, 0) :
                    new Size(0, delta.Height);
                _moveRequest.MoveDelta = moveDelta;
                _targets.HideFeedback(_moveRequest);
                _targets.PerformCompositeRequest(_moveRequest, _Host.CommandExecutor);

                _Host.RootEditor.Content.HideFeedback(_spaceRequest);
            }

            _canvas.Tool = _toolOnFinished;
            return true;
        }

        public override bool HandleDragCancel(EventArgs e) {
            if (_targets != null) {
                _targets.HideFeedback(_moveRequest);
            }
            _Host.RootEditor.Content.HideFeedback(_spaceRequest);

            return true;
        }

        public override bool HandleMouseClick(MouseEventArgs e) {
            return true;
        }

        public override bool HandleMouseDoubleClick(MouseEventArgs e) {
            return true;
        }

        public override bool HandleMouseEnter(EventArgs e) {
            return true;
        }

        public override bool HandleMouseLeave(EventArgs e) {
            return true;
        }

        public override bool HandleMouseHover(EventArgs e) {
            return true;
        }

        public override bool HandleKeyDown(KeyEventArgs e) {
            return true;
        }

        public override bool HandleKeyUp(KeyEventArgs e) {
            return true;
        }

        public override bool HandleKeyPress(KeyPressEventArgs e) {
            return true;
        }

        public override bool HandlePreviewKeyDown(PreviewKeyDownEventArgs e) {
            return true;
        }

    }
}
