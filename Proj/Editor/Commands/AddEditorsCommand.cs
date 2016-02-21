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
using Mkamo.Editor.Internal.Core;
using Mkamo.Figure.Figures;
using System.Drawing;

namespace Mkamo.Editor.Commands {
    public class AddEditorsCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private IEnumerable<IEditor> _addingEditors;
        private Size _moveDelta;

        // ========================================
        // constructor
        // ========================================
        public AddEditorsCommand(IEditor target, IEnumerable<IEditor> addingEditors, Size moveDelta) {
            _target = target;
            _addingEditors = addingEditors;
            _moveDelta = moveDelta;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _addingEditors != null && _addingEditors.Any(); }
        }

        public override bool CanUndo {
            get { return true; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var containerCtrl = _target.Controller as IContainerController;

            /// 移動量
            foreach (var editor in _addingEditors) {
                if (containerCtrl.CanContainChild(editor.Controller.ModelDescriptor)) {
                    if (!_moveDelta.IsEmpty) {
                        editor.Figure.Move(_moveDelta);
                    }
                    _target.AddChildEditor(editor);
                    containerCtrl.InsertChild(editor.Model, containerCtrl.ChildCount);
                    editor.Enable();
                }
            }

            /// Route()だけやり直し
            foreach (var editor in _addingEditors) {
                if (editor.Figure is LineEdge) {
                    var edge = editor.Figure as LineEdge;
                    edge.Route();
                }
            }
        }

        public override void Undo() {
            var containerCtrl = _target.Controller as IContainerController;

            foreach (var editor in _addingEditors) {
                var model = editor.Model;
                if (containerCtrl != null) {
                    _target.RemoveChildEditor(editor);
                    containerCtrl.RemoveChild(model);
                    editor.Disable();
                    editor.Controller.DisposeModel(model);
                }
            }
        }

        public override void Redo() {
            var containerCtrl = _target.Controller as IContainerController;

            foreach (var editor in _addingEditors) {
                editor.Controller.RestoreModel(editor.Model);
                _target.AddChildEditor(editor);
                containerCtrl.InsertChild(editor.Model, containerCtrl.ChildCount);
                editor.Enable();
            }

            /// Route()だけやり直し
            foreach (var editor in _addingEditors) {
                if (editor.Figure is LineEdge) {
                    var edge = editor.Figure as LineEdge;
                    edge.Route();
                }
            }
        }
    }
}
