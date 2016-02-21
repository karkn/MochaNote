/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using System.Drawing;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Externalize;

namespace Mkamo.Figure.Layouts {
    [Externalizable]
    public class ListLayout: AbstractLayout {
        // ========================================
        // field
        // ========================================
        private Insets _padding;
        private int _itemSpace;
        private bool _extendLast;
        private bool _adjustItemWidth;
        private Size _emptyPreferredSize;

        // ========================================
        // constructor
        // ========================================
        public ListLayout(Insets padding, bool extendLast) {
            _padding = padding;
            _extendLast = extendLast;
            _itemSpace = 0;
            _adjustItemWidth = true;
        }

        public ListLayout(Insets padding): this(padding, false) {

        }

        public ListLayout(): this(new Insets(2), false) {

        }

        // ========================================
        // property
        // ========================================
        [External]
        public Insets Padding {
            get { return _padding; }
            set { _padding = value; }
        }

        [External]
        public int ItemSpace {
            get { return _itemSpace; }
            set { _itemSpace = value; }
        }

        [External]
        public bool ExtendLast {
            get { return _extendLast; }
            set { _extendLast = value; }
        }

        [External]
        public bool AdjustItemWidth {
            get { return _adjustItemWidth; }
            set { _adjustItemWidth = value; }
        }

        [External]
        public Size EmptyPreferredSize {
            get { return _emptyPreferredSize; }
            set { _emptyPreferredSize = value; }
        }

        // ========================================
        // method
        // ========================================
        public override object Clone() {
            var ret = base.Clone() as ListLayout;
            ret._padding = _padding;
            ret._itemSpace = _itemSpace;
            ret._extendLast = _extendLast;
            ret._adjustItemWidth = _adjustItemWidth;
            ret._emptyPreferredSize = _emptyPreferredSize;
            return ret;
        }

        public override Rectangle GetClientRect(IFigure figure) {
            return Padding.GetClientArea(figure.Bounds);
        }

        public override void Arrange(IFigure parent) {
            var clientArea = _padding.GetClientArea(parent.Bounds);
            var constraint = new SizeConstraint(clientArea.Size);

            var itemTop = parent.Top + _padding.Top;
            foreach (var child in parent.Children) {
                if (child.IsVisible) {
                    var csize = child.PreferredSize;
                    child.Location = new Point(parent.Left + _padding.Left, itemTop);
                    if (_adjustItemWidth) {
                        child.Width = clientArea.Width;
                    }
                    child.Height = csize.Height;
                    itemTop += child.Height + ItemSpace;
                }
            }

            if (_extendLast && parent.Children.Count > 0) {
                /// 最後のchildを残りいっぱいの大きさにする
                var lastItem = parent.Children.Last();
                lastItem.Height = clientArea.Bottom - lastItem.Top;
            }
        }

        public override Size Measure(IFigure parent, SizeConstraint constraint) {
            if (!parent.Children.Any()) {
                return _emptyPreferredSize + _padding.Size;
            }

            var ret = new Size(0, _padding.Top);
            var clientAreaConstraint = constraint.Deflate(_padding.Size);

            foreach (var child in parent.Children) {
                if (child.IsVisible) {
                    child.Measure(clientAreaConstraint);
                    var prefSize = child.PreferredSize;
                    ret.Width = Math.Max(prefSize.Width, ret.Width);
                    ret.Height += prefSize.Height + _itemSpace;
                }
            }

            ret.Width += _padding.Width;
            ret.Height += _padding.Bottom - _itemSpace;
            return constraint.MeasureConstrainedSize(ret);
        }

    }
}
