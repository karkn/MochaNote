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
using System.Drawing;
using Mkamo.Common.Command;

namespace Mkamo.Editor.Commands {
    public class ConnectCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private IAnchor _connectingAnchor;
        private IEditor _newConnectableEditor;
        private Point _newLocation;

        private IEditor _oldConnectableEditor;
        private Point _oldLocation;
        private bool _connectionModified;

        /// <summary>
        /// 切断のときはnewConnectableEditorにnullを渡す．
        /// </summary>
        public ConnectCommand(IEditor target, IAnchor connectingAnchor, IEditor newConnectableEditor, Point newLocation) {
            _target = target;
            _newConnectableEditor = newConnectableEditor;
            _connectingAnchor = connectingAnchor;
            _newLocation = newLocation;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute{
            get {
                if (_target == null || _connectingAnchor == null) {
                    return false;
                }

                var isValidTarget = _target.Model != null && _target.IsConnection;
                if (!isValidTarget) {
                    return false;
                }
                var connControler = _target.Controller as IConnectionController;

                var isValidNewConnectableEditor =
                    _newConnectableEditor == null ||
                    (
                        _newConnectableEditor.Model != null &&
                        _newConnectableEditor.IsConnectable
                    );
                if (!isValidNewConnectableEditor) {
                    return false;
                }

                if (_connectingAnchor.Kind == ConnectionAnchorKind.Source) {
                    if (_newConnectableEditor == null) {
                        return connControler.CanDisconnectSource;
                    } else {
                        return connControler.CanConnectSource(_newConnectableEditor.Model);
                    }
                } else {
                    if (_newConnectableEditor == null) {
                        return connControler.CanDisconnectTarget;
                    } else {
                        return connControler.CanConnectTarget(_newConnectableEditor.Model);
                    }
                }
            }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public IEditor ConnectionEditor {
            get { return _target; }
        }

        public IAnchor ConnectingAnchor {
            get { return _connectingAnchor; }
        }

        public IEditor NewConnectableEditor {
            get { return _newConnectableEditor; }
        }

        public Point Location {
            get { return _newLocation; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            /// for undo
            var oldConnectable = _connectingAnchor.Connectable;
            _oldConnectableEditor = oldConnectable == null? null: oldConnectable.GetEditor();
            _oldLocation = _connectingAnchor.Location;

            if (_newConnectableEditor == _oldConnectableEditor) {
                _connectingAnchor.Location = _newLocation;
                _connectionModified = false;
            } else {
                Connect(_newConnectableEditor, _newLocation);
                _connectionModified = true;
            }
        }

        public override void Undo() {
            if (_connectionModified) {
                Connect(_oldConnectableEditor, _oldLocation);
            } else {
                _connectingAnchor.Location = _oldLocation;
            }
        }


        private void Connect(IEditor connecting, Point location) {
            var connectionCtrl = _target.Controller as IConnectionController;

            /// controllerに切断させる
            if (_connectingAnchor.IsConnected) {
                var disconnEditor = _connectingAnchor.Connectable.GetEditor();
                var disconnCtrl = disconnEditor.Controller as IConnectableController;
                if (_connectingAnchor.Kind == ConnectionAnchorKind.Source) {
                    disconnCtrl.DisconnectOutgoing(_target.Model);
                    connectionCtrl.DisconnectSource();
                } else {
                    disconnCtrl.DisconnectIncoming(_target.Model);
                    connectionCtrl.DisconnectTarget();
                }
            }

            /// controllerに接続させる
            if (connecting != null) {
                var connectingCtrl = connecting.Controller as IConnectableController;
                if (_connectingAnchor.Kind == ConnectionAnchorKind.Source) {
                    connectingCtrl.ConnectOutgoing(_target.Model);
                    connectionCtrl.ConnectSource(connecting.Model);
                } else {
                    connectingCtrl.ConnectIncoming(_target.Model);
                    connectionCtrl.ConnectTarget(connecting.Model);
                }
            }

            /// figureの切断
            if (_connectingAnchor.IsConnected) {
                _connectingAnchor.Disconnect();
            }

            /// figureの接続
            _connectingAnchor.Location = location;
            if (connecting != null) {
                var connectable = connecting.Figure as IConnectable;
                _connectingAnchor.Connect(connectable);
            }
        }
    }
}
