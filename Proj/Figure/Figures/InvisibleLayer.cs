/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mkamo.Figure.Figures {
    /// <summary>
    /// 自身の矩形は持つ．
    /// Containsの結果は配下のFigureだけを対象とする．
    /// 自身と子のFigureは描画されない．
    /// </summary>
    public class InvisibleLayer: Layer {
        // ========================================
        // constructor
        // ========================================
        public InvisibleLayer() {
        }

        // ========================================
        // method
        // ========================================
        protected override void PaintChildren(Graphics g) {
            /// 子を表示しない
        }
    }
}
