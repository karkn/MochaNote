/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.StyledText.Core;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.DataType;
using Mkamo.Common.Command;

namespace Mkamo.StyledText.Commands {
    public class SetParagraphPropertiesCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Paragraph _target;
        
        private Insets _padding;
        private int _lineSpace;
        private HorizontalAlignment _horizontalAlignment;
        private ParagraphKind _paragraphKind;
        private ListKind _listKind;
        private int _indentLevel;
        private ListStateKind _listState;

        private Insets _oldPadding;
        private int _oldLineSpace;
        private HorizontalAlignment _oldHorizontalAlignment;
        private ParagraphKind _oldParagraphKind;
        private ListKind _oldListKind;
        private int _oldIndentLevel;
        private ListStateKind _oldListState;

        // ========================================
        // constructor
        // ========================================
        public SetParagraphPropertiesCommand(
            Paragraph target,
            Insets padding,
            int lineSpace,
            HorizontalAlignment horizontalAlignment,
            ParagraphKind paragraphKind,
            ListKind listKind,
            int indentLevel,
            ListStateKind listState
        ) {
            _target = target;
            _padding = padding;
            _lineSpace = lineSpace;
            _horizontalAlignment = horizontalAlignment;
            _paragraphKind = paragraphKind;
            _listKind = listKind;
            _indentLevel = indentLevel;
            _listState = listState;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null; }
        }

        public override bool  CanUndo {
            get { return true; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _oldPadding = _target.Padding;
            _oldLineSpace = _target.LineSpace;
            _oldHorizontalAlignment = _target.HorizontalAlignment;
            _oldParagraphKind = _target.ParagraphKind;
            _oldListKind = _target.ListKind;
            _oldIndentLevel = _target.ListLevel;
            _oldListState = _target.ListState;

            if (_padding != _oldPadding) {
                _target.Padding = _padding;
            }
            if (_lineSpace != _oldLineSpace) {
                _target.LineSpace = _lineSpace;
            }
            if (_horizontalAlignment != _oldHorizontalAlignment) {
                _target.HorizontalAlignment = _horizontalAlignment;
            }
            if (_paragraphKind != _oldParagraphKind) {
                _target.ParagraphKind = _paragraphKind;
            }
            if (_listKind != _oldListKind) {
                _target.ListKind = _listKind;
            }
            if (_indentLevel != _oldIndentLevel) {
                _target.ListLevel = _indentLevel;
            }
            if (_listState != _oldListState) {
                _target.ListState = _listState;
            }
        }

        public override void Undo() {
            if (_oldPadding != _target.Padding) {
                _target.Padding = _oldPadding;
            }
            if (_oldLineSpace != _target.LineSpace) {
                _target.LineSpace = _oldLineSpace;
            }
            if (_oldHorizontalAlignment != _target.HorizontalAlignment) {
                _target.HorizontalAlignment = _oldHorizontalAlignment;
            }
            if (_oldParagraphKind != _target.ParagraphKind) {
                _target.ParagraphKind = _oldParagraphKind;
            }
            if (_oldListKind != _target.ListKind) {
                _target.ListKind = _oldListKind;
            }
            if (_oldIndentLevel != _target.ListLevel) {
                _target.ListLevel = _oldIndentLevel;
            }
            if (_oldListState != _target.ListState) {
                _target.ListState = _oldListState;
            }
        }

    }
}
