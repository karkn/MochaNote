/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Drawing;
using Mkamo.Common.Diagnostics;
using Mkamo.Figure.Core;
using Mkamo.Common.Forms.Table;
using Mkamo.Figure.Layouts;
using Mkamo.Common.Externalize;

namespace Mkamo.Figure.Figures {
    using StyledText = StyledText.Core.StyledText;

    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = false)]
    public class TableFigure: AbstractNode {

        // ========================================
        // field
        // ========================================
        private TableData<TableCellFigure> _tableData;

        private bool _isInBoundsChange;
        private bool _suppressInvalidation;

        // ========================================
        // constructor
        // ========================================
        public TableFigure() {
            _tableData = new TableData<TableCellFigure>();
            Layout = new TableLayout();

            _isInBoundsChange = false;
            _suppressInvalidation = false;

            _tableData.RowChanged += HandleTableDataRowAndColumnChanged;
            _tableData.ColumnChanged += HandleTableDataRowAndColumnChanged;
        }

        // ========================================
        // property
        // ========================================
        public TableData<TableCellFigure> TableData {
            get { return _tableData; }
        }

        public bool SuppressInvalidation {
            get { return _suppressInvalidation; }
            set { _suppressInvalidation = value; }
        }

        // ========================================
        // method
        // ========================================
        // === IPersistable ==========
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);

            /// ValueにFigureのMementoを入れたTableDataを作る
            var memTableData = new TableData<IMemento>();
            memTableData.Redim(_tableData.RowCount, _tableData.ColumnCount);

            memTableData.MinRowHeight = _tableData.MinRowHeight;
            memTableData.MinColumnWidth = _tableData.MinColumnWidth;

            /// row
            for (int i = 0, len = _tableData.RowCount; i < len; ++i) {
                var row = _tableData.Rows.ElementAt(i);
                var memRow = memTableData.Rows.ElementAt(i);
                memRow.Height = row.Height;
            }

            /// column
            for (int i = 0, len = _tableData.ColumnCount; i < len; ++i) {
                var col = _tableData.Columns.ElementAt(i);
                var memCol = memTableData.Columns.ElementAt(i);
                memCol.Width = col.Width;
            }

            /// cell
            for (int i = 0, len = _tableData.CellCount; i < len; ++i) {
                var cell = _tableData.Cells.ElementAt(i);
                var memCell = memTableData.Cells.ElementAt(i);
                memCell.Value = context.GetMemento("Cell", cell.Value);
                //memCell.ColumnSpan = cell.ColumnSpan;
                //memCell.RowSpan = cell.RowSpan;
                if (cell.ColumnSpan != 1 || cell.RowSpan != 1) {
                    memTableData.SetSpan(memCell, cell.ColumnSpan, cell.RowSpan);
                }
                memCell.Color = cell.Color;
            }

            memento.WriteSerializable("TableData", memTableData);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);

            /// ValueにFigureのMementoが入ったTableDataから復元する
            var memTableData = memento.ReadSerializable("TableData") as TableData<IMemento>;
            _tableData.Redim(memTableData.RowCount, memTableData.ColumnCount);

            _tableData.MinRowHeight = memTableData.MinRowHeight;
            _tableData.MinColumnWidth = memTableData.MinColumnWidth;

            /// row
            for (int i = 0, len = _tableData.RowCount; i < len; ++i) {
                var row = _tableData.Rows.ElementAt(i);
                var memRow = memTableData.Rows.ElementAt(i);
                row.Height = memRow.Height;
            }

            /// column
            for (int i = 0, len = _tableData.ColumnCount; i < len; ++i) {
                var col = _tableData.Columns.ElementAt(i);
                var memCol = memTableData.Columns.ElementAt(i);
                col.Width = memCol.Width;
            }
            /// cell
            for (int i = 0, len = _tableData.CellCount; i < len; ++i) {
                var cell = _tableData.Cells.ElementAt(i);
                var memCell = memTableData.Cells.ElementAt(i);
                cell.Value = context.GetExternalizable("Cell", memCell.Value) as TableCellFigure;
                
                //cell.ColumnSpan = memCell.ColumnSpan;
                //cell.RowSpan = memCell.RowSpan;
                if (memCell.ColumnSpan != 1 || memCell.RowSpan != 1) {
                    _tableData.SetSpan(cell, memCell.ColumnSpan, memCell.RowSpan);
                }
                cell.Color = memCell.Color;
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected override void PaintSelf(Graphics g) {
            using (_ResourceCache.UseResource()) {
                //var brush = _BrushResource;
                var pen = _PenResource;

                //if (IsBackgroundEnabled && brush != null) {
                //    /// 背景
                //    g.FillRectangle(brush, Left, Top, Width - 1, Height - 1);
                //}
                if (IsForegroundEnabled && BorderWidth > 0) {
                    /// 枠
                    g.DrawRectangle(pen, Left, Top, Width - 1, Height - 1);
                }

                // todo: セル背景の表示

                /// 内部の罫線
                //if (_tableData.Columns.Any()) {
                //    var lastCol = _tableData.Columns.Last();
                //    var x = Left;
                //    foreach (var col in _tableData.Columns) {
                //        if (col == lastCol) {
                //            break;
                //        }
                //        x += col.Width;
                //        g.DrawLine(pen, new Point(x, Top), new Point(x, Bottom - 1));
                //    }
                //}

                //if (_tableData.Rows.Any()) {
                //    var lastRow = _tableData.Rows.Last();
                //    var y = Top;
                //    foreach (var row in _tableData.Rows) {
                //        if (row == lastRow) {
                //            break;
                //        }
                //        y += row.Height;
                //        g.DrawLine(pen, new Point(Left, y), new Point(Right - 1, y));
                //    }
                //}
            }
        }

        protected override Rectangle _Bounds {
            get { return base._Bounds; }
            set {
                var oldSize = base._Bounds.Size;
                var newSize = value.Size;
                if (newSize != oldSize) {
                    _isInBoundsChange = true;
                    try {
                        _tableData.Size = newSize;
                    } finally {
                        _isInBoundsChange = false;
                    }

                }

                base._Bounds = value;
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private void HandleTableDataRowAndColumnChanged(object sender, EventArgs e) {
            if (_isInBoundsChange) {
                return;
            }
            if (_suppressInvalidation) {
                return;
            }
            using (DirtManager.BeginDirty()) {
                if (_tableData.Size != Size) {
                    Size = _tableData.Size;
                }

                if (_tableData.MinSize != MinSize) {
                    MinSize = _tableData.MinSize;
                }

                InvalidateLayout();
                InvalidatePaint();
            }
        }

    }
}
