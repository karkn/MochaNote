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
using Mkamo.Editor.Internal.Core;
using Mkamo.Common.Command;
using Mkamo.Common.Core;

namespace Mkamo.Editor.Commands {
    /// <summary>
    /// EditorとFigureをReorderするコマンド．
    /// </summary>
    public class ReorderCommand: AbstractCommand {
        // ========================================
        // type
        // ========================================

        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private ReorderKind _kind;

        private int _oldIndex;

        // ========================================
        // constructor
        // ========================================
        public ReorderCommand(IEditor target, ReorderKind kind) {
            _oldIndex = -1;
            _target = target;
            _kind = kind;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get {
                if (_target == null || _target.Parent == null) {
                    return false;
                }
                switch (_kind) {
                    case ReorderKind.Front:
                    case ReorderKind.FrontMost: {
                        var parent = _target.Parent;
                        return parent.Children.IndexOf(_target) != parent.Children.Count() - 1;
                    }
                    case ReorderKind.Back:
                    case ReorderKind.BackMost: {
                        var parent = _target.Parent;
                        return parent.Children.IndexOf(_target) != 0;
                    }
                }

                throw new Exception("Unknown reorder kind");
            }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public IEditor Target {
            get { return _target; }
        }

        public ReorderKind Kind {
            get { return _kind; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var parent = _target.Parent;
            _oldIndex = parent.Children.IndexOf(_target);
            
            parent.RemoveChildEditor(_target);
            switch (_kind) {
                case ReorderKind.Front: {
                    parent.InsertChildEditor(_target, _oldIndex + 1);
                    break;
                }
                case ReorderKind.Back: {
                    parent.InsertChildEditor(_target, _oldIndex - 1);
                    break;
                }
                case ReorderKind.FrontMost: {
                    parent.InsertChildEditor(_target, parent.Children.Count());
                    break;
                }
                case ReorderKind.BackMost: {
                    parent.InsertChildEditor(_target, 0);
                    break;
                }
            }

        }

        public override void Undo() {
            var parent = _target.Parent;
            parent.RemoveChildEditor(_target);
            parent.InsertChildEditor(_target, _oldIndex);
        }
    }
}
