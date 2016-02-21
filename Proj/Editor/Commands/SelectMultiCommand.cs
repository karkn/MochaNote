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
    public class SelectMultiCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEnumerable<IEditor> _targets;
        private bool _deselectOthers;
        private SelectKind _value;

        public SelectMultiCommand(IEnumerable<IEditor> targets, SelectKind value, bool deselectOthers) {
            _targets = targets;
            _deselectOthers = deselectOthers;
            _value = value;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _targets != null && _targets.Any(); }
        }

        public override bool CanUndo {
            get { return false; }
        }

        public IEnumerable<IEditor> Targets {
            get { return _targets; }
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
            var man = _targets.First().Site.SelectionManager;

            if (_deselectOthers) {
                man.DeselectAll();
            }


            switch (_value) {
                case SelectKind.True: {
                    man.SelectMulti(_targets, false);
                    break;
                }
                case SelectKind.False: {
                    throw new NotImplementedException();
                }
                case SelectKind.Toggle: {
                    man.SelectMulti(_targets, true);
                    break;
                }
            }
        }

        public override void Undo() {
            
        }
    }
}
