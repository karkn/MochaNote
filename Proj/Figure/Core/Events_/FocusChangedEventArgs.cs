/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Figure.Core {
    public class FocusChangedEventArgs: EventArgs {
        private bool _isFocused;

        public FocusChangedEventArgs(bool isFocused) {
            _isFocused = isFocused;
        }

        public bool IsFocused {
            get { return _isFocused; }
        }
    }

}
