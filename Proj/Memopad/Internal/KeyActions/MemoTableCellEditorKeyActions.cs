/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Figure.Figures;
using Mkamo.Figure.Core;
using Mkamo.Memopad.Internal.Commands;
using Mkamo.Model.Memo;
using Mkamo.Common.Command;
using Mkamo.Common.Forms.KeyMap;
using System.Reflection;

namespace Mkamo.Memopad.Internal.KeyActions {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    internal static class MemoTableCellEditorKeyActions {
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
        [KeyAction("")]
        public static void MoveLeftCell(IEditor editor) {
            var parent = editor.Parent;
            var tableFig = parent.Figure as TableFigure;
            var tableData = tableFig.TableData;

            var cellFig = editor.Figure as TableCellFigure;
            var cell = tableData.GetCell(cellFig);

            if (tableData.HasPreviousCell(cell)) {
                var prevCell = tableData.GetPreviousCell(cell);
                if (prevCell.IsMerged) {
                    prevCell = prevCell.Merging;
                }

                if (!prevCell.IsMerged) {
                    var prevCellFig = prevCell.Value;
                    SelectEditor(parent, prevCellFig);
                }
            }
        }

        [KeyAction("")]
        public static void MoveRightCell(IEditor editor) {
            var parent = editor.Parent;
            var tableFig = parent.Figure as TableFigure;
            var tableData = tableFig.TableData;

            var cellFig = editor.Figure as TableCellFigure;
            var cell = tableData.GetCell(cellFig);

            if (tableData.HasNextCell(cell)) {
                var nextCell = tableData.GetNextCell(cell);
                while (
                    nextCell.IsMerged &&
                    nextCell.Merging == cell &&
                    tableData.HasNextCell(nextCell)
                ) {
                    nextCell = tableData.GetNextCell(nextCell);
                }

                if (nextCell.IsMerged) {
                    nextCell = nextCell.Merging;
                }

                if (!nextCell.IsMerged) {
                    var nextCellFig = nextCell.Value;
                    SelectEditor(parent, nextCellFig);
                }
            }
        }

        [KeyAction("")]
        public static void MoveUpCell(IEditor editor) {
            var parent = editor.Parent;
            var tableFig = parent.Figure as TableFigure;
            var tableData = tableFig.TableData;

            var cellFig = editor.Figure as TableCellFigure;
            var cell = tableData.GetCell(cellFig);

            if (tableData.HasPreviousRowCell(cell)) {
                var prevRowCell = tableData.GetPreviousRowCell(cell);
                while (
                    prevRowCell.IsMerged &&
                    prevRowCell.Merging == cell &&
                    tableData.HasPreviousRowCell(prevRowCell)
                ) {
                    prevRowCell = tableData.GetPreviousRowCell(prevRowCell);
                }

                if (prevRowCell.IsMerged) {
                    prevRowCell = prevRowCell.Merging;
                }

                if (prevRowCell.Value.IsVisible) {
                    var prevRowCellFig = prevRowCell.Value;
                    SelectEditor(parent, prevRowCellFig);
                }
            }
        }

        [KeyAction("")]
        public static void MoveDownCell(IEditor editor) {
            var parent = editor.Parent;
            var tableFig = parent.Figure as TableFigure;
            var tableData = tableFig.TableData;

            var cellFig = editor.Figure as TableCellFigure;
            var cell = tableData.GetCell(cellFig);

            if (tableData.HasNextRowCell(cell)) {
                var nextRowCell = tableData.GetNextRowCell(cell);
                while (
                    nextRowCell.IsMerged &&
                    nextRowCell.Merging == cell &&
                    tableData.HasNextRowCell(nextRowCell)
                ) {
                    nextRowCell = tableData.GetNextRowCell(nextRowCell);
                }

                if (nextRowCell.IsMerged) {
                    nextRowCell = nextRowCell.Merging;
                }

                if (nextRowCell.Value.IsVisible) {
                    var nextRowCellFig = nextRowCell.Value;
                    SelectEditor(parent, nextRowCellFig);
                }
            }
        }

        [KeyAction("")]
        public static void MoveLeftmostCell(IEditor editor) {
            var parent = editor.Parent;
            var tableFig = parent.Figure as TableFigure;
            var tableData = tableFig.TableData;

            var cellFig = editor.Figure as TableCellFigure;
            var cell = tableData.GetCell(cellFig);

            var row = tableData.GetRow(cell);
            var leftmostCell = row.Cells.First();
            if (leftmostCell.Value.IsVisible) {
                var leftmostCellFig = leftmostCell.Value;
                SelectEditor(parent, leftmostCellFig);
            }
        }

        [KeyAction("")]
        public static void MoveRightmostCell(IEditor editor) {
            var parent = editor.Parent;
            var tableFig = parent.Figure as TableFigure;
            var tableData = tableFig.TableData;

            var cellFig = editor.Figure as TableCellFigure;
            var cell = tableData.GetCell(cellFig);

            var row = tableData.GetRow(cell);
            var rightmostCell = row.Cells.Last();

            if (rightmostCell.IsMerged) {
                rightmostCell = rightmostCell.Merging;
            }

            if (rightmostCell.Value.IsVisible) {
                var rightmostCellFig = rightmostCell.Value;
                SelectEditor(parent, rightmostCellFig);
            }
        }

        [KeyAction("")]
        public static void MoveRightCellOrCreateRow(IEditor editor) {
            var parent = editor.Parent;
            var tableFig = parent.Figure as TableFigure;
            var tableData = tableFig.TableData;

            var cellFig = editor.Figure as TableCellFigure;
            var cell = tableData.GetCell(cellFig);

            if (tableData.HasNextCell(cell)) {
                var colIndex = tableData.GetColumnIndex(cell);
                var rowIndex = tableData.GetRowIndex(cell);
                if (
                    colIndex + cell.ColumnSpan == tableData.ColumnCount &&
                    rowIndex + cell.RowSpan == tableData.RowCount
                ) {
                    var model = parent.Model as MemoTable;
                    var cmd = new InsertTableRowCommand(model, model.RowCount);
                    editor.Site.CommandExecutor.Execute(cmd);

                    var nextCell = tableData.GetCell(rowIndex + cell.RowSpan, 0);
                    var nextCellFig = nextCell.Value;
                    SelectEditor(parent, nextCellFig);

                } else {
                    var nextCell = tableData.GetNextCell(cell);
                    while (
                        nextCell.IsMerged &&
                        nextCell.Merging == cell &&
                        tableData.HasNextCell(nextCell)
                    ) {
                        nextCell = tableData.GetNextCell(nextCell);
                    }

                    if (nextCell.IsMerged) {
                        nextCell = nextCell.Merging;
                    }

                    if (nextCell.Value.IsVisible) {
                        var nextCellFig = nextCell.Value;
                        SelectEditor(parent, nextCellFig);
                    }
                }

            } else {
                var model = parent.Model as MemoTable;
                var cmd = new InsertTableRowCommand(model, model.RowCount);
                editor.Site.CommandExecutor.Execute(cmd);

                var nextCell = tableData.GetNextCell(cell);
                var nextCellFig = nextCell.Value;
                SelectEditor(parent, nextCellFig);
            }
        }

        [KeyAction("")]
        public static void EmptyCell(IEditor editor) {
            var model = editor.Model as MemoTableCell;
            if (model != null) {
                var oldStext = model.StyledText;
                var newStext = new StyledText.Core.StyledText();

                var cmd = new DelegatingCommand(
                    () =>model.StyledText = newStext,
                    () =>model.StyledText = oldStext
                );
                editor.Site.CommandExecutor.Execute(cmd);
            }
        }

        [KeyAction("")]
        public static void ScrollRecenter(IEditor editor) {
            var canvas = editor.Site.EditorCanvas;
            canvas.ScrollRecenter();
        }

        // --- focus ---
        [KeyAction("")]
        public static void BeginFocus(IEditor editor) {
            editor.RequestFocus(FocusKind.Begin, null);
        }

        // ------------------------------
        // private
        // ------------------------------
        private static void SelectEditor(IEditor tableEditor, IFigure cellFigure) {
            var nextEditor = cellFigure.GetEditor();
            if (nextEditor != null) {
                nextEditor.RequestSelect(SelectKind.True, true);
            }
        }
    }

}
