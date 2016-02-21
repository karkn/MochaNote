/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Controllers;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using Mkamo.Figure.Figures;
using Mkamo.Editor.Handles;
using Mkamo.Editor.Roles;
using Mkamo.Common.DataType;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using Mkamo.StyledText.Core;
using Mkamo.Editor.Focuses;
using System.Windows.Forms;
using Mkamo.Memopad.Internal.Handles;
using Mkamo.Editor.Requests;
using Mkamo.Model.Memo;
using Mkamo.Model.Core;
using Mkamo.Common.Externalize;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Editor.Utils;
using Mkamo.Memopad.Internal.Roles;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Common.Core;
using Mkamo.Memopad.Internal.Focuses;
using Mkamo.Memopad.Properties;
using Mkamo.Common.Collection;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Forms.MouseOperatable;
using Mkamo.Common.Forms.Mouse;
using Mkamo.Memopad.Internal.Controllers.UIProviders;
using Mkamo.Common.Forms.Clipboard;
using System.IO;

namespace Mkamo.Memopad.Internal.Controllers {
    using StyledText = Mkamo.StyledText.Core.StyledText;
    using Mkamo.StyledText.Writer;

    internal class MemoTextController: AbstractMemoContentController<MemoText, SimpleRect>, IConnectableController {
        // ========================================
        // static field
        // ========================================
        private static readonly Insets DefaultPadding = new Insets(10, 4, 10, 4);
        private static readonly Size DefaultMinSize = new Size(40, 8);

        // ========================================
        // field
        // ========================================
        private Lazy<IUIProvider> _uiProvider;

        private bool _firstFocus = true;

        // ========================================
        // constructor
        // ========================================
        public MemoTextController() {
            _uiProvider = new Lazy<IUIProvider>(() => new MemoTextUIProvider(this));
        }

        // ========================================
        // property
        // ========================================
        // === IController ==========
        public override IUIProvider UIProvider {
            get { return _uiProvider.Value; }
        }

        public override bool NeedAdjustSizeOnPrint {
            get { return true; }
        }

        // ========================================
        // method
        // ========================================
        // === IController ==========
        protected override SimpleRect CreateFigure(MemoText model) {
            return new SimpleRect() {
                Foreground = Color.DarkGray,
                IsForegroundEnabled = false,
                IsBackgroundEnabled = false,
                AutoSizeKinds = AutoSizeKinds.FitBoth,
                MinSize = DefaultMinSize,
                Padding = DefaultPadding,
            };
        }

        protected override void RefreshEditor(RefreshContext context, SimpleRect figure, MemoText model) {
            if (InUpdating) {
                return;
            }

            if (figure.DragTarget == null) {
                figure.DragTarget = CreateHostFigureDragTarget();
            }
            if (figure.Padding != DefaultPadding) {
                figure.Padding = DefaultPadding;
            }
            figure.StyledText = (StyledText) model.StyledText.CloneDeeply();
            figure.AdjustSize();

            UpdateMemoMarkHandles(model);
        }

        public override void ConfigureEditor(IEditor editor) {
            var editorHandle = new MemoTextEditorHandle();
            var app = MemopadApplication.Instance;
            editorHandle.KeyMap = app.KeySchema.MemoContentEditorKeyMap;
            editor.InstallEditorHandle(editorHandle);

            //var brush = new SolidBrushDescription(Color.WhiteSmoke);
            var brush = new GradientBrushDescription(Color.FromArgb(250, 250, 250), Color.FromArgb(235, 235, 235), 90);
            var color = Color.FromArgb(200, 200, 200); /// SilverとGainsboroの間
            var frame = new MemoTextFrameHandle(0, 1, new Size(8, 9), color, color, brush);
            editor.InstallHandle(frame, GetHandleStickyKindSetting());

            editor.InstallRole(new SelectRole());
            editor.InstallRole(new FocusRole(InitFocus, CommitFocus));
            editor.InstallRole(new MemoTextResizeRole());
            editor.InstallRole(new RemoveRole());
            editor.InstallRole(new CopyRole());
            editor.InstallRole(new ReorderRole());
            editor.InstallRole(new DropTextRole());
            editor.InstallRole(new SetStyledTextFontRole(() => Model.StyledText, FontModificationKinds.All));
            editor.InstallRole(new SetStyledTextColorRole(() => Model.StyledText));
            editor.InstallRole(new SetStyledTextAlignmentRole(() => Model.StyledText, AlignmentModificationKinds.Horizontal));
            editor.InstallRole(new SetStyledTextListKindRole(() => Model.StyledText));
            editor.InstallRole(new CombineRole(CanCombine, Combine));

            var export = new ExportRole();
            export.RegisterExporter("Html", ExportHtmlFile);
            export.RegisterExporter("Text", ExportTextFile);
            editor.InstallRole(export);

            InitEditorFocus(editor);

            Figure.ShowLineBreak = app.WindowSettings.ShowLineBreak;
            Figure.ShowBlockBreak = app.WindowSettings.ShowBlockBreak;
        }


        public override IMemento GetModelMemento() {
            var ext = new Externalizer();
            return ext.Save(Model, (key, obj) => false);
        }

        public override string GetText() {
            var ret = Model.StyledText.Text;
            if (Model.Keywords != null) {
                ret = ret + Environment.NewLine + Model.Keywords;
            }
            return ret;
        }

        // === INodeController ==========
        public void ConnectOutgoing(object connected) {
            Model.Outgoings.Add(connected as MemoEdge);
        }

        public void ConnectIncoming(object connected) {
            Model.Incomings.Add(connected as MemoEdge);
        }

        public void DisconnectOutgoing(object disconnected) {
            Model.Outgoings.Remove(disconnected as MemoEdge);
        }

        public void DisconnectIncoming(object disconnected) {
            Model.Incomings.Remove(disconnected as MemoEdge);
        }

        // ------------------------------
        // private
        // ------------------------------
        private void InitEditorFocus(IEditor editor) {
            var app = MemopadApplication.Instance;
            var editorFocus = new MemoStyledTextFocus(editor, app.Settings.KeyScheme == KeySchemeKind.Emacs, true);
            editorFocus.IsCurrentLineBackgroundEnable = true;
            editorFocus.IsConsiderImeWindowSize = true;
            editorFocus.IsConsiderHostBounds = true;
            editorFocus.Figure.Padding = DefaultPadding;
            editorFocus.Figure.DragTarget = CreateFocusFigureDragTarget();
            editorFocus.Figure.DragSource = CreateFocusFigureDragSource();

            /// キー入力時にテキストがemptyなら削除
            editorFocus.ShortcutKeyProcess += (sender, e) => {
                RemoveIfEmpty(editorFocus, e.KeyData);
            };
            editorFocus.KeyDown += (sender, e) => {
                RemoveIfEmpty(editorFocus, e.KeyData);
            };
            //editorFocus.KeyDown += (sender, e) => {
            //    var mtext = Host.Model as MemoText;
            //    if (mtext.IsSticky) {
            //        /// stickyなら何もしない
            //        return;
            //    }

            //    var editorCanvas = Host.Site.EditorCanvas;
            //    if (
            //        editorFocus.StyledText.IsEmpty &&
            //        e.KeyData != Keys.ProcessKey &&
            //        editorCanvas.GetImeString().Length == 0
            //    ) {
            //        var parent = Host.Parent;
            //        Host.RequestFocus(FocusKind.Rollback, null);
            //        Host.RequestRemove();
            //        parent.RequestSelect(SelectKind.True, true);
            //    }
            //};
            editorFocus.LinkClicked += (sender, e) => {
                LinkUtil.GoLink(e.Link);
            };

            editor.InstallFocus(editorFocus);

            /// コミット時にStyledTextがemptyなら削除
            editor.FocusChanged += (sender, e) => {
                var mtext = Host.Model as MemoText;
                if (mtext.IsSticky) {
                    /// stickyなら何もしない
                    return;
                }

                if (!editor.IsFocused) {
                    var styledText = Model.StyledText;
                    if (styledText.IsEmpty) {
                        var parent = editor.Parent;
                        editor.RequestRemove();
                        parent.RequestSelect(SelectKind.True, true);
                    }
                }
            };
        }

        private void RemoveIfEmpty(StyledTextFocus focus, Keys keyData) {
            var mtext = Host.Model as MemoText;
            if (mtext.IsSticky) {
                /// stickyなら何もしない
                return;
            }

            var editorCanvas = Host.Site.EditorCanvas;
            if (
                focus.StyledText.IsEmpty &&
                keyData != Keys.ProcessKey &&
                editorCanvas.GetImeString().Length == 0
            ) {
                var parent = Host.Parent;
                Host.RequestFocus(FocusKind.Rollback, null);
                Host.RequestRemove();
                parent.RequestSelect(SelectKind.True, true);
            }
        }

        private object InitFocus(IFocus focus, object model) {
            if (_firstFocus) {
                var app = MemopadApplication.Instance;
                focus.KeyMap = app.KeySchema.MemoContentFocusKeyMap;
                //app.KeySchema.MemoContentFocusKeyBinder.Bind(focus.KeyMap);
                _firstFocus = false;
            }


            /// 自動折り返しのためのMaxSize制限
            var focusNode = (INode) focus.Figure;
            var editorNode = (INode) focus.Host.Figure;
            focusNode.MinSize = editorNode.MinSize;
            focusNode.MaxSize = editorNode.MaxSize;

            focus.Figure.Foreground = Color.FromArgb(200, 200, 200); /// SilverとGainsBoroの間
            focus.Figure.Background = new SolidBrushDescription(Color.White);
            //focus.Figure.Background = new SolidBrushDescription(Color.Ivory);

            focus.Figure.ShowLineBreak = Figure.ShowLineBreak;
            focus.Figure.ShowBlockBreak = Figure.ShowBlockBreak;

            return ((MemoText) model).StyledText.CloneDeeply();
        }

        private FocusUndoer CommitFocus(IFocus focus, object model, object value, bool isRedo, out bool isCancelled) {
            var memoText = (MemoText) model;
            var stfocus = (StyledTextFocus) focus;

            if (stfocus.IsModified || isRedo) {
                var oldSTText = memoText.StyledText;
                memoText.StyledText = (StyledText) value;
                isCancelled = false;

                /// 自動折り返しのためのMaxSize制限解除
                /// やっておかないとStyledTextFocusのRelocateのMinSize設定で例外が起こる。
                /// 
                /// memoText.StyledText = (StyledText) value; の後でやらないと
                /// モデル更新 => RefreshEditor() => StyledTextFocus.Relocate()が起こって
                /// StyledTextFocusFigureのWidthが文字列が折り返しされないときのWidthになってしまう。
                /// その後HandleのRelocate()が呼ばれてまだIsFocused=trueなのでFrameの枠のWidthがおかしくなる。
                var focusNode = (INode) focus.Figure;
                focusNode.MaxSize = new Size(int.MaxValue, int.MaxValue);

                /// コメント線の処理
                DisconnectRemovedAnchor(Figure, focusNode);

                using (Host.Site.CommandExecutor.BeginChain()) {
                    var moved = new List<IEditor>();
                    MoveOverlapped(Host, moved);
                }

                return (f, m) => {
                    memoText.StyledText = oldSTText;
                };
            } else {
                isCancelled = false;
                return null;
            }
        }        


        private void MoveOverlapped(IEditor editor, List<IEditor> moved) {
            moved.Add(editor);
            var bounds = editor.Figure.Bounds;
            foreach (var edi in Host.Root.Content.GetChildrenByPosition()) {
                if (moved.Contains(edi)) {
                    continue;
                }

                if (!(edi.Model is MemoText || edi.Model is MemoImage || edi.Model is MemoTable || edi.Model is MemoFile)) {
                    continue;
                }

                var ediBounds = edi.Figure.Bounds;
                if (
                    (ediBounds.Left > bounds.Left || ediBounds.Top > bounds.Top) &&
                    ediBounds.IntersectsWith(bounds)
                ) {
                    var needMoveLower = false;
                    var needMoveRight = false;
                    if (bounds.Contains(ediBounds)) {
                        var center = RectUtil.GetCenter(bounds);
                        var ediCenter = RectUtil.GetCenter(ediBounds);
                        var dir = PointUtil.GetDirectionStraightly(center, ediCenter);
                        if (EnumUtil.HasAnyFlags((int) dir, (int) (Directions.Left | Directions.Down))) {
                            needMoveLower = true;
                        } else {
                            needMoveRight = true;
                        }
                    } else if (ediBounds.Left <= bounds.Left) {
                        needMoveLower = true;
                    } else if (ediBounds.Top <= bounds.Top) {
                        needMoveRight = true;
                    } else {
                        var intersect = Rectangle.Intersect(bounds, ediBounds);
                        if (intersect.Width == ediBounds.Width) {
                            /// LeftもRightもbounds内
                            needMoveLower = true;
                        } else if (intersect.Height == ediBounds.Height) {
                            /// TopもBottomもbounds内
                            needMoveRight = true;
                        } else if (intersect.Width >= intersect.Height) {
                            needMoveLower = true;
                        } else {
                            needMoveRight = true;
                        }
                    }

                    if (needMoveLower) {
                        var newTop = bounds.Bottom + MemopadConsts.DefaultElementSpace;
                        edi.RequestMove(new Size(0, newTop - ediBounds.Top));
                        MoveOverlapped(edi, moved);
                    } else if (needMoveRight) {
                        var newLeft = bounds.Right + MemopadConsts.DefaultElementSpace;
                        edi.RequestMove(new Size(newLeft- ediBounds.Left, 0));
                        MoveOverlapped(edi, moved);
                    }
                }
            }

        }

        private string CopyStyledText(IEditor editor, IDataObject dataObj) {
            if (Model != null) {
                return Model.StyledText.ToHtmlText();
            }
            return string.Empty;
        }

        //private MemoryStream AggregateCopied(IEnumerable<string> parts) {
        //    var buf = new StringBuilder();
        //    foreach (var part in parts) {
        //        buf.AppendLine(part);
        //    }
        //    return ClipboardUtil.GetCFHtmlMemoryStream(buf.ToString());
        //}

        private HandleStickyKind GetHandleStickyKindSetting() {
            var app = MemopadApplication.Instance;
            return (HandleStickyKind) app.Settings.MemoTextFrameVisiblePolicy;
        }

        // --- combine ---
        private bool CanCombine(IEditor target, IEnumerable<IEditor> combineds) {
            return combineds != null && combineds.All(editor => editor.Model is MemoText);
        }

        private EditorCombinatorUndoer Combine(IEditor target, IEnumerable<IEditor> combineds) {
            var old = Model.StyledText;
            var stext = (StyledText) old.CloneDeeply();

            var ordereds = combineds.OrderBy(
                editor => editor.Figure.Bounds,
                new RectanglePositionalComparer()
            );

            foreach (var combined in ordereds) {
                var mtext = combined.Model as MemoText;
                if (mtext != null) {
                    var cloned = (StyledText) mtext.StyledText.CloneDeeply();
                    foreach (var block in cloned.Blocks) {
                        stext.InsertAfter(block);
                    }
                }
            }

            Model.StyledText = stext;
            return (edi) => Model.StyledText = old;
        }

        // --- export ---
        private void ExportHtmlFile(IEditor editor, string outputPath) {
            var stext = Model.StyledText;
            var html = stext.ToHtmlText();
            File.WriteAllText(outputPath, html, Encoding.UTF8);
        }

        private void ExportTextFile(IEditor editor, string outputPath) {
            var stext = Model.StyledText;
            var settings = new PlainTextWriterSettings();
            var html = stext.ToPlainText(settings);
            File.WriteAllText(outputPath, html, Encoding.UTF8);
        }
    }
}
