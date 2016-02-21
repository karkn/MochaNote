/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Control.SelectorDropDown;
using System.Drawing;
using Mkamo.Memopad.Properties;
using Mkamo.Common.Command;
using Mkamo.Editor.Commands;
using Mkamo.Editor.Core;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing.Drawing2D;
using Mkamo.Memopad.Internal.Forms;
using Mkamo.Model.Memo;
using Mkamo.Editor.Tools;
using Mkamo.Model.Core;

namespace Mkamo.Memopad.Internal.Controls {
    internal class FreehandSelectorDropDown: SelectorDropDown {
        // ========================================
        // field
        // ========================================
        private MemopadFormBase _form;
        private bool _isPrepared;
        private ITool _defaultTool;

        // ========================================
        // constructor
        // ========================================
        public FreehandSelectorDropDown(MemopadFormBase form) {
            _form = form;
            _isPrepared = false;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public void Prepare() {
            if (_isPrepared) {
                return;
            }

            SuspendLayout();

            var operation = new SelectorCategory("操作");
            {
                operation.LabelSize = new Size(36, 36);
                operation.BackColor = Color.White;
                operation.MaxCols = 5;

                var select = new SelectTool();
                _defaultTool = select;
                operation.AddLabel(
                    Resources.cursor24,
                    "選択",
                    () => {
                        _form.EditorCanvas.Tool = select;
                        Hide();
                        _form.Activate();
                        _form.EditorCanvas.Select();
                    }
                );

                var remove = new EraserTool(
                    editor => editor.Model is MemoFreehand
                );
                operation.AddLabel(
                    Resources.eraser24,
                    "消しゴム",
                    () => {
                        _form.EditorCanvas.Tool = remove;
                        Hide();
                        _form.Activate();
                        _form.EditorCanvas.Select();
                    }
                );

                var dragSelect = new DragSelectTool(
                    editor => editor.Model is MemoFreehand
                );
                operation.AddLabel(
                    Resources.drag_select24,
                    "ドラッグ選択",
                    () => {
                        _form.EditorCanvas.SelectionManager.DeselectAll();
                        _form.EditorCanvas.Tool = dragSelect;
                        Hide();
                        _form.Activate();
                        _form.EditorCanvas.Select();
                    }
                );
                dragSelect.Finished += HandleDragSelectToolFinished;

                AddCategory(operation);
            }

            var thin = new SelectorCategory("ペン (細)");
            {
                thin.LabelSize = new Size(36, 36);
                thin.BackColor = Color.White;
                thin.MaxCols = 5;

                AddFreehand(thin, Resources.black_thin, "黒 太さ1pt", 1, Color.Black);
                AddFreehand(thin, Resources.red_thin, "赤 太さ1pt", 1, Color.Red);
                AddFreehand(thin, Resources.blue_thin, "青 太さ1pt", 1, Color.Blue);
                AddFreehand(thin, Resources.green_thin, "緑 太さ1pt", 1, Color.Green);
                AddFreehand(thin, Resources.gray_thin, "灰色 太さ1pt", 1, Color.Gray);

                AddCategory(thin);
            }

            var middle = new SelectorCategory("ペン (中)");
            {
                middle.LabelSize = new Size(36, 36);
                middle.BackColor = Color.White;
                middle.MaxCols = 5;

                AddFreehand(middle, Resources.black_middle, "黒 太さ2pt", 2, Color.Black);
                AddFreehand(middle, Resources.red_middle, "赤 太さ2pt", 2, Color.Red);
                AddFreehand(middle, Resources.blue_middle, "青 太さ2pt", 2, Color.Blue);
                AddFreehand(middle, Resources.green_middle, "緑 太さ2pt", 2, Color.Green);
                AddFreehand(middle, Resources.gray_middle, "灰色 太さ2pt", 2, Color.Gray);

                AddCategory(middle);
            }

            var thick = new SelectorCategory("ペン (太)");
            {
                thick.LabelSize = new Size(36, 36);
                thick.BackColor = Color.White;
                thick.MaxCols = 5;

                AddFreehand(thick, Resources.black_thick, "黒 太さ4pt", 4, Color.Black);
                AddFreehand(thick, Resources.red_thick, "赤 太さ4pt", 4, Color.Red);
                AddFreehand(thick, Resources.blue_thick, "青 太さ4pt", 4, Color.Blue);
                AddFreehand(thick, Resources.green_thick, "緑 太さ4pt", 4, Color.Green);
                AddFreehand(thick, Resources.gray_thick, "灰色 太さ4pt", 4, Color.Gray);

                AddCategory(thick);
            }

            ResumeLayout();

            _isPrepared = true;
        }

        private void AddFreehand(
            SelectorCategory cate,
            Image image,
            string text,
            int width,
            Color color
        ) {
            AddTool(
                cate,
                image,
                text,
                new FreehandTool(
                    null,
                    new DelegatingModelFactory<MemoFreehand>(
                        () => MemoFactory.CreateFreehand()
                    ),
                    width,
                    color
                )
            );
        }

        private void AddTool(
            SelectorCategory cate,
            Image image,
            string text,
            ITool tool
        ) {
            cate.AddLabel(
                image,
                text,
                () => {
                    if (_form.EditorCanvas != null) {
                        _form.EditorCanvas.Tool = tool;
                        Hide();
                        _form.Activate();
                        _form.EditorCanvas.Select();
                    }
                }
            );

        }

        private void HandleDragSelectToolFinished(object sender, EventArgs e) {
            if (_form.EditorCanvas != null) {
                _form.EditorCanvas.Tool = _defaultTool;
            }
        }
    }
}
