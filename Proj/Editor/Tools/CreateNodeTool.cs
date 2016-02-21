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
using Mkamo.Common.Forms.Input;

namespace Mkamo.Editor.Tools {
    public class CreateNodeTool: AbstractTool {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private CreateNodeRequest _request;
        private Point _startPoint;
        private Size _defaultNodeSize;

        private Action<INode> _nodeInitializer;

        private bool _focusOnCreated;

        private ITool _defaultTool;

        // ========================================
        // constructor
        // ========================================
        public CreateNodeTool(IModelFactory modelFactory, INode feedback, Action<INode> nodeInitializer): base(feedback) {
            _focusOnCreated = false;
            _request = new CreateNodeRequest();
            _request.ModelFactory = modelFactory;
            _defaultNodeSize = new Size(80, 64);
            _nodeInitializer = nodeInitializer;
        }

        public CreateNodeTool(IModelFactory modelFactory, INode feedback): this(modelFactory, feedback, null) {
        }

        public CreateNodeTool(IModelFactory modelFactory): this(modelFactory, null, null) {
        }


        // ========================================
        // event
        // ========================================
        public event EventHandler NodeCreated;

        // ========================================
        // property
        // ========================================
        public Size DefaultNodeSize {
            get { return _defaultNodeSize; }
            set { _defaultNodeSize = value; }
        }

        public bool FocusOnCreated {
            get { return _focusOnCreated; }
            set { _focusOnCreated = value; }
        }

        public ITool DefaultTool {
            get { return _defaultTool; }
            set { _defaultTool = value; }
        }

        // ========================================
        // method
        // ========================================
        public void CreateNode(IEditor target, Point loc) {
            CreateNode(target, new Rectangle(loc, _defaultNodeSize));
        }

        public override bool HandleMouseDown(MouseEventArgs e) {
            _request.CustomFeedback = CustomFeedback;
            _startPoint = e.Location;
            _request.Bounds = Rectangle.Empty;

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
                _request.Bounds = RectUtil.GetRectangleFromDiagonalPoints(_startPoint, e.Location);
                _target.ShowFeedback(_request);
            }
            return true;
        }

        public override bool HandleDragFinish(MouseEventArgs e) {
            if (_target != null) {
                var bounds = RectUtil.GetRectangleFromDiagonalPoints(_startPoint, e.Location);
                _request.Bounds = bounds;
                _target.HideFeedback(_request);
                CreateNode(_target, bounds);
                if (!KeyUtil.IsControlPressed()) {
                    _target.Site.EditorCanvas.Tool = _defaultTool;
                }
                _target = null;
            }
            return true;
        }

        public override bool HandleDragCancel(EventArgs e) {
            if (_target != null) {
                _target.HideFeedback(_request);
                _target.Site.EditorCanvas.Tool = _defaultTool;
                _target = null;
            }
            return true;
        }

        public override bool HandleMouseClick(MouseEventArgs e) {
            if (_target != null) {
                var bounds = new Rectangle(_startPoint, _defaultNodeSize);
                _request.Bounds = bounds;
                _target.HideFeedback(_request);
                if (!KeyUtil.IsControlPressed()) {
                    _target.Site.EditorCanvas.Tool = _defaultTool;
                }
                CreateNode(_target, bounds);
                _target = null;
            }
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


        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnNodeCreated() {
            var handler = NodeCreated;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private CreateNodeCommand CreateNode(IEditor target, Rectangle bounds) {
            if (target == null) {
                return null;
            }

            _request.Bounds = bounds;
            var cmd = target.PerformRequest(_request) as CreateNodeCommand;

            var created = cmd.CreatedEditor;
            if (_nodeInitializer != null && cmd != null) {
                var node = created.Figure as INode;
                if (node != null) {
                    _nodeInitializer(node);
                }
            }
            OnNodeCreated();
            if (created != null && _focusOnCreated) {
                created.RequestFocus(FocusKind.Begin, Point.Empty);
                var focus = created.Focus as StyledTextFocus;
                if (focus != null) {
                    focus.SelectAll();
                }
            }

            return cmd;
        }

    }
}
