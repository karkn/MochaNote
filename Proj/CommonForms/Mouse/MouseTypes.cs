/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Common.Forms.Mouse {
    public class CancelMouseEventArgs: MouseEventArgs {
        private bool _isCanceled = false;

        public CancelMouseEventArgs(
	        MouseButtons button,
            int clicks,
	        int x,
	        int y,
	        int delta
        ): base(button, clicks, x, y, delta) {

        }

        public CancelMouseEventArgs(MouseEventArgs e):
            base(e.Button, e.Clicks, e.X, e.Y, e.Delta) {

        }

        public bool IsCanceled {
            get { return _isCanceled; }
            set { _isCanceled = value; }
        }
    }
}
