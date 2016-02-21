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
using Mkamo.Common.Command;

namespace Mkamo.Editor.Commands {
    public class SelectCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private bool _deselectOthers;
        private SelectKind _value;

        public SelectCommand(IEditor target, SelectKind value, bool deselectOthers) {
            _target = target;
            _deselectOthers = deselectOthers;
            _value = value;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _target.Root != null; }
        }

        public override bool CanUndo {
            get { return false; }
        }

        public IEditor Target {
            get { return _target; }
        }

        public bool DeselectOthers {
            get { return _deselectOthers; }
        }

        public SelectKind Value {
            get { return _value; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            if (!CanExecute) {
                throw new ArgumentException();
            }

            if (_deselectOthers) {
                _target.Site.SelectionManager.DeselectAll();
            }

            switch (_value) {
                case SelectKind.True: {
                    _target.IsSelected = true;
                    break;
                }
                case SelectKind.False: {
                    _target.IsSelected = false;
                    break;
                }
                case SelectKind.Toggle: {
                    _target.IsSelected = !_target.IsSelected;
                    break;
                }
            }
        }

        public override void Undo() {
            
        }
    }
}
