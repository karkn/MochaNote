/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Control.TabControlEx {
    public class CloseButtonPressedEventArgs: EventArgs {
        private TabPage _closed;

        public CloseButtonPressedEventArgs(TabPage closed) {
            _closed = closed;
        }
        public TabPage Closed {
            get { return _closed; }
        }
    }
}
