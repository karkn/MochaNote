/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.StyledText;
using Mkamo.Common.Command;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    internal class InsertStringToInlineCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Inline _target;
        private int _index;
        private string _string;

        // ========================================
        // constructor
        // ========================================
        public InsertStringToInlineCommand(Inline target, string str, int index) {
            _target = target;
            _string = str;
            _index = index;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public Inline Target {
            get { return _target; }
        }

        public int Index {
            get { return _index; }
        }

        public string String {
            get { return _string; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _target.Insert(_index, _string);
        }

        public override void Undo() {
            _target.Remove(_index, _string.Length);
        }
    }
}
