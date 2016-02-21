/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Roles;
using Mkamo.Common.Command;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Commands;
using Mkamo.Figure.Core;
using Mkamo.Editor.Core;
using System.Drawing;

namespace Mkamo.Memopad.Internal.Roles {
    internal class MemoTextResizeRole: ResizeRole {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override ICommand CreateCommand(IRequest request) {
            var req = request as ChangeBoundsRequest;
            if (req == null) {
                return null;
            }

            /// ただの移動なら普通の処理
            if (req.SizeDelta.IsEmpty) {
                return base.CreateCommand(req);
            }

            
            var node = (INode) _Host.Figure;
            var oldAutoSizeKind = node.AutoSizeKinds;
            var oldMaxWidth = node.MaxSize;

            var newMaxWidth = new Size(
                node.Width + req.SizeDelta.Width + GetGridAdjustedDiffX(node.Left + node.Width + req.SizeDelta.Width),
                node.MaxSize.Height
            );

            var maxSizeCmd = new DelegatingCommand(
                () => {
                    node.AutoSizeKinds = AutoSizeKinds.FitHeight;
                    node.MaxSize = newMaxWidth;
                    node.AdjustSize();
                },
                () => {
                    node.AutoSizeKinds = oldAutoSizeKind;
                    node.MaxSize = oldMaxWidth;
                    node.AdjustSize();
                }
            );

            var resizeCmd = base.CreateCommand(req);

            if (req.SizeDelta.Width < 0) {
                return resizeCmd.Chain(maxSizeCmd);
            } else {
                return maxSizeCmd.Chain(resizeCmd);
            }

        }

        public override IFigure CreateFeedback(IRequest request) {
            var ret = base.CreateFeedback(request);
            var node = (INode) ret;
            node.AutoSizeKinds = AutoSizeKinds.FitHeight;
            return ret;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
            var req = request as ChangeBoundsRequest;
            if (req == null) {
                return;
            }

            if (!req.SizeDelta.IsEmpty) {
                var newBounds = GetNewBounds(_Host.Figure.Bounds, req);
                ((INode) feedback).MaxSize = new Size(newBounds.Width, int.MaxValue);
            }
            base.UpdateFeedback(request, feedback);
        }

    }
}
