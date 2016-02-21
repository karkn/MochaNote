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
using Mkamo.Editor.Requests;
using Mkamo.Common.Diagnostics;

namespace Mkamo.Editor.Commands {
    public class CutCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEnumerable<IEditor> _targets;

        private ICommand _command;

        // ========================================
        // constructor
        // ========================================
        public CutCommand(IEnumerable<IEditor> targets) {
            Contract.Requires(targets != null);
            _targets = targets.ToArray();

        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get {
                if (_targets == null) {
                    return false;
                }
                if (_command == null) {
                    _command = GetCutCommand(_targets);
                }
                return _command.CanExecute;
            }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            if (_command == null) {
                _command = GetCutCommand(_targets);
            }
            if (_command != null) {
                _command.Execute();
            }
        }

        public override void Undo() {
            if (_command != null) {
                _command.Undo();
            }
        }

        private ICommand GetCutCommand(IEnumerable<IEditor> targets) {
            var bundle = new EditorBundle(targets);
            var req = new CopyRequest(targets);
            var copy = bundle.GetGroupCommand(req);
            var remove = bundle.GetCompositeCommand(new RemoveRequest());
            return copy.Chain(remove);
        }
    }
}
