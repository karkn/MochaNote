/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using System.Drawing;
using Mkamo.Figure.Core;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Internal.Core;
using Mkamo.Common.Command;
using Mkamo.Common.Diagnostics;

namespace Mkamo.Editor.Commands {
    using Editor = Mkamo.Editor.Internal.Editors.Editor;

    public class CreateEdgeCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private IModelFactory _modelFactory;
        private Point[] _edgePoints;
        private IEditor _edgeSourceEditor;
        private IEditor _edgeTargetEditor;

        private IEditor _createdEditor;

        // ========================================
        // constructor
        // ========================================
        public CreateEdgeCommand(
            IEditor target,
            IModelFactory modelFactory,
            Point[] edgePoints,
            IEditor edgeSourceEditor,
            IEditor edgeTargetEditor
        ) {
            _target = target;
            _modelFactory = modelFactory;
            _edgePoints = edgePoints;
            _edgeSourceEditor = edgeSourceEditor;
            _edgeTargetEditor = edgeTargetEditor;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get {
                var containerCtl = _target.Controller as IContainerController;
                return
                    _target != null &&
                    _target is Editor &&
                    _modelFactory != null &&
                    containerCtl != null &&
                    containerCtl.CanContainChild(_modelFactory.ModelDescriptor) &&
                    (_edgeSourceEditor == null || _edgeSourceEditor.IsConnectable) &&
                    (_edgeTargetEditor == null || _edgeTargetEditor.IsConnectable);
            }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public IEditor Target {
            get { return _target; }
        }

        public IModelFactory ModelFactory {
            get { return _modelFactory; }
        }

        public Point[] EdgePoints {
            get { return _edgePoints; }
        }

        public IEditor EdgeSourceEditor {
            get { return _edgeSourceEditor; }
        }

        public IEditor EdgeTargetEditor {
            get { return _edgeTargetEditor; }
        }


        public IEditor CreatedEditor {
            get { return _createdEditor; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            /// modelの生成
            var model = _modelFactory.CreateModel();
            Contract.Requires(model != null);

            _createdEditor = _target.AddChild(model);
            SetUp(_createdEditor, model);

            var select = new SelectRequest();
            select.DeselectOthers = true;
            select.Value = SelectKind.True;
            _createdEditor.PerformRequest(select);
        }

        public override void Undo() {
            var model = _createdEditor.Model;
            var containerCtl = _target.Controller as IContainerController;
            if (containerCtl != null) {

                /// 切断
                var edge = _createdEditor.Figure as IEdge;
                var createdConnectionCtrl = _createdEditor.Controller as IConnectionController;
                if (edge.IsSourceConnected) {
                    var srcConnectableCtrl = _edgeSourceEditor == null ?
                        null :
                        _edgeSourceEditor.Controller as IConnectableController;
                    edge.Source = null;
                    createdConnectionCtrl.DisconnectSource();
                    srcConnectableCtrl.DisconnectOutgoing(model);
                }
                if (edge.IsTargetConnected) {
                    var tgtConnectableCtrl = _edgeTargetEditor == null ?
                        null :
                        _edgeTargetEditor.Controller as IConnectableController;
                    edge.Target = null;
                    createdConnectionCtrl.DisconnectTarget();
                    tgtConnectableCtrl.DisconnectIncoming(model);
                }


                _target.RemoveChildEditor(_createdEditor);
                containerCtl.RemoveChild(model);
                _createdEditor.Disable();
                _createdEditor.Controller.DisposeModel(model);
            }
        }

        public override void Redo() {
            var containerCtrl = _target.Controller as IContainerController;

            _createdEditor.Controller.RestoreModel(_createdEditor.Model);

            var model = _createdEditor.Model;
            _target.AddChildEditor(_createdEditor);
            containerCtrl.InsertChild(_createdEditor.Model, containerCtrl.ChildCount);
            SetUp(_createdEditor, model);

            var select = new SelectRequest();
            select.DeselectOthers = true;
            select.Value = SelectKind.True;
            _createdEditor.PerformRequest(select);
        }

        private void SetUp(IEditor createdEditor, object model) {
            /// 親controllerへの通知
            var containerCtrl = _target.Controller as IContainerController;
            Contract.Requires(
                containerCtrl != null && containerCtrl.CanContainChild(_modelFactory.ModelDescriptor)
            );

            var createdConnectionCtrl = createdEditor.Controller as IConnectionController;
            var createdEdge = createdEditor.Figure as IEdge;
            if (createdEditor == null || createdConnectionCtrl == null || createdEdge == null) {
                /// rollback
                containerCtrl.RemoveChild(model);
                _target.RemoveChildEditor(createdEditor);
                throw new ArgumentException();
            }

            var srcConnectableCtrl = _edgeSourceEditor == null?
                null:
                _edgeSourceEditor.Controller as IConnectableController;
            var srcConnectableFig = _edgeSourceEditor == null?
                null:
                _edgeSourceEditor.Figure as IConnectable;
            var tgtConnectableCtrl = _edgeTargetEditor == null?
                null:
                _edgeTargetEditor.Controller as IConnectableController;
            var tgtConnectableFig = _edgeTargetEditor == null?
                null:
                _edgeTargetEditor.Figure as IConnectable;
            
            /// パラメタの妥当性検査
            var isValidSrcConnect =
                _edgeSourceEditor != null &&
                srcConnectableFig != null &&
                createdConnectionCtrl.CanConnectSource(_edgeSourceEditor.Model);
            var isValidSrcDisconnect =
                (_edgeSourceEditor == null ||
                    srcConnectableCtrl == null ||
                    !createdConnectionCtrl.CanConnectSource(_edgeSourceEditor.Model)
                ) &&
                createdConnectionCtrl.CanDisconnectSource;
            var isValidSrc = isValidSrcConnect || isValidSrcDisconnect;

            var isValidTgtConnect =
                _edgeTargetEditor != null &&
                tgtConnectableCtrl != null &&
                createdConnectionCtrl.CanConnectTarget(_edgeTargetEditor.Model);
            var isValidTgtDisconnect =
                (_edgeTargetEditor == null ||
                    tgtConnectableCtrl == null ||
                    !createdConnectionCtrl.CanConnectTarget(_edgeTargetEditor.Model)
                ) &&
                createdConnectionCtrl.CanDisconnectTarget;
            var isValidTgt = isValidTgtConnect || isValidTgtDisconnect;

            if (!isValidSrc || !isValidTgt) {
                /// rollback
                containerCtrl.RemoveChild(model);
                _target.RemoveChildEditor(createdEditor);
                throw new ArgumentException();
            }

            /// controllerの通知
            if (isValidSrcConnect) {
                createdConnectionCtrl.ConnectSource(_edgeSourceEditor.Model);
                srcConnectableCtrl.ConnectOutgoing(model);
            }
            if (isValidTgtConnect) {
                createdConnectionCtrl.ConnectTarget(_edgeTargetEditor.Model);
                tgtConnectableCtrl.ConnectIncoming(model);
            }

            /// figureの編集
            createdEdge.SetEdgePoints(_edgePoints);
            createdEdge.Route();
            if (isValidSrcConnect) {
                createdEdge.Source = srcConnectableFig;
            }
            if (isValidTgtConnect) {
                createdEdge.Target = tgtConnectableFig;
            }

            createdEditor.Enable();
        }

    }
}
