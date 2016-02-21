/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Internal.Core;
using Mkamo.Figure.Figures;
using Mkamo.Common.Collection;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using log4net;
using System.Reflection;

namespace Mkamo.Figure.Core {
    public static class FigureConsts {
        // ------------------------------
        // public
        // ------------------------------
        public static readonly IFigure NullFigure = new NullFigure();

        public static readonly IList<IFigure> EmptyFigureList = new EmptyList<IFigure>();
        public static readonly IDirtManager NullDirtManager = new NullDirtManager();

        public static readonly Color WindowTextColor = SystemColors.WindowText;
        public static readonly IBrushDescription WindowBrush = new SolidBrushDescription(SystemColors.Window);
        public static readonly IBrushDescription WindowTextBrush = new SolidBrushDescription(SystemColors.WindowText);

        //public static readonly Color HighlightBorder = SystemColors.Highlight;
        public static readonly Color HighlightColor = Color.FromArgb(51, 153, 255);
        public static readonly IBrushDescription HighlightBrush = new SolidBrushDescription(Color.FromArgb(32, HighlightColor));

        public static readonly int LineEndCharWidth = 8;
        public static readonly int AnchorCharWidth = 6;
    }
}
