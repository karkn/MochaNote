/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Requests;
using Mkamo.Figure.Core;
using Mkamo.Editor.Commands;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Figure.Figures;
using Mkamo.Common.Command;
using Mkamo.Common.Core;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Editor.Roles;
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Internal.Requests;
using Mkamo.Memopad.Internal.Commands;

namespace Mkamo.Memopad.Internal.Roles {
    internal class ChangeRowHeightRole: AbstractRole {
        // ========================================
        // constructor
        // ========================================
        public ChangeRowHeightRole() {
        }

        // ========================================
        // method
        // ========================================
        public override bool CanUnderstand(IRequest request) {
            return request.Id == MemopadRequestIds.ChangeRowHeight;
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as ChangeRowHeightRequest;
            if (req == null) {
                return null;
            }

            return new ChangeRowHeightCommand(
                _Host.Figure as TableFigure,
                req.RowIndex,
                req.NewHeight
            );
        }

        public override IFigure CreateFeedback(IRequest request) {
            var req = request as ChangeRowHeightRequest;
            if (req == null) {
                return null;
            }

            var feedback = _Host.Figure.CloneFigure();

            {
                var node = feedback as INode;
                if (node != null) {
                    if (!node.IsForegroundEnabled) {
                        node.IsForegroundEnabled = true;
                        node.Foreground = Color.Silver;
                    }
                }
            }

            feedback.Accept(
                elem => {
                    elem.MakeTransparent(0.5f);
                    return false;
                }
            );

            UpdateFeedback(req, feedback);
            return feedback;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
            var req = request as ChangeRowHeightRequest;
            if (req == null) {
                return;
            }

            var hostTable = _Host.Figure as TableFigure;
            var feedbackTable = feedback as TableFigure;
            if (hostTable != null && feedbackTable != null) {

                /// 必要ないし重くなるのでRow.HeightやColumn.Widthの変更で
                /// feedbackTableをInvalidationさせない
                feedbackTable.SuppressInvalidation = true;

                /// row heightをもとにもどす
                for (int i = 0, len = hostTable.TableData.Rows.Count(); i < len; ++i) {
                    var hostRow = hostTable.TableData.Rows.ElementAt(i);
                    var feedbackRow = feedbackTable.TableData.Rows.ElementAt(i);
                    feedbackRow.Height = hostRow.Height;
                }
                /// col widthをもとにもどす
                for (int i = 0, len = hostTable.TableData.Columns.Count(); i < len; ++i) {
                    var hostCol = hostTable.TableData.Columns.ElementAt(i);
                    var feedbackCol = feedbackTable.TableData.Columns.ElementAt(i);
                    feedbackCol.Width = hostCol.Width;
                }

                feedbackTable.SuppressInvalidation = false;

                /// 新しい値に設定
                var row = feedbackTable.TableData.Rows.ElementAt(req.RowIndex);
                row.Height = req.NewHeight;
            }
            
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {

        }
    }
}
