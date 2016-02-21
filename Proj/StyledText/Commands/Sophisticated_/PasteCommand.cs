/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;
using Mkamo.Common.DataType;
using System.Windows.Forms;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    using StyledText = Mkamo.StyledText.Core.StyledText;
    using Mkamo.StyledText.Util;

    public class PasteCommand: AbstractCommand, IRangeCommand {
        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private int _index;

        private InsertBlocksAndInlinesCommand _insertCommand;

        // ========================================
        // constructor
        // ========================================
        public PasteCommand(StyledText target, int index) {
            _target = target;
            _index = index;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && ClipboardUtil.ContainsBlocksAndInlines(); }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public Range ExecutedRange {
            get { return _insertCommand.ExecutedRange; }
        }

        public Range UndoneRange {
            get { return _insertCommand.UndoneRange; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var data = Clipboard.GetDataObject();
            if (!ClipboardUtil.ContainsBlocksAndInlines()) {
                return;
            }

            var flows = data.GetData(StyledTextConsts.BlocksAndInlinesFormat.Name) as List<Flow>;
            _insertCommand = new InsertBlocksAndInlinesCommand(_target, _index, flows);
            _insertCommand.Execute();
        }

        public override void Undo() {
            if (_insertCommand != null) {
                _insertCommand.Undo();
            }
        }

        public override void Redo() {
            if (_insertCommand != null) {
                _insertCommand.Redo();
            }
        }
    }
}
