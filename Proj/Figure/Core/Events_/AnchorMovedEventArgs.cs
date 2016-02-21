/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mkamo.Figure.Core;

namespace Mkamo.Figure.Core {
    public class AnchorMovedEventArgs: EventArgs {
        // ========================================
        // field
        // ========================================
        private ConnectionAnchorKind _kind;
        private Point _oldLocation;
        private Point _newLocation;

        // ========================================
        // constructor
        // ========================================
        public AnchorMovedEventArgs(ConnectionAnchorKind kind, Point oldLocation, Point newLocation) {
            _kind = kind;
            _oldLocation = oldLocation;
            _newLocation = newLocation;
        }

        // ========================================
        // property
        // ========================================
        public ConnectionAnchorKind Kind {
            get { return _kind; }
            set { _kind = value; }
        }

        public Point OldLocation {
            get { return _oldLocation; }
            set { _oldLocation = value; }
        }

        public Point NewLocation {
            get { return _newLocation; }
            set { _newLocation = value; }
        }

    }
}
