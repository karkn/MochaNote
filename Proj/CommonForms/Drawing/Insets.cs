/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mkamo.Common.Forms.Drawing {
    [Serializable]
    public struct Insets: IEquatable<Insets> {
        // ========================================
        // static field
        // ========================================
        public static readonly Insets Empty = new Insets();

        // ========================================
        // static method
        // ========================================
        public static bool operator ==(Insets a, Insets b) {
            return a.Equals(b);
        }

        public static bool operator !=(Insets a, Insets b) {
            return !a.Equals(b);
        }

        public static Insets operator +(Insets i1, Insets i2) {
            return new Insets(
                i1.Left + i2.Left,
                i1.Top + i2.Top,
                i1.Right + i2.Right,
                i1.Bottom + i2.Bottom
            );
        }

        public static Insets operator -(Insets i1, Insets i2) {
            return new Insets(
                i1.Left - i2.Left,
                i1.Top - i2.Top,
                i1.Right - i2.Right,
                i1.Bottom - i2.Bottom
            );
        }

        // ========================================
        // field
        // ========================================
        private int _left;
        private int _top;
        private int _right;
        private int _bottom;

        // ========================================
        // constructor
        // ========================================
        public Insets(int i): this(i, i, i, i) {
            
        }

        public Insets(int left, int top, int right, int bottom) {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }

        // ========================================
        // property
        // ========================================
        public int Left {
            get { return _left; }
            set { _left = value; }
        }

        public int Top {
            get { return _top; }
            set { _top = value; }
        }

        public int Right {
            get { return _right; }
            set { _right = value; }
        }

        public int Bottom {
            get { return _bottom; }
            set { _bottom = value; }
        }

        public int Width {
            get { return _left + _right; }
        }

        public int Height {
            get { return _top + _bottom; }
        }

        public Size Size {
            get { return new Size(Width, Height); }
        }

        // ========================================
        // method
        // ========================================
        // === object ==========
        public override bool Equals(object obj) {
            if (obj is Insets) {
                return Equals((Insets) obj);
            } else {
                return false;
            }
        }

        public override int GetHashCode() {
            return _left ^ _top ^ _right ^ _bottom;
        }

        public bool Equals(Insets other) {
            return
                Left == other.Left && 
                Top == other.Top && 
                Right == other.Right && 
                Bottom == other.Bottom;
        }

        // === Insets ==========
        /// <summary>
        /// rectからこのInsets分の領域を除いたRectangleを返す．
        /// </summary>
        public Rectangle GetClientArea(Rectangle bounds) {
            return new Rectangle(
                bounds.Left + Left,
                bounds.Top + Top,
                bounds.Width - Width,
                bounds.Height - Height
            );
        }

        /// <summary>
        /// clientAreaにPaddingを付加したBoundsを返す．
        /// </summary>
        public Rectangle GetBounds(Rectangle clientArea) {
            return new Rectangle(
                clientArea.Left - Left,
                clientArea.Top - Top,
                clientArea.Width + Width,
                clientArea.Height + Height
            );
        }

        public Insets GetLeftChanged(int left) {
            return new Insets(
                left,
                Top,
                Right,
                Bottom
            );
        }

        public Insets GetTopChanged(int top) {
            return new Insets(
                Left,
                top,
                Right,
                Bottom
            );
        }

        public Insets GetRightChanged(int right) {
            return new Insets(
                Left,
                Top,
                right,
                Bottom
            );
        }

        public Insets GetBottomChanged(int bottom) {
            return new Insets(
                Left,
                Top,
                Right,
                bottom
            );
        }

    }
}
