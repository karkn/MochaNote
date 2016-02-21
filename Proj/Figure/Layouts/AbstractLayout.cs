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
using Mkamo.Common.Externalize;

namespace Mkamo.Figure.Layouts {
    [Externalizable]
    public abstract class AbstractLayout: ILayout {
        // ========================================
        // field
        // ========================================
        private IFigure _owner;

        // ========================================
        // constructor
        // ========================================
        protected AbstractLayout() {
        }


        // ========================================
        // property
        // ========================================
        public virtual IFigure Owner {
            get { return _owner; }
            set { _owner = value; }
        }

        protected virtual IDictionary<IFigure, object> _Constraints {
            get { return _owner == null? null: _owner.LayoutConstraints; }
        }

        // ========================================
        // method
        // ========================================
        public virtual object Clone() {
            var ret = MemberwiseClone() as AbstractLayout;
            return ret;
        }

        public abstract void Arrange(IFigure parent);

        /// <summary>
        /// デフォルトではfigureのBoundsをそのまま返す．
        /// </summary>
        public virtual Rectangle GetClientRect(IFigure parent) {
            return parent.Bounds;
        }

        /// <summary>
        /// デフォルトではfigureのサイズをconstraintに適用させただけのサイズを返す．
        /// </summary>
        public virtual Size Measure(IFigure parent, SizeConstraint constraint) {
            return constraint.MeasureConstrainedSize(parent.Size);
        }
    }
}
