/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Mkamo.Figure.Figures;
using System.Drawing;
using Mkamo.Common.Forms.Table;
using Mkamo.Figure.Core;

namespace Mkamo.Memopad.Internal.Figures {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = false)]
    internal class MemoTableRuledLineHandleFigure: AbstractNode {
        // ========================================
        // field
        // ========================================
        private Func<TableFigure> _tableFigureProvider;
        private int _hitMargin;


        // ========================================
        // constructor
        // ========================================
        public MemoTableRuledLineHandleFigure(Func<TableFigure> tableFigureProvider) {
            _hitMargin = 4;
            _tableFigureProvider = tableFigureProvider;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        protected override void PaintSelf(Graphics g) {
            /// do nothing
        }

        public override bool ContainsPoint(Point pt) {
            return IsOnHorizontalLine(pt) || IsOnVerticalLine(pt);
        }

        /// <summary>
        /// ptを含む横罫線のインデクスを返す。
        /// 横罫線のインデクスはその罫線を下側に持つ行のインデクスと等しい。
        /// 含まない場合は-1を返す。
        /// </summary>
        public int GetHorizontalLineIndex(Point pt) {
            var tableData = _tableFigureProvider().TableData;
            var bounds = Bounds;
            if (bounds.Contains(pt) && tableData.Rows.Any()) {
                /// 横線上
                var lastRow = tableData.Rows.Last();
                var cy = bounds.Top;
                var i = 0;
                foreach (var row in tableData.Rows) {
                    if (row == lastRow) {
                        break;
                    }
                    cy += row.Height;
                    if (pt.Y >= cy - _hitMargin && pt.Y <= cy + _hitMargin) {
                        return i;
                    }
                    ++i;
                }
            }
            return -1;
        }

        /// <summary>
        /// ptを含む縦罫線のインデクスを返す。
        /// 縦罫線のインデクスはその罫線を右側に持つ列のインデクスと等しい。
        /// 含まない場合は-1を返す。
        /// </summary>
        public int GetVerticalLineIndex(Point pt) {
            var tableData = _tableFigureProvider().TableData;
            var bounds = Bounds;
            if (bounds.Contains(pt) && tableData.Columns.Any()) {
                /// 縦線上
                var lastCol = tableData.Columns.Last();
                var cx = bounds.Left;
                var i = 0;
                foreach (var col in tableData.Columns) {
                    if (col == lastCol) {
                        break;
                    }
                    cx += col.Width;
                    if (pt.X >= cx - _hitMargin && pt.X <= cx + _hitMargin) {
                        return i;
                    }
                    ++i;
                }
            }

            return -1;
        }

        public bool IsOnHorizontalLine(Point pt) {
            var tableData = _tableFigureProvider().TableData;
            var bounds = Bounds;
            if (bounds.Contains(pt) && tableData.Rows.Any()) {
                /// マージを考えないときの横線判定
                var ret = false;
                var lastRow = tableData.Rows.Last();
                var cy = bounds.Top;
                var i = 0;
                foreach (var row in tableData.Rows) {
                    if (row == lastRow) {
                        break;
                    }
                    cy += row.Height;
                    if (pt.Y >= cy - _hitMargin && pt.Y <= cy + _hitMargin) {
                        ret = true;
                        break;
                    }
                    ++i;
                }

                if (!ret) {
                    return ret;
                }

                /// マージ判定
                var cx = bounds.Left;
                foreach (var col in tableData.Columns) {
                    if (pt.X >= cx && pt.X <= cx + col.Width) {
                        if (i == tableData.CellCount - 1) {
                            return true;
                        }
                        var cell = col.Cells.ElementAt(i + 1);
                        return !cell.IsMerged;
                    }
                    cx += col.Width;
                }
            }
            return false;
        }

        public bool IsOnVerticalLine(Point pt) {
            var tableData = _tableFigureProvider().TableData;
            var bounds = Bounds;
            if (bounds.Contains(pt) && tableData.Columns.Any()) {
                /// マージを考えないときの縦線判定
                var ret = false;
                var lastCol = tableData.Columns.Last();
                var cx = bounds.Left;
                var i = 0;
                foreach (var col in tableData.Columns) {
                    if (col == lastCol) {
                        break;
                    }
                    cx += col.Width;
                    if (pt.X >= cx - _hitMargin && pt.X <= cx + _hitMargin) {
                        ret = true;
                        break;
                    }
                    ++i;
                }

                if (!ret) {
                    return ret;
                }

                /// マージ判定
                var cy = bounds.Top;
                foreach (var row in tableData.Rows) {
                    if (pt.Y >= cy && pt.Y <= cy + row.Height) {
                        if (i == tableData.CellCount - 1) {
                            return true;
                        }
                        var cell = row.Cells.ElementAt(i + 1);
                        return !cell.IsMerged;
                    }
                    cy += row.Height;
                }
            }

            return false;
        }

    }
}
