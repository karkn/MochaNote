/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mkamo.Common.DataType;
using Mkamo.Figure.Figures;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Externalize;
using Mkamo.Common.Disposable;
using System.Reflection;

namespace Mkamo.Editor.Internal.Figures {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = false)]
    internal class FrameResizeFigure: AbstractPathBoundingNode {
        // ========================================
        // static field
        // ========================================
        public const int HitMarginDefault = 8;
        public static readonly Size CornerSizeDefault = new Size(16, 16);

        private static readonly string HitMarginPenResourceKey = "FrameResizeFigure.HitMarginPen";

        // ========================================
        // field
        // ========================================
        private int _hitMargin;
        private Size _cornerSize;

        // ========================================
        // constructor
        // ========================================
        public FrameResizeFigure(int hitMargin, Size cornerSize) {
            _hitMargin = hitMargin;
            _cornerSize = cornerSize;

            _ResourceCache.RegisterResourceCreator(
                HitMarginPenResourceKey,
                () => new Pen(Color.Black, _hitMargin),
                ResourceDisposingPolicy.Explicit
            );
        }

        public FrameResizeFigure(): this(HitMarginDefault, CornerSizeDefault) {
        }

        // ========================================
        // property
        // ========================================
        protected Pen _HitMarginPenResource {
            get { return (Pen) _ResourceCache.GetResource(HitMarginPenResourceKey); }
        }

        // ========================================
        // method
        // ========================================
        public Directions GetDirection(Point pt) {
            if (pt.X < Left - BorderWidth + _cornerSize.Width) {
                /// Xが左のコーナー範囲内
                
                if (pt.Y < Top + BorderWidth + _cornerSize.Height) {
                    /// 上のコーナー
                    return Directions.UpLeft;
                } else if (pt.Y > Bottom + BorderWidth - _cornerSize.Height) {
                    /// 下のコーナー
                    return Directions.DownLeft;
                } else {
                    /// 左辺
                    return Directions.Left;
                }

            } else if (pt.X > Right + BorderWidth - _cornerSize.Width) {
                /// Xが右のコーナー範囲内

                if (pt.Y < Top + BorderWidth + _cornerSize.Height) {
                    /// 上のコーナー
                    return Directions.UpRight;
                } else if (pt.Y > Bottom + BorderWidth - _cornerSize.Height) {
                    /// 下のコーナー
                    return Directions.DownRight;
                } else {
                    /// 右辺
                    return Directions.Right;
                }
            } else {
                if (pt.Y < Top + BorderWidth + _cornerSize.Height) {
                    /// 上辺
                    return Directions.Up;
                } else if (pt.Y > Bottom + BorderWidth - _cornerSize.Height) {
                    /// 下辺
                    return Directions.Down;
                }
            }

            return Directions.None;
        }

        protected override GraphicsPathDescription CreatePath(Rectangle bounds) {
            var ret = new GraphicsPathDescription();
            ret.AddRectangle(bounds);
            return ret;
        }

        public override bool ContainsPoint(Point pt) {
            if (_hitMargin > 0) {
                using (_ResourceCache.UseResource()) {
                    return _PathResouce.IsOutlineVisible(pt, _HitMarginPenResource);
                }
            }
            return false;
        }

        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);
            memento.WriteInt("HitMargin", _hitMargin);
            memento.WriteSerializable("CornerSize", _cornerSize);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);
            _hitMargin = memento.ReadInt("HitMargin");
            _cornerSize = (Size) memento.ReadSerializable("CornerSize");
        }

    }
}
