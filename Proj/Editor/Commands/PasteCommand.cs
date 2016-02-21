/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Requests;
using Mkamo.Common.Externalize;
using Mkamo.Editor.Internal.Core;
using System.Drawing;
using Mkamo.Common.Command;
using Mkamo.Common.DataType;
using System.Windows.Forms;
using Mkamo.Figure.Figures;

namespace Mkamo.Editor.Commands {
    public class PasteCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private Point _location;
        private string _description;

        private string _format;
        private EditorPaster _creator;

        private IEnumerable<IEditor> _pastedEditors;

        // ========================================
        // constructor
        // ========================================
        public PasteCommand(IEditor target, Point location, string description, string format, EditorPaster creator) {
            _target = target;
            _location = location;
            _description = description;

            _format = format;
            _creator = creator;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get {
                var data = Clipboard.GetDataObject();
                return
                    _target != null &&
                    (
                        (_format != null && _creator != null) ||
                        EditorFactory.CanRestoreDataObject(data, _target)
                    );
            }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public IEnumerable<IEditor> PastedEditors {
            get { return _pastedEditors; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var container = _target.Controller as IContainerController;
            if (container == null) {
                return;
            }

            var data = Clipboard.GetDataObject();
            if (EditorFactory.CanRestoreDataObject(data)) {
                /// Editorを復元
                _pastedEditors = EditorFactory.RestoreDataObject(data, _target.Site.ControllerFactory);
    
                /// 一番左上の位置計算
                var left = int.MaxValue;
                var top = int.MaxValue;
                foreach (var editor in _pastedEditors) {
                    if (container.CanContainChild(editor.Controller.ModelDescriptor)) {
                        left = left < editor.Figure.Left? left: editor.Figure.Left;
                        top = top < editor.Figure.Top? top: editor.Figure.Top;
                    }
                }

                /// 各editorの移動量
                var moveDelta = new Size(_location.X - left, _location.Y - top);
    
                /// _targetに追加
                foreach (var editor in _pastedEditors) {
                    if (container.CanContainChild(editor.Controller.ModelDescriptor)) {
                        editor.Figure.Location += moveDelta;
                        _target.AddChildEditor(editor);
                        container.InsertChild(editor.Model, container.ChildCount);
                        editor.Enable();
                    }
                }

                /// Route()だけやり直し
                foreach (var editor in _pastedEditors) {
                    if (editor.Figure is LineEdge) {
                        var edge = editor.Figure as LineEdge;
                        edge.Route();
                    }
                }

            } else if (_format != null && _creator != null) {
                /// 一般的なフォーマットから復元
                var format = DataFormats.GetFormat(_format);
                if (data.GetDataPresent(format.Name)) {
                    _pastedEditors = _creator(_target, _location, _description);
                }
            }
        }

        public override void Undo() {
            var containerCtrl = _target.Controller as IContainerController;

            foreach (var editor in _pastedEditors) {
                var model = editor.Model;
                if (containerCtrl != null) {
                    editor.Disable();
                    _target.RemoveChild(model);
                    editor.Controller.DisposeModel(model);
                }
            }
        }

        public override void Redo() {
            var containerCtrl = _target.Controller as IContainerController;

            foreach (var editor in _pastedEditors) {
                editor.Controller.RestoreModel(editor.Model);
                _target.AddChildEditor(editor);
                containerCtrl.InsertChild(editor.Model, containerCtrl.ChildCount);
                editor.Enable();
            }

            /// Route()だけやり直し
            foreach (var editor in _pastedEditors) {
                if (editor.Figure is LineEdge) {
                    var edge = editor.Figure as LineEdge;
                    edge.Route();
                }
            }
        }
    }
}
