/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mkamo.Common.Win32.Gdi32;
using Mkamo.Common.Win32.Util;

namespace Mkamo.Common.Win32.Gdi32 {

    using Graphics = System.Drawing.Graphics;

    /// <summary>
    /// Sometimes you need a Graphics instance when you don't really have access to one.
    /// Example situations include retrieving the bounds or scanlines of a Region.
    /// So use this to create a 'null' Graphics instance that effectively eats all
    /// rendering calls.
    /// </summary>
    public sealed class Gdi32NullGraphics: IDisposable{

        // ========================================
        // field
        // ========================================
        private IntPtr _hdc = IntPtr.Zero;
        private Graphics _graphics = null;
        private bool _disposed = false;

        // ========================================
        // constructor
        // ========================================
        public Gdi32NullGraphics() {
            _hdc = Gdi32PI.CreateCompatibleDC(IntPtr.Zero);

            if (_hdc == IntPtr.Zero) {
                ExceptionUtil.ThrowOnWin32Error("CreateCompatibleDC returned NULL");
            }

            _graphics = Graphics.FromHdc(_hdc);
        }

        // ========================================
        // destructor
        // ========================================
        ~Gdi32NullGraphics() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    _graphics.Dispose();
                    _graphics = null;
                }

                Gdi32PI.DeleteDC(_hdc);
                _disposed = true;
            }
        }

        // ========================================
        // property
        // ========================================
        public Graphics Graphics {
            get { return _graphics; }
        }

    }
}
