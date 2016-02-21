/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Figures;
using System.Drawing;
using Mkamo.Figure.Core;
using System.Reflection;
using System.Windows.Forms;

namespace Mkamo.Memopad.Internal.Figures {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = false)]
    internal class MemoFileFigure: ImageFigure {
        // ========================================
        // static field
        // ========================================
        private const int Margin = 2;

        // ========================================
        // constructor
        // ========================================
        public MemoFileFigure() {
            NeedImageFitted = false;
            AutoSizeKinds = AutoSizeKinds.FitBoth;
            TextHorizontalAlignment = Mkamo.Common.DataType.HorizontalAlignment.Left;
            TextVerticalAlignment = Mkamo.Common.DataType.VerticalAlignment.Center;
        }

        // ========================================
        // method
        // ========================================
        // ------------------------------
        // protected
        // ------------------------------
        // === AbstractFigure ==========
        protected override void PaintSelf(Graphics g) {
            using (_ResourceCache.UseResource()) {
                if (IsForegroundEnabled && BorderWidth > 0) {
                    g.DrawRectangle(_PenResource, Left, Top, Width - 1, Height - 1);
                }

                var area = ClientArea;
                g.DrawImage(_ImageResource, area.Location);

                var r = new Rectangle(
                    area.Left + ImageSize.Width + Margin,
                    area.Top,
                    area.Width - (ImageSize.Width + Margin),
                    ImageSize.Height
                );

                DrawTextCenter(g, Text, _FontResource, r, FontColor);
            }
        }

        protected override Size MeasureSelf(SizeConstraint constraint) {
            if (Root == null || Root.Canvas == null || Root.Canvas.IsDisposed) {
                /// 測れないので現状維持
                return constraint.MeasureConstrainedSize(Size);
            }

            if (!IsVisible) {
                /// 現状維持
                return constraint.MeasureConstrainedSize(Size);
            }

            using (var g = Root.Canvas.CreateGraphics())
            using (_ResourceCache.UseResource()) {
                /// _ImageResourceを参照しておかないとImageSizeが正しく取れないことがある
                var image = _ImageResource;

                if (Text != null) {
                    var textSize = MeasureText(g, Text, _FontResource, int.MaxValue);
                    var size = new Size(
                        ImageSize.Width + Margin + textSize.Width + Padding.Width,
                        Math.Max(ImageSize.Height, textSize.Height) + Padding.Height
                    );
                    return constraint.MeasureConstrainedSize(size);
                } else {
                    return constraint.MeasureConstrainedSize(ImageSize);
                }
            }
        }

    }
}
