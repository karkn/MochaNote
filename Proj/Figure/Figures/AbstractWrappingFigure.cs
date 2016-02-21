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
using Mkamo.Common.Event;
using System.Runtime.Serialization;
using Mkamo.Common.Externalize;

namespace Mkamo.Figure.Figures {
    /// <summary>
    /// 自身では矩形を持たず，他の属性から矩形が導出される図形．
    /// figureをグループ化したfigure groupや点の集合から矩形が決まるedgeなど．
    /// </summary>
    public abstract class AbstractWrappingFigure: AbstractFigure {
        // ========================================
        // field
        // ========================================
        private Rectangle _boundsCache;

        private bool _isInMoving;

        // ========================================
        // constructor
        // ========================================
        public AbstractWrappingFigure() {
            _isInMoving = false;
        }

        // ========================================
        // property
        // ========================================
        // === ILocatable ==========
        public override Rectangle Bounds {
            get { return _boundsCache; }
            set {
                // todo: resizeもやる
                Size locDelta = (Size) value.Location - (Size) Bounds.Location;
                Move(locDelta);
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        // === AbstractFigure ==========
        protected override bool _ChildrenFollowOnBoundsChanged {
            get { return false; }
        }

        // === WrappingFigure ==========
        protected virtual Rectangle _BoundsCache {
            get { return _boundsCache; }
            set { _boundsCache = value; }
        }

        // ========================================
        // method
        // ========================================
        // === IPersistable ==========
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);
            memento.WriteSerializable("BoundsCache", _boundsCache);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);
            _boundsCache = (Rectangle) memento.ReadSerializable("BoundsCache");
        }

        
        // === IFigure ==========
        public override bool ContainsPoint(Point pt) {
            foreach (var child in Children) {
                if (child.ContainsPoint(pt) && child.IsVisible) {
                    return true;
                }
            }
            return false;
        }

        public override bool IntersectsWith(Rectangle rect) {
            foreach (var child in Children) {
                if (child.IntersectsWith(rect) && child.IsVisible) {
                    return true;
                }
            }
            return false;
        }

        public override void Move(Size delta, IEnumerable<IFigure> movingFigures) {
            _isInMoving = true;
            try {
                foreach (var child in Children) {
                    child.Move(delta, movingFigures);
                }
            } finally {
                _isInMoving = false;
                DirtyBoundsCache(movingFigures);
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        // === AbstractFigure ==========
        protected override void OnChildrenChanged(DetailedPropertyChangedEventArgs e) {
            switch (e.Kind) {
                case PropertyChangeKind.Add: {
                    var child = e.NewValue as IFigure;
                    child.BoundsChanged += HandleChildrenBoundsChanged;
                    break;
                }
                case PropertyChangeKind.Remove: {
                    var child = e.OldValue as IFigure;
                    child.BoundsChanged -= HandleChildrenBoundsChanged;
                    break;
                }
                case PropertyChangeKind.Clear: {
                    var children = e.OldValue as IFigure[];
                    foreach (var child in children) {
                        child.BoundsChanged -= HandleChildrenBoundsChanged;
                    }
                    break;
                }
                case PropertyChangeKind.Set: {
                    var oldChild = e.OldValue as IFigure;
                    var newChild = e.NewValue as IFigure;
                    oldChild.BoundsChanged -= HandleChildrenBoundsChanged;
                    newChild.BoundsChanged += HandleChildrenBoundsChanged;
                    break;
                }
                default: {
                    throw new ArgumentException("kind");
                }
            }
            DirtyBoundsCache(null);
            base.OnChildrenChanged(e);
        }

        // === WrappingFigure ==========
        protected void DirtyBoundsCache(IEnumerable<IFigure> movingFigures) {
            var oldBounds = _boundsCache;

            Rectangle? selfBounds = CalcSelfBounds();
            Rectangle? childrenBounds = CalcChildrenBounds();

            Rectangle? newBounds = null;
            if (selfBounds.HasValue && childrenBounds.HasValue) {
                newBounds = Rectangle.Union(selfBounds.Value, childrenBounds.Value);
            } else if (selfBounds.HasValue) {
                newBounds = selfBounds.Value;
            } else if (childrenBounds.HasValue) {
                newBounds = childrenBounds.Value;
            }

            _boundsCache = newBounds?? Rectangle.Empty;
            if (_boundsCache != oldBounds) {
                OnBoundsChanged(oldBounds, _boundsCache, movingFigures);
            }
        }

        /// <summary>
        /// デフォルトでは自分自身の矩形はないという計算結果を返す．
        /// </summary>
        protected virtual Rectangle? CalcSelfBounds() {
            return null;
        }

        protected Rectangle? CalcChildrenBounds() {
            if (Children.Count == 0) {
                return null;
            }
            var rects = Children.Select(child => child.Bounds);
            return rects.Aggregate((agg, r) => Rectangle.Union(agg, r));
        }

        protected void HandleChildrenBoundsChanged(object sender, BoundsChangedEventArgs e) {
            /// 自分自身の移動によって引き起こされた子の移動であれば
            /// 無限ループになってしまうのでDirtyしない．
            if (!_isInMoving) {
                DirtyBoundsCache(null);
            }
        }
    }
}
