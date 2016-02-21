/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Controllers;
using System.Windows.Forms;
using Mkamo.Editor.Forms;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Figure.Figures;
using Mkamo.Model.Memo;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Commands;
using Mkamo.Editor.Requests;
using ICommandExecutor = Mkamo.Common.Command.ICommandExecutor;
using Mkamo.Common.Forms.Table;
using Mkamo.Common.Command;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Memopad.Internal.Core;

namespace Mkamo.Memopad.Internal.Controllers.UIProviders {

    internal class MemoTableCellUIProvider: AbstractMemoContentUIProvider {
        // ========================================
        // field
        // ========================================
        private MemoTableCellController _owner;

        private ToolStripMenuItem _addPreviousColumn;
        private ToolStripMenuItem _addNextColumn;

        private ToolStripMenuItem _addPreviousRow;
        private ToolStripMenuItem _addNextRow;

        private ToolStripMenuItem _removeRow;
        private ToolStripMenuItem _removeColumn;
        private ToolStripMenuItem _removeTable;

        private ToolStripMenuItem _mergeCells;
        private ToolStripMenuItem _unmergeCells;

        private ToolStripMenuItem _adjustRowHeight;
        private ToolStripMenuItem _adjustColumnWidth;
        private ToolStripMenuItem _adjustRowHeightsAtEvenInterval;
        private ToolStripMenuItem _adjustColumnWidthsAtEvenInterval;


        private ToolStripMenuItem _copyTable;

        private ToolStripMenuItem _export;

        // ========================================
        // constructor
        // ========================================
        public MemoTableCellUIProvider(MemoTableCellController owner): base(owner, false) {
            _owner = owner;
        }

        // ========================================
        // method
        // ========================================
        public override ContextMenuStrip GetContextMenu(MouseEventArgs e) {
            if (_addPreviousColumn == null) {
                InitItems();
            }

            _ContextMenu.Items.Clear();

            var rowCount = GetRowCount();
            var colCount = GetColumnCount();

            _ContextMenu.Items.Add(_export);
            _ContextMenu.Items.Add(_Separator1);

            _ContextMenu.Items.Add(_addPreviousColumn);
            _ContextMenu.Items.Add(_addNextColumn);
            _ContextMenu.Items.Add(_addPreviousRow);
            _ContextMenu.Items.Add(_addNextRow);

            _ContextMenu.Items.Add(_Separator2);

            if (rowCount > 1) {
                _ContextMenu.Items.Add(_removeRow);
            }
            if (colCount > 1) {
                _ContextMenu.Items.Add(_removeColumn);
            }
            _ContextMenu.Items.Add(_removeTable);

            _ContextMenu.Items.Add(_Separator3);

            _ContextMenu.Items.Add(_adjustRowHeight);
            _ContextMenu.Items.Add(_adjustColumnWidth);

            _ContextMenu.Items.Add(_Separator4);

            _ContextMenu.Items.Add(_adjustRowHeightsAtEvenInterval);
            _ContextMenu.Items.Add(_adjustColumnWidthsAtEvenInterval);

            _ContextMenu.Items.Add(_Separator5);

            _ContextMenu.Items.Add(_mergeCells);
            _ContextMenu.Items.Add(_unmergeCells);

            _ContextMenu.Items.Add(_Separator6);
            _ContextMenu.Items.Add(_copyTable);

            _ContextMenu.Items.Add(_Separator7);
            _ContextMenu.Items.Add(_CutInNewMemo);

            UpdateEnabled();

            return _ContextMenu;
        }

        protected override IEnumerable<IEditor> GetCutInNewMemoTargets() {
            return new [] { _owner.Host.Parent };
        }
        

        protected virtual void InitItems() {

            _export = new ToolStripMenuItem("取り出す(&E)");
            var exportCsv = new ToolStripMenuItem("CSV(&C)...");
            exportCsv.Click += (sender, e) => {
                var memoFile = _owner.Model;
                var dialog = new SaveFileDialog();
                dialog.RestoreDirectory = true;
                dialog.ShowHelp = true;
                dialog.FileName = "Export.csv";
                dialog.Filter = "Csv Files(*.csv)|*.csv";
                if (dialog.ShowDialog() == DialogResult.OK) {
                    var outputPath = dialog.FileName;
                    _owner.Host.Parent.RequestExport("Csv", outputPath);
                }
            };
            _export.DropDownItems.Add(exportCsv);

            _addPreviousColumn = new ToolStripMenuItem();
            _addPreviousColumn.Text = "左に列を挿入(&L)";
            _addPreviousColumn.Click += (sender, ev) => {
                var tableModel = GetTableModel();
                var col = GetColumnIndex();
                GetExecutor().Execute(new InsertTableColumnCommand(tableModel, col));
            };

            _addNextColumn = new ToolStripMenuItem();
            _addNextColumn.Text = "右に列を挿入(&R)";
            _addNextColumn.Click += (sender, ev) => {
                var tableModel = GetTableModel();
                var col = GetColumnIndex();
                GetExecutor().Execute(new InsertTableColumnCommand(tableModel, col + 1));
            };

            _addPreviousRow = new ToolStripMenuItem();
            _addPreviousRow.Text = "上に行を挿入(&U)";
            _addPreviousRow.Click += (sender, ev) => {
                var tableModel = GetTableModel();
                var row = GetRowIndex();
                GetExecutor().Execute(new InsertTableRowCommand(tableModel, row));
            };

            _addNextRow = new ToolStripMenuItem();
            _addNextRow.Text = "下に行を挿入(&D)";
            _addNextRow.Click += (sender, ev) => {
                var tableModel = GetTableModel();
                var row = GetRowIndex();
                GetExecutor().Execute(new InsertTableRowCommand(tableModel, row + 1));
            };


            
            _removeRow = new ToolStripMenuItem();
            _removeRow.Text = "行を削除(&R)";
            _removeRow.Click += (sender, ev) => {
                var rowIndex = GetRowIndex();


                var host = _owner.Host;
                var tableEditor = host.Parent;
                var tableFig = GetTableFigure();
                var tableData = tableFig.TableData;

                var cmd = default(ICommand);
                var unmerged = new HashSet<Cell<TableCellFigure>>();
                var row = tableData.Rows.ElementAt(rowIndex);
                var colIndex = 0;
                foreach (var cell in row.Cells) {
                    if (cell.IsMerging) {
                        var rs = cell.RowSpan;
                        var cs = cell.ColumnSpan;
                        cmd = new MergeTableCellsCommand(tableEditor, colIndex, rowIndex, 1, 1);
                        if (rowIndex < tableData.RowCount - 1) {
                            cmd = cmd.Chain(new MergeTableCellsCommand(tableEditor, colIndex, rowIndex + 1, cs, rs - 1));
                        }
                        unmerged.Add(cell);

                    } else if (cell.IsMerged) {
                        var merging = cell.Merging;
                        if (!unmerged.Contains(merging)) {
                            var mColIndex = tableData.GetColumnIndex(merging);
                            var mRowIndex = tableData.GetRowIndex(merging);
                            cmd = new MergeTableCellsCommand(tableEditor, mColIndex, mRowIndex, merging.ColumnSpan, merging.RowSpan - 1);
                        }
                    }
                    ++colIndex;
                }

                var remove = new RemoveTableRowCommand(_owner.Host.Parent, rowIndex);
                cmd = cmd == null ? remove : cmd.Chain(remove);
                GetExecutor().Execute(cmd);
            };

            _removeColumn = new ToolStripMenuItem();
            _removeColumn.Text = "列を削除(&C)";
            _removeColumn.Click += (sender, ev) => {
                var colIndex = GetColumnIndex();

                var host = _owner.Host;
                var tableEditor = host.Parent;
                var tableFig = GetTableFigure();
                var tableData = tableFig.TableData;

                var cmd = default(ICommand);
                var unmerged = new HashSet<Cell<TableCellFigure>>();
                var col = tableData.Columns.ElementAt(colIndex);
                var rowIndex = 0;
                foreach (var cell in col.Cells) {
                    if (cell.IsMerging) {
                        var rs = cell.RowSpan;
                        var cs = cell.ColumnSpan;
                        cmd = new MergeTableCellsCommand(tableEditor, colIndex, rowIndex, 1, 1);
                        if (colIndex < tableData.ColumnCount - 1) {
                            cmd = cmd.Chain(new MergeTableCellsCommand(tableEditor, colIndex + 1, rowIndex, cs - 1, rs));
                        }
                        unmerged.Add(cell);

                    } else if (cell.IsMerged) {
                        var merging = cell.Merging;
                        if (!unmerged.Contains(merging)) {
                            var mColIndex = tableData.GetColumnIndex(merging);
                            var mRowIndex = tableData.GetRowIndex(merging);
                            cmd = new MergeTableCellsCommand(tableEditor, mColIndex, mRowIndex, merging.ColumnSpan - 1, merging.RowSpan);
                        }
                    }
                    ++rowIndex;
                }

                var remove = new RemoveTableColumnCommand(_owner.Host.Parent, colIndex);
                cmd = cmd == null ? remove : cmd.Chain(remove);
                GetExecutor().Execute(cmd);
            };

            _removeTable = new ToolStripMenuItem();
            _removeTable.Text = "表を削除(&T)";
            _removeTable.Click += (sender, ev) => {
                var canvas = _owner.Host.Site.EditorCanvas;
                var oldCursor = canvas.Cursor;
                canvas.Cursor = Cursors.WaitCursor;

                /// ここでsite変数に入れておかないとRemoveRequest後には
                /// _ownerからはたどれなくなってしまう
                var site = _owner.Host.Site;

                try {
                    _owner.Host.RequestSelect(SelectKind.False, true);

                    site.SuppressUpdateHandleLayer = true;
                    var parent = _owner.Host.Parent;
                    parent.PerformRequest(new RemoveRequest());
                    site.SuppressUpdateHandleLayer = false;
                    site.UpdateHandleLayer();
                } finally {
                    canvas.Cursor = oldCursor;
                }
            };

            _adjustRowHeight = new ToolStripMenuItem("行の高さを調節(&H)");
            _adjustRowHeight.Click += (sender, ev) => {
                var edi = GetTableEditor();
                var fig = GetTableFigure();
                using (edi.Site.CommandExecutor.BeginChain()) {
                    int fr, lr, fc, lc;
                    GetSelectedCellRange(out fr, out lr, out fc, out lc);
                    for (int i = fr; i <= lr; ++i) {
                        TableFigureHelper.AdjustRowSize(edi, fig, i);
                    }
                }
            };

            _adjustColumnWidth = new ToolStripMenuItem("列の幅を調節(&W)");
            _adjustColumnWidth.Click += (sender, ev) => {
                var edi = GetTableEditor();
                var fig = GetTableFigure();
                using (edi.Site.CommandExecutor.BeginChain()) {
                    int fr, lr, fc, lc;
                    GetSelectedCellRange(out fr, out lr, out fc, out lc);
                    for (int i = fc; i <= lc; ++i) {
                        TableFigureHelper.AdjustColumnSize(edi, fig, i);
                    }
                }
            };

            _adjustRowHeightsAtEvenInterval = new ToolStripMenuItem("行の高さを揃える(&H)");
            _adjustRowHeightsAtEvenInterval.Click += (sender, ev) => {
                var edi = GetTableEditor();
                var fig = GetTableFigure();
                using (edi.Site.CommandExecutor.BeginChain()) {
                    int fr, lr, fc, lc;
                    GetSelectedCellRange(out fr, out lr, out fc, out lc);
                    TableFigureHelper.AdjustRowSizesAtEvenInterval(edi, fig, fr, lr);
                }
            };

            _adjustColumnWidthsAtEvenInterval= new ToolStripMenuItem("列の幅を揃える(&W)");
            _adjustColumnWidthsAtEvenInterval.Click += (sender, ev) => {
                var edi = GetTableEditor();
                var fig = GetTableFigure();
                using (edi.Site.CommandExecutor.BeginChain()) {
                    int fr, lr, fc, lc;
                    GetSelectedCellRange(out fr, out lr, out fc, out lc);
                    TableFigureHelper.AdjustColumnSizesAtEvenInterval(edi, fig, fc, lc);
                }
            };


            _mergeCells = new ToolStripMenuItem();
            _mergeCells.Text = "セルを結合(&M)";
            _mergeCells.Click += (sender, ev) => {
                var host = _owner.Host;
                var tableEditor = host.Parent;
                var tableFig = tableEditor.Figure as TableFigure;
                var tableData = tableFig.TableData;
                var tableModel = tableEditor.Model as MemoTable;

                var leftColIndex = -1;
                var rightColIndex = -1;
                var topRowIndex = -1;
                var bottomRowIndex = -1;

                var selecteds = host.Site.SelectionManager.SelectedEditors;
                foreach (var selected in selecteds) {
                    var colIndex = tableData.GetColumnIndex(selected.Figure as TableCellFigure);
                    var rowIndex = tableData.GetRowIndex(selected.Figure as TableCellFigure);
                    leftColIndex = leftColIndex == -1? colIndex: Math.Min(leftColIndex, colIndex);
                    rightColIndex = rightColIndex == -1? colIndex: Math.Max(rightColIndex, colIndex);
                    topRowIndex = topRowIndex == -1? rowIndex: Math.Min(topRowIndex, rowIndex);
                    bottomRowIndex = bottomRowIndex == -1? rowIndex : Math.Max(bottomRowIndex, rowIndex);
                }


                GetExecutor().Execute(
                    new MergeTableCellsCommand(
                        tableEditor,
                        leftColIndex,
                        topRowIndex,
                        rightColIndex - leftColIndex + 1,
                        bottomRowIndex - topRowIndex + 1
                    )
                );

                var cell = tableData.GetCell(topRowIndex, leftColIndex);
                var cellEditor = cell.Value.GetEditor();
                cellEditor.RequestSelect(SelectKind.True, true);
            };

            _unmergeCells = new ToolStripMenuItem();
            _unmergeCells.Text = "セルを分割(&U)";
            _unmergeCells.Click += (sender, ev) => {
                var host = _owner.Host;
                var tableEditor = host.Parent;
                var tableFig = tableEditor.Figure as TableFigure;
                var tableData = tableFig.TableData;
                var tableModel = tableEditor.Model as MemoTable;

                var leftColIndex = -1;
                var topRowIndex = -1;

                var selecteds = host.Site.SelectionManager.SelectedEditors;
                foreach (var selected in selecteds) {
                    var colIndex = tableData.GetColumnIndex(selected.Figure as TableCellFigure);
                    var rowIndex = tableData.GetRowIndex(selected.Figure as TableCellFigure);
                    leftColIndex = leftColIndex == -1? colIndex: Math.Min(leftColIndex, colIndex);
                    topRowIndex = topRowIndex == -1? rowIndex: Math.Min(topRowIndex, rowIndex);
                }

                GetExecutor().Execute(
                    new MergeTableCellsCommand(
                        tableEditor,
                        leftColIndex,
                        topRowIndex,
                        1,
                        1
                    )
                );

                var cell = tableData.GetCell(topRowIndex, leftColIndex);
                var cellEditor = cell.Value.GetEditor();
                cellEditor.RequestSelect(SelectKind.True, true);
            };


            _copyTable = new ToolStripMenuItem();
            _copyTable.Text = "表をコピー(&C)";
            _copyTable.Click += (sender, ev) => {
                var tableEditor = GetTableEditor();
                if (tableEditor != null) {
                    var req = new CopyRequest(new [] { tableEditor });
                    tableEditor.PerformRequest(req);
                }
            };
        }

        protected void UpdateEnabled() {
            var cells = GetSelectedCells();

            int fr, lr, fc, lc;
            GetSelectedCellRange(out fr, out lr, out fc, out lc);
            var rowCount = lr - fr + 1;
            var colCount = lc - fc + 1;
            var count = rowCount * colCount;

            _mergeCells.Enabled = count > 1 && cells.All(cell => !cell.IsMerged && !cell.IsMerging);

            if (count == 1) {
                var cell = cells.First();
                _unmergeCells.Enabled = cell.IsMerged || cell.IsMerging;
            } else {
                _unmergeCells.Enabled = false;
            }

            _adjustRowHeightsAtEvenInterval.Enabled = rowCount > 1;
            _adjustColumnWidthsAtEvenInterval.Enabled = colCount > 1;
        }

        // ------------------------------
        // private
        // ------------------------------
        private ICommandExecutor GetExecutor() {
            var host = _owner.Host;
            return host.Site.CommandExecutor;
        }

        private IEditor GetTableEditor() {
            var host = _owner.Host;
            return host.Parent;
        }

        private MemoTable GetTableModel() {
            var host = _owner.Host;
            var tableEditor = host.Parent;
            return tableEditor.Model as MemoTable;
        }

        private TableFigure GetTableFigure() {
            var host = _owner.Host;
            var tableEditor = host.Parent;
            return tableEditor.Figure as TableFigure;
        }

        private Cell<TableCellFigure> GetCell() {
            var host = _owner.Host;
            var cellFig = host.Figure as TableCellFigure;

            var tableEditor = host.Parent;
            var tableFig = tableEditor.Figure as TableFigure;
            var tableData = tableFig.TableData;
            var tableModel = tableEditor.Model as MemoTable;

            return tableData.GetCell(cellFig);
        }

        private int GetRowIndex() {
            var host = _owner.Host;
            var cellFig = host.Figure as TableCellFigure;

            var tableEditor = host.Parent;
            var tableFig = tableEditor.Figure as TableFigure;
            var tableData = tableFig.TableData;
            var tableModel = tableEditor.Model as MemoTable;

            return tableData.GetRowIndex(cellFig);
        }

        private int GetColumnIndex() {
            var host = _owner.Host;
            var cellFig = host.Figure as TableCellFigure;

            var tableEditor = host.Parent;
            var tableFig = tableEditor.Figure as TableFigure;
            var tableData = tableFig.TableData;
            var tableModel = tableEditor.Model as MemoTable;

            return tableData.GetColumnIndex(cellFig);
        }

        private int GetRowCount() {
            var host = _owner.Host;
            var tableEditor = host.Parent;
            var tableFig = tableEditor.Figure as TableFigure;
            var tableData = tableFig.TableData;
            return tableData.RowCount;
        }

        private int GetColumnCount() {
            var host = _owner.Host;
            var tableEditor = host.Parent;
            var tableFig = tableEditor.Figure as TableFigure;
            var tableData = tableFig.TableData;
            return tableData.ColumnCount;
        }

        private IEnumerable<Cell<TableCellFigure>> GetSelectedCells() {
            var host = _owner.Host;
            var tableEditor = host.Parent;
            var tableFig = tableEditor.Figure as TableFigure;
            var tableData = tableFig.TableData;

            var selecteds = host.Site.SelectionManager.SelectedEditors;
            return selecteds.Select(editor => tableData.GetCell(editor.Figure as TableCellFigure));
        }

        private void GetSelectedCellRange(out int firstRowIndex, out int lastRowIndex, out int firstColIndex, out int lastColIndex) {
            var host = _owner.Host;
            var tableEditor = host.Parent;
            var tableFig = tableEditor.Figure as TableFigure;
            var tableData = tableFig.TableData;

            var selecteds = host.Site.SelectionManager.SelectedEditors;
            if (selecteds.Count() < 1) {
                firstRowIndex = -1;
                lastRowIndex = -1;
                firstColIndex = -1;
                lastColIndex = -1;
                return;
            }

            var rowIndecies = new List<int>();
            var colIndecies = new List<int>();
            foreach (var selected in selecteds) {
                var fig = selected.Figure as TableCellFigure;
                var rowIndex = tableData.GetRowIndex(fig);
                var colIndex = tableData.GetColumnIndex(fig);
                rowIndecies.Add(rowIndex);
                colIndecies.Add(colIndex);
            }

            firstRowIndex = rowIndecies.Min();
            lastRowIndex = rowIndecies.Max();
            firstColIndex = colIndecies.Min();
            lastColIndex = colIndecies.Max();
        }
    }
}
