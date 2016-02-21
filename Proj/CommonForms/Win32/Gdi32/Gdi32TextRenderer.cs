/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Mkamo.Common.Win32.Core;
using Mkamo.Common.Win32.User32;

namespace Mkamo.Common.Win32.Gdi32 {
    public class Gdi32TextRenderer: IDisposable {
        // ========================================
        // field
        // ========================================
        private Graphics _graphics;
        private IntPtr _hdc;
        private IntPtr _hfont;
        private IntPtr _hOldFont;
        private TextAlignModes _oldAlignMode;

        // ========================================
        // constructor
        // ========================================
        public Gdi32TextRenderer(Graphics g) {
            _graphics = g;
            _hdc = _graphics.GetHdc();
            _oldAlignMode = Gdi32PI.GetTextAlign(_hdc);
        }

        public Gdi32TextRenderer(Graphics g, Font font): this(g) {
            Font = font;
        }

        // ========================================
        // destructor
        // ========================================
        ~Gdi32TextRenderer() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (_hOldFont != IntPtr.Zero) {
                Gdi32PI.SelectObject(_hdc, _hOldFont);
            }
            if (_hfont != IntPtr.Zero) {
                Gdi32PI.DeleteObject(_hfont);
            }
            TextAlignMode = _oldAlignMode;
            _graphics.ReleaseHdc(_hdc);
            _graphics = null;
        }

        // ========================================
        // property
        // ========================================
        public Font Font {
            set {
                if (_hfont != IntPtr.Zero) {
                    Gdi32PI.DeleteObject(_hfont);
                }
                _hfont = value.ToHfont();
                _hOldFont = Gdi32PI.SelectObject(_hdc, _hfont);
            }
        }

        public Color TextColor {
            set { Gdi32PI.SetTextColor(_hdc, Gdi32Util.ToColorRefInt(value)); }
        }

        public BkMode BkMode {
            set { Gdi32PI.SetBkMode(_hdc, value); }
        }

        public Color BackColor {
            set { Gdi32PI.SetBkColor(_hdc, Gdi32Util.ToColorRefInt(value)); }
        }

        public TextAlignModes TextAlignMode {
            set { Gdi32PI.SetTextAlign(_hdc, value); }
        }

        // ========================================
        // method
        // ========================================
        // todo: tab文字を自前で描画
        public void DrawText(string text, Rectangle rect) {
            var r = new RECT() {
                left = rect.Left,
                top = rect.Top,
                right = rect.Right,
                bottom = rect.Bottom,
            };
            if (
                !Gdi32PI.ExtTextOutW(
                    _hdc,
                    rect.Left,
                    rect.Top,
                    ExtTextOutFormatOptions.ETO_NONE,
                    ref r,
                    text,
                    text.Length,
                    null
               )
            ) {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            //var flags =
            //    DRAWTEXTFORMATS.DT_LEFT | DRAWTEXTFORMATS.DT_TOP | DRAWTEXTFORMATS.DT_TABSTOP | //DRAWTEXTFORMATS.DT_NOCLIP |
            //    DRAWTEXTFORMATS.DT_NOPREFIX | DRAWTEXTFORMATS.DT_SINGLELINE;
            //var paras = new DRAWTEXTPARAMS();
            //paras.CalcCbSize();
            //paras.iTabLength = 4;
            //paras.iLeftMargin = paras.iRightMargin = 0;
            //User32PI.DrawTextExW(
            //    _hdc,
            //    text,
            //    text.Length,
            //    ref r,
            //    flags,
            //    ref paras
            //);
            
        }

        public Size MeasureText(string text) {
            var dummy = -1;
            return MeasureText(text, int.MaxValue, out dummy);
        }

        public Size MeasureText(string text, int clipWidth, out int drawableLength) {
            var extents = default(int[]);
            return MeasureText(text, clipWidth, out drawableLength, out extents);
        }

        private Size MeasureText(string text, int clipWidth, out int drawableLength, out int[] extents) {
            var size = GetTextExtent(_hdc, text, text.Length, clipWidth, out drawableLength, out extents);

            if (drawableLength == 0) {
				size.Width = 0;
			} else if (drawableLength < extents.Length) {
                size.Width = extents[drawableLength - 1];
			}

			return size;
		}

        //private static Size GetTextExtent(
        //    IntPtr hdc, string text, int textLen, int maxWidth, out int fitLength, out int[] extents
        //) {
        //    Int32 bOk;
        //    SIZE size;
        //    extents = new int[text.Length];

        //    unsafe {
        //        fixed (int* pExtents = extents)
        //        fixed (int* pFitLength = &fitLength)
        //            bOk = Gdi32PI.GetTextExtentExPointW(hdc, text, textLen, maxWidth, pFitLength, pExtents, &size);
        //        //Debug.Assert(bOk != 0, "failed to calculate text width");
        //        return new Size(size.cx, size.cy);
        //    }
        //}

        private static Size GetTextExtent(
            IntPtr hdc, string text, int textLen, int maxWidth, out int fitLength, out int[] extents
        ) {
            SIZE size;
            extents = new int[text.Length];
            var result = Gdi32PI.GetTextExtentExPointW(hdc, text, textLen, maxWidth, out fitLength, extents, out size);
            return new Size(size.cx, size.cy);
        }
    }
}
