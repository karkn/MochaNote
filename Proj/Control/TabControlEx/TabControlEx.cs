/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using Mkamo.Common.Win32;
using System.Collections.Generic;
using Mkamo.Common.Win32.Core;
using Mkamo.Common.Core;
using Mkamo.Common.Forms.Input;
using System.Security.Permissions;

namespace Mkamo.Control.TabControlEx {
    [ToolboxBitmap(typeof(TabControl))]
    public class TabControlEx: TabControl {
        // ========================================
        // field
        // ========================================
		private System.ComponentModel.Container components = null;


        //private SubClass _scrollWndSubclass = null;
        //private bool _isScrollWndVisible;
        //private ImageList _scrollImages = null;
		private const int _margin = 5;

        private Color _backColor;
        private Color _borderColor;

        // --- dnd ---
        private Point _dragStartPoint = Point.Empty;
        private Rectangle _dragStartRect = Rectangle.Empty;

        // --- close button ---
        //private ImageList _closeTabImages;
        private Image _closeTabImage;

        private Rectangle _currentCloseButtonBounds;
        private CloseButtonState _closeButtonState;
        
        // ========================================
        // constructor
        // ========================================
		public TabControlEx() {
			InitializeComponent();

            AllowDrop = true;
            ClearDragTarget();

            SetStyle(ControlStyles.UserPaint, true);
			//SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            //SetStyle(ControlStyles.DoubleBuffer, true);
			//SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            //SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, false);
            SetStyle(ControlStyles.Opaque, false);
            DoubleBuffered = true;
            ResizeRedraw = true;

            SizeMode = TabSizeMode.Fixed;
            Appearance = TabAppearance.Normal;
            Multiline = true;

            _backColor = SystemColors.Control;
            _borderColor = SystemColors.ControlDarkDark;
            
            //_isScrollWndVisible = false;

            //_closeTabImages = new ImageList();
            //_closeTabImages.ColorDepth = ColorDepth.Depth32Bit;
            //var img = Properties.Resources.close_tab;
            //if (img != null) {
            //    _closeTabImages.ImageSize = new Size(img.Height, img.Height);
            //    _closeTabImages.Images.AddStrip(img);
            //}
            
            //_closeTabImage = Properties.Resources.cross_button;
            _closeButtonState = CloseButtonState.None;
        }

        //protected ImageList _ScrollImages {
        //    get {
        //        if (_scrollImages == null) {
        //            _scrollImages = new ImageList();
        //            var scrollImage = Properties.Resources.scroll;
        //            if (scrollImage != null) {
        //                scrollImage.MakeTransparent(Color.White);
        //                _scrollImages.Images.AddStrip(scrollImage);
        //            }
        //        }
        //        return _scrollImages;
        //    }
        //}


        // ========================================
        // event
        // ========================================
        public event EventHandler<DragStartEventArgs> DragStart;
        public event EventHandler<CloseButtonPressedEventArgs> CloseButtonPressed;

        // ========================================
        // property
        // ========================================
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get {
                var ret = base.CreateParams;
                if (!DesignMode) {
                    ret.ExStyle |= 0x02000000; /// WS_EX_COMPOSITED
                }
                return ret;
            }
        }

        new public TabAlignment Alignment {
            get { return base.Alignment; }
            set {
                var align = value;
                if ((align != TabAlignment.Top) && (align != TabAlignment.Bottom)) {
                    align = TabAlignment.Top;
                }
				
                base.Alignment = align;
            }
        }

        public new Color BackColor {
            get { return _backColor ; }
            set {
                if (value == _backColor) {
                    return;
                }

                _backColor = value;
                Invalidate();
            }
        }

        public Color BorderColor {
            get { return _borderColor ; }
            set {
                if (value == _borderColor) {
                    return;
                }

                _borderColor = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        new public bool Multiline {
            get { return base.Multiline; }
            set { base.Multiline = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image CloseTabImage {
            get { return _closeTabImage; }
            set { _closeTabImage = value; }
        }

        // ========================================
        // method
        // ========================================
        public bool IsInCloseButtonBounds(Point loc) {
            var tabPage = GetTabPage(loc);
            if (tabPage == null) {
                return false;
            }

            var r = GetCloseButtonBounds(tabPage);
            return r.Contains(loc);
        }


        
        /// <summary> 
		/// Clean up any resources being used.
		/// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
					components.Dispose();
				}

                //if (_scrollImages != null) {
                //    _scrollImages.Dispose();
                //}
                //_closeTabImage.Dispose();
            }
            base.Dispose(disposing);
        }

		protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            DrawControl(e.Graphics);
		}

        protected override void OnPaintBackground(PaintEventArgs pevent) {
        }

        //protected override void OnSelectedIndexChanged(EventArgs e) {
        //    base.OnSelectedIndexChanged(e);
        //    UpdateScrollWnd();
            
        //    //Invalidate();	// we need to update border and background colors
        //}

        // タブ上以外でホイールスクロールしてもページが変わってしまう
        //protected override void OnMouseWheel(MouseEventArgs e) {
        //    base.OnMouseWheel(e);

        //    var index = SelectedIndex;
        //    var count = TabPages.Count;
        //    var newIndex = (index - e.Delta / 120) % count;
        //    if (newIndex < 0) {
        //        newIndex += count;
        //    }
        //    SelectedIndex = newIndex;
        //}

        // ------------------------------
        // private
        // ------------------------------
        private void DrawControl(Graphics g) {
            if (!Visible) {
				return;
            }

            var tabControlArea = ClientRectangle;
			var tabArea = DisplayRectangle;

			//----------------------------
			// fill client area
            using (var backBrush = new SolidBrush(_backColor)) {
                g.FillRectangle(backBrush, tabControlArea);
            }

            //----------------------------

			//----------------------------
			// draw border
            var systemBorderWidth = SystemInformation.Border3DSize.Width;
            tabArea.Inflate(systemBorderWidth, systemBorderWidth);
            tabArea = new Rectangle(tabArea.Left, tabArea.Top, tabArea.Width - 1, tabArea.Height - 1);
            using (var pen = new Pen(_borderColor)) {
                g.DrawRectangle(pen, tabArea);
            }

			//----------------------------
			// clip region for drawing tabs
            //var clipOrg = g.Clip;
            //Rectangle rreg;

            var width = tabArea.Width + _margin;

            //rreg = new Rectangle(
            //    tabArea.Left,
            //    tabControlArea.Top,
            //    width - _margin,
            //    tabControlArea.Height
            //);

            //g.SetClip(rreg);

            // draw tabs
			for (int i = 0, len = TabCount; i < len; ++i) {
                DrawTab(g, TabPages[i], i);
            }

            //g.Clip = clipOrg;
			//----------------------------


			//----------------------------
			// draw background to cover flat border areas
			if (this.SelectedTab != null) {
                var selectedTabPage = this.SelectedTab;
                var color = selectedTabPage.BackColor;
                using (var border = new Pen(color)) {
                    tabArea.Offset(1, 1);
                    tabArea.Width -= 2;
                    tabArea.Height -= 2;
                    g.DrawRectangle(border, tabArea);

                    tabArea.Width -= 1;
                    tabArea.Height -= 1;
                    g.DrawRectangle(border, tabArea);
                }
			}
			//----------------------------
		}

		private void DrawTab(Graphics g, TabPage tabPage, int nIndex) {
            var tabBounds = GetTabRect(nIndex);
            if (!g.ClipBounds.IntersectsWith(tabBounds)) {
                return;
            }

            var tabTextRect = tabBounds;
            tabTextRect.Inflate(-4, 0);
            tabTextRect = new Rectangle(
                tabTextRect.Left + 2,
                tabTextRect.Top + 2,
                tabTextRect.Width - 2,
                tabTextRect.Height - 2
            );

			var isTabSelected = (SelectedIndex == nIndex);

            var corner = 5;
            using (var path = new GraphicsPath()) {
    			if (this.Alignment == TabAlignment.Top) {
                    var pts = new Point[7];
    				pts[0] = new Point(tabBounds.Left, tabBounds.Bottom);
    				pts[1] = new Point(tabBounds.Left, tabBounds.Top + corner);
    				pts[2] = new Point(tabBounds.Left + corner, tabBounds.Top);
    				pts[3] = new Point(tabBounds.Right - corner, tabBounds.Top);
    				pts[4] = new Point(tabBounds.Right, tabBounds.Top + corner);
    				pts[5] = new Point(tabBounds.Right, tabBounds.Bottom);
    				pts[6] = new Point(tabBounds.Left, tabBounds.Bottom);
    
                    path.AddLine(pts[0], pts[1]);
                    path.AddArc(new Rectangle(pts[1].X, pts[2].Y, corner, corner), 180, 90);
                    path.AddLine(pts[2], pts[3]);
                    path.AddArc(new Rectangle(pts[3].X, pts[3].Y, corner, corner), 270, 90);
                    path.AddLine(pts[4], pts[5]);
                    path.AddLine(pts[5], pts[6]);
                    path.CloseFigure();
    			} else {
                    // todo: Bottomのときもpathを作る．
    			}
    
    			//----------------------------
    			// fill this tab with background color
                if (isTabSelected) {
                    using (var br = new LinearGradientBrush(
                        tabBounds,
                        SystemColors.ControlLightLight,
                        tabPage.BackColor,
                        LinearGradientMode.Vertical
                    )) {
                        g.FillPath(br, path);
                    }
                } else {
                    using (var br = new SolidBrush(tabPage.BackColor)) {
                        g.FillPath(br, path);
                    }
                }
                //----------------------------

                //----------------------------
                // draw border
                //g.DrawPath(SystemPens.ControlDark, path);
                using (var pen = new Pen(_borderColor)) {
                    g.DrawPath(pen, path);
                }
            }

            //----------------------------
            // clear bottom lines
            if (isTabSelected) {
                using (var pen = new Pen(tabPage.BackColor)) {
                    switch (this.Alignment) {
                        case TabAlignment.Top: {
                            g.DrawLine(pen, tabBounds.Left + 1, tabBounds.Bottom, tabBounds.Right - 1, tabBounds.Bottom);
                            g.DrawLine(pen, tabBounds.Left + 1, tabBounds.Bottom + 1, tabBounds.Right - 1, tabBounds.Bottom + 1);
                            break;
                        }

                        case TabAlignment.Bottom: {
                            g.DrawLine(pen, tabBounds.Left + 1, tabBounds.Top, tabBounds.Right - 1, tabBounds.Top);
                            g.DrawLine(pen, tabBounds.Left + 1, tabBounds.Top - 1, tabBounds.Right - 1, tabBounds.Top - 1);
                            g.DrawLine(pen, tabBounds.Left + 1, tabBounds.Top - 2, tabBounds.Right - 1, tabBounds.Top - 2);
                            break;
                        }
                    }
                }

                //----------------------------
			}
			//----------------------------

			//----------------------------
			// draw tab's icon
            //if ((tabPage.ImageIndex >= 0) && (ImageList != null) && (ImageList.Images[tabPage.ImageIndex] != null)) {
            //    var nLeftMargin = 8;
            //    var nRightMargin = 2;

            //    var img = ImageList.Images[tabPage.ImageIndex];

            //    var imgRect = new Rectangle(tabBounds.X + nLeftMargin, tabBounds.Y + 1, img.Width, img.Height);

            //    // adjust rectangles
            //    //var nAdj = (float) (nLeftMargin + img.Width + nRightMargin);
            //    var nAdj = nLeftMargin + img.Width + nRightMargin;

            //    imgRect.Y += (tabBounds.Height - img.Height) / 2;
            //    tabTextRect.X += nAdj;
            //    tabTextRect.Width -= nAdj;

            //    // draw icon
            //    g.DrawImage(img, imgRect);
            //}
			//----------------------------

			//----------------------------
			// draw string
            var format =
                TextFormatFlags.EndEllipsis |
                TextFormatFlags.Left |
                TextFormatFlags.NoClipping |
                TextFormatFlags.NoPrefix |
                TextFormatFlags.SingleLine |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.NoPadding;

            TextRenderer.DrawText(g, tabPage.Text, Font, tabTextRect, Color.Black, format);

			//----------------------------

            //if (_closeButtonState == CloseButtonState.Normal) {
            //    g.DrawImageUnscaled(_closeTabImages.Images[0], _currentCloseButtonBounds.Location);
            //} else if (_closeButtonState == CloseButtonState.Hovered) {
            //    g.DrawImageUnscaled(_closeTabImages.Images[1], _currentCloseButtonBounds.Location);
            //} else if (_closeButtonState == CloseButtonState.Pressed) {
            //    g.DrawImageUnscaled(_closeTabImages.Images[2], _currentCloseButtonBounds.Location);
            //}
            if (_closeTabImage != null && _closeButtonState != CloseButtonState.None) {
                g.DrawImage(_closeTabImage, new Rectangle(_currentCloseButtonBounds.Location, _closeTabImage.Size));
            }
		}

        //private void DrawScrollIcons(Graphics g) {
        //    var imgs = _ScrollImages;
        //    if ((imgs == null) || (imgs.Images.Count != 4)) {
        //        return;
        //    }

        //    //----------------------------
        //    // calc positions
        //    var tabControlArea = ClientRectangle;

        //    var scrollWndRect = new Rectangle();
        //    Win32.GetClientRect(_scrollWndSubclass.Handle, ref scrollWndRect);

        //    using (var br = new SolidBrush(SystemColors.Control)) {
        //        g.FillRectangle(br, scrollWndRect);
        //    }
			
        //    using (var borderPen = new Pen(SystemColors.ControlDark)) {
        //        var borderRect = scrollWndRect;
        //        borderRect.Inflate(-1, -1);
        //        g.DrawRectangle(borderPen, borderRect);
        //    }

        //    int middle = (scrollWndRect.Width / 2);
        //    int top = (scrollWndRect.Height - 16) / 2;
        //    int left = (middle - 16) / 2;

        //    var leftButtonRect = new Rectangle(left, top, 16, 16);
        //    var rightButtonRect = new Rectangle(middle + left, top, 16, 16);
        //    //----------------------------

        //    //----------------------------
        //    // draw buttons
        //    var img = imgs.Images[1];
        //    if (img != null) {
        //        if (TabCount > 0) {
        //            var firstTabRect = GetTabRect(0);
        //            if (firstTabRect.Left < tabControlArea.Left) {
        //                g.DrawImage(img, leftButtonRect);
        //            } else {
        //                img = imgs.Images[3];
        //                if (img != null)
        //                    g.DrawImage(img, leftButtonRect);
        //            }
        //        }
        //    }

        //    img = imgs.Images[0];
        //    if (img != null) {
        //        if (TabCount > 0) {
        //            var lastTabRect = GetTabRect(TabCount - 1);
        //            if (lastTabRect.Right > (tabControlArea.Width - scrollWndRect.Width)) {
        //                g.DrawImage(img, rightButtonRect);
        //            } else {
        //                img = imgs.Images[2];
        //                if (img != null) {
        //                    g.DrawImage(img, rightButtonRect);
        //                }
        //            }
        //        }
        //    }
        //    //----------------------------
        //}

        //private void FindScrollWnd() {
        //    var found = false;

        //    var pWnd = Win32.FindWindowEx(this.Handle, IntPtr.Zero, "msctls_updown32", "\0");
        //    if (pWnd != IntPtr.Zero) {
        //        found = true;
        //        if (!_isScrollWndVisible) {
        //            _scrollWndSubclass = new SubClass(pWnd, true);
        //            _scrollWndSubclass.SubClassedWndProc += HandleScrollWndSubClassWndProc;
        //            _isScrollWndVisible = true;
        //        }
        //    }

        //    if ((!found) && (_isScrollWndVisible)) {
        //        _isScrollWndVisible = false;
        //    }
        //}

        //private void UpdateScrollWnd() {
        //    if (_isScrollWndVisible) {
        //        if (Win32.IsWindowVisible(_scrollWndSubclass.Handle)) {
        //            var rect = new Rectangle();
        //            Win32.GetClientRect(_scrollWndSubclass.Handle, ref rect);
        //            Win32.InvalidateRect(_scrollWndSubclass.Handle, ref rect, true);
        //        }
        //    }
        //}

        //private int HandleScrollWndSubClassWndProc(ref Message m) {
        //    switch (m.Msg) {
        //        case Win32.WM_PAINT: {
        //            //------------------------
        //            // redraw
        //            var hDC = Win32.GetWindowDC(_scrollWndSubclass.Handle);
        //            using (var g = Graphics.FromHdc(hDC)) {
        //                DrawScrollIcons(g);
        //            }
        //            Win32.ReleaseDC(_scrollWndSubclass.Handle, hDC);
        //            //------------------------

        //            // return 0 (processed)
        //            m.Result = IntPtr.Zero;

        //            //------------------------
        //            // validate current rect
        //            var rect = new Rectangle();

        //            Win32.GetClientRect(_scrollWndSubclass.Handle, ref rect);
        //            Win32.ValidateRect(_scrollWndSubclass.Handle, ref rect);
        //            //------------------------
        //            return 1;
        //        }
        //    }

        //    return 0;
        //}

		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			components = new System.ComponentModel.Container();
		}

        // --- mouse ---
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);

            ClearDragTarget();
            if (e.Button != MouseButtons.Left || e.Clicks >= 2) {
                return;
            }

            var tabPage = GetTabPage(e.Location);
            if (tabPage != null) {
                if (e.Button == MouseButtons.Left && !KeyUtil.IsShiftPressed() && !KeyUtil.IsControlPressed()) {
                    var r = GetCloseButtonBounds(tabPage);
                    if (r.Contains(e.Location)) {
                        OnCloseButtonPressed(tabPage);
                        _closeButtonState = CloseButtonState.None;
                        Invalidate(r);
                        return;
                    }
                }
            }

            SetupDragTarget(e.X, e.Y);
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            ClearDragTarget();
        }

        protected virtual void OnCloseButtonPressed(TabPage tabPage) {
            var handler = CloseButtonPressed;
            if (handler != null) {
                handler(this, new CloseButtonPressedEventArgs(tabPage));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (
                _dragStartRect != Rectangle.Empty &&
                !_dragStartRect.Contains(e.X, e.Y)
            ) {
                var tabPage = GetTabPage(_dragStartPoint);
                if (tabPage != null) {
                    var dse = new DragStartEventArgs();
                    OnDragStart(dse);

                    var dataObj = new DataObject();
                    if (dse.DragDataObjects != null) {
                        foreach (var obj in dse.DragDataObjects) {
                            dataObj.SetData(obj);
                        }
                    }
                    dataObj.SetData(tabPage);
                    DoDragDrop(dataObj, DragDropEffects.Move | dse.AllowedEffect);
                }
            } else {
                if (_closeButtonState != CloseButtonState.Pressed) {
                    var tabPage = GetTabPage(e.Location);
                    if (tabPage != null) {
                        var r = GetCloseButtonBounds(tabPage);
                        var oldCloseButtonBounds = _currentCloseButtonBounds;
                        if (r.Contains(e.Location)) {
                            _closeButtonState = CloseButtonState.Hovered;
                        } else {
                            _closeButtonState = CloseButtonState.Normal;
                        }
                        if (r != oldCloseButtonBounds) {
                            Invalidate(oldCloseButtonBounds);
                            Invalidate(r);
                        }
                        _currentCloseButtonBounds = r;
                    } else {
                        _currentCloseButtonBounds = Rectangle.Empty;
                    }
                } else {
                    _currentCloseButtonBounds = Rectangle.Empty;
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            ClearDragTarget();
            if (_closeButtonState != CloseButtonState.None) {
                _closeButtonState = CloseButtonState.None;
                Invalidate(_currentCloseButtonBounds);
            }
            _currentCloseButtonBounds = Rectangle.Empty;
        }

        protected virtual void OnDragStart(DragStartEventArgs e) {
            if (_closeButtonState != CloseButtonState.None) {
                _closeButtonState = CloseButtonState.None;
                Invalidate(_currentCloseButtonBounds);
            }
            _currentCloseButtonBounds = Rectangle.Empty;

            var handler = DragStart;
            if (handler != null) {
                DragStart(this, e);
            }
        }

        protected override void OnDragOver(System.Windows.Forms.DragEventArgs e) {
            base.OnDragOver(e);

            var pt = PointToClient(new Point(e.X, e.Y));
            var hoveredTab = GetTabPage(pt);
            if (hoveredTab != null && e.Data.GetDataPresent(typeof(TabPage))) {
                e.Effect = DragDropEffects.Move;
            } else {
                e.Effect = DragDropEffects.None;
            }
        }

        protected override void OnDragDrop(System.Windows.Forms.DragEventArgs e) {
            base.OnDragDrop(e);

            var pt = PointToClient(new Point(e.X, e.Y));
            var hoveredTab = GetTabPage(pt);

            if (hoveredTab != null && e.Data.GetDataPresent(typeof(TabPage))) {
                e.Effect = DragDropEffects.Move;
                var draggedTab = (TabPage) e.Data.GetData(typeof(TabPage));

                var draggedTabIndex = FindIndex(draggedTab);
                var hoveredTabIndex = FindIndex(hoveredTab);

                if (draggedTabIndex != hoveredTabIndex) {
                    SuspendLayout();

                    /// Remove/Insertすると再描画されてしまう
                    if (draggedTabIndex < hoveredTabIndex) {
                        for (int i = draggedTabIndex; i < hoveredTabIndex; ++i) {
                            var tmp = TabPages[i];
                            TabPages[i] = TabPages[i + 1];
                            TabPages[i + 1] = tmp;
                        }
                    } else {
                        for (int i = draggedTabIndex; i > hoveredTabIndex; --i) {
                            var tmp = TabPages[i - 1];
                            TabPages[i - 1] = TabPages[i];
                            TabPages[i] = tmp;
                        }
                    }

                    SelectedTab = draggedTab;

                    ResumeLayout();
                }
            } else {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ClearDragTarget() {
            _dragStartRect = Rectangle.Empty;
            _dragStartPoint = Point.Empty;
        }

        private void SetupDragTarget(int x, int y) {
            var dragSize = SystemInformation.DragSize;
            _dragStartRect = new Rectangle(
                new Point(x - (dragSize.Width / 2), y - (dragSize.Height / 2)),
                dragSize
            );
            _dragStartPoint = new Point(x, y);
        }

        private TabPage GetTabPage(Point pt) {
            for (int i = 0, len = TabPages.Count; i < len; ++i) {
                if (GetTabRect(i).Contains(pt)) {
                    return TabPages[i];
                }
            }
            return null;
        }

        private int FindIndex(TabPage page) {
            for (int i = 0, len = TabPages.Count; i < len; ++i) {
                if (TabPages[i] == page)
                    return i;
            }

            return -1;
        }

        private Rectangle GetCloseButtonBounds(TabPage tabPage) {
            if (_closeTabImage == null || tabPage == null) {
                return Rectangle.Empty;
            }

            var tabRect = GetTabRect(FindIndex(tabPage));
            var size = _closeTabImage.Size;
            return new Rectangle(
                tabRect.Right - size.Width - 6,
                tabRect.Top + (tabRect.Height - _closeTabImage.Height) / 2 + 1,
                size.Width,
                size.Height
            );
        }


        // ========================================
        // type
        // ========================================
        [Serializable]
        private enum CloseButtonState {
            None,
            Normal,
            Hovered,
            Pressed,
        }

    }
}
