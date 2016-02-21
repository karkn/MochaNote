/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Figure.Figures;
using Mkamo.Common.Diagnostics;
using System.Drawing;
using Mkamo.Common.Externalize;
using System.Diagnostics;

namespace Mkamo.Figure.Layouts {

    [Externalizable]
    public class TableLayout: AbstractLayout {
        // ========================================
        // static field
        // ========================================

        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        public TableLayout() {
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override void Arrange(IFigure parent) {
            var tableFig = parent as TableFigure;
            Contract.Requires(tableFig != null);

            var tableData = tableFig.TableData;

            if (!tableData.Rows.Any() || !tableData.Columns.Any()) {
                return;
            }

            var cTop = tableFig.Top;
            var rowSpanSkips = new Dictionary<int, int>(); /// colIndex, skip
            var lastRow = tableData.Rows.Last();
            var lastCol = tableData.Columns.Last();
            foreach (var row in tableData.Rows) {
                var colSpanSkip = 0;
                var cLeft = tableFig.Left;
                var rowHeight = 0;
                var lastCell = row.Cells.Last();
                foreach (var cell in row.Cells) {
                    var cellFig = cell.Value;

                    /// colspan範囲内なので飛ばす
                    if (colSpanSkip > 0) {
                        if (cellFig != null) {
                            cellFig.IsVisible = false;
                        }
                        --colSpanSkip;
                        continue;
                    }

                    var col = tableData.GetColumn(cell);
                    rowHeight = row.Height;
                    var width = col.Width;
                    var height = row.Height;

                    /// rowspan範囲内なので飛ばす
                    if (rowSpanSkips.ContainsKey(col.ColumnIndex)) {
                        var rowSpanSkip = rowSpanSkips[col.ColumnIndex];
                        if (rowSpanSkip > 0) {
                            if (cellFig != null) {
                                cellFig.IsVisible = false;
                            }
                            --rowSpanSkip;
                        }
                        if (rowSpanSkip == 0) {
                            rowSpanSkips.Remove(col.ColumnIndex);
                        } else {
                            rowSpanSkips[col.ColumnIndex] = rowSpanSkip;
                        }
                        cLeft += cell == lastCell? width - 1: width;
                        continue;
                    }

                    var colSpan = cell.ColumnSpan;
                    var rowSpan = cell.RowSpan;

                    if (cellFig != null) {
                        cellFig.IsVisible = true;
                    }
                    colSpanSkip = colSpan - 1;
                    if (rowSpan > 1) {
                        for (int i = colSpan, c = 0; i > 0; --i, ++c) {
                            rowSpanSkips[col.ColumnIndex + c] = rowSpan - 1;
                        }
                    }

                    /// colSpan分widthを足す
                    {
                        var colSpanLeft = colSpan;
                        var nextCol = col;
                        while (colSpanLeft > 1 && tableData.HasNextColumn(nextCol)) {
                            nextCol = tableData.GetNextColumn(nextCol);
                            width += nextCol == lastCol? nextCol.Width - 1: nextCol.Width;
                            --colSpanLeft;
                        }
                    }

                    /// rowSpan分heightを足す
                    {
                        var rowSpanLeft = rowSpan;
                        var nextRow = row;
                        while (rowSpanLeft > 1 && tableData.HasNextRow(nextRow)) {
                            nextRow = tableData.GetNextRow(nextRow);
                            height += nextRow == lastRow? nextRow.Height - 1: nextRow.Height;
                            --rowSpanLeft;
                        }
                    }

                    if (cellFig != null) {
                        var figWidth = cell == lastCell? width: width + 1;
                        var figHeight = row == lastRow? height: height + 1;
                        cellFig.Bounds = new Rectangle(cLeft, cTop, figWidth, figHeight);
                    }
                    cLeft += width;
                }
                cTop += rowHeight;
            }

        }

        public override Size Measure(IFigure parent, SizeConstraint constraint) {
            var tableFig = parent as TableFigure;
            Contract.Requires(tableFig != null);

            var tableData = tableFig.TableData;

            var width = tableData.Columns.Sum(col => col.Width);
            var height = tableData.Rows.Sum(row => row.Height);
            return constraint.MeasureConstrainedSize(new Size(width, height));
        }
    }
}
