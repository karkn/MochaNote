/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Figures;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using Mkamo.Common.Externalize;
using Mkamo.Figure.Core;
using System.Windows.Forms;
using Mkamo.Common.DataType;
using Mkamo.Editor.Core;
using Mkamo.Editor.Handles;

namespace Mkamo.Memopad.Internal.Handles {
    internal class MemoTextFrameHandle: CompositeHandle {
        // ========================================
        // constructor
        // ========================================
        public MemoTextFrameHandle(
            int hitMargin, int frameWidth, Size cornerSize, Color color, Color moveFore, IBrushDescription moveBack
        ): base(false) {
            Children.Add(new FrameResizeHandle(hitMargin, frameWidth, cornerSize, color));
            Children.Add(new MemoTextFrameMoveAndCombineHandle(cornerSize.Height, frameWidth, moveFore, moveBack));
            Children.Add(new MemoTextWidthHandle());
        }

    }


}
