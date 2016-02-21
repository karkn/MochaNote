/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mkamo.Figure.Utils {
    public static class EdgeUtil {
        // ========================================
        // static field
        // ========================================
        private const int DefaultLoopLen = 32;

        // ========================================
        // static method
        // ========================================
        /// <summary>
        /// boundsに対してループ接続する形になる点を返す．
        /// </summary>
        public static Point[] GetLoopPoints(Rectangle bounds, int loopLen) {
            var ret = new Point[5];

            /// 上の方に領域がなければ下の方につける
            var addToBottom = bounds.Top - loopLen < 0;

            if (addToBottom) {
                var x = bounds.Right - bounds.Width / 3 - 1;
                var y = bounds.Bottom - bounds.Height / 3 - 1;

                ret[0] = new Point(bounds.Right - 1, y);
                ret[1] = new Point(bounds.Right + loopLen - 1, y);
                ret[2] = new Point(bounds.Right + loopLen - 1, bounds.Bottom + loopLen - 1);
                ret[3] = new Point(x, bounds.Bottom + loopLen - 1);
                ret[4] = new Point(x, bounds.Bottom - 1);
            
            } else {
                var x = bounds.Right - bounds.Width / 3 - 1;
                var y = bounds.Top + bounds.Height / 3;

                ret[0] = new Point(bounds.Right - 1, y);
                ret[1] = new Point(bounds.Right + loopLen - 1, y);
                ret[2] = new Point(bounds.Right + loopLen - 1, bounds.Top - loopLen);
                ret[3] = new Point(x, bounds.Top - loopLen);
                ret[4] = new Point(x, bounds.Top);
            }

            return ret;
        }

        public static Point[] GetLoopPoints(Rectangle bounds) {
            return GetLoopPoints(bounds, DefaultLoopLen);
        }
    }
}
