/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mkamo.Editor.Core {
    public interface IGridService {
        // ========================================
        // property
        // ========================================
        Size GridSize { get; }

        // ========================================
        // method
        // ========================================
        /// <summary>
        /// ptをグリッドに合わせた値を返す．
        /// </summary>
        Point GetAdjustedPoint(Point pt);
        int GetAdjustedX(int x);
        int GetAdjustedY(int y);

        /// <summary>
        /// adjustSizeはサイズも調整するかどうか。falseならばLocationだけ調整。
        /// </summary>
        Rectangle GetAdjustedRect(Rectangle rect, bool adjustSize);

        /// <summary>
        /// ptをグリッドに合わせたときの移動値を返す．
        /// </summary>
        Size GetAdjustedDiff(Point pt);
        int GetAdjustedDiffX(int x);
        int GetAdjustedDiffY(int y);

    }
}
