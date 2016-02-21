/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Forms.KeyMap {
    public class KeyActionInfo<T> {
        // ========================================
        // field
        // ========================================
        private string _id;
        private string _description;
        private Action<T> _action;

        // ========================================
        // constructor
        // ========================================
        public KeyActionInfo(string id, string description, Action<T> action) {
            _id = id;
            _description = description;
            _action = action;
        }

        // ========================================
        // property
        // ========================================
        public string Id {
            get { return _id; }
        }
        public string Description {
            get { return _description; }
        }
        public Action<T> Action {
            get { return _action; }
        }
    }
}
