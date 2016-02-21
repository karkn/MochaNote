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
    /// 矩形で囲まれた図形．
    /// </summary>
    public abstract class AbstractBoundingFigure: AbstractFigure {
        // ========================================
        // field
        // ========================================
        private Rectangle _bounds;

        // ========================================
        // constructor
        // ========================================
        protected AbstractBoundingFigure(): base() {
            _bounds = new Rectangle(0, 0, 4, 4);
        }

        // ========================================
        // property
        // ========================================
        // === IFiguer ==========
        public override Rectangle Bounds {
            get { return _Bounds; }
            set { _Bounds = value; }
        }

        protected virtual Rectangle _Bounds {
            get { return _bounds; }
            set {
                if (value == _bounds) {
                    return;
                }
                var old = _bounds;
                _bounds = value;
                OnBoundsChanged(old, _bounds, null);
            }
        }

        // ========================================
        // method
        // ========================================
        // === IPersistable ==========
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);
            memento.WriteSerializable("Bounds", _bounds);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);
            _bounds = (Rectangle) memento.ReadSerializable("Bounds");
        }

        
        // === IFiguer ==========
        public override bool ContainsPoint(Point pt) {
            return Bounds.Contains(pt);
        }

        public override bool IntersectsWith(Rectangle rect) {
            return Bounds.IntersectsWith(rect);
        }

        public override void Move(Size delta, IEnumerable<IFigure> movingFigures) {
            if (delta.IsEmpty) {
                return;
            }

            var old = _bounds;
            _bounds = new Rectangle(Bounds.Location + delta, _bounds.Size);
            OnBoundsChanged(old, _bounds, movingFigures);
        }

    }
}
