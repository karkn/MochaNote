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
    public class HighlightRegistry {
        // ========================================
        // static field
        // ========================================
        //private static HighlightRegistry _instance;
        //public static HighlightRegistry Instance {
        //    get { return _instance ?? (_instance = new HighlightRegistry()); }
        //}

        private static readonly Highlight[] EmptyHighlightArray = new Highlight[0];

        // ========================================
        // field
        // ========================================
        private Dictionary<string, Highlight[]> _classToHighlights;
        private Highlight[] _globalHighlights;

        // ========================================
        // constructor
        // ========================================
        public HighlightRegistry() {
            _classToHighlights = new Dictionary<string, Highlight[]>();

            //var hl1 = new Highlight(new Regex("今日"), HighlightRange.Keyword, Color.Blue, null);
            //var hl2 = new Highlight(new Regex("明日"), HighlightRange.Keyword, Color.Red, null);
            //Register("", new[] { hl1, hl2, });
        }

        // ========================================
        // property
        // ========================================
        public Highlight[] GlobalHighlights {
            get { return _globalHighlights ?? EmptyHighlightArray; }
            set {
                if (value == _globalHighlights) {
                    return;
                }
                _globalHighlights = value;
            }
        }

        // ========================================
        // method
        // ========================================
        public void Register(string clazz, Highlight[] highlight) {
            if (clazz == null) {
                return;
            }

            _classToHighlights[clazz] = highlight;
        }

        public Highlight[] GetHighlights(string clazz) {
            if (clazz == null) {
                return null;
            }

            if (_classToHighlights.ContainsKey(clazz)) {
                return _classToHighlights[clazz];
            } else {
                return null;
            }
        }


    }
}
