/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing;
using Mkamo.Common.Event;

namespace Mkamo.Figure.Figures {
    /// <summary>
    /// マウスでポイントされている子Figureだけが表示されるLayer．
    /// </summary>
    public class BuriedLayer: Layer {
        // ========================================
        // field
        // ========================================
        private IFigure _dug;
        private bool _digChildrenOnly;

        // ========================================
        // constructor
        // ========================================
        public BuriedLayer(bool digChildrenOnly) {
            _digChildrenOnly = digChildrenOnly;
        }

        public BuriedLayer(): this(true) {
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        /// <summary>
        /// _dugだけを表示する．
        /// </summary>
        protected override void PaintChildren(Graphics g) {
            if (_dug != null) {
                var clipBounds = g.ClipBounds;
                if (_dug.IsVisible && clipBounds.IntersectsWith(_dug.PaintBounds)) {
                    try {
                        g.SetClip(Bounds, CombineMode.Intersect);
                        _dug.Paint(g);
                    } finally {
                        g.SetClip(clipBounds);
                    }
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (_dug == null) {
                if (_digChildrenOnly) {
                    for (int i = Children.Count - 1; i >= 0; --i) {
                        var child = Children[i];
                        if (child.IsVisible && child.ContainsPoint(e.Location)) {
                            _dug = child;
                            _dug.InvalidatePaint();
                            return;
                        }
                    }

                } else {
                    var found = FindFigure(fig => fig != this && fig.IsVisible && fig.ContainsPoint(e.Location));
                    if (found != null) {
                        _dug = found;
                        _dug.InvalidatePaint();
                    }
                }
            }
        }

        protected override void OnMouseLeave() {
            base.OnMouseLeave();

            if (_dug != null) {
                var old = _dug;
                _dug = null;
                old.InvalidatePaint();
            }
        }

        protected override void OnChildrenChanged(DetailedPropertyChangedEventArgs e) {
            base.OnChildrenChanged(e);
            _dug = null;
        }

        protected override void OnDescendantChanged(DetailedPropertyChangedEventArgs e) {
            base.OnDescendantChanged(e);

            switch (e.Kind) {
                case PropertyChangeKind.Add: {
                    (e.NewValue as IFigure).Accept(
                        fig => {
                            fig.ForwardMouseEvents(this);
                            return false;
                        }
                    );
                    break;
                }
                case PropertyChangeKind.Remove: {
                    (e.OldValue as IFigure).Accept(
                        fig => {
                            fig.StopForwardMouseEvents(this);
                            return false;
                        }
                    );
                    break;
                }
                case PropertyChangeKind.Clear: {
                    foreach (var child in (IFigure[]) e.OldValue) {
                        child.Accept(
                            fig => {
                                fig.StopForwardMouseEvents(this);
                                return false;
                            }
                        );
                    }
                    break;
                }
                case PropertyChangeKind.Set: {
                    (e.OldValue as IFigure).Accept(
                        fig => {
                            fig.StopForwardMouseEvents(this);
                            return false;
                        }
                    );
                    (e.NewValue as IFigure).Accept(
                        fig => {
                            fig.ForwardMouseEvents(this);
                            return false;
                        }
                    );
                    break;
                }
            }
        }

    }
}
