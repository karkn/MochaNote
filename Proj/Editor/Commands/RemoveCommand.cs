/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Internal.Core;
using Mkamo.Editor.Requests;
using Mkamo.Common.Command;
using Mkamo.Common.Core;
using Mkamo.Figure.Core;

namespace Mkamo.Editor.Commands {
    using Editor = Mkamo.Editor.Internal.Editors.Editor;

    public class RemoveCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;

        private IEditor _oldParent;
        private int _oldEditorIndex;
        private int _oldModelIndex;

        private ICommand _disconnCommand;

        // ========================================
        // constructor
        // ========================================
        public RemoveCommand(IEditor target) {
            _target = target;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get {
                return
                    _target != null &&
                    _target.Parent != null &&
                    _target.Parent.Controller is IContainerController;
            }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public IEditor Target {
            get { return _target; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _target.IsSelected = false;

            _oldParent = _target.Parent;
            _oldEditorIndex = _oldParent.Children.IndexOf(_target);

            var parentCtrl = _oldParent.Controller as IContainerController;
            _oldModelIndex = parentCtrl.Children.IndexOf(_target.Model);

            _disconnCommand = CreateDisconnCommand();
            _disconnCommand.Execute();

            var model = _target.Model;
            _target.Disable();
            _oldParent.RemoveChild(model);
            _target.Controller.DisposeModel(model);
        }

        public override void Undo() {
            var model = _target.Model;
            var parentCtrl = _oldParent.Controller as IContainerController;

            _target.Controller.RestoreModel(model);
            _oldParent.InsertChildEditor(_target, _oldEditorIndex);
            parentCtrl.InsertChild(model, _oldModelIndex);
            _target.Enable();

            _disconnCommand.Undo();
        }

        private ICommand CreateDisconnCommand() {
            var ret = new CompositeCommand() as ICommand;

            var removeReq = new RemoveRequest();
            var disconnReq = new ConnectRequest();

            if (_target.IsConnectable) {
                /// connectableの接続の処理
                var connectable = _target.Figure as IConnectable;

                var conns = connectable.Outgoings;
                foreach (var connection in conns) {
                    var connEditor = connection.GetEditor();
                    var connCtrl = connEditor.Controller as IConnectionController;
                    
                    disconnReq.ConnectingAnchor = connection.SourceAnchor;
                    disconnReq.NewLocation = connection.SourceAnchor.Location;
                    ret = ret.Chain(connEditor.GetCommand(disconnReq));

                    if (!connCtrl.CanDisconnectSource) {
                        ret = ret.Chain(connEditor.GetCommand(removeReq));
                    }
                }

                conns = connectable.Incomings;
                foreach (var connection in conns) {
                    var connEditor = connection.GetEditor();
                    var connCtrl = connEditor.Controller as IConnectionController;

                    disconnReq.ConnectingAnchor = connection.TargetAnchor;
                    disconnReq.NewLocation = connection.TargetAnchor.Location;
                    ret = ret.Chain(connEditor.GetCommand(disconnReq));

                    if (!connCtrl.CanDisconnectTarget) {
                        ret = ret.Chain(connEditor.GetCommand(removeReq));
                    }
                }

            } else if (_target.IsConnection) {
                /// connectionの接続の処理
                var connection = _target.Figure as IConnection;
                var connectionEditor = connection.GetEditor();

                if (connection.IsSourceConnected) {
                    disconnReq.ConnectingAnchor = connection.SourceAnchor;
                    disconnReq.NewLocation = connection.SourceAnchor.Location;
                    ret = ret.Chain(connectionEditor.GetCommand(disconnReq));
                }

                if (connection.IsTargetConnected) {
                    disconnReq.ConnectingAnchor = connection.TargetAnchor;
                    disconnReq.NewLocation = connection.TargetAnchor.Location;
                    ret = ret.Chain(connectionEditor.GetCommand(disconnReq));
                }
            }

            return ret;
        }
    }
}
