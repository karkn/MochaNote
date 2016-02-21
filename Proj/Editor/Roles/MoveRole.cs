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

namespace Mkamo.Editor.Roles {
    public class MoveRole: AbstractRole {
        // ========================================
        // field
        // ========================================
        private bool _adjustToGrid;

        // ========================================
        // constructor
        // ========================================
        public MoveRole(bool adjustToGrid) {
            _adjustToGrid = adjustToGrid;
        }

        public MoveRole(): this(true) {
        }

        // ========================================
        // method
        // ========================================
        public override bool CanUnderstand(IRequest request) {
            return request.Id == RequestIds.ChangeBounds;
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as ChangeBoundsRequest;
            if (req == null) {
                return null;
            }

            var moveDelta = Size.Empty;
            var sizeDelta = Size.Empty;
            if (_adjustToGrid) {
                moveDelta = GetGridAdjustedMoveDelta(req.MoveDelta);
                sizeDelta = req.AdjustSize ? GetGridAdjustedSizeDelta(req.MoveDelta, req.SizeDelta) : req.SizeDelta;
            } else {
                moveDelta = req.MoveDelta;
                sizeDelta = req.SizeDelta;
            }
            return new ChangeBoundsCommand(
                _Host,
                moveDelta,
                sizeDelta,
                req.ResizeDirection,
                req.MovingEditors
            );
        }

        public override IFigure CreateFeedback(IRequest request) {
            var req = request as ChangeBoundsRequest;
            if (req == null) {
                return null;
            }

            var feedback = _Host.Figure.CloneFigure();

            {
                var node = feedback as INode;
                if (node != null) {
                    var image = node as ImageFigure;
                    if (image != null) {
                        /// サイズ変更してもキャッシュされたimageをそのまま引き延ばして使う
                        image.NeedRecreateImageOnBoundsChanged = false;
                    } else {
                        if (!node.IsForegroundEnabled) {
                            node.IsForegroundEnabled = true;
                            node.Foreground = Color.Silver;
                        }
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
            var req = request as ChangeBoundsRequest;
            if (req == null) {
                return;
            }

            // todo: InstallRole()時に指定させるべきか?
            /// TableFigureのresize
            {
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
                }
            }

            feedback.Bounds = GetNewBounds(_Host.Figure.Bounds, req);
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {

        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual Rectangle GetNewBounds(Rectangle oldBounds, ChangeBoundsRequest request) {
            var r = new Rectangle(oldBounds.Location + request.MoveDelta, oldBounds.Size);
            if (_adjustToGrid) {
                return GetGridAdjustedRect(r, request.AdjustSize);
            } else {
                return r;
            }
        }
    }
}
