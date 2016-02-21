/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Mkamo.Common.Collection;
using Mkamo.StyledText.Core;

namespace Mkamo.Figure.Internal.Figures {
    using StyledText = Mkamo.StyledText.Core.StyledText;

    internal class SizeCache {
        // ========================================
        // field
        // ========================================
        private StyledText _target;

        private Func<Graphics, Inline, Size> _inlineSizeProvider;
        private Func<Graphics, LineSegment, Size> _lineSegSizeProvider;
        private Func<Graphics, Block, Size> _blockSizeProvider;

        private Dictionary<Block, Size> _blockSizes;
        private Dictionary<LineSegment, Size> _lineSegSizes;
        private Dictionary<Inline, Size> _inlineSizes;

        // ========================================
        // constructor
        // ========================================
        public SizeCache(
            StyledText target,
            Func<Graphics, Inline, Size> inlineSizeProvider,
            Func<Graphics, LineSegment, Size> lineSegSizeProvider,
            Func<Graphics, Block, Size> blockSizeProvider
        ) {
            _target = target;
            
            _inlineSizeProvider = inlineSizeProvider;
            _lineSegSizeProvider = lineSegSizeProvider;
            _blockSizeProvider = blockSizeProvider;

            _blockSizes = new Dictionary<Block,Size>();
            _lineSegSizes = new Dictionary<LineSegment, Size>();
            _inlineSizes = new Dictionary<Inline, Size>();
        }


        // ========================================
        // method
        // ========================================
        // ------------------------------
        // public
        // ------------------------------
        public void Dirty(Inline inline) {
            _inlineSizes.Remove(inline);
        }

        public void Dirty(LineSegment lineSeg) {
            _lineSegSizes.Remove(lineSeg);
        }

        public void Dirty(Block block) {
            _blockSizes.Remove(block);
        }

        //public void DirtyAllInlines() {
        //    _inlineSizes.Clear();
        //}

        public void DirtyAllLineSeguments() {
            _lineSegSizes.Clear();
        }

        public void DirtyAllBlocks() {
            _blockSizes.Clear();
        }

        public void DirtyAll() {
            _inlineSizes.Clear();
            _lineSegSizes.Clear();
            _blockSizes.Clear();
        }

        public Size GetSize(Graphics g, Inline inline) {
            Size size;
            if (_inlineSizes.TryGetValue(inline, out size)) {
                return size;
            } else {
                size = _inlineSizeProvider(g, inline);
                _inlineSizes.Add(inline, size);
                return size;
            }
        }

        public Size GetSize(Graphics g, LineSegment lineSeg) {
            Size size;
            if (_lineSegSizes.TryGetValue(lineSeg, out size)) {
                return size;
            } else {
                size = _lineSegSizeProvider(g, lineSeg);
                _lineSegSizes.Add(lineSeg, size);
                return size;
            }
        }

        public Size GetSize(Graphics g, Block block) {
            Size size;
            if (_blockSizes.TryGetValue(block, out size)) {
                return size;
            } else {
                size = _blockSizeProvider(g, block);
                _blockSizes.Add(block, size);
                return size;
            }
        }

    }


}
