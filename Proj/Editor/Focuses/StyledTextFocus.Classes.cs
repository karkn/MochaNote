/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Editor.Controllers;
using System.Drawing;
using Mkamo.Figure.Figures;
using Mkamo.Figure.Core;
using Mkamo.StyledText.Core;
using System.Reflection;

namespace Mkamo.Editor.Focuses {
    partial class StyledTextFocus {
        // ========================================
        // class
        // ========================================
        /// <summary>
        /// IMEの状態によって最適なサイズが変わるfigure．
        /// </summary>
        [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = false)]
        private class StyledTextFocusFigure: SimpleRect {
            // ========================================
            // field
            // ========================================
            private StyledTextFocus _owner;
            private bool _isImeOpened;
            private Rectangle _imeWindowBounds;

            private bool _isCurrentLineBackgroundEnable;

            // ========================================
            // constructor
            // ========================================
            public StyledTextFocusFigure(StyledTextFocus owner) {
                _owner = owner;
                _isImeOpened = false;
                _imeWindowBounds = Rectangle.Empty;
                _isCurrentLineBackgroundEnable = false;
            }

            // ========================================
            // property
            // ========================================
            public bool IsImeOpened {
                get { return _isImeOpened; }
                set { _isImeOpened = value; }
            }

            public Rectangle ImeWindowBounds {
                get { return _imeWindowBounds; }
                set { _imeWindowBounds = value; }
            }

            public bool IsCurrentLineBackgroundEnable {
                get { return _isCurrentLineBackgroundEnable; }
                set { _isCurrentLineBackgroundEnable = value; }
            }

            // ========================================
            // method
            // ========================================
            protected override Size MeasureSelf(SizeConstraint constraint) {
                var ret = Size.Empty;

                if (_isImeOpened) {
                    var bounds = new Rectangle(Location, base.MeasureSelf(constraint));
                    var imeBounds = _imeWindowBounds;
                    imeBounds.Width += Padding.Width;
                    imeBounds.Height += Padding.Height;
                    var r = Rectangle.Union(bounds, imeBounds);
                    ret = r.Size;
                } else {
                    ret = base.MeasureSelf(constraint);
                }

                return ret;
            }

            protected override void  PaintSelf(Graphics g) {
                /// Win7 Aero上ではなぜかStyledText == nullのときがある。
                /// MemoTextをMouseDownしたときに，
                /// StyledTextFocus.Begin()でStyledTextがセットされる前に再描画がおこるっぽい。
                /// なので，ここでStyledText == nullなら抜けておかないと
                /// 背景だけが描画されてちらつきが発生してしまう。
                if (StyledText == null) {
                    return;
                }

                using (_ResourceCache.UseResource()) {
                    var brush = _BrushResource;
                    if (IsBackgroundEnabled && brush != null) {
                        g.FillRectangle(brush, Left, Top, Width - 1, Height - 1);
                    }
                    if (IsForegroundEnabled && BorderWidth > 0) {
                        g.DrawRectangle(_PenResource, Left, Top, Width - 1, Height - 1);
                    }

                    if (_isCurrentLineBackgroundEnable && StyledText != null && Selection.IsEmpty) {
                        /// 現在行色付け
                        var vlr = GetVisualLineBounds(_owner.Referer.CaretIndex);
                        var r = new Rectangle(Left + 1, vlr.Top, Width - 3, vlr.Height);
                        if (g.ClipBounds.IntersectsWith(r)) {
                            using (var b = new SolidBrush(Color.FromArgb(232, 242, 254))) {
                                g.FillRectangle(b, r);
                            }
                        }
                    }

                    PaintText(g);
                    PaintSelection(g);
                    PaintStyledText(g);
                }
            }

            public void OnOwnerCaretMoved(CaretMovedEventArgs e) {
                if (StyledText == null) {
                    return;
                }

                /// 現在行色付けのためのInvalidatePaint()処理
                using (DirtManager.BeginDirty()) {
                    var len = _owner.Referer.Target.Length;
                    if (e.OldIndex < len) {
                        if (e.NewIndex < len) {
                            var oldVisLineBounds = GetVisualLineBounds(e.OldIndex);
                            var newVisLineBounds = GetVisualLineBounds(e.NewIndex);
                            if (newVisLineBounds != oldVisLineBounds) {
                                var r = oldVisLineBounds;
                                r = new Rectangle(Left, r.Top, Width, r.Height);
                                InvalidatePaint(r);
                                r = newVisLineBounds;
                                r = new Rectangle(Left, r.Top, Width, r.Height);
                                InvalidatePaint(r);
                            }
                        } else {
                            Logger.Warn(
                                "Illegal caret move: StyledTextLength=" + len +
                                ",NewIndex=" + e.NewIndex + ",OldIndex=" + e.OldIndex
                            );
                            InvalidatePaint();
                        }
                    } else {
                        if (e.NewIndex < len) {
                            var newVisLineBounds = GetVisualLineBounds(e.NewIndex);
                            var r = new Rectangle(Left, newVisLineBounds.Top, Width, Bottom);
                            InvalidatePaint(r);
                        } else {
                            Logger.Warn(
                                "Illegal caret move: StyledTextLength=" + len +
                                ",NewIndex=" + e.NewIndex + ",OldIndex=" + e.OldIndex
                            );
                            InvalidatePaint();
                        }
                    }
                }
            }

            public void OnOwnerSelectionChanged() {
                if (StyledText == null) {
                    return;
                }

                /// OnOwnerCaretMoved()のタイミングだけでは
                /// ・範囲選択なしの状態から
                ///   カーソルが行頭ちょうどでSelection.Range.Endも行頭ちょうどになったときに
                ///   カーソル行がInvalidatePaint()されない
                /// ・カーソルが行頭ちょうどでSelection.Range.Endも行頭ちょうどを範囲選択している状態から
                ///   カーソルを右に移動したときにカーソル行がInvalidatePaint()されない
                /// のでこのタイミングでもInvalidatePaint()する

                if (!InUpdatingStyledText) {
                    /// CutRegion()のBeginUpdateStyledText()中に呼ばれると
                    /// GetVisualLineBounds()の呼び出しでキャッシュがおかしくなる
                    using (DirtManager.BeginDirty()) {
                        var newVisLineBounds = GetVisualLineBounds(_owner.Referer.CaretIndex);
                        var r = newVisLineBounds;
                        r = new Rectangle(Left, r.Top, Width, r.Height);
                        InvalidatePaint(r);
                    }
                }
            }

            //public bool IsInSelection(Point loc) {
            //    if (Selection.IsEmpty) {
            //        return false;
            //    }

            //    if (Root != null && Root.Canvas != null) {
            //        using (var g = Root.Canvas.CreateGraphics()) {
            //            var rects = GetStringRect(g, Selection.Offset, Selection.Length);
            //            foreach (var rect in rects) {
            //                if (rect.Contains(loc)) {
            //                    return true;
            //                }
            //            }

            //        }
            //    }

            //    return false;
            //}
        }

    }
}
