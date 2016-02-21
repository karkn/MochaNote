/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using System.Drawing;
using System.Windows.Forms;
using Mkamo.Editor.Commands;
using Mkamo.Common.Forms.Input;

namespace Mkamo.Editor.Tools {
    public class CreateEdgeTool: AbstractTool {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private CreateEdgeRequest _request;
        private Predicate<object> _sourceConnectionJudge;
        private Predicate<object> _targetConnectionJudge;
        private Action<IEdge> _edgeInitializer;
        private ITool _defaultTool;

        // ========================================
        // constructor
        // ========================================
        public CreateEdgeTool(
            IModelFactory modelFactory, IEdge feedback,
            Predicate<object> sourceConnectionJudge,
            Predicate<object> targetConnectionJudge,
            Action<IEdge> edgeInitializer
        ):
            base(feedback)
        {
            _request = new CreateEdgeRequest();
            _request.ModelFactory = modelFactory;
            _sourceConnectionJudge = sourceConnectionJudge;
            _targetConnectionJudge = targetConnectionJudge;
            _edgeInitializer = edgeInitializer;
        }

        public CreateEdgeTool(
            IModelFactory modelFactory,
            IEdge feedback,
            IConnectionController controller,
            Action<IEdge> edgeInitializer
        ):
            this(modelFactory, feedback, controller.CanConnectSource, controller.CanConnectTarget, edgeInitializer)
        {
        }

        public CreateEdgeTool(
            IModelFactory modelFactory,
            IEdge feedback,
            IConnectionController controller
        ):
            this(modelFactory, feedback, controller.CanConnectSource, controller.CanConnectTarget, null)
        {
        }

        //public CreateEdgeTool(IModelFactory modelFactory)
        //    : this(modelFactory, null, null, null)
        //{
        //}


        // ========================================
        // event
        // ========================================
        public event EventHandler EdgeCreated;

        // ========================================
        // property
        // ========================================
        public ITool DefaultTool {
            get { return _defaultTool; }
            set { _defaultTool = value; }
        }


        // ========================================
        // method
        // ========================================
        public override bool HandleMouseDown(MouseEventArgs e) {
            return true;
        }

        public override bool HandleMouseMove(MouseEventArgs e) {
            return true;
        }

        public override bool HandleMouseUp(MouseEventArgs e) {
            return true;
        }

        public override bool HandleDragStart(MouseEventArgs e) {
            _request.CustomFeedback = CustomFeedback;
            _request.StartPoint = e.Location; 
            _request.EndPoint = e.Location;

            _target = _Host.RootEditor.FindEditor(
                e.Location,
                editor => editor.CanUnderstand(_request)
            );
            if (_target != null) {
                _request.EdgeSourceEditor = _Host.RootEditor.FindEditor(
                    _request.StartPoint,
                    editor =>
                        !editor.IsRoot &&
                        editor != _target &&
                        editor.IsConnectable &&
                        (_sourceConnectionJudge == null? true: _sourceConnectionJudge(editor.Model))
                );
                _target.ShowFeedback(_request);
            }
            return true;
        }

        public override bool HandleDragMove(MouseEventArgs e) {
            if (_target != null) {
                _request.EndPoint = e.Location;
                _request.EdgeTargetEditor = _Host.RootEditor.FindEditor(
                    _request.EndPoint,
                    editor =>
                        !editor.IsRoot &&
                        editor != _target &&
                        editor.IsConnectable &&
                        (_targetConnectionJudge == null? true: _targetConnectionJudge(editor.Model))
                );
                _target.ShowFeedback(_request);
            }
            return true;
        }

        public override bool HandleDragFinish(MouseEventArgs e) {
            if (_target != null) {
                _request.EndPoint = e.Location;

                _request.EdgeSourceEditor = _Host.RootEditor.FindEditor(
                    _request.StartPoint,
                    editor =>
                        !editor.IsRoot &&
                        editor != _target &&
                        editor.IsConnectable &&
                        (_sourceConnectionJudge == null? true: _sourceConnectionJudge(editor.Model))
                );
                _request.EdgeTargetEditor = _Host.RootEditor.FindEditor(
                    _request.EndPoint,
                    editor =>
                        !editor.IsRoot &&
                        editor != _target &&
                        editor.IsConnectable &&
                        (_targetConnectionJudge == null? true: _targetConnectionJudge(editor.Model))
                );

                _target.HideFeedback(_request);
                var cmd = _target.PerformRequest(_request) as CreateEdgeCommand;
                if (_edgeInitializer != null && cmd != null) {
                    var created = cmd.CreatedEditor;
                    var edge = created.Figure as IEdge;
                    if (edge != null) {
                        _edgeInitializer(edge);
                    }
                }
                if (!KeyUtil.IsControlPressed()) {
                    _target.Site.EditorCanvas.Tool = _defaultTool;
                }
                OnEdgeCreated();
                _target = null;
            }
            return true;
        }

        public override bool HandleDragCancel(EventArgs e) {
            if (_target != null) {
                _target.HideFeedback(_request);
                _target.Site.EditorCanvas.Tool = _defaultTool;
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
        protected virtual void OnEdgeCreated() {
            var handler = EdgeCreated;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
