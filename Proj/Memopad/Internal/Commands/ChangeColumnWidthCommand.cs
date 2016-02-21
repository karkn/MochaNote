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
    internal class ChangeColumnWidthCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private TableFigure _target;
        private int _columnIndex;
        private int _newWidth;

        private int _oldWidth;

        // ========================================
        // constructor
        // ========================================
        public ChangeColumnWidthCommand(TableFigure target, int columnIndex, int newWidth) {
            Contract.Requires(target != null);
            Contract.Requires(columnIndex > -1);

            _target = target;
            _columnIndex = columnIndex;
            _newWidth = newWidth;
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
            var col = _target.TableData.Columns.ElementAt(_columnIndex);
            _oldWidth = col.Width;
            col.Width = _newWidth;
        }

        public override void Undo() {
            var col = _target.TableData.Columns.ElementAt(_columnIndex);
            col.Width = _oldWidth;
        }
    }
}
