/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Figures;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Requests;

namespace Mkamo.Memopad.Internal.Helpers {
    internal static class TableFigureHelper {
        // ========================================
        // static field
        // ========================================
        public static void AdjustRowAndColumnSizes(TableFigure tableFig) {
            if (tableFig == null) {
                return;
            }

            foreach (var row in tableFig.TableData.Rows) {
                var height = row.Cells.Where(cell => !(cell.IsMerging && cell.RowSpan != 1) && !cell.IsMerged).Max(cell => cell.Value.StyledTextBounds.Height);
                row.Height = height + tableFig.Padding.Height;
            }
            foreach (var col in tableFig.TableData.Columns) {
                var width = col.Cells.Where(cell => !(cell.IsMerging && cell.ColumnSpan != 1) && !cell.IsMerged).Max(cell => cell.Value.StyledTextBounds.Width);
                col.Width = width + tableFig.Padding.Width;
            }
        }

        //public static void AdjustAllRowSizes(TableFigure tableFig) {
        //    if (tableFig == null) {
        //        return;
        //    }

        //    foreach (var row in tableFig.TableData.Rows) {
        //        var height = row.Cells.Where(cell => !(cell.IsMerging && cell.RowSpan != 1) && !cell.IsMerged).Max(cell => cell.Value.StyledTextBounds.Height);
        //        row.Height = height + tableFig.Padding.Height;
        //    }
        //}

        public static void AdjustRowSizesAtEvenInterval(IEditor tableEditor, TableFigure tableFig, int firstRowIndex, int lastRowIndex) {
            if (tableEditor == null || tableFig == null) {
                return;
            }

            if (firstRowIndex < 0 || lastRowIndex > tableFig.TableData.RowCount - 1 || firstRowIndex >= lastRowIndex) {
                return;
            }

            /// calc height
            var rowIndex = 0;
            var height = 0;
            foreach (var row in tableFig.TableData.Rows) {
                if (rowIndex >= firstRowIndex && rowIndex <= lastRowIndex) {
                    height += row.Height;
                }
                ++rowIndex;
            }

            var rowCount = lastRowIndex - firstRowIndex + 1;
            var rowHeight = height / rowCount;
            var lastRowHeight = height - (rowHeight * (rowCount - 1));
            rowIndex = 0;
            foreach (var row in tableFig.TableData.Rows) {
                if (rowIndex >= firstRowIndex && rowIndex <= lastRowIndex) {
                    var req = new ChangeRowHeightRequest(rowIndex, rowIndex == lastRowIndex ? lastRowHeight : rowHeight);
                    tableEditor.PerformRequest(req);
                }
                ++rowIndex;
            }
        }

        public static void AdjustRowSize(IEditor tableEditor, TableFigure tableFig, int rowIndex) {
            if (tableEditor == null || tableFig == null) {
                return;
            }

            if (rowIndex < 0 || rowIndex >= tableFig.TableData.RowCount) {
                throw new ArgumentException("rowIndex");
            }

            var row = tableFig.TableData.Rows.ElementAt(rowIndex);
            var height = row.Cells.Where(cell => !(cell.IsMerging && cell.RowSpan != 1) && !cell.IsMerged).Max(cell => cell.Value.StyledTextBounds.Height);
            var req = new ChangeRowHeightRequest(rowIndex, height + tableFig.Padding.Height);
            tableEditor.PerformRequest(req);
        }

        //public static void AdjustAllColumnSizes(TableFigure tableFig) {
        //    if (tableFig == null) {
        //        return;
        //    }

        //    foreach (var col in tableFig.TableData.Columns) {
        //        var width = col.Cells.Where(cell => !(cell.IsMerging && cell.ColumnSpan != 1) && !cell.IsMerged).Max(cell => cell.Value.StyledTextBounds.Width);
        //        col.Width = width + tableFig.Padding.Width;
        //    }
        //}

        public static void AdjustColumnSizesAtEvenInterval(IEditor tableEditor, TableFigure tableFig, int firstColumnIndex, int lastColumnIndex) {
            if (tableEditor == null || tableFig == null) {
                return;
            }

            if (firstColumnIndex < 0 || lastColumnIndex > tableFig.TableData.ColumnCount - 1 || firstColumnIndex >= lastColumnIndex) {
                return;
            }

            /// calc width
            var colIndex = 0;
            var width = 0;
            foreach (var col in tableFig.TableData.Columns) {
                if (colIndex >= firstColumnIndex && colIndex <= lastColumnIndex) {
                    width += col.Width;
                }
                ++colIndex;
            }

            var colCount = lastColumnIndex - firstColumnIndex + 1;
            var colWidth = width / colCount;
            var lastColWidth = width - (colWidth * (colCount - 1));
            colIndex = 0;
            foreach (var col in tableFig.TableData.Columns) {
                if (colIndex >= firstColumnIndex && colIndex <= lastColumnIndex) {
                    var req = new ChangeColumnWidthRequest(colIndex, colIndex == lastColumnIndex ? lastColWidth : colWidth);
                    tableEditor.PerformRequest(req);
                }
                ++colIndex;
            }
        }

        public static void AdjustColumnSize(IEditor tableEditor, TableFigure tableFig, int colIndex) {
            if (tableEditor == null || tableFig == null) {
                return;
            }

            if (colIndex < 0 || colIndex >= tableFig.TableData.ColumnCount) {
                throw new ArgumentException("colIndex");
            }

            var col = tableFig.TableData.Columns.ElementAt(colIndex);
            var width = col.Cells.Where(cell => !(cell.IsMerging && cell.ColumnSpan != 1) && !cell.IsMerged).Max(cell => cell.Value.StyledTextBounds.Width);
            var req = new ChangeColumnWidthRequest(colIndex, width + tableFig.Padding.Width);
            tableEditor.PerformRequest(req);
        }

    }
}
