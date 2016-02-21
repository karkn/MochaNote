/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;
using Mkamo.Figure.Figures;
using Mkamo.Common.Diagnostics;

namespace Mkamo.Memopad.Internal.Commands {
    internal class ChangeRowHeightCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private TableFigure _target;
        private int _rowIndex;
        private int _newHeight;

        private int _oldHeight;

        // ========================================
        // constructor
        // ========================================
        public ChangeRowHeightCommand(TableFigure target, int rowIndex, int newHeight) {
            Contract.Requires(target != null);
            Contract.Requires(rowIndex > -1);

            _target = target;
            _rowIndex = rowIndex;
            _newHeight = newHeight;
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


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var row = _target.TableData.Rows.ElementAt(_rowIndex);
            _oldHeight = row.Height;
            row.Height = _newHeight;
        }

        public override void Undo() {
            var row = _target.TableData.Rows.ElementAt(_rowIndex);
            row.Height = _oldHeight;
        }
    }
}
