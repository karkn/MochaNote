/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Core;

namespace Mkamo.Memopad.Internal.Utils {
    internal static class CaretUtil {
        // ========================================
        // method
        // ========================================
        public static Point GetExpectedCaretPosition(Point memoTextPos, IGridService gridService) {
            var pt = gridService.GetAdjustedPoint(new Point(memoTextPos.X, memoTextPos.Y));
            return pt + MemopadConsts.MemoTextFirstCharDelta;
        }

        public static Point GetExpectedMemoTextPosition(Point caretPos) {
            return caretPos - MemopadConsts.MemoTextFirstCharDelta;// - new Size(0, MemopadConsts.BlockLineSpace);
        }
    }
}
