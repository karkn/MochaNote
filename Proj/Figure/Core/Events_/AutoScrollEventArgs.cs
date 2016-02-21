/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Internal.Core;

namespace Mkamo.Figure.Core {
    public class AutoScrollEventArgs: EventArgs {
        private ScrollDirection _scrollDirection;
        private int _scrollInterval;

        public AutoScrollEventArgs(ScrollDirection direction, int scrollInterval) {
            _scrollDirection = direction;
            _scrollInterval = scrollInterval;
        }

        public ScrollDirection ScrollDirection {
            get { return _scrollDirection; }
        }
        public int ScrollInterval {
            get { return _scrollInterval; }
        }
    }

    [Serializable]
    public enum ScrollDirection {
        None,
        Up,
        Down,
        Left,
        Right,
    }
}
