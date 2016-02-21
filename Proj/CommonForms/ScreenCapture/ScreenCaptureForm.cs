/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;
using Mkamo.Common.Forms.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Mkamo.Common.Forms.ScreenCapture {
    public partial class ScreenCaptureForm: Form {
        // ========================================
        // field
        // ========================================
        private Bitmap _allScreenImage;

        // --- dnd ---
        private bool _isDragPrepared;
        private bool _isDragStarted;
        private Point _dragStartPoint;
        private Rectangle _dragStartRect;

        private Rectangle _rubberbandRect;

        // ========================================
        // constructor
        // ========================================
        public ScreenCaptureForm() {
            InitializeComponent();

            DoubleBuffered = true;
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
        }

        // ========================================
        // destructor
        // ========================================
        private void CleanUp() {
            if (_allScreenImage != null) {
                _allScreenImage.Dispose();
            }
        }

        // ========================================
        // property
        // ========================================
        public bool IsCaptured {
            get { return !_rubberbandRect.IsEmpty; }
        }

        // ========================================
        // method
        // ========================================
        public void Setup() {
            var bounds = GetAllScreenBounds();
            Bounds = bounds;

            if (_allScreenImage != null) {
                _allScreenImage.Dispose();
            }
            _allScreenImage = new Bitmap(bounds.Size.Width, bounds.Size.Height);
            using (var g = Graphics.FromImage(_allScreenImage)) {
                g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
            }
        }

        public Image CreateCaptured() {
            var ret = new Bitmap(_rubberbandRect.Width, _rubberbandRect.Height);
            using (var g = Graphics.FromImage(ret)) {
                g.DrawImage(_allScreenImage, new Rectangle(0, 0, ret.Width, ret.Height), _rubberbandRect, GraphicsUnit.Pixel);
            }
            return ret;
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (_allScreenImage != null) {
                var g = e.Graphics;

                //GraphicsUtil.SetupGraphics(g, GraphicQuality.MaxQuality);
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //g.CompositingQuality = CompositingQuality.HighQuality; /// 遅い

                g.DrawImage(_allScreenImage, Point.Empty);
                g.FillRectangle(new SolidBrush(Color.FromArgb(64, Color.Gray)), Bounds);
                g.FillRectangle(new SolidBrush(Color.FromArgb(32, SystemColors.Highlight)), _rubberbandRect);
                g.DrawRectangle(new Pen(SystemColors.Highlight), _rubberbandRect);

                using (var path = new GraphicsPath()) {
                    path.AddString(
                        "マウスでドラッグ&ドロップした範囲を画像として取り込みます",
                        new FontFamily("MS UI Gothic"),
                        (int) FontStyle.Bold,
                        24,
                        new Point(10, 10),
                        StringFormat.GenericDefault
                    );
                    g.FillPath(Brushes.White, path);
                    g.DrawPath(Pens.Red, path);
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e) {
            base.OnMouseClick(e);
            Close();
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left) {
                PrepareUpDragState(new Point(e.X, e.Y));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (_isDragStarted) {
                var oldRect = _rubberbandRect;

                _rubberbandRect = RectUtil.GetRectangleFromDiagonalPoints(_dragStartPoint, e.Location);

                var newRect = _rubberbandRect;

                oldRect.Inflate(8, 8);
                newRect.Inflate(8, 8);

                Invalidate(oldRect);
                Invalidate(newRect);

            } else if (_isDragPrepared) {
                if (!_dragStartRect.Contains(e.X, e.Y)) {
                    _isDragStarted = true;
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            if (_isDragStarted) {
                _rubberbandRect = RectUtil.GetRectangleFromDiagonalPoints(_dragStartPoint, e.Location);
            }

            ClearDragState();
        }

        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);

            Close();
        }

        // ------------------------------
        // private
        // ------------------------------

        private void PrepareUpDragState(Point pt) {
            _isDragPrepared = true;
            _isDragStarted = false;
            _dragStartPoint = pt;
            _dragStartRect = new Rectangle(
                _dragStartPoint.X - SystemInformation.DragSize.Width / 2,
                _dragStartPoint.Y - SystemInformation.DragSize.Height / 2,
                SystemInformation.DragSize.Width,
                SystemInformation.DragSize.Height
            );
        }

        private void ClearDragState() {
            _isDragPrepared = false;
            _isDragStarted = false;
            _dragStartPoint = Point.Empty;
            _dragStartRect = Rectangle.Empty;
        }

        private Rectangle GetAllScreenBounds() {
            var ret = Rectangle.Empty;

            var screens = Screen.AllScreens;
            var first = true;
            foreach (var scr in screens) {
                if (first) {
                    first = false;
                    ret = scr.Bounds;
                } else {
                    ret = Rectangle.Union(ret, scr.Bounds);
                }
            }

            return ret;
        }
    }
}
