/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Handles;
using Mkamo.Figure.Core;
using Mkamo.Model.Memo;
using Mkamo.Editor.Core;
using System.Drawing;
using Mkamo.Figure.Figures;

namespace Mkamo.Memopad.Internal.Handles {
    internal class CompositeMemoMarkHandle: CompositeHandle {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        public CompositeMemoMarkHandle(): base(false) {
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override void Relocate(IFigure hostFigure) {
            base.Relocate(hostFigure);

            /// 左側と右側に振り分け
            var leftSides = new List<MemoMarkHandle>();
            var rightSides = new List<MemoMarkHandle>();

            foreach (var child in Children) {
                var markHandle = child as MemoMarkHandle;
                switch (markHandle.Location) {
                    case MemoMarkLocation.LeftTop: {
                        leftSides.Add(markHandle);
                        break;
                    }
                    case MemoMarkLocation.RightTop: {
                        rightSides.Add(markHandle);
                        break;
                    }
                }
            }

            var top = hostFigure.Top;

            /// 左側のhandleを順に並べる
            var leftSidesWidth = leftSides.Sum((handle) => handle.Figure.Width);
            var cLeft = hostFigure.Left - leftSidesWidth;
            foreach (var leftSide in leftSides) {
                var left = cLeft;
                leftSide.Figure.Location = new Point(left, top);
                cLeft += leftSide.Figure.Width;
            }

            /// 右側のhandleを順に並べる
            cLeft = hostFigure.Right;
            foreach (var rightSide in rightSides) {
                var left = cLeft;
                rightSide.Figure.Location = new Point(left, top);
                cLeft += rightSide.Figure.Width;
            }
        }

    }
}
