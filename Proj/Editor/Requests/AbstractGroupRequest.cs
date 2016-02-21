/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;

namespace Mkamo.Editor.Requests {
    /// <summary>
    /// 複数のeditorに対して一つのcommandを実行しなければならないrequest
    /// </summary>
    public abstract class AbstractGroupRequest: AbstractRequest {
        // ========================================
        // field
        // ========================================
        private IEnumerable<IEditor> _targetEditors;

        // ========================================
        // constructor
        // ========================================
        public AbstractGroupRequest(IEnumerable<IEditor> targetEditors) {
            _targetEditors = targetEditors;
        }

        // ========================================
        // property
        // ========================================
        // === GroupRequest ==========
        public IEnumerable<IEditor> TargetEditors {
            get { return _targetEditors; }
        }
    }
}
