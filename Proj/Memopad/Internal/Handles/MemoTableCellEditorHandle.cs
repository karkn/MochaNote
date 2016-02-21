/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Handles;
using Mkamo.Editor.Core;
using System.Windows.Forms;
using Mkamo.Figure.Figures;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Commands;
using Mkamo.Editor.Focuses;
using Mkamo.Common.Forms.Input;
using Mkamo.Editor.Handles.Scenarios;
using Mkamo.Common.Forms.Table;
using Mkamo.Figure.Core;
using Mkamo.StyledText.Core;
using System.Drawing;
using Mkamo.Memopad.Internal.Utils;
using System.Diagnostics;
using Mkamo.Memopad.Internal.Core;

namespace Mkamo.Memopad.Internal.Handles {
    internal class MemoTableCellEditorHandle: DefaultEditorHandle {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;

        private SelectScenario _selectScenario;

        private bool _isScenarioHandled;

        private int _startRowIndex;
        private int _startColIndex;

        private int _endRowIndex;
        private int _endColIndex;

        // ========================================
        // constructor
        // ========================================
        public MemoTableCellEditorHandle() {
            _facade = MemopadApplication.Instance;

            _selectScenario = new SelectScenario(this, false);
            _isScenarioHandled = false;

            _startRowIndex = -1;
            _startColIndex = -1;
            _endRowIndex = -1;
            _endColIndex = -1;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        protected override Cursor GetMouseCursor(MouseEventArgs e) {
            var run = GetRun(e.Location);
            if (run != null && run.HasLink) {
                return Cursors.Hand;
            } else {
                return Cursors.Default;
            }
        }

        protected override void OnFigureMouseDown(MouseEventArgs e) {
            _startRowIndex = -1;
            _startColIndex = -1;
            _endRowIndex = -1;
            _endColIndex = -1;

            _isScenarioHandled = false;

            if (KeyUtil.IsShiftPressed()) {
                var parent = Host.Parent;
                var man = Host.Site.SelectionManager;
                var selectedEditors = man.SelectedEditors;
                if (selectedEditors.Any() && selectedEditors.First().Parent == parent) {
                    var tableData = GetParentTableData();
                    var firstEditor = selectedEditors.First();
                    _startRowIndex = tableData.GetRowIndex(firstEditor.Figure as TableCellFigure);
                    _startColIndex = tableData.GetColumnIndex(firstEditor.Figure as TableCellFigure);

                    _endRowIndex = tableData.GetRowIndex(Host.Figure as TableCellFigure);
                    _endColIndex = tableData.GetColumnIndex(Host.Figure as TableCellFigure);
                    SelectRange(_startRowIndex, _startColIndex, _endRowIndex, _endColIndex);
                }

            } else if (
                e.Button == MouseButtons.Left &&
                !KeyUtil.IsAltPressed() &&
                !KeyUtil.IsControlPressed() &&
                !KeyUtil.IsShiftPressed()
            ) {
                var run = GetRun(e.Location);
                if (run != null && run.HasLink) {
                    LinkUtil.GoLink(run.Link);
                    _isScenarioHandled = true;

                } else {
                    _selectScenario.HandleMouseDown(this, e);
                }

            } else {
                _selectScenario.HandleMouseDown(this, e);
            }

            base.OnFigureMouseDown(e);
        }

        protected override void OnFigureMouseUp(MouseEventArgs e) {
            if (!_isScenarioHandled) {
                _selectScenario.HandleMouseUp(this, e);
            }
            base.OnFigureMouseUp(e);
        }

        protected override void OnFigureMouseDoubleClick(MouseEventArgs e) {
            if (!_isScenarioHandled) {
                _selectScenario.HandleMouseDoubleClick(this, e);
            }
            base.OnFigureMouseDoubleClick(e);
        }

        protected override void OnFigureDragMove(MouseEventArgs e) {
            if (!_isScenarioHandled) {
                var parent = Host.Parent;
                var tableFig = parent.Figure as TableFigure;
                var tableData = tableFig.TableData;
    
                if (_startRowIndex == -1) {
                    _startRowIndex = tableData.GetRowIndex(Host.Figure as TableCellFigure);
                    _startColIndex = tableData.GetColumnIndex(Host.Figure as TableCellFigure);
                }
    
                var endCellEditor = parent.FindEditor(
                    editor => {
                        if (editor.Figure.ContainsPoint(e.Location)) {
                            var cell = tableData.GetCell((TableCellFigure) editor.Figure);
                            return !cell.IsMerged;
                        }
                        return false;
                    }
                );
                if (endCellEditor != null) {
                    var endRowIndex = tableData.GetRowIndex(endCellEditor.Figure as TableCellFigure);
                    var endColIndex = tableData.GetColumnIndex(endCellEditor.Figure as TableCellFigure);
    
                    if (endRowIndex != _endRowIndex || endColIndex != _endColIndex) {
                        SelectRange(_startRowIndex, _startColIndex, endRowIndex, endColIndex);
                        _endRowIndex = endRowIndex;
                        _endColIndex = endColIndex;
                    }
                }
            }

            base.OnFigureDragMove(e);
        }

        protected override void OnFigureKeyPress(KeyPressEventArgs e) {
            if (!Char.IsControl(e.KeyChar)) {
                Host.RequestFocus(FocusKind.Begin, null);
                var stFocus = Host.Focus as StyledTextFocus;
                if (stFocus != null) {
                    stFocus.SelectAll();
                    stFocus.RemoveForward();
                    stFocus.Insert(e.KeyChar.ToString());
                }
            }

            base.OnFigureKeyPress(e);
        }

        // ------------------------------
        // private
        // ------------------------------
        private void SelectRange(int startRow, int startCol, int endRow, int endCol) {
            var parent = Host.Parent;
            var tableData = GetParentTableData();

            var rstart = Math.Min(startRow, endRow);
            var cstart = Math.Min(startCol, endCol);
            var rend = Math.Max(startRow, endRow);
            var cend = Math.Max(startCol, endCol);

            /// マージを考慮してrstartなどを計算し直す
            var prstart = 0;
            var pcstart = 0;
            var prend = 0;
            var pcend = 0;

            /// 処理済みの範囲 (m == marked)
            var mrstart = 0;
            var mcstart = 0;
            var mrend = 0;
            var mcend = 0;
            do {
                prstart = rstart;
                pcstart = cstart;
                prend = rend;
                pcend = cend;

                for (int r = rstart, rlen = rend; r <= rlen; ++r) {
                    for (int c = cstart, clen = cend; c <= clen; ++c) {
                        if (r >= mrstart && r <= mrend && c >= mcstart && c <= mcend) {
                            continue;
                        }
                        var row = tableData.Rows.ElementAt(r);
                        var cell = row.Cells.ElementAt(c);
                        if (cell.IsMerging) {
                            rend = Math.Max(rend, r + cell.RowSpan - 1);
                            cend = Math.Max(cend, c + cell.ColumnSpan - 1);
                        }
                        if (cell.IsMerged) {
                            var merging = cell.Merging;
                            var mr = tableData.GetRowIndex(merging);
                            var mc = tableData.GetColumnIndex(merging);
                            rstart = Math.Min(rstart, mr);
                            cstart = Math.Min(cstart, mc);
                            rend = Math.Max(rend, mr + merging.RowSpan - 1);
                            cend = Math.Max(cend, mc + merging.ColumnSpan - 1);
                        }
                    }
                }
                mrstart = prstart;
                mcstart = pcstart;
                mrend = prend;
                mcend = pcend;

            } while (rstart != prstart || cstart != pcstart || rend != prend || cend != pcend);

            /// 範囲内のeditorを集める
            var editors = new List<IEditor>();
            for (int r = rstart; r <= rend; ++r) {
                for (int c = cstart; c <= cend; ++c) {
                    var row = tableData.Rows.ElementAt(r);
                    var cell = row.Cells.ElementAt(c);
                    var editor = cell.Value.GetEditor();
                    if (editor != null) {
                        editors.Add(editor);
                    }
                }
            }

            if (editors.Count > 0) {
                var cmd = new SelectMultiCommand(editors, SelectKind.True, true);
                Host.Site.CommandExecutor.Execute(cmd);
            }
        }

        private TableData<TableCellFigure> GetParentTableData() {
            var parent = Host.Parent;
            var tableFig = parent.Figure as TableFigure;
            return tableFig.TableData;
        }

        private Run GetRun(Point loc) {
            var fig = _Figure as INode;
            return fig.GetInlineAt(loc) as Run;
        }
    }
}
