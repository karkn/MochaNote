/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Drawing.Imaging;

namespace Mkamo.Common.Win32.Gdi32 {
    
    [TestClass]
    public class GdiTextRendererTest {
        private Bitmap _bitmap;
        private Gdi32TextRenderer _gdiTextRenderer;
        private Graphics _graphics;

        [TestInitialize]
        public void Setup() {
            _bitmap = new Bitmap(128, 128);
            _graphics = Graphics.FromImage(_bitmap);
            _graphics.Clear(Color.White);
            _gdiTextRenderer = new Gdi32TextRenderer(_graphics);
        }

        [TestCleanup]
        public void TearDown() {
            _gdiTextRenderer.Dispose();
            _graphics.Dispose();
            _bitmap.Dispose();
        }

        [TestMethod]
        public void TestMeasureString() {
            _gdiTextRenderer.Font = new Font("MS PGothic", 9);
            _gdiTextRenderer.BkMode = BkMode.TRANSPARENT;
            _gdiTextRenderer.TextColor = Color.Black;

            Size textSize = _gdiTextRenderer.MeasureText("faobar");
            Assert.AreEqual(32, textSize.Width);
            Assert.AreEqual(12, textSize.Height);

            textSize = _gdiTextRenderer.MeasureText("‚ ‚¢‚¤‚¦‚¨");
            Assert.AreEqual(52, textSize.Width);
            Assert.AreEqual(12, textSize.Height);

            int fitlen;
            textSize = _gdiTextRenderer.MeasureText("‚ ‚¢‚¤‚¦‚¨", 100, out fitlen);
            Assert.AreEqual(52, textSize.Width);
            Assert.AreEqual(12, textSize.Height);
            Assert.AreEqual(5, fitlen);

            textSize = _gdiTextRenderer.MeasureText("‚ ‚¢‚¤‚¦‚¨", 30, out fitlen);
            Assert.AreEqual(30, textSize.Width);
            Assert.AreEqual(12, textSize.Height);
            Assert.AreEqual(3, fitlen);

        }
    }
}
