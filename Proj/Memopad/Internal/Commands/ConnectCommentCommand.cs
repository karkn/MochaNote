/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using System.Drawing;
using Mkamo.Common.Win32.Gdi32;
using Mkamo.Model.Memo;
using Mkamo.StyledText.Commands;
using Mkamo.StyledText.Core;
using Mkamo.Editor.Commands;

namespace Mkamo.Memopad.Internal.Commands {
    internal class ConnectCommentCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private IAnchor _connectingAnchor;
        private IEditor _newConnectableEditor;
        private Point _newLocation;

        private ICommand _command;
        private StyledText.Core.StyledText _newStyledText;
        private StyledText.Core.StyledText _oldStyledText;
        private string _oldTargetAnchorId;
        private string _newTargetAnchorId;

        // ========================================
        // constructor
        // ========================================
        public ConnectCommentCommand(IEditor target, IAnchor connectingAnchor, IEditor newConnectableEditor, Point newLocation) {
            _target = target;
            _connectingAnchor = connectingAnchor;
            _newConnectableEditor = newConnectableEditor;
            _newLocation = newLocation;
        }


        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get {
                if (_target == null || _connectingAnchor == null) {
                    return false;
                }

                var isValidTarget = _target.Model != null && _target.IsConnection;
                if (!isValidTarget) {
                    return false;
                }
                var connControler = _target.Controller as IConnectionController;

                if (_connectingAnchor.Kind == ConnectionAnchorKind.Source) {
                    var isLoop = _newConnectableEditor != null && _newConnectableEditor.Figure == _connectingAnchor.Owner.Target;
                    var isValidNewConnectableEditor =
                        _newConnectableEditor == null ||
                        (_newConnectableEditor.Model != null && _newConnectableEditor.IsConnectable);
                    if (!isValidNewConnectableEditor && !isLoop) {
                        return false;
                    }
                    
                    if (_newConnectableEditor == null) {
                        return connControler.CanDisconnectSource;
                    } else {
                        return connControler.CanConnectSource(_newConnectableEditor.Model);
                    }
                } else {
                    var isLoop = _newConnectableEditor != null && _newConnectableEditor.Figure == _connectingAnchor.Owner.Source;
                    var isValidNewTextEditor =
                        _newConnectableEditor == null || (_newConnectableEditor.Model is MemoText && _newConnectableEditor.IsConnectable);
                    if (!isValidNewTextEditor && !isLoop) {
                        return false;
                    }

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

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _newStyledText = null;
            _oldStyledText = null;
            _command = null;
            _oldTargetAnchorId = null;

            if (_connectingAnchor.Kind == ConnectionAnchorKind.Source) {
                _command = new ConnectCommand(_target, _connectingAnchor, _newConnectableEditor, _newLocation);
                _command.Execute();

            } else {
                var edge = _target.Figure as IEdge;
                _oldTargetAnchorId = edge.TargetConnectionOption as string;

                if (_newConnectableEditor == null) {
                    _command = new ConnectCommand(_target, _connectingAnchor, _newConnectableEditor, _newLocation);
                    _command.Execute();

                } else {
                    var node = _newConnectableEditor.Figure as INode;
                    var model = _newConnectableEditor.Model as MemoText;
                    _oldStyledText = model.StyledText.CloneDeeply() as StyledText.Core.StyledText;
                    _newStyledText = model.StyledText;

                    var charIndex = node.GetCharIndexAt(_newLocation);

                    var inline = node.StyledText.GetInlineAt(charIndex);
                    var anc = default(Anchor);
                    if (inline.IsAnchorCharacter) {
                        anc = inline as Anchor;
                    } else {
                        /// nodeのcharIndex位置にAnchor追加
                        anc = new Anchor();
                        var insertAnchorCommand = new InsertInlineCommand(_newStyledText, charIndex, anc);
                        insertAnchorCommand.Execute();
                    }

                    _newTargetAnchorId = anc.Id;
                    edge.TargetConnectionOption = _newTargetAnchorId;

                    _command = new ConnectCommand(_target, _connectingAnchor, _newConnectableEditor, _newLocation);
                    _command.Execute();
                }
            }
        }

        public override void Undo() {
            if (_command != null) {
                _command.Undo();
            }

            if (_connectingAnchor.Kind == ConnectionAnchorKind.Target) {
                var edge = _target.Figure as IEdge;
                edge.TargetConnectionOption = _oldTargetAnchorId;
            }
            
            if (_oldStyledText != null) {
                var model = _newConnectableEditor.Model as MemoText;
                model.StyledText = _oldStyledText;
            }
        }

        public override void Redo() {
            if (_newStyledText != null) {
                var model = _newConnectableEditor.Model as MemoText;
                model.StyledText = _newStyledText;
            }

            if (_connectingAnchor.Kind == ConnectionAnchorKind.Target) {
                var edge = _target.Figure as IEdge;
                edge.TargetConnectionOption = _newTargetAnchorId;
            }

            if (_command != null) {
                _command.Execute();
            }
        }
    }
}
