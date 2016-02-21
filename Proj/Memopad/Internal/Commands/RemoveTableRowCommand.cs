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
using Mkamo.Figure.Figures;
using Mkamo.Editor.Core;
using Mkamo.Common.DataType;
using Mkamo.Common.Core;
using Mkamo.Editor.Internal.Core;

namespace Mkamo.Memopad.Internal.Commands {
    using StyledText = Mkamo.StyledText.Core.StyledText;
    using Mkamo.Common.Forms.Table;

    internal class RemoveTableRowCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private int _rowIndex;

        private MemoTableRow _oldModelRow;
        private Row<TableCellFigure> _oldFigureRow;
        private List<Tuple<int, IEditor>> _oldCellEditors;

        // ========================================
        // constructor
        // ========================================
        public RemoveTableRowCommand(IEditor target, int rowIndex) {
            _target = target;
            _rowIndex = rowIndex;
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

            _oldModelRow = model.Rows.ElementAt(_rowIndex);
            _oldFigureRow = fig.TableData.Rows.ElementAt(_rowIndex);

            /// editorの保持とdisable
            _oldCellEditors = new List<Tuple<int, IEditor>>();
            foreach (var cell in _oldFigureRow.Cells) {
                var cellFig = cell.Value;
                var cellEditor = cellFig.GetEditor();
                var cellEditorIndex = _target.Children.IndexOf(cellEditor);
                _oldCellEditors.Add(Tuple.Create(cellEditorIndex, cellEditor));
                cellEditor.Disable();
            }

            /// rowのContainer.RemoveはRemoveRowAt()内で実行される
            /// 更新通知でMemoTableController.RefreshEditor()が呼ばれて
            /// figureも更新される
            model.RemoveRowAt(_rowIndex);
        }


        public override void Undo() {
            var model = GetTableModel();
            var fig = GetTableFigure();

            /// row下のcellもpersistされる
            var container = MemopadApplication.Instance.Container;
            container.Persist(_oldModelRow);

            /// 更新通知なしでmodelを復元
            model.SuppressNotification = true;
            model.InsertRow(_rowIndex, _oldModelRow);
            model.SuppressNotification = false;

            /// editorの復元
            foreach (var editorInfo in _oldCellEditors) {
                var index = editorInfo.Item1;
                var editor = editorInfo.Item2;
                _target.InsertChildEditor(editor, index);
                editor.Enable();
            }

            /// figureの復元
            fig.TableData.InsertRow(_rowIndex, _oldFigureRow);
            
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
