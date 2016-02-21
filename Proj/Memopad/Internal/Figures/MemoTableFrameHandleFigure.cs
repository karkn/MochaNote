/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Figures;
using Mkamo.Common.Externalize;
using System.Drawing;
using System.Reflection;
using Mkamo.Common.Forms.Descriptions;

namespace Mkamo.Memopad.Internal.Figures {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = false)]
    internal class MemoTableFrameHandleFigure: AbstractNode {
        // ========================================
        // field
        // ========================================
        private int _borderWidth;
        private int _moveBarHeight;
        private int _cornerWidth;

        // ========================================
        // constructor
        // ========================================
        public MemoTableFrameHandleFigure() {
            _borderWidth = 4;
            _moveBarHeight = 12;
            _cornerWidth = 8;
            Foreground = Color.DarkGray;
            Background = new SolidBrushDescription(Color.WhiteSmoke);
        }

        // ========================================
        // property
        // ========================================
        public Rectangle InnerBounds {
            get {
                var bounds = Bounds;
                return new Rectangle(
                    bounds.Left + _borderWidth,
                    bounds.Top + _moveBarHeight + _borderWidth,
                    bounds.Width - _borderWidth * 2,
                    bounds.Height - _moveBarHeight - _borderWidth * 2
                );
            }
            set {
                var bounds = new Rectangle(
                    value.Left - _borderWidth,
                    value.Top - _moveBarHeight - _borderWidth,
                    value.Width + _borderWidth * 2,
                    value.Height + _moveBarHeight + _borderWidth * 2
                );
                if (bounds == Bounds) {
                    return;
                }
                Bounds = bounds;
            }
        }


        // ========================================
        // method
        // ========================================
        protected override void PaintSelf(Graphics g) {
            using (var barBrush = new SolidBrush(Color.Silver))
            using (_ResourceCache.UseResource()) {
                var brush = _BrushResource;
                var pen = _PenResource;

                var bounds = Bounds;
                var innerBounds = InnerBounds;

                /// 左
                g.FillRectangle(
                    brush,
                    bounds.Left,
                    bounds.Top,
                    _borderWidth,
                    bounds.Height - 1
                );
                /// 右
                g.FillRectangle(
                    brush,
                    bounds.Right - _borderWidth - 1,
                    bounds.Top,
                    _borderWidth,
                    bounds.Height - 1
                );
                /// 上
                g.FillRectangle(
                    brush,
                    bounds.Left,
                    bounds.Top,
                    bounds.Width,
                    //_hitMargin
                    _moveBarHeight + _borderWidth
                );
                /// 下
                g.FillRectangle(
                    brush,
                    bounds.Left,
                    bounds.Bottom - _borderWidth - 1,
                    bounds.Width,
                    _borderWidth
                );

                /// bar
                //g.FillRectangle(
                //    barBrush,
                //    bounds.Left + _borderWidth,
                //    bounds.Top + _borderWidth,
                //    bounds.Width - _borderWidth * 2,
                //    _moveBarHeight
                //);

                /// inner
                g.DrawRectangle(
                    pen,
                    innerBounds.Left,
                    innerBounds.Top,
                    innerBounds.Width - 1,
                    innerBounds.Height - 1
                );
                /// outer
                g.DrawRectangle(
                    pen,
                    bounds.Left,
                    bounds.Top,
                    bounds.Width - 1,
                    bounds.Height - 1
                );

                /// bar
                g.DrawRectangle(
                    pen,
                    bounds.Left + _borderWidth,
                    bounds.Top + _borderWidth,
                    bounds.Width - _borderWidth * 2 - 1,
                    _moveBarHeight
                );
            }
        }

        public override bool ContainsPoint(Point pt) {
            var bounds = Bounds;

            if (!bounds.Contains(pt)) {
                return false;
            }

            var innerBounds = InnerBounds;
            return !innerBounds.Contains(pt);
        }

        public bool IsOnMoveBar(Point pt) {
            if (!ContainsPoint(pt)) {
                return false;
            }

            return
                pt.X >= Left + _borderWidth &&
                pt.X <= Right - _borderWidth &&
                pt.Y >= Top + _borderWidth &&
                pt.Y <= Top + _moveBarHeight + _borderWidth;
        }

        public bool IsOnLeftBorder(Point pt) {
            if (!ContainsPoint(pt)) {
                return false;
            }

            return pt.X <= Left + _borderWidth;
        }

        public bool IsOnRightBorder(Point pt) {
            if (!ContainsPoint(pt)) {
                return false;
            }

            return pt.X >= Right - _borderWidth;
        }

        public bool IsOnTopBorder(Point pt) {
            if (!ContainsPoint(pt)) {
                return false;
            }

            return pt.Y <= Top + _borderWidth;
        }

        public bool IsOnBottomBorder(Point pt) {
            if (!ContainsPoint(pt)) {
                return false;
            }

            return pt.Y >= Bottom - _borderWidth;
        }

        public bool IsOnLeftTopCorner(Point pt) {
            if (!ContainsPoint(pt)) {
                return false;
            }

            return
                pt.X <= Left + _cornerWidth &&
                pt.Y <= Top + _cornerWidth;
        }

        public bool IsOnRightTopCorner(Point pt) {
            if (!ContainsPoint(pt)) {
                return false;
            }

            return
                pt.X >= Right - _cornerWidth &&
                pt.Y <= Top + _cornerWidth;
        }

        public bool IsOnLeftBottomCorner(Point pt) {
            if (!ContainsPoint(pt)) {
                return false;
            }

            return
                pt.X <= Left + _cornerWidth &&
                pt.Y >= Bottom - _cornerWidth;
        }

        public bool IsOnRightBottomCorner(Point pt) {
            if (!ContainsPoint(pt)) {
                return false;
            }

            return
                pt.X >= Right - _cornerWidth &&
                pt.Y >= Bottom - _cornerWidth;
        }


    }
}
