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
using Mkamo.Common.Visitor;

namespace Mkamo.Figure.Internal.Core {
    internal class DirtManager: IDirtManager {
        // ========================================
        // static field
        // ========================================
        private static readonly Size RepairMargin = new Size(4, 4);

        // ========================================
        // field
        // ========================================
        private Canvas _canvas;
        private int _dirtyingDepth = 0;

        private bool _isPaintDirty = false;
        private Rectangle _dirtyPaintBounds = Rectangle.Empty;

        private List<IFigure> _dirtyLayoutReserveds = new List<IFigure>();

        private HashSet<IFigure> _dirtyLayoutFigures = new HashSet<IFigure>();

        private bool _isEnabled = true;

        private DirtyingContext _dirtyingContext;

        // ========================================
        // constructor
        // ========================================
        public DirtManager(Canvas canvas) {
            _canvas = canvas;
            _dirtyingContext = new DirtyingContext(this);
        }

        // ========================================
        // property
        // ========================================
        public bool IsInDirtying {
            get { return _dirtyingDepth > 0; }
        }

        public bool IsEnabled {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        // ========================================
        // method
        // ========================================
        public DirtyingContext BeginDirty() {
            ++_dirtyingDepth;
            return _dirtyingContext;
        }

        public void EndDirty() {
            if (_dirtyingDepth > 0) {
                // 現在安全のためDirtyLayout()は逐次実行させている
                //if (_dirtyingDepth == 1) {
                //    /// IsInDirtyingがtrueの状態でrepair layoutするために
                //    RepairLayout();
                //}
                --_dirtyingDepth;
                if (_dirtyingDepth == 0) {
                    /// IsInDirtyingがfalseの状態でrepair paintする
                    _canvas.AdjustRootFigureBounds();

                    RepairLayout();
                    RepairPaint();

                }
            }
        }

        public void DirtyPaint(Rectangle rect) {
            if (IsInDirtying) {
                _dirtyPaintBounds = _isPaintDirty? Rectangle.Union(_dirtyPaintBounds, rect): rect;
                _isPaintDirty = true;
            } else {
                rect.Inflate(RepairMargin);
                _canvas.Invalidate(TranslateToControlRect(rect), false);
            }
        }

        public void DirtyLayout(IFigure figure) {
            if (IsInDirtying) {
                if (_dirtyLayoutReserveds.Contains(figure)) {
                    _dirtyLayoutReserveds.Remove(figure);
                }
                _dirtyLayoutReserveds.Add(figure);
                return;
            }

            DirtyLayoutCore(figure);
        }

        private void DirtyLayoutCore(IFigure figure) {
            if (_dirtyLayoutFigures.Contains(figure)) {
                return;
            }

            _dirtyLayoutFigures.Add(figure);
            try {
                var target = figure;

                if (target.Layout != null) {
                    var constraint = new SizeConstraint();
                    foreach (var child in target.Children) {
                        child.Measure(constraint);
                    }
                    target.Arrange();
                }

            } finally {
                _dirtyLayoutFigures.Remove(figure);
            }
        }

        private void RepairLayout() {
            //foreach (var fig in _dirtyLayoutReserveds.ToArray().Reverse()) {
            /// 逆順にするのに根拠はない
            /// この順番にしないとClassをCtrl+DnDしてCloneしたときに属性がうまくarrangeされない
            /// 逆順にするとうまくいくことから_dirtyLayoutReserveds内のfigureの親子関係を見て，
            /// 親が先にDirtyLayoutCoreするようにしないといけなさそう
            var figs = _dirtyLayoutReserveds.ToArray();
            for (int i = figs.Length - 1; i >= 0; --i) {
                var fig = figs[i];
                DirtyLayoutCore(fig);
            }
            _dirtyLayoutReserveds.Clear();
        }


        // ------------------------------
        // protected
        // ------------------------------
        protected Rectangle TranslateToControlRect(Rectangle rect) {
            return new Rectangle(_canvas.TranslateToControlPoint(rect.Location), rect.Size);
        }

        protected void RepairPaint() {
            if (_isPaintDirty && _isEnabled) {
                var rect = _dirtyPaintBounds;
                rect.Inflate(RepairMargin);
                _canvas.Invalidate(TranslateToControlRect(rect), false);

                // todo: 拡大縮小
                //var ratio = 0.8f;
                //var r = new Rectangle(
                //    (int) (rect.X * ratio), (int) (rect.Y * ratio), (int) (rect.Width * ratio), (int) (rect.Height * ratio)
                //);
                //_canvas.Invalidate(TranslateToControlRect(r));

                _isPaintDirty = false;
            }
        }

    }
}
