/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mkamo.Figure.Core;
using Mkamo.Common.Externalize;

namespace Mkamo.Figure.Figures {
    /// <summary>
    /// 自身の矩形は持つが，Containsの結果は配下のFigureだけを対象とするLayer．
    /// </summary>
    public class Layer: SolidLayer {
        // ========================================
        // method
        // ========================================
        // === IFiguer ==========
        public override bool ContainsPoint(Point pt) {
            if (!base.ContainsPoint(pt)) {
                return false;
            }
            foreach (var child in Children) {
                if (child.ContainsPoint(pt)) {
                    return true;
                }
            }
            return false;
        }

        public override bool IntersectsWith(Rectangle rect) {
            if (!base.IntersectsWith(rect)) {
                return false;
            }
            foreach (var child in Children) {
                if (child.IntersectsWith(rect)) {
                    return true;
                }
            }
            return false;
        }
    }
}
