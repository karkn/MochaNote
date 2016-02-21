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

namespace Mkamo.Figure.Internal.Figures {
    using StyledText = Mkamo.StyledText.Core.StyledText;

    internal class BoundsCache {
        // ========================================
        // field
        // ========================================
        private StyledText _target;

        private Func<Graphics, Block, Rectangle> _blockBoundsProvider;
        private Func<Graphics, LineSegment, Rectangle> _lineBoundsProvider;

        private Dictionary<Block, Rectangle> _blockBounds;
        private Dictionary<LineSegment, Rectangle> _lineBounds;

        // ========================================
        // constructor
        // ========================================
        public BoundsCache(
            StyledText target,
            Func<Graphics, Block, Rectangle> blockBoundsProvider,
            Func<Graphics, LineSegment, Rectangle> lineBoundsProvider
        ) {
            _target = target;
            
            _blockBoundsProvider = blockBoundsProvider;
            _lineBoundsProvider = lineBoundsProvider;

            _blockBounds = new Dictionary<Block, Rectangle>();
            _lineBounds = new Dictionary<LineSegment, Rectangle>();
        }


        // ========================================
        // method
        // ========================================
        // ------------------------------
        // public
        // ------------------------------
        public void Dirty(Block block) {
            _blockBounds.Remove(block);
        }

        public void Dirty(LineSegment line) {
            _lineBounds.Remove(line);
        }

        public void Update(Block block, Func<Rectangle, Rectangle> updater) {
            if (_blockBounds.ContainsKey(block)) {
                _blockBounds[block] = updater(_blockBounds[block]);
            }
        }

        public void Update(LineSegment line, Func<Rectangle, Rectangle> updater) {
            if (_lineBounds.ContainsKey(line)) {
                _lineBounds[line] = updater(_lineBounds[line]);
            }
        }

        public void DirtyAll() {
            DirtyAllBlocks();
            DirtyAllLines();
        }

        public void DirtyAllBlocks() {
            _blockBounds.Clear();
        }

        public void DirtyAllLines() {
            _lineBounds.Clear();
        }

        public Rectangle GetBounds(Graphics g, Block block) {
            Rectangle bounds;
            if (_blockBounds.TryGetValue(block, out bounds)) {
                return bounds;
            } else {
                bounds = _blockBoundsProvider(g, block);
                _blockBounds.Add(block, bounds);
                return bounds;
            }
        }

        public Rectangle GetBounds(Graphics g, LineSegment line) {
            Rectangle bounds;
            if (_lineBounds.TryGetValue(line, out bounds)) {
                return bounds;
            } else {
                bounds = _lineBoundsProvider(g, line);
                _lineBounds.Add(line, bounds);
                return bounds;
            }
        }
    }
}
