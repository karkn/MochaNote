/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Common.Visitor;

namespace Mkamo.Figure.Utils {
    public class FigureStructurePrintVisitor: IVisitor<IFigure> {
        public bool Visit(IFigure fig) {
            var p = fig.Parent;
            var sb = new StringBuilder();
            while (p != null) {
                sb.Append("  ");
                p = p.Parent;
            }
            //Console.WriteLine(
            //    sb.ToString() + "+ " + fig
            //);
            //Console.WriteLine(
            //    sb.ToString() + "    " + fig.Bounds
            //);
            //Console.WriteLine(
            //    sb.ToString() + "    " + (fig.Layout == null ? "no layout" : fig.Layout.ToString())
            //);
            return false;
        }

        public void EndVisit(IFigure elem) {

        }
    }
}
