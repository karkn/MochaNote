/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using System.Drawing;

namespace Mkamo.Editor.Internal.Core {
    internal class DefaultGridService: IGridService {
        // ========================================
        // field
        // ========================================
        private Size _gridSize;
        private Size _gridHalfSize;

        // ========================================
        // constructor
        // ========================================
        public DefaultGridService(Size gridSize) {
            GridSize = gridSize;
        }

        public DefaultGridService(): this(new Size(8, 8)) {
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public Size GridSize {
            get { return _gridSize; }
            set {
                _gridSize = value;
                _gridHalfSize = new Size(value.Width / 2, value.Height / 2);
            }
        }

        public Point GetAdjustedPoint(Point pt) {
            return new Point(GetAdjustedX(pt.X), GetAdjustedY(pt.Y));
        }

        public int GetAdjustedX(int x) {
            return ((x + _gridHalfSize.Width) / _gridSize.Width) * _gridSize.Width;
            //return ((int) Math.Round((double) x / _gridSize.Width)) * _gridSize.Width;
        }

        public int GetAdjustedY(int y) {
            return ((y + _gridHalfSize.Height) / _gridSize.Height) * _gridSize.Height;
            //return ((int) Math.Round((double) y / _gridSize.Height)) * _gridSize.Height;
        }

        public Size GetAdjustedDiff(Point pt) {
            return new Size(GetAdjustedDiffX(pt.X), GetAdjustedDiffY(pt.Y));
        }

        public int GetAdjustedDiffX(int x) {
            return GetAdjustedX(x) - x;
        }

        public int GetAdjustedDiffY(int y) {
            return GetAdjustedY(y) - y;
        }

        public Rectangle GetAdjustedRect(Rectangle rect, bool adjustSize) {
            var topLeft = GetAdjustedPoint(rect.Location);
            if (adjustSize) {
                var bottomRight = GetAdjustedPoint(rect.Location + rect.Size);
                return new Rectangle(topLeft, (Size) bottomRight - (Size) topLeft);
            } else {
                return new Rectangle(topLeft, rect.Size);
            }
        }
    }
}
