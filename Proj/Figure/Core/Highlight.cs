/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Mkamo.Figure.Core {
    [Serializable]
    public class Highlight {
        // ========================================
        // static field
        // ========================================
        private static readonly Color[] HighlightColors = new [] {
            Color.Yellow,
            Color.Aqua,
            Color.Pink,
            Color.YellowGreen,
            Color.Orange,
        };

        // ========================================
        // static method
        // ========================================
        public static Highlight[] CreateHighlights(string highlight) {
            if (string.IsNullOrEmpty(highlight)) {
                return null;
            } else {
                var keywords = highlight.Split(' ');
                var ret = new List<Highlight>();
                for (int i = 0, ilen = keywords.Length; i < ilen; ++i) {
                    var keyword = keywords[i].Trim();
                    if (!string.IsNullOrEmpty(keyword) && !keyword.StartsWith("-")) {
                        var color = HighlightColors[ret.Count % HighlightColors.Length];
                        ret.Add(new Highlight(new Regex(keyword, RegexOptions.IgnoreCase), HighlightRange.Keyword, null, color));
                    }
                }
                return ret.ToArray();
            }
        }

        // ========================================
        // field
        // ========================================
        private Regex _regex;
        private Regex _endRegex;
        private HighlightRange _highlightRange;
        private Color? _foreColor;
        private Color? _backColor;
        private bool? _isUnderline;

        // ========================================
        // constructor
        // ========================================
        public Highlight(Regex regex, Regex endRegex, HighlightRange highlightRange, Color? foreColor, Color? backColor, bool? isUnderline) {
            _regex = regex;
            _endRegex = endRegex;
            _highlightRange = highlightRange;
            _foreColor = foreColor;
            _backColor = backColor;
            _isUnderline = isUnderline;
        }

        public Highlight(Regex regex, HighlightRange highlightRange, Color? foreColor)
            : this(regex, null, highlightRange, foreColor, null, null) {
        }

        public Highlight(Regex regex, HighlightRange highlightRange, Color? foreColor, Color? backColor)
            : this(regex, null, highlightRange, foreColor, backColor, null) {
        }

        // ========================================
        // property
        // ========================================
        public Regex Regex {
            get { return _regex; }
        }

        public Regex EndRegex {
            get { return _endRegex; }
        }

        public HighlightRange HighlightRange {
            get { return _highlightRange; }
        }

        public Color? ForeColor {
            get { return _foreColor; }
        }

        public Color? BackColor {
            get { return _backColor; }
        }
        
        public bool? IsUnderline {
            get { return _isUnderline; }
        }
    }
}
