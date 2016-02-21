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
    /// ModelをReorderするコマンド．
    /// </summary>
    public class ReorderModelCommand: AbstractCommand {
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
        public ReorderModelCommand(IEditor target, ReorderKind kind) {
            _oldIndex = -1;
            _target = target;
            _kind = kind;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get {
                if (_target == null || _target.Parent == null || !_target.Parent.IsContainer) {
                    return false;
                }
                switch (_kind) {
                    case ReorderKind.Front:
                    case ReorderKind.FrontMost: {
                        var parent = _target.Parent;
                        var containerCtrl = parent.Controller as IContainerController;
                        return containerCtrl.Children.IndexOf(_target.Model) != containerCtrl.Children.Count() - 1;
                    }
                    case ReorderKind.Back:
                    case ReorderKind.BackMost: {
                        var parent = _target.Parent;
                        var containerCtrl = parent.Controller as IContainerController;
                        return containerCtrl.Children.IndexOf(_target.Model) != 0;
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
            var model = _target.Model;

            var parent = _target.Parent;
            var containerCtrl = parent.Controller as IContainerController;
            _oldIndex = containerCtrl.Children.IndexOf(model);

            containerCtrl.RemoveChild(_target.Model);
            switch (_kind) {
                case ReorderKind.Front: {
                    containerCtrl.InsertChild(model, _oldIndex + 1);
                    break;
                }
                case ReorderKind.Back: {
                    containerCtrl.InsertChild(model, _oldIndex - 1);
                    break;
                }
                case ReorderKind.FrontMost: {
                    containerCtrl.InsertChild(model, containerCtrl.Children.Count());
                    break;
                }
                case ReorderKind.BackMost: {
                    containerCtrl.InsertChild(model, 0);
                    break;
                }
            }

        }

        public override void Undo() {
            var model = _target.Model;
            var parent = _target.Parent;
            var containerCtrl = parent.Controller as IContainerController;
            containerCtrl.RemoveChild(model);
            containerCtrl.InsertChild(model, _oldIndex);
        }
    }
}
