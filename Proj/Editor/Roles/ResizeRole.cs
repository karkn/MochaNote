/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Commands;
using Mkamo.Figure.Figures;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.DataType;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Core;

namespace Mkamo.Editor.Roles {
    public class ResizeRole: MoveRole {
        protected override Rectangle GetNewBounds(Rectangle oldBounds, ChangeBoundsRequest request) {
            var moveDelta = request.MoveDelta;
            var sizeDelta = request.SizeDelta;

            var left = GetGridAdjustedX(oldBounds.Left + moveDelta.Width);
            var top = GetGridAdjustedY(oldBounds.Top + moveDelta.Height);

            var width = oldBounds.Width;
            var height = oldBounds.Height;

            if (!sizeDelta.IsEmpty) {
                // todo: request.AdjustSize == false対応
                var node = _Host.Figure as INode;
                var gridAdLeft = GetGridAdjustedX(left + sizeDelta.Width);
                var gridAdWidth = width - (gridAdLeft - left);
                var gridAdTop = GetGridAdjustedY(top + sizeDelta.Height);
                var gridAdHeight = height - (gridAdTop - top);
                var gridAdSize = node.MeasureAutoSize(new Size(gridAdWidth, gridAdHeight));
                
                if (EnumUtil.HasAllFlags((int) request.ResizeDirection, (int) Directions.Left)) {
                    width = gridAdSize.Width;
                    left = left - (width - oldBounds.Width);
                }
                if (EnumUtil.HasAllFlags((int) request.ResizeDirection, (int) Directions.Up)) {
                    height = gridAdSize.Height;
                    top = top - (height - oldBounds.Height);
                }
                if (EnumUtil.HasAllFlags((int) request.ResizeDirection, (int) Directions.Right)) {
                    width = width + sizeDelta.Width + GetGridAdjustedDiffX(left + width + sizeDelta.Width);
                }
                if (EnumUtil.HasAllFlags((int) request.ResizeDirection, (int) Directions.Down)) {
                    height = height + sizeDelta.Height + GetGridAdjustedDiffY(top + height + sizeDelta.Height);
                }
            }

            return new Rectangle(left, top, width, height);
        }
    }
}
