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

namespace Mkamo.Editor.Commands {
    public class CombineCommand: AbstractCommand {

        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private IEnumerable<IEditor> _combineds;
        private EditorCombinator _combinator;

        private ICommand _commands;

        // ========================================
        // constructor
        // ========================================
        public CombineCommand(IEditor target, IEnumerable<IEditor> combineds, EditorCombinator combinator) {
            _target = target;
            _combineds = combineds;
            _combinator = combinator;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _combineds != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _commands = new CompositeCommand();

            var undoer = default(EditorCombinatorUndoer);
            var combine = new DelegatingCommand(
                () => undoer = _combinator(_target, _combineds),
                () => undoer(_target)
            );
            _commands.Chain(combine);
            
            foreach (var combined in _combineds) {
                var remove = new RemoveCommand(combined);
                _commands.Chain(remove);
            }

            _commands.Execute();
        }

        public override void Undo() {
            if (_commands != null) {
                _commands.Undo();
            }
        }
    }
}
