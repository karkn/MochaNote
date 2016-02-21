/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;

namespace Mkamo.Figure.Internal.Figures {
    internal class FontCache {
        // ========================================
        // field
        // ========================================
        private Dictionary<FontDescription, Font> _fontCache;

        // ========================================
        // constructor
        // ========================================
        public FontCache() {
            _fontCache = new Dictionary<FontDescription, Font>();
        }

        // ========================================
        // property
        // ========================================


        // ========================================
        // method
        // ========================================
        public void Dispose() {
            foreach (var font in _fontCache.Values) {
                if (font != null) {
                    font.Dispose();
                }
            }
            _fontCache.Clear();
            GC.SuppressFinalize(this);
        }

        public Font GetFont(FontDescription font) {
            var ret = default(Font);
            if (!_fontCache.TryGetValue(font, out ret)) {
                ret = font.CreateFont();
                _fontCache[font] = ret;
            }
            return ret;
        }
    }
}
