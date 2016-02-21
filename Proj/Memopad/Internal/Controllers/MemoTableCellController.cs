/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Model.Memo;
using Mkamo.Figure.Figures;
using Mkamo.Editor.Core;
using Mkamo.Common.Externalize;
using System.Drawing;

namespace Mkamo.Memopad.Internal.Controllers {
    using StyledText = Mkamo.StyledText.Core.StyledText;
    using System.Reflection;
    using Mkamo.Editor.Roles;
    using Mkamo.Editor.Handles;
    using Mkamo.Editor.Handles.Scenarios;
    using Mkamo.Memopad.Internal.Focuses;
    using Mkamo.Memopad.Core;
    using Mkamo.Memopad.Internal.Core;
    using Mkamo.Figure.Core;
    using Mkamo.Common.Core;
    using Mkamo.Memopad.Internal.Controllers.UIProviders;
    using Mkamo.Memopad.Internal.Handles;
    using Mkamo.Common.Forms.Descriptions;
    using Mkamo.Memopad.Internal.Utils;

    internal class MemoTableCellController: AbstractMemoContentController<MemoTableCell, TableCellFigure> {
        // ========================================
        // field
        // ========================================
        private Lazy<IUIProvider> _uiProvider;

        private bool _firstFocus;

        // ========================================
        // constructor
        // ========================================
        public MemoTableCellController() {
            _uiProvider = new Lazy<IUIProvider>(() => new MemoTableCellUIProvider(this));
            _firstFocus = true;
        }

        // ========================================
        // property
        // ========================================
        public override IUIProvider UIProvider {
            get { return _uiProvider.Value; }
        }

        // ========================================
        // method
        // ========================================
        protected override TableCellFigure CreateFigure(MemoTableCell model) {
            var ret = new TableCellFigure();
            ret.Foreground = Color.Gray;
            ret.IsBackgroundEnabled = false;
            return ret;
        }

        protected override void RefreshEditor(RefreshContext context, TableCellFigure figure, MemoTableCell model) {
            var text = model.StyledText;
            if (text != null) {
                figure.StyledText = text.CloneDeeply() as StyledText;
            }

            var parent = Host.Parent;
            var tableFig = parent.Figure as TableFigure;
            var tableData = tableFig.TableData;

            var figCell = tableData.GetCell(figure);
            if (
                figCell != null &&
                (figCell.ColumnSpan != model.ColumnSpan || figCell.RowSpan != model.RowSpan)
            ) {
                tableData.SetSpan(figCell, model.ColumnSpan, model.RowSpan);
                tableFig.InvalidateLayout();
            }
        }

        public override void ConfigureEditor(IEditor editor) {
            var facade = MemopadApplication.Instance;

            var editorHandle = new MemoTableCellEditorHandle();
            editorHandle.KeyMap = facade.KeySchema.MemoTableCellEditorKeyMap;
            editor.InstallEditorHandle(editorHandle);

            editor.InstallHandle(new SelectionIndicatingHandle());

            editor.InstallRole(new SelectRole());
            editor.InstallRole(new FocusRole(InitFocus, CommitFocus));
            editor.InstallRole(new SetStyledTextFontRole(() => Model.StyledText, FontModificationKinds.All));
            editor.InstallRole(new SetStyledTextColorRole(() => Model.StyledText));
            editor.InstallRole(new SetStyledTextAlignmentRole(() => Model.StyledText, AlignmentModificationKinds.All));

            var editorFocus = new MemoStyledTextFocus(Host, facade.Settings.KeyScheme == KeySchemeKind.Emacs, false);
            editorFocus.IsConsiderHostBounds = true;
            editorFocus.LinkClicked += (sender, e) => {
                LinkUtil.GoLink(e.Link);
            };
            editor.InstallFocus(editorFocus);

            editor.SelectionChanged += (sender, e) => {
                if (editor.IsSelected) {
                    var canvas = Host.Site.EditorCanvas;
                    var rect = Figure.GetCharRect(0);
                    canvas.Caret.Position = rect.Location;

                    var loc = canvas.TranslateToControlPoint(rect.Location);
                    canvas.SetImePosition(loc);

                    canvas.EnsureVisible(editor);
                }
            };
        }

        public override IMemento GetModelMemento() {
            var externalizer = new Externalizer();
            var ex = new MemoTableCellReplacingExternalizable();
            ex.Editor = Host;
            return externalizer.Save(ex, null);
        }

        public override string GetText() {
            var ret = "";

            var stext = Model.StyledText;
            if (stext != null) {
                ret = stext.Text;
            }

            if (Model.Keywords != null) {
                ret = ret + Environment.NewLine + Model.Keywords;
            }

            return ret;
        }


        // ------------------------------
        // private
        // ------------------------------
        private object InitFocus(IFocus focus, object model) {
            if (_firstFocus) {
                var app = MemopadApplication.Instance;
                focus.KeyMap = app.KeySchema.MemoTableCellFocusKeyMap;
                _firstFocus = false;
            }
            var bgColor = Color.Ivory;
            if (Figure.Background != null && Figure.Background.IsDark) {
                bgColor = Color.DimGray;
            }
            focus.Figure.Background = new SolidBrushDescription(bgColor);

            return Model.StyledText == null? null: Model.StyledText.CloneDeeply();
        }

        private FocusUndoer CommitFocus(IFocus focus, object model, object value, bool isRedo, out bool isCancelled) {
            if (focus.IsModified || isRedo) {
                var memoStyledText = (MemoStyledText) model;
                var oldValue = memoStyledText.StyledText;
                var oldBounds = Figure.Bounds;

                isCancelled = false;
                memoStyledText.StyledText = (StyledText) value;

                var parent = Host.Parent;
                var tableFig = parent.Figure as TableFigure;
                var tableData = tableFig.TableData;
                var row = tableData.GetRow(Figure);
                var oldRowHeight = row.Height;

                /// 高さの自動調節
                if (Model.RowSpan == 1) {
                    var focusFig = focus.Figure;
                    var bounds = focusFig.StyledTextBounds;

                    var height = row.Cells.Max(cell => cell.Value.StyledTextBounds.Height);
                    height = Math.Max(bounds.Height, height);

                    if (row.Height != height + Figure.Padding.Height) {
                        row.Height = height + Figure.Padding.Height;
                    }
                }

                return (f, m) => {
                    memoStyledText.StyledText = oldValue;
                    row.Height = oldRowHeight;
                };
            } else {
                isCancelled = false;
                return null;
            }
        }
        
        // ========================================
        // class
        // ========================================
        [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = false)]
        protected class MemoTableCellReplacingExternalizable: IReplacingExternalizable {
            private IEditor _editor;

            private MemoTable _restoredParent;
            private int _restoredRowIndex;
            private int _restoredColumnIndex;

            public MemoTableCellReplacingExternalizable() {
            }

            public IEditor Editor {
                get { return _editor; }
                set { _editor = value; }
            }

            public void WriteExternal(IMemento memento, ExternalizeContext context) {
                var parentEditor = _editor.Parent;
                var table = parentEditor.Model as MemoTable;
                var cell = _editor.Model as MemoTableCell;

                for (int rowIndex = 0; rowIndex < table.RowCount; ++rowIndex) {
                    var row = table.Rows.ElementAt(rowIndex);
                    for (int colIndex = 0; colIndex < table.ColumnCount; ++colIndex) {
                        var c = row.Cells.ElementAt(colIndex);
                        if (c == cell) {
                            memento.WriteInt("RowIndex", rowIndex);
                            memento.WriteInt("ColumnIndex", colIndex);
                            break;
                        }
                    }
                }
            }

            public void ReadExternal(IMemento memento, ExternalizeContext context) {
                _restoredParent = context.ExtendedData[EditorConsts.RestoreEditorStructureParentModel] as MemoTable;
                _restoredRowIndex = memento.ReadInt("RowIndex");
                _restoredColumnIndex = memento.ReadInt("ColumnIndex");

            }

            public object GetReplaced() {
                var row = _restoredParent.Rows.ElementAt(_restoredRowIndex);
                return row.Cells.ElementAt(_restoredColumnIndex);
            }
        }
    }
}
