/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Controllers;
using Mkamo.Figure.Core;
using Mkamo.Figure.Figures;
using Mkamo.Editor.Roles;
using Mkamo.Editor.Handles;
using Mkamo.Memopad.Internal.Handles;
using Mkamo.Common.Core;
using System.Windows.Forms;
using Mkamo.Model.Memo;
using Mkamo.Container.Core;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Common.Externalize;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Model.Core;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Core;
using System.Drawing;
using Mkamo.Editor.Commands;
using Mkamo.Editor.Focuses;
using System.IO;
using System.Drawing.Imaging;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Win32.User32;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Common.Forms.MouseOperatable;
using System.Collections.Specialized;
using Mkamo.Common.Forms.Clipboard;
using Mkamo.Common.Forms.Mouse;
using Mkamo.StyledText.Core;
using Mkamo.Editor.Roles.Container;
using Mkamo.Memopad.Internal.Roles;

namespace Mkamo.Memopad.Internal.Controllers {
    internal class MemoController: AbstractModelController<Memo, SolidLayer>, IContainerController {
        // ========================================
        // field
        // ========================================
        private Lazy<IUIProvider> _uiProvider;

        // ========================================
        // constructor
        // ========================================
        public MemoController() {
            _uiProvider = new Lazy<IUIProvider>(() => new UI(this));
        }


        // ========================================
        // property
        // ========================================
        public IEnumerable<object> Children {
            get { return Model.Contents.Items.As<MemoElement, object>(); }
            //get { return null; }
        }

        public override IUIProvider UIProvider {
            get { return _uiProvider.Value; }
        }

        public int ChildCount {
            get { return Model.Contents.Count; }
            //get { return 0; }
        }

        public bool SyncChildEditors {
            get { return false; }
        }

        // ========================================
        // method
        // ========================================
        public override void Activate() {

        }

        public override void Deactivate() {

        }

        public override void ConfigureEditor(IEditor editor) {
            var app = MemopadApplication.Instance;

            var containerRole = new ContainerRole();
            editor.InstallRole(containerRole);
            {
                var pasteRole = containerRole.PasteRole;
                pasteRole.RegisterPaster(StyledTextConsts.BlocksAndInlinesFormat.Name, MemoEditorHelper.PasteBlocksAndInlines);
                pasteRole.RegisterPaster(DataFormats.CommaSeparatedValue, MemoEditorHelper.PasteCsv);
                pasteRole.RegisterPaster(DataFormats.Html, MemoEditorHelper.PasteHtml);
                pasteRole.RegisterPaster(DataFormats.EnhancedMetafile, MemoEditorHelper.PasteMetafile);
                pasteRole.RegisterPaster(DataFormats.Bitmap, MemoEditorHelper.PasteImage);
                pasteRole.RegisterPaster(DataFormats.FileDrop, MemoEditorHelper.PasteFileDrops);
                pasteRole.RegisterPaster(DataFormats.UnicodeText, MemoEditorHelper.PasteText);
            }
            editor.InstallRole(new CreateFreehandRole());
            editor.InstallRole(new CreateChildCommentEdgeRole());
            editor.InstallRole(new AdjustSpaceRole());

            var editorHandle = new DefaultEditorHandle();
            editorHandle.Cursor = Cursors.IBeam;
            var scenario = new MemoEditorScenario(editorHandle);
            scenario.Apply();
            var facade = MemopadApplication.Instance;
            editorHandle.KeyMap = facade.KeySchema.MemoEditorKeyMap;
            editor.InstallEditorHandle(editorHandle);
        }

        protected override SolidLayer CreateFigure(Memo model) {
            return new SolidLayer();
        }

        protected override void RefreshEditor(RefreshContext context, SolidLayer figure, Memo model) {
            if (figure.DragTarget == null) {
                figure.DragTarget = CreateDragTarget();
            }
        }

        public override IMemento GetModelMemento() {
            return null;
        }

        public bool CanContainChild(IModelDescriptor descriptor) {
            return typeof(MemoElement).IsAssignableFrom(descriptor.ModelType);
        }

        public void InsertChild(object child, int index) {
            Model.Contents.Insert(index, (MemoElement) child);
        }

        public void RemoveChild(object child) {
            Model.Contents.Remove((MemoElement) child);
        }

        public override string GetText() {
            var ret = Model.Title;
            //if (Model.Keywords != null) {
            //    ret = ret + Environment.NewLine + Model.Keywords;
            //}
            return ret;
        }

        // ------------------------------
        // private
        // ------------------------------
        private IDragTarget CreateDragTarget() {
            var ret = MouseOperatableFactory.CreateDragTarget();

            var app = MemopadApplication.Instance;

            ret.DragOver += (sender, e) => {
                e.Effect = DragDropEffects.None;

                var data = e.Data;

                if (data.GetDataPresent(typeof(MemoInfo[]))) {
                    if (DragDropUtil.IsLinkAllowed(e)) {
                        e.Effect = DragDropEffects.Link;
                    }

                } else if (data.GetDataPresent(DataFormats.FileDrop)) {
                    if (DragDropUtil.IsControlPressed(e)) {
                        if (DragDropUtil.IsCopyAllowed(e)) {
                            e.Effect = DragDropEffects.Copy;
                        }
                    } else {
                        if (DragDropUtil.IsLinkAllowed(e)) {
                            e.Effect = DragDropEffects.Link;
                        }
                    }

                } else if (data.GetDataPresent(StyledTextConsts.BlocksAndInlinesFormat.Name)) {
                    if (DragDropUtil.IsControlPressed(e)) {
                        if (DragDropUtil.IsCopyAllowed(e)) {
                            e.Effect = DragDropEffects.Copy;
                        } else if (DragDropUtil.IsMoveAllowed(e)) {
                            e.Effect = DragDropEffects.Move;
                        }
                    } else {
                        if (DragDropUtil.IsMoveAllowed(e)) {
                            e.Effect = DragDropEffects.Move;
                        } else if (DragDropUtil.IsCopyAllowed(e)) {
                            e.Effect = DragDropEffects.Copy;
                        }
                    }

                } else if (data.GetDataPresent(DataFormats.Html)) {
                    if (DragDropUtil.IsControlPressed(e)) {
                        if (DragDropUtil.IsCopyAllowed(e)) {
                            e.Effect = DragDropEffects.Copy;
                        } else if (DragDropUtil.IsMoveAllowed(e)) {
                            e.Effect = DragDropEffects.Move;
                        }
                    } else {
                        if (DragDropUtil.IsMoveAllowed(e)) {
                            e.Effect = DragDropEffects.Move;
                        } else if (DragDropUtil.IsCopyAllowed(e)) {
                            e.Effect = DragDropEffects.Copy;
                        }
                    }

                } else if (data.GetDataPresent("UniformResourceLocator") && data.GetDataPresent(DataFormats.UnicodeText)) {
                    if (DragDropUtil.IsLinkAllowed(e)) {
                        e.Effect = DragDropEffects.Link;
                    }
                } else if (data.GetDataPresent(DataFormats.UnicodeText)) {
                    if (DragDropUtil.IsControlPressed(e)) {
                        if (DragDropUtil.IsCopyAllowed(e)) {
                            e.Effect = DragDropEffects.Copy;
                        } else if (DragDropUtil.IsMoveAllowed(e)) {
                            e.Effect = DragDropEffects.Move;
                        }
                    } else {
                        if (DragDropUtil.IsMoveAllowed(e)) {
                            e.Effect = DragDropEffects.Move;
                        } else if (DragDropUtil.IsCopyAllowed(e)) {
                            e.Effect = DragDropEffects.Copy;
                        }
                    }
                }
            };

            ret.DragDrop += (sender, e) => {
                if (DragDropUtil.IsNone(e)) {
                    return;
                }

                var oldCursor = Host.Site.EditorCanvas.Cursor;
                Host.Site.EditorCanvas.Cursor = Cursors.WaitCursor;

                try {
                    var data = e.Data;
                    var loc = Host.Site.GridService.GetAdjustedPoint(new Point(e.X, e.Y));
    
                    if (data.GetDataPresent(typeof(MemoInfo[]))) {
                        /// MemoInfo[]
                        if (DragDropUtil.IsLink(e)) {
                            var infos = (MemoInfo[]) data.GetData(typeof(MemoInfo[]));
                            foreach (var info in infos) {
                                var text = info.Title;
                                var uri = UriUtil.GetUri(info);
                                var created = MemoEditorHelper.AddTextAsLink(Host, loc, text, uri, null);
                            }
                            Host.Site.EditorCanvas.Select();
                        }
    
                    } else if (data.GetDataPresent(DataFormats.FileDrop)) {
                        /// FileDrop
                        if (DragDropUtil.IsCopy(e) || DragDropUtil.IsLink(e)) {
                            MemoEditorHelper.AddFileDrops(
                                Host,
                                loc,
                                (string[]) data.GetData(DataFormats.FileDrop),
                                e.Effect == DragDropEffects.Copy,
                                true,
                                false
                            );
                            Host.Site.EditorCanvas.Select();
                        }
    
                    } else if (data.GetDataPresent(StyledTextConsts.BlocksAndInlinesFormat.Name)) {
                        /// StyledText flows
                        if (DragDropUtil.IsCopy(e) || DragDropUtil.IsMove(e)) {
                            MemoEditorHelper.AddBlocksAndInlines(
                                Host,
                                loc,
                                data.GetData(StyledTextConsts.BlocksAndInlinesFormat.Name) as IEnumerable<Flow>,
                                true
                            );
                            Host.Site.EditorCanvas.Select();
                        }
    
                    } else if (data.GetDataPresent(DataFormats.Html)) {
                        /// Html
                        if (DragDropUtil.IsCopy(e) || DragDropUtil.IsMove(e)) {
                            var html = Common.Forms.Clipboard.ClipboardUtil.GetCFHtmlFromDataObject(data);
                            MemoEditorHelper.AddHtml(
                                Host,
                                loc,
                                html,
                                true
                            );
                            Host.Site.EditorCanvas.Select();
                        }
    
                    } else if (data.GetDataPresent("UniformResourceLocator") && data.GetDataPresent(DataFormats.UnicodeText)) {
                        /// URL
                        if (DragDropUtil.IsLink(e)) {
                            var url = (string) data.GetData(DataFormats.UnicodeText);
                            var created = MemoEditorHelper.AddTextAsLink(Host, loc, url, url, null);
                            Host.Site.EditorCanvas.Select();
                        }
    
                    } else if (data.GetDataPresent(DataFormats.UnicodeText)) {
                        /// text
                        if (DragDropUtil.IsCopy(e) || DragDropUtil.IsMove(e)) {
                            MemoEditorHelper.AddText(Host, loc, (string) data.GetData(DataFormats.UnicodeText), false);
                            Host.Site.EditorCanvas.Select();
                        }
                    }
                } finally {
                    Host.Site.EditorCanvas.Cursor = oldCursor;
                }
            };

            return ret;
        }

        // ========================================
        // class
        // ========================================
        private class UI: AbstractUIProvider {
            private MemoController _owner;

            private ToolStripMenuItem _copyAsImage;
            private ToolStripMenuItem _saveAsImage;

            private ToolStripMenuItem _spaceRight;
            private ToolStripMenuItem _spaceBottom;

            public UI(MemoController owner): base(false) {
                _owner = owner;
            }

            public override ContextMenuStrip GetContextMenu(MouseEventArgs e) {
                if (_copyAsImage == null || _saveAsImage == null) {
                    InitItems();
                }

                _ContextMenu.Items.Clear();
                _ContextMenu.Items.Add(_copyAsImage);
                _ContextMenu.Items.Add(_saveAsImage);

                _ContextMenu.Items.Add(_Separator1);

                _ContextMenu.Items.Add(_spaceRight);
                _ContextMenu.Items.Add(_spaceBottom);

                return _ContextMenu;
            }

            public override void ConfigureDetailForm(DetailSettingsForm detailForm) {
            }

            private void InitItems() {
                _copyAsImage = new ToolStripMenuItem("全体を画像としてコピー(&I)");
                _copyAsImage.Click += (sender, ev) => {
                    _owner.Host.Site.EditorCanvas.CopyAsImage();
                };

                _saveAsImage = new ToolStripMenuItem("全体を画像として保存(&S)");

                var saveAsEmf = new ToolStripMenuItem("EMFファイル(&E)");
                saveAsEmf.Click += (sender, ev) => {
                    var dialog = new SaveFileDialog();
                    dialog.RestoreDirectory = true;
                    dialog.ShowHelp = true;
                    dialog.Filter = "EMF Files(*.emf)|*.emf";
                    if (dialog.ShowDialog() == DialogResult.OK) {
                        _owner.Host.Site.EditorCanvas.SaveAsEmf(dialog.FileName);
                    }
                    dialog.Dispose();
                };
                _saveAsImage.DropDownItems.Add(saveAsEmf);

                var saveAsPng = new ToolStripMenuItem("PNGファイル(&P)");
                saveAsPng.Click += (sender, ev) => {
                    var dialog = new SaveFileDialog();
                    dialog.RestoreDirectory = true;
                    dialog.ShowHelp = true;
                    dialog.Filter = "PNG Files(*.png)|*.png";
                    if (dialog.ShowDialog() == DialogResult.OK) {
                        _owner.Host.Site.EditorCanvas.SaveAsPng(dialog.FileName);
                    }
                    dialog.Dispose();
                };
                _saveAsImage.DropDownItems.Add(saveAsPng);

                var saveAsJpeg = new ToolStripMenuItem("JPEGファイル(&J)");
                saveAsJpeg.Click += (sender, ev) => {
                    var dialog = new SaveFileDialog();
                    dialog.RestoreDirectory = true;
                    dialog.ShowHelp = true;
                    dialog.Filter = "JPEG Files(*.jpg)|*.jpg";
                    if (dialog.ShowDialog() == DialogResult.OK) {
                        _owner.Host.Site.EditorCanvas.SaveAsJpeg(dialog.FileName);
                    }
                    dialog.Dispose();
                };
                _saveAsImage.DropDownItems.Add(saveAsJpeg);

                _spaceRight = new ToolStripMenuItem("右に余白(&R)");
                _spaceRight.Click += (sender, ev) => {
                    var canvas = _owner.Host.Site.EditorCanvas;
                    var size = canvas.AutoScrollMinSize;
                    canvas.ReservedMinSize = new Size(size.Width + 400, size.Height);
                };

                _spaceBottom = new ToolStripMenuItem("下に余白(&B)");
                _spaceBottom.Click += (sender, ev) => {
                    var canvas = _owner.Host.Site.EditorCanvas;
                    var size = canvas.AutoScrollMinSize;
                    canvas.ReservedMinSize = new Size(size.Width, size.Height + 400);
                };
            }
        }
    }
}
