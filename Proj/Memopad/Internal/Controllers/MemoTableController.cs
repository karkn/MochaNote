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
using Mkamo.Common.Externalize;
using Mkamo.Editor.Core;
using Mkamo.Common.Core;
using Mkamo.Editor.Roles;
using Mkamo.Common.Collection;
using Mkamo.Common.Event;
using System.Drawing;
using Mkamo.Editor.Handles;
using Mkamo.Common.DataType;
using System.Windows.Forms;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Internal.Handles;
using Mkamo.Memopad.Internal.Roles;
using Mkamo.Memopad.Internal.Controllers.UIProviders;

namespace Mkamo.Memopad.Internal.Controllers {
    internal class MemoTableController: AbstractMemoContentController<MemoTable, TableFigure>, IContainerController {
        // ========================================
        // field
        // ========================================
        private Lazy<IUIProvider> _uiProvider;

        // ========================================
        // constructor
        // ========================================
        public MemoTableController() {
            _uiProvider = new Lazy<IUIProvider>(() => new MemoTableUIProvider(this));
        }

        // ========================================
        // property
        // ========================================
        public IEnumerable<object> Children {
            get { return Model.Cells.As<MemoTableCell, object>(); }
        }

        public int ChildCount {
            get { return Model.ColumnCount * Model.RowCount; }
        }

        public bool SyncChildEditors {
            get { return true; }
        }

        public override IUIProvider UIProvider {
            get { return _uiProvider.Value; }
        }

        // ========================================
        // method
        // ========================================
        protected override TableFigure CreateFigure(MemoTable model) {
            var ret = new TableFigure();
            ret.Foreground = Color.FromArgb(165, 165, 165);
            return ret;
        }

        protected override void RefreshEditor(RefreshContext context, TableFigure figure, MemoTable model) {
            /// modelに従ってfigure.Childrenをfigure.TableDataのCellに割り当てる
            /// figure.TableDataのCellのRow/ColSpanも設定する
            var tableData = figure.TableData;

            if (context.Kind == EditorRefreshKind.Inserted) {
                if (tableData.RowCount == 0 && tableData.ColumnCount == 0) {
                    /// 初めて作られたときはともに0
                    /// Pasteされたときは違う
                    tableData.Redim(model.RowCount, model.ColumnCount);

                    for (int r = 0, rlen = model.RowCount; r < rlen; ++r) {
                        var modelRow = model.Rows.ElementAt(r);
                        var figRow = tableData.Rows.ElementAt(r);
                        for (int c = 0, clen = model.ColumnCount; c < clen; ++c) {
                            var modelCell = modelRow.Cells.ElementAt(c);
                            if (modelCell.RowSpan != 1 || modelCell.ColumnSpan != 1) {
                                var figCell = figRow.Cells.ElementAt(c);
                                tableData.SetSpan(figCell, modelCell.ColumnSpan, modelCell.RowSpan);
                            }
                        }
                    }
                }
            }

            if (context.Kind == EditorRefreshKind.ModelUpdated) {
                /// modelの行または列が変更された
                
                var facade = MemopadApplication.Instance;
                var cause = context.Cause as DetailedPropertyChangedEventArgs;
                if (cause != null) {
                    if (cause.PropertyName == "Rows") {
                        switch (cause.Kind) {
                            case PropertyChangeKind.Add: {
                                if (cause.IsIndexed) {
                                    var index = cause.Index;

                                    var figRow = tableData.InsertRow(index);

                                    /// 10を足すとちょうどMemoTabelCellController.CommitFocus()での自動調整と同じ高さになる
                                    figRow.Height = facade.Settings.GetDefaultMemoTextFontHeight() + 10;

                                    var row = cause.NewValue as MemoTableRow;
                                    foreach (var cell in row.Cells) {
                                        cell.StyledText.Font = facade.Settings.GetDefaultMemoTextFont();
                                    }
                                }
                                break;
                            }
                            case PropertyChangeKind.Remove: {
                                if (cause.IsIndexed) {
                                    var index = cause.Index;
                                    tableData.RemoveRowAt(index);
                                }
                                break;
                            }
                            default: {
                                break;
                            }
                        }
                    } else if (cause.PropertyName == "Columns") {
                        switch (cause.Kind) {
                            case PropertyChangeKind.Add: {
                                if (cause.IsIndexed) {
                                    var index = cause.Index;
                                    tableData.InsertColumn(index);

                                    var col = cause.NewValue as MemoTableColumn;
                                    foreach (var cell in col.Cells) {
                                        cell.StyledText.Font = facade.Settings.GetDefaultMemoTextFont();
                                    }
                                }
                                break;
                            }
                            case PropertyChangeKind.Remove: {
                                if (cause.IsIndexed) {
                                    var index = cause.Index;
                                    tableData.RemoveColumnAt(index);
                                }
                                break;
                            }
                            default: {
                                break;
                            }
                        }
                    }
                }
            }

            /// cellにfigureを割り当て
            var rowIndex = 0;
            foreach (var modelRow in model.Rows) {
                var figRow = tableData.Rows.ElementAt(rowIndex);

                var colIndex = 0;
                foreach (var modelCell in modelRow.Cells) {
                    var figCell = figRow.Cells.ElementAt(colIndex);

                    // todo: Spanの設定はここじゃない？
                    //figCell.RowSpan = modelCell.RowSpan;
                    //figCell.ColumnSpan = modelCell.ColumnSpan;

                    var valueEditor = Host.FindEditorFor(modelCell);
                    if (valueEditor != null) {
                        figCell.Value = valueEditor.Figure as TableCellFigure;
                    }

                    ++colIndex;
                }

                ++rowIndex;
            }

            /// 先にSizeを変更しておかないとMinSizeより小さい値がSizeに入っていておかしなことになる
            figure.Size = tableData.Size;
            figure.MinSize = tableData.MinSize;

            figure.InvalidatePaint();
            figure.InvalidateLayout();
        }

        public override void ConfigureEditor(IEditor editor) {
            var facade = MemopadApplication.Instance;

            var editorHandle = new DefaultEditorHandle();
            editorHandle.KeyMap = facade.KeySchema.MemoContentEditorKeyMap;
            editor.InstallEditorHandle(editorHandle);

            editor.InstallHandle(
                new MemoTableRuledLineHandle(() => Figure),
                HandleStickyKind.Always
            );
            editor.InstallHandle(
                new MemoTableFrameHandle(),
                HandleStickyKind.SelectedIncludingChildren
            );

            editor.InstallRole(new ContainerRole());
            editor.InstallRole(new ResizeRole());
            editor.InstallRole(new RemoveRole());
            editor.InstallRole(new CopyRole());
            editor.InstallRole(new ChangeRowHeightRole());
            editor.InstallRole(new ChangeColumnWidthRole());

            var export = new ExportRole();
            export.RegisterExporter("Csv", ExportCsvFile);
            editor.InstallRole(export);
        }

        public override IMemento GetModelMemento() {
            var externalizer = new Externalizer();
            //return externalizer.Save(Model, (key, obj) => key == "Rows");
            return externalizer.Save(Model);
        }

        public override string GetText() {
            var ret = "";
            if (!string.IsNullOrEmpty(Model.Keywords)) {
                ret = Model.Keywords;
            }
            return ret;
        }

        // --- IContainerControl ---
        public bool CanContainChild(IModelDescriptor descriptor) {
            return typeof(MemoTableCell).IsAssignableFrom(descriptor.ModelType);
        }

        public void InsertChild(object child, int index) {

        }

        public void RemoveChild(object child) {

        }

        // --- export ---
        private void ExportCsvFile(IEditor editor, string outputPath) {
            var exporter = new CsvExporter();
            exporter.Export(outputPath, Host);
        }
    }
}
