/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Structure;
using Mkamo.Figure.Core;
using System.Drawing;
using Mkamo.Common.Visitor;
using Mkamo.Common.Collection;
using Mkamo.Common.Event;
using Mkamo.Common.Externalize;

namespace Mkamo.Figure.Figures {
    /// <summary>
    /// Figureをグループ化する．
    /// 自身の矩形は子のFigureで決まり，ContainsもPointが子に含まれるかどうかを返す．
    /// </summary>
    public class FigureGroup: AbstractWrappingFigure {
        // ========================================
        // constructor
        // ========================================
        public FigureGroup() {

        }

        // ========================================
        // method
        // ========================================
        // === IFigure ==========
        protected override void PaintSelf(Graphics g) {
            
        }

        public override void MakeTransparent(float ratio) {
            
        }

        //// FigureGroupは子より自分を優先して結果として返す
        //public override IFigure FindFigure(Predicate<IFigure> finder) {
        //    IFigure found = null;
        //    if (finder(this)) {
        //        return this;
        //    }
        //    for (int i = Children.Count - 1; i >= 0; --i) {
        //        found = Children[i].FindFigure(finder);
        //        if (found != null) {
        //            return found;
        //        }
        //    }
        //    return null;
        //}

        //// FigureGroupは子より自分を優先して結果として返す
        //public override IList<IFigure> FindFigures(Predicate<IFigure> finder) {
        //    List<IFigure> ret = new List<IFigure>();

        //    Accept(
        //        fig => {
        //            if (finder(fig)) {
        //                ret.Add(fig);
        //            }
        //            return false;
        //        },
        //        null,
        //        ChildrenVisitOrder.NegativeOrder
        //    );

        //    return ret;
        //}

    }
}
