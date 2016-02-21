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
    public class RectanglePositionalComparer: IComparer<Rectangle> {
        // ========================================
        // method
        // ========================================
        public int Compare(Rectangle x, Rectangle y) {
            var xOrder = GetXOrder(x, y);
            var yOrder = GetYOrder(x, y);

            if (yOrder == -1) {
                return -1;
            } else if (yOrder == 1) {
                return 1;
            } else {
                if (xOrder == -1) {
                    return -1;
                } else if (xOrder == 1) {
                    return 1;
                } else {
                    /// どこか重なりがあった場合

                    var xDelta = x.Left - y.Left;
                    var yDelta = x.Top - y.Top;
                    if (xDelta < yDelta) {
                        return x.Top - y.Top;
                    } else {
                        return x.Left - y.Left;
                    }
                }
            }
        }

        
        /// <summary>
        /// 重なる個所があれば0，xが左なら-1，yが左なら1
        /// </summary>
        private int GetXOrder(Rectangle x, Rectangle y) {
            if (x.Left < y.Left) {
                if (x.Right < y.Left) {
                    return -1;
                } else {
                    return 0;
                }
            } else {
                if (y.Right < x.Left) {
                    return 1;
                } else {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 重なる個所があれば0，xが上なら-1，yが上なら1
        /// </summary>
        private int GetYOrder(Rectangle x, Rectangle y) {
            if (x.Top < y.Top) {
                if (x.Bottom < y.Top) {
                    return -1;
                } else {
                    return 0;
                }
            } else {
                if (y.Bottom < x.Top) {
                    return 1;
                } else {
                    return 0;
                }
            }
        }
    }
}
