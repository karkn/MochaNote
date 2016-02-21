/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using System.Windows.Forms;
using Mkamo.Editor.Requests;
using System.Drawing;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Editor.Commands;
using Mkamo.Editor.Focuses;

namespace Mkamo.Editor.Tools {
    public class FreehandTool: AbstractTool {
        // ========================================
        // field
        // ========================================
        private IModelFactory _modelFactory;

        private IEditor _target;
        private FreehandRequest _request;
        private Point _startPoint;
        private Point _prevPoint;

        private int _width;
        private Color _color;

        // ========================================
        // constructor
        // ========================================
        public FreehandTool(IFigure feedback, IModelFactory modelFactory, int width, Color color)
            : base(feedback) {
            _modelFactory = modelFactory;
            _width = width;
            _color = color;
        }


        // ========================================
        // event
        // ========================================
        public event EventHandler FreehandCreated;

        // ========================================
        // property
        // ========================================
        public int Width {
            get { return _width; }
            set {
                _width = value;
                if (_request != null) {
                    _request.Width = _width;
                }
            }
        }

        public Color Color {
            get { return _color; }
            set {
                _color = value;
                if (_request != null) {
                    _request.Color = _color;
                }
            }
        }


        // ========================================
        // method
        // ========================================
        public override bool HandleMouseDown(MouseEventArgs e) {
            _startPoint = e.Location;
            _prevPoint = e.Location;

            _request = new FreehandRequest();
            _request.CustomFeedback = CustomFeedback;
            _request.ModelFactory = _modelFactory;
            _request.Width = _width;
            _request.Color = _color;
            _request.Points.Add(_startPoint);

            _target = _Host.RootEditor.FindEditor(
                e.Location,
                editor => editor.CanUnderstand(_request)
            );
            return true;
        }

        public override bool HandleMouseMove(MouseEventArgs e) {
            return true;
        }

        public override bool HandleMouseUp(MouseEventArgs e) {
            return true;
        }

        public override bool HandleDragStart(MouseEventArgs e) {
            if (_target != null) {
                _target.ShowFeedback(_request);
            }
            return true;
        }

        public override bool HandleDragMove(MouseEventArgs e) {
            if (_target != null) {
                if (_prevPoint != e.Location) {
                    _request.Points.Add(e.Location);
                    _target.ShowFeedback(_request);
                    _prevPoint = e.Location;
                }
            }
            return true;
        }

        public override bool HandleDragFinish(MouseEventArgs e) {
            if (_target != null) {
                //var bounds = RectUtil.GetRectangleFromDiagonalPoints(_startPoint, e.Location);
                //_request.Bounds = bounds;
                if (_prevPoint != e.Location) {
                    _request.Points.Add(e.Location);
                }
                _target.HideFeedback(_request);
                _target.PerformRequest(_request);

            }
            return true;
        }

        public override bool HandleDragCancel(EventArgs e) {
            if (_target != null) {
                _target.HideFeedback(_request);
            }
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
            /// KeyBindが効くように
            return false;
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


        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnFreehandCreated() {
            var handler = FreehandCreated;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        //private CreateNodeCommand CreateNode(IEditor target, Rectangle bounds) {
        //    if (target == null) {
        //        return null;
        //    }

        //    _request.Bounds = bounds;
        //    var cmd = target.PerformRequest(_request) as CreateNodeCommand;

        //    var created = cmd.CreatedEditor;
        //    if (_nodeInitializer != null && cmd != null) {
        //        var node = created.Figure as INode;
        //        if (node != null) {
        //            _nodeInitializer(node);
        //        }
        //    }
        //    OnNodeCreated();
        //    if (created != null && _focusOnCreated) {
        //        created.RequestFocus(FocusKind.Begin, Point.Empty);
        //        var focus = created.Focus as StyledTextFocus;
        //        if (focus != null) {
        //            focus.SelectAll();
        //        }
        //    }

        //    return cmd;
        //}

    }
}
