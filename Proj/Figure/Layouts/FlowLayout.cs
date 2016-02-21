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
using Mkamo.Common.DataType;
using Mkamo.Common.Externalize;

namespace Mkamo.Figure.Layouts {
    [Externalizable]
    public class FlowLayout: AbstractLayout {
        // ========================================
        // field
        // ========================================
        private Insets _padding;
        private HorizontalAlignment _hAlign;
        private int _hGap;
        private int _vGap;

        private bool _adjustChildrenSize;
        private Size _emptyPreferredSize;

        // ========================================
        // constructor
        // ========================================
        public FlowLayout(Insets padding, HorizontalAlignment hAlign, int hGap, int vGap) {
            _padding = padding;
            _hAlign = hAlign;
            _hGap = hGap;
            _vGap = vGap;

            _adjustChildrenSize = false;
            _emptyPreferredSize = Size.Empty;
        }

        public FlowLayout(Insets padding): this(padding, HorizontalAlignment.Left, 2, 2) {

        }

        public FlowLayout(): this(new Insets(2), HorizontalAlignment.Left, 2, 2) {

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
        public HorizontalAlignment HorizontalAlignment {
            get { return _hAlign; }
            set { _hAlign = value; }
        }

        [External]
        public int HGap{
            get { return _hGap; }
            set { _hGap = value; }
        }

        [External]
        public int VGap {
            get { return _vGap; }
            set { _vGap = value; }
        }

        [External]
        public Size EmptyPreferredSize {
            get { return _emptyPreferredSize; }
            set { _emptyPreferredSize = value; }
        }

        // ========================================
        // method
        // ========================================
        public override Rectangle GetClientRect(IFigure figure) {
            return Padding.GetClientArea(figure.Bounds);
        }

        public override void Arrange(IFigure parent) {
            var clientArea = _padding.GetClientArea(parent.Bounds);
            var constraint = new SizeConstraint(clientArea.Size);

            /// childrenを行に分割，各行とそのwidth，heightを保持
            var lines = new List<Tuple<List<IFigure>, int, int>>();
            {
                List<IFigure> line = null;
                var cWidth = 0;
                var cHeight = 0;
                foreach (var child in parent.Children) {
                    if (child.IsVisible) {
                        var cSize = Size.Empty;
                        if (_adjustChildrenSize) {
                            child.Measure(constraint);
                            cSize = child.PreferredSize;
                        } else {
                            cSize = child.Size;
                        }

                        if (cWidth + cSize.Width + _hGap >= clientArea.Width) {
                            lines.Add(Tuple.Create(line, cWidth, cHeight));
                            line = null;
                            cWidth = 0;
                            cHeight = 0;
                        }

                        if (line == null) {
                            line = new List<IFigure>();
                            line.Add(child);
                            cWidth = cSize.Width + _padding.Width;
                            cHeight = cSize.Height;
                        } else {
                            line.Add(child);
                            cWidth += cSize.Width + _hGap;
                            cHeight = Math.Max(cHeight, cSize.Height);
                        }

                    }
                }
                if (line != null) {
                    lines.Add(Tuple.Create(line, cWidth, cHeight));
                }
            }

            var cLeft = clientArea.Left + _padding.Left;
            var cTop = clientArea.Top + _padding.Top;
            foreach (var line in lines) {
                var figs = line.Item1;
                var width = line.Item2;
                var height = line.Item3;
                foreach (var fig in figs) {
                    fig.Location = new Point(cLeft, cTop);
                    if (_adjustChildrenSize) {
                        var pSize = fig.PreferredSize;
                        fig.Size = new Size(Math.Min(pSize.Width, clientArea.Width), pSize.Height);
                    }
                    cLeft += fig.Width + _hGap;
                }
                cTop += height + _vGap;
            }

        }

        public override Size Measure(IFigure parent, SizeConstraint constraint) {
            if (!parent.Children.Any()) {
                return _emptyPreferredSize + _padding.Size;
            }

            var clientArea = _padding.GetClientArea(parent.Bounds);

            var width = clientArea.Width;
            if (_adjustChildrenSize) {
                /// clientArea.Widthか最も大きいpreferred size + padding width * 2の大きい方
                width = Math.Max(width, parent.Children.Max(fig => fig.Width + _padding.Width * 2));
            }

            var height = 0;
            {
                var inLine = false;
                var cWidth = 0;
                var cHeight = 0;
                foreach (var child in parent.Children) {
                    if (child.IsVisible) {
                        var cSize = Size.Empty;
                        if (_adjustChildrenSize) {
                            child.Measure(constraint);
                            cSize = child.PreferredSize;
                        } else {
                            cSize = child.Size;
                        }

                        if (cWidth + cSize.Width + _hGap >= width) {
                            height += cHeight;
                            inLine = false;
                            cWidth = 0;
                            cHeight = 0;
                        }

                        if (!inLine) {
                            cWidth = cSize.Width + _padding.Width;
                            cHeight = cSize.Height;
                        } else {
                            cWidth += cSize.Width + _hGap;
                            cHeight = Math.Max(cHeight, cSize.Height);
                        }

                    }
                }
                if (inLine) {
                    height += cHeight;
                }
            }

            return constraint.MeasureConstrainedSize(new Size(width, height));
        }

    }
}
