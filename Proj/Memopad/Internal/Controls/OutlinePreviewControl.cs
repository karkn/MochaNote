/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal partial class OutlinePreviewControl: UserControl {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static int Space = 5;

        // ========================================
        // field
        // ========================================
        private Bitmap _bitmap;
        private float _ratio;
        private Size _size;
        private Screen _screen;
        private string _title;
        private int _maxWidth = int.MaxValue;
        private int _maxHeight = int.MaxValue;

        // ========================================
        // method
        // ========================================
        internal OutlinePreviewControl(Screen screen, string title, Bitmap bitmap, float ratio) {
            InitializeComponent();

            _title = title;
            _bitmap = bitmap;
            _ratio = ratio;
            _size = bitmap.Size;

            _screen = screen;

            Size = new Size(
                Math.Min((int) (_size.Width * _ratio) + 2, _screen.WorkingArea.Width),
                Math.Min((int) (_size.Height * _ratio) + 2, _screen.WorkingArea.Height)
            );
        }

        // ========================================
        // destructor
        // ========================================
        private void CleanUp() {
            if (_bitmap != null) {
                _bitmap.Dispose();
            }
        }


        // ========================================
        // property
        // ========================================
        public int MaxWidth {
            get { return _maxWidth; }
            set { _maxWidth = value; }
        }

        public int MaxHeight {
            get { return _maxHeight; }
            set { _maxHeight = value; }
        }

        // ========================================
        // method
        // ========================================
        public override Size GetPreferredSize(Size constrainingSize) {
            var titleSize = GetTitleSize();
            var imageWidth = (int) (_size.Width * _ratio);
            var imageHeight = (int) (_size.Height * _ratio);
            var width = Math.Max(imageWidth, titleSize.Width + Space * 2);
            var height = imageHeight + titleSize.Height + Space * 2;
            return new Size(
                Math.Min(Math.Min(width + 2, _screen.WorkingArea.Width), _maxWidth),
                Math.Min(Math.Min(height + 2, _screen.WorkingArea.Height), _maxHeight)
            );
        }

        private Size GetTitleSize() {
            using (var g = CreateGraphics()) {
                var fmt = StringFormat.GenericTypographic;
                fmt.FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.NoWrap;
                return Size.Ceiling(g.MeasureString(_title, Font, int.MaxValue, fmt));
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            GraphicsUtil.SetupGraphics(e.Graphics, GraphicQuality.MaxQuality);

            var titleSize = GetTitleSize();
            var headerHeight = titleSize.Height + Space * 2;

            var rect = new Rectangle(Bounds.Location, new Size(Bounds.Width - 1, Bounds.Height - 1));
            var titleRect = new Rectangle(Bounds.Location, new Size(Bounds.Width - 1, headerHeight));
            e.Graphics.FillRectangle(Brushes.White, rect);
            e.Graphics.FillRectangle(Brushes.WhiteSmoke, titleRect);
            e.Graphics.DrawRectangle(Pens.Gray, rect);

            var imgRect = new Rectangle(
                Bounds.Location + new Size(1, 1 + headerHeight),
                Bounds.Size - new Size(2, 2 + headerHeight)
            );
            if (_bitmap.Width < imgRect.Width) {
                imgRect.Width = _bitmap.Width;
            }
            try {
                var fmt = StringFormat.GenericTypographic;
                fmt.FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.NoWrap;
                e.Graphics.DrawString(_title, Font, Brushes.Black, new Point(Space + 1, Space + 1), fmt);
                e.Graphics.DrawLine(Pens.LightGray, new Point(1, headerHeight), new Point(Bounds.Right - 2, headerHeight));
                e.Graphics.DrawImageUnscaledAndClipped(_bitmap, imgRect);
            } catch (Exception ex) {
                Logger.Warn("プレビューの表示に失敗しました。", ex);
            }
        }
    }
}
