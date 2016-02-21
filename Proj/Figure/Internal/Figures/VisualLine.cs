/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mkamo.StyledText.Core;
using Mkamo.Common.Forms.Descriptions;

namespace Mkamo.Figure.Internal.Figures {
    /// <summary>
    /// VisualLineの情報はすべてLine内で0起点の値．
    /// </summary>
    internal class VisualLine {
        // ========================================
        // field
        // ========================================
        private LineSegment _line;
        private int _lineIndex;
        private int _charOffset;
        private int _charLength;
        private Rectangle _bounds;
        private List<VisualInline> _visualInlines;

        // ========================================
        // constructor
        // ========================================
        public VisualLine() {
            _visualInlines = new List<VisualInline>();
        }
        
        // ========================================
        // property
        // ========================================
        public LineSegment Line {
            get { return _line; }
            internal set { _line = value; }
        }
        public int LineIndex {
            get { return _lineIndex; }
            internal set { _lineIndex = value; }
        }
        public int CharOffset {
            get { return _charOffset; }
            internal set { _charOffset = value; }
        }
        public int CharLength {
            get { return _charLength; }
            internal set { _charLength = value; }
        }
        public Rectangle Bounds {
            get { return _bounds; }
            internal set { _bounds = value; }
        }

        internal List<VisualInline> VisualInlines {
            get { return _visualInlines; }
            set { _visualInlines = value; }
        }

        // ========================================
        // method
        // ========================================
        public override string ToString() {
            return "CharOffset=" + _charOffset + ", CharLength=" + _charLength;
        }
    }


    internal struct VisualInline {
        public Inline Inline;
        public string Text;
        public FontDescription Font;
        public Color ForeColor;
        public Color? BackColor;
        public Size Size;

        public VisualInline(Inline inline, Size size): this(inline, inline.Text, inline.Font, inline.Color, null, size) {
        }

        public VisualInline(Inline inline, string text, Size size): this(inline, text, inline.Font, inline.Color, null, size) {
        }

        private VisualInline(Inline inline, string text, FontDescription font, Color foreColor, Color? backColor, Size size) {
            Inline = inline;
            Text = text;
            BackColor = backColor;
            Size = size;

            var run = inline as Run;
            if (run != null && run.Link != null) {
                Font = new FontDescription(inline.Font, inline.Font.Style | FontStyle.Underline);
                ForeColor = Color.FromArgb(0x33, 0x33, 0xff);
            } else {
                Font = font;
                ForeColor = foreColor;
            }
        }

        public override string ToString() {
            return "Text=" + Text;
        }
    }
}
