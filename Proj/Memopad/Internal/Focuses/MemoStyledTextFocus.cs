/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Focuses;
using Mkamo.Common.Core;
using Mkamo.Editor.Core;

namespace Mkamo.Memopad.Internal.Focuses {
    internal class MemoStyledTextFocus: StyledTextFocus {
        // ========================================
        // field
        // ========================================
        private IEditor _host;

        private Lazy<MemoStyledTextFocusContextMenuProvider> _menuProvider;

        // ========================================
        // constructor
        // ========================================
        public MemoStyledTextFocus(IEditor host, bool isEmacsEdit, bool canSplit): base(isEmacsEdit) {
            _host = host;
            _menuProvider = new Lazy<MemoStyledTextFocusContextMenuProvider>(
                () => new MemoStyledTextFocusContextMenuProvider(this, host, canSplit)
            );
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override IContextMenuProvider GetContextMenuProvider() {
            return _menuProvider.Value;
        }
    }
}
