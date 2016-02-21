/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Externalize;
using System.Drawing;
using Mkamo.Figure.Core;

namespace Mkamo.Figure.Figures {
    /// <summary>
    /// 自身も検索の対象となるレイヤ．
    /// </summary>
    public class SolidLayer: AbstractBoundingFigure, ILayer {
        // ========================================
        // property
        // ========================================
        // ------------------------------
        // protected
        // ------------------------------
        protected override bool _ChildrenFollowOnBoundsChanged {
            get { return false; }
        }

        // ========================================
        // method
        // ========================================
        protected override void PaintSelf(Graphics g) {
            
        }

        protected override Size MeasureSelf(SizeConstraint constraint) {
            return Size.Empty;
        }

        public override void MakeTransparent(float ratio) {
            
        }
    }
}
