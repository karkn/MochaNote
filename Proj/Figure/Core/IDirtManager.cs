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
    /// <summary>
    /// 領域のdirty状態を管理する．
    /// </summary>
    public interface IDirtManager {
        // ========================================
        // property
        // ========================================
        /// <summary>
        /// BeginDirty()が呼ばれ，かつ，その後EndDirtyがまだ呼ばれていないかどうか．
        /// </summary>
        bool IsInDirtying { get; }

        /// <summary>
        /// Repairするかどうか．
        /// </summary>
        bool IsEnabled { get; set; }

        // ========================================
        // method
        // ========================================
        /// <summary>
        /// EndDirty()が呼ばれるまでDirtyPaint()・DirtyLayout()によるdirty状態を蓄積する．
        /// </summary>
        DirtyingContext BeginDirty();

        /// <summary>
        /// BegeinDirty()が呼ばれてから蓄積されたdirty状態を解消するように更新する．
        /// </summary>
        void EndDirty();

        /// <summary>
        /// rectの領域の表示をdirtyにする．
        /// </summary>
        void DirtyPaint(Rectangle rect);

        /// <summary>
        /// figのレイアウトをdirtyにする．
        /// </summary>
        void DirtyLayout(IFigure fig);
    }
}
