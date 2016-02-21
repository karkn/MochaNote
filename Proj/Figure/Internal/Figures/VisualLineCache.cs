/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.StyledText.Core;
using System.Drawing;

namespace Mkamo.Figure.Internal.Figures {
    using VisualLinesProvider = Func<Graphics, LineSegment, VisualLine[]>;

    internal class VisualLineCache {

        // ========================================
        // field
        // ========================================
        private Dictionary<LineSegment, VisualLine[]> _visualLines;

        private VisualLinesProvider _visualLinesProvider;

        // ========================================
        // constructor
        // ========================================
        public VisualLineCache(VisualLinesProvider provider) {
            _visualLinesProvider = provider;
            _visualLines = new Dictionary<LineSegment, VisualLine[]>();
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public VisualLine[] GetVisualLines(Graphics g, LineSegment line) {
            var ret = default(VisualLine[]);
            if (_visualLines.TryGetValue(line, out ret)) {
                return ret;
            } else {
                ret = _visualLinesProvider(g, line);
                _visualLines.Add(line, ret);
                return ret;
            }
        }

        public void Dirty(LineSegment line) {
            _visualLines.Remove(line);
        }

        public void DirtyAll() {
            _visualLines.Clear();
        }

    }
}
