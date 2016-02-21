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
    // todo: AbstractFigureを継承するのではなく，IFigureだけを実装する
    //       ChildrenやParentも自分でnull実装する
    public class NullFigure: AbstractFigure {
        // ========================================
        // property
        // ========================================
        public override Rectangle Bounds {
            get { return Rectangle.Empty; }
            set { }
        }

        // ========================================
        // method
        // ========================================
        // === IFigure ==========
        public override bool ContainsPoint(Point pt) {
            return false;
        }

        public override bool IntersectsWith(Rectangle rect) {
            return false;
        }

        public override void Move(Size delta, IEnumerable<IFigure> movingFigures) {
            
        }

        // ------------------------------
        // protected
        // ------------------------------
        // === AbstractFigure ==========
        protected override void PaintSelf(Graphics g) {
            
        }

        protected override Size MeasureSelf(SizeConstraint constraint) {
            return Size.Empty;
        }

        public override void MakeTransparent(float ratio) {
            
        }

    }
}
