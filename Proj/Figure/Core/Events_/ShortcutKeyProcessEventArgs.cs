/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Figure.Core {
    public class ShortcutKeyProcessEventArgs: EventArgs {
        private bool _handled = false;
        private Keys _keyData;

        public ShortcutKeyProcessEventArgs(Keys keyData) {
            _keyData = keyData;
        }

        public bool Handled {
            get { return _handled; }
            set { _handled = value; }
        }

        public Keys KeyData {
            get { return _keyData; }
        }

    }
}
