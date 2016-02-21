/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Handles;
using Mkamo.Memopad.Internal.Figures;
using Mkamo.Common.Core;
using Mkamo.Figure.Core;
using System.Windows.Forms;
using Mkamo.Figure.Figures;
using Mkamo.Common.Forms.Table;
using System.Drawing;
using Mkamo.Memopad.Internal.Requests;
using Mkamo.Editor.Handles.Scenarios;
using Mkamo.Memopad.Internal.Helpers;

namespace Mkamo.Memopad.Internal.Handles {
    internal class MemoTableRuledLineHandle: AbstractAuxiliaryHandle {
        // ========================================
        // field
        // ========================================
        private Lazy<MemoTableRuledLineHandleFigure> _figure;
        private Func<TableFigure> _tableFigureProvider;

        private ChangeRowHeightRequest _changeRowHeightRequest;
        private ChangeColumnWidthRequest _changeColumnWidthRequest;

        private SelectScenario _selectScenario;

        // --- dnd ---
        private Point _dragStartPoint;
        private int _lineIndex;
        private bool _isVertical;
        private bool _isDragStarted;

        // ========================================
        // constructor
        // ========================================
        public MemoTableRuledLineHandle(Func<TableFigure> tableFigureProvider) {
            _tableFigureProvider = tableFigureProvider;
            _figure = new Lazy<MemoTableRuledLineHandleFigure>(
                () => new MemoTableRuledLineHandleFigure(tableFigureProvider)
            );

            _changeRowHeightRequest = new ChangeRowHeightRequest();
            _changeColumnWidthRequest = new ChangeColumnWidthRequest();

            _dragStartPoint = Point.Empty;
            _lineIndex = 0;
            _isVertical = false;

            _selectScenario = new SelectScenario(this);
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override IFigure Figure {
            get { return _figure.Value; }
        }

        public override void Relocate(IFigure hostFigure) {
            _figure.Value.Bounds = hostFigure.Bounds;
        }

        protected override Cursor GetMouseCursor(MouseEventArgs e) {
            if (_figure.Value.IsOnVerticalLine(e.Location)) {
                return Cursors.VSplit;
            } else if (_figure.Value.IsOnHorizontalLine(e.Location)) {
                return Cursors.HSplit;
            } else {
                return base.GetMouseCursor(e);
            }
        }

        protected override void OnFigureMouseDown(MouseEventArgs e) {
            _selectScenario.HandleMouseDown(this, e);
            base.OnFigureMouseDown(e);
        }

        protected override void OnFigureMouseUp(MouseEventArgs e) {
            _selectScenario.HandleMouseUp(this, e);
            base.OnFigureMouseUp(e);
        }

        protected override void OnFigureMouseDoubleClick(MouseEventArgs e) {
            base.OnFigureMouseDoubleClick(e);

            _lineIndex = _figure.Value.GetVerticalLineIndex(e.Location);
            if (_lineIndex > -1) {
                TableFigureHelper.AdjustColumnSize(Host, _tableFigureProvider(), _lineIndex);
            } else {
                _lineIndex = _figure.Value.GetHorizontalLineIndex(e.Location);
                if (_lineIndex > -1) {
                    TableFigureHelper.AdjustRowSize(Host, _tableFigureProvider(), _lineIndex);
                }
            }
        }


        protected override void OnFigureDragStart(MouseEventArgs e) {
            _isDragStarted = false;
            _dragStartPoint = e.Location;

            _lineIndex = _figure.Value.GetVerticalLineIndex(e.Location);
            if (_lineIndex > -1) {
                _isVertical = true;
                _changeColumnWidthRequest.ColumnIndex = _lineIndex;
                _isDragStarted = true;
                Host.ShowFeedback(_changeColumnWidthRequest);

            } else {
                _lineIndex = _figure.Value.GetHorizontalLineIndex(e.Location);
                if (_lineIndex > -1) {
                    _isVertical = false;
                    _changeRowHeightRequest.RowIndex = _lineIndex;
                    _isDragStarted = true;
                    Host.ShowFeedback(_changeRowHeightRequest);
                }
            }

            base.OnFigureDragStart(e);
        }

        protected override void OnFigureDragMove(MouseEventArgs e) {
            if (_isDragStarted) {
                var tableData = _tableFigureProvider().TableData;
                var delta = e.Location - (Size) _dragStartPoint;

                if (_isVertical) {
                    var col = tableData.Columns.ElementAt(_lineIndex);
                    _changeColumnWidthRequest.ColumnIndex = _lineIndex;
                    _changeColumnWidthRequest.NewWidth = col.Width + delta.X;
                    Host.ShowFeedback(_changeColumnWidthRequest);

                } else {
                    var row = tableData.Rows.ElementAt(_lineIndex);
                    _changeRowHeightRequest.RowIndex = _lineIndex;
                    _changeRowHeightRequest.NewHeight = row.Height + delta.Y;
                    Host.ShowFeedback(_changeRowHeightRequest);
                }
            }

            base.OnFigureDragMove(e);
        }

        protected override void OnFigureDragFinish(MouseEventArgs e) {
            if (_isDragStarted) {
                var tableData = _tableFigureProvider().TableData;
                var delta = e.Location - (Size) _dragStartPoint;

                if (_isVertical) {
                    var col = tableData.Columns.ElementAt(_lineIndex);
                    _changeColumnWidthRequest.NewWidth = col.Width + delta.X;
                    Host.PerformRequest(_changeColumnWidthRequest);
                    Host.HideFeedback(_changeColumnWidthRequest);

                } else {
                    var row = tableData.Rows.ElementAt(_lineIndex);
                    _changeRowHeightRequest.NewHeight = row.Height + delta.Y;
                    Host.PerformRequest(_changeRowHeightRequest);
                    Host.HideFeedback(_changeRowHeightRequest);
                }
            }

            _isDragStarted = false;

            base.OnFigureDragFinish(e);
        }

        protected override void OnFigureDragCancel() {
            if (_isDragStarted) {
                if (_isVertical) {
                    Host.HideFeedback(_changeColumnWidthRequest);
                } else {
                    Host.HideFeedback(_changeRowHeightRequest);
                }
            }

            _isDragStarted = false;

            base.OnFigureDragCancel();
        }

    }
}
