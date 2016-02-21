/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mkamo.Figure.Core {
    public class BoundsChangedEventArgs: EventArgs {
        // ========================================
        // field
        // ========================================
        private Rectangle _oldBounds;
        private Rectangle _newBounds;
        private IEnumerable<IFigure> _movingFigures;

        // ========================================
        // constructor
        // ========================================
        public BoundsChangedEventArgs(Rectangle oldBounds, Rectangle newBounds, IEnumerable<IFigure> movingFigures) {
            _oldBounds = oldBounds;
            _newBounds = newBounds;
            _movingFigures = movingFigures;
        }

        // ========================================
        // property
        // ========================================
        public Rectangle OldBounds {
            get { return _oldBounds; }
        }

        public Rectangle NewBounds {
            get { return _newBounds; }
        }

        public IEnumerable<IFigure> MovingFigures {
            get { return _movingFigures; }
        }


        public bool IsMove {
            get { return _newBounds.Location != _oldBounds.Location; }
        }

        public bool IsResize {
            get { return _newBounds.Size != _oldBounds.Size; }
        }

        public bool IsBulk {
            get { return _movingFigures != null && _movingFigures.Any(); }
        }

        public Size LocationDelta {
            get { return (Size) _newBounds.Location - (Size) _oldBounds.Location; }
        }

        public Size SizeDelta {
            get { return _newBounds.Size - _oldBounds.Size; }
        }
    }
}
