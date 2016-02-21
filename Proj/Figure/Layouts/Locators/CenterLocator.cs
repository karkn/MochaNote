/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using System.Drawing;

namespace Mkamo.Figure.Layouts.Locators {
    [Serializable]
    public class CenterLocator: ILocator {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public void Relocate(IFigure figure, IFigure parent) {
            var pCenter = parent.Center;
            figure.Location = new Point(pCenter.X - figure.Width / 2, pCenter.Y - figure.Height / 2);
        }
    }
}
