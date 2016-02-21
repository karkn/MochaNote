/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;
using Mkamo.Model.Memo;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Editor.Core;
using Mkamo.Common.Forms.Table;
using Mkamo.Figure.Figures;
using Mkamo.Common.DataType;
using Mkamo.Common.Core;
using Mkamo.Editor.Internal.Core;

namespace Mkamo.Memopad.Internal.Commands {
    using StyledText = Mkamo.StyledText.Core.StyledText;

    internal class RemoveTableColumnCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private int _columnIndex;

        private int _oldColumnWidth;
        private List<MemoTableCell> _oldModelCells;
        private List<Cell<TableCellFigure>> _oldFigureCells;
        private List<Tuple<int, IEditor>> _oldCellEditors;

        // ========================================
        // constructor
        // ========================================
        public RemoveTableColumnCommand(IEditor target, int columnIndex) {
            _target = target;
            _columnIndex = columnIndex;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var model = GetTableModel();
            var fig = GetTableFigure();

            _oldModelCells = new List<MemoTableCell>();
            _oldFigureCells = new List<Cell<TableCellFigure>>();
            _oldCellEditors = new List<Tuple<int, IEditor>>();

            var figCol = fig.TableData.Columns.ElementAt(_columnIndex);
            _oldColumnWidth = figCol.Width;

            /// modelの保持
            var modelCol = model.Columns.ElementAt(_columnIndex);
            foreach (var cell in modelCol.Cells) {
                _oldModelCells.Add(cell);
            }

            /// editorの保持とdisable
            foreach (var cell in figCol.Cells) {
                var cellFig = cell.Value;
                var cellEditor = cellFig.GetEditor();
                var cellEditorIndex = _target.Children.IndexOf(cellEditor);
                _oldFigureCells.Add(cell);
                _oldCellEditors.Add(Tuple.Create(cellEditorIndex, cellEditor));
                cellEditor.Disable();
            }

            /// cellのContainer.RemoveはRemoveColumnAt()内で実行される
            /// 更新通知でMemoTableController.RefreshEditor()が呼ばれて
            /// figureも更新される
            model.RemoveColumnAt(_columnIndex);
        }

        public override void Undo() {
            var model = GetTableModel();
            var fig = GetTableFigure();

            /// cellをpersist
            var container = MemopadApplication.Instance.Container;
            foreach (var cell in _oldModelCells) {
                container.Persist(cell);
            }

            /// 更新通知なしでmodelを復元
            model.SuppressNotification = true;
            model.InsertColumn(_columnIndex, _oldModelCells);
            model.SuppressNotification = false;

            /// editorの復元
            foreach (var editorInfo in _oldCellEditors) {
                var index = editorInfo.Item1;
                var editor = editorInfo.Item2;
                _target.InsertChildEditor(editor, index);
                editor.Enable();
            }

            /// figureの復元
            fig.TableData.InsertColumn(_columnIndex, _oldFigureCells);
            fig.TableData.Columns.ElementAt(_columnIndex).Width = _oldColumnWidth;
            
            fig.Size = fig.TableData.Size;
            fig.MinSize = fig.TableData.MinSize;
            fig.InvalidateLayout();
            fig.InvalidatePaint();
        }

        // ------------------------------
        // private
        // ------------------------------
        private MemoTable GetTableModel() {
            return _target.Model as MemoTable;
        }

        private TableFigure GetTableFigure() {
            return _target.Figure as TableFigure;
        }
    }
}
