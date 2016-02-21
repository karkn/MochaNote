/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Common.Core;
using Mkamo.Memopad.Internal.Handles;
using Mkamo.Editor.Core;
using Mkamo.Model.Memo;
using Mkamo.Common.Collection;
using Mkamo.Common.Forms.MouseOperatable;
using System.Windows.Forms;
using Mkamo.Memopad.Core;
using System.Drawing;
using Mkamo.Editor.Requests;
using Mkamo.Common.Forms.Mouse;
using Mkamo.Editor.Focuses;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Common.DataType;
using Mkamo.StyledText.Commands;
using Mkamo.StyledText.Core;
using Mkamo.StyledText.Writer;
using Mkamo.Common.Forms.Clipboard;
using Mkamo.Memopad.Internal.Core;

namespace Mkamo.Memopad.Internal.Controllers {
    internal abstract class AbstractMemoContentController<TModel, TFigure>: AbstractModelController<TModel, TFigure>
        where TModel: MemoContent
        where TFigure: class, IFigure {

        // ========================================
        // field
        // ========================================
        private Lazy<CompositeMemoMarkHandle> _compositeMemoMarkHandle;

        private Range _selectionRangeOnDragStart;

        private int _updateDepth;

        // ========================================
        // constructor
        // ========================================
        public AbstractMemoContentController() {
            _compositeMemoMarkHandle = new Lazy<CompositeMemoMarkHandle>(
                () => {
                    var ret = new CompositeMemoMarkHandle();
                    Host.InstallHandle(ret, HandleStickyKind.Always);
                    return ret;
                }
            );

            _updateDepth = 0;
        }

        // ========================================
        // property
        // ========================================
        public bool InUpdating {
            get { return _updateDepth > 0; }
        }

        // ========================================
        // method
        // ========================================
        public void BeginUpdate() {
            ++_updateDepth;
        }

        public void EndUpdate() {
            --_updateDepth;
            if (_updateDepth == 0) {
                RefreshEditor(new RefreshContext(EditorRefreshKind.ModelUpdated), Host.Figure, Host.Model);
            }
        }

        protected void UpdateMemoMarkHandles(TModel model) {
            if (!model.IsMarkable) {
                return;
            }

            /// MarkのHandleを更新
            if (model.Marks.Count > 0) {
                var markHandle = _compositeMemoMarkHandle.Value;

                /// いらないmark handleを削除
                ICollectionUtil.Remove(
                    markHandle.Children,
                    handle => {
                        var mh = handle as MemoMarkHandle;
                        if (mh != null) {
                            return !model.Marks.Contains(mh.Mark);
                        }
                        return true;
                    }
                );

                /// mark handleを追加
                foreach (var mark in model.Marks) {
                    var contained = markHandle.Children.Any(
                        handle => {
                            var mh = handle as MemoMarkHandle;
                            if (mh != null) {
                                return mh.Mark == mark;
                            }
                            return false;
                        }
                    );
                    if (!contained) {
                        var handle = new MemoMarkHandle();
                        handle.Mark = mark;
                        markHandle.Children.Add(handle);
                    }
                }

                if (Host != null && Host.Figure != null) {
                    markHandle.Relocate(Host.Figure);
                }

            } else {
                if (_compositeMemoMarkHandle.IsValueCreated && _compositeMemoMarkHandle.Value.Children.Count > 0) {
                    /// いらないmark handleを削除
                    var markHandle = _compositeMemoMarkHandle.Value;

                    ICollectionUtil.Remove(
                        markHandle.Children,
                        handle => {
                            var mh = handle as MemoMarkHandle;
                            if (mh != null) {
                                return !model.Marks.Contains(mh.Mark);
                            }
                            return true;
                        }
                    );
                }
            }
        }


        protected void DisconnectRemovedAnchor(INode anchorIncomingNode, INode edittedNode) {
            var disconnectings = new List<IEditor>();
            foreach (var incoming in anchorIncomingNode.Incomings) {
                var edge = incoming as IEdge;
                if (edge != null) {
                    var opt = edge.TargetConnectionOption;
                    /// optがあるものはコメント線と決め打ち
                    if (opt != null) {
                        var pt = edittedNode.GetConnectionPoint(opt);
                        if (pt == null) {
                            /// 対応するanchorが削除されたとき
                            disconnectings.Add(edge.GetEditor());
                        }

                        /// MemoTextのBoundsが変わらなくても
                        /// 編集されたらRoute()するようにする
                        edge.Route();
                    }
                }
            }

            /// 対応するanchorを削除されたedgeは切断
            foreach (var disconn in disconnectings) {
                var edge = disconn.Figure as IEdge;
                disconn.RequestConnect(edge.TargetAnchor, null, edge.TargetAnchor.Location);
            }
        }

        protected IDragTarget CreateHostFigureDragTarget() {
            var ret = MouseOperatableFactory.CreateDragTarget();

            ret.DragOver += (sender, e) => {
                e.Effect = DragDropEffects.None;

                var data = e.Data;

                if (data.GetDataPresent(typeof(MemoInfo[]))) {
                    if (DragDropUtil.IsLinkAllowed(e)) {
                        e.Effect = DragDropEffects.Link;
                        ShowHostFigureDropTextFeedback(new Point(e.X, e.Y));
                    }
                } else if (data.GetDataPresent(StyledTextConsts.BlocksAndInlinesFormat.Name)) {
                    if (DragDropUtil.IsControlPressed(e)) {
                        if (DragDropUtil.IsCopyAllowed(e)) {
                            e.Effect = DragDropEffects.Copy;
                            ShowHostFigureDropTextFeedback(new Point(e.X, e.Y));
                        } else {
                            e.Effect = DragDropEffects.Move;
                            ShowHostFigureDropTextFeedback(new Point(e.X, e.Y));
                        }
                    } else {
                        if (DragDropUtil.IsMoveAllowed(e)) {
                            e.Effect = DragDropEffects.Move;
                            ShowHostFigureDropTextFeedback(new Point(e.X, e.Y));
                        } else if (DragDropUtil.IsCopyAllowed(e)) {
                            e.Effect = DragDropEffects.Copy;
                            ShowHostFigureDropTextFeedback(new Point(e.X, e.Y));
                        }
                    }

                } if (data.GetDataPresent("UniformResourceLocator") && data.GetDataPresent(DataFormats.UnicodeText)) {
                    if (DragDropUtil.IsLinkAllowed(e)) {
                        e.Effect = DragDropEffects.Link;
                        ShowHostFigureDropTextFeedback(new Point(e.X, e.Y));
                    }

                } else if (data.GetDataPresent(DataFormats.UnicodeText)) {
                    if (DragDropUtil.IsControlPressed(e)) {
                        if (DragDropUtil.IsCopyAllowed(e)) {
                            e.Effect = DragDropEffects.Copy;
                            ShowHostFigureDropTextFeedback(new Point(e.X, e.Y));
                        } else {
                            e.Effect = DragDropEffects.Move;
                            ShowHostFigureDropTextFeedback(new Point(e.X, e.Y));
                        }
                    } else {
                        if (DragDropUtil.IsMoveAllowed(e)) {
                            e.Effect = DragDropEffects.Move;
                            ShowHostFigureDropTextFeedback(new Point(e.X, e.Y));
                        } else if (DragDropUtil.IsCopyAllowed(e)) {
                            e.Effect = DragDropEffects.Copy;
                            ShowHostFigureDropTextFeedback(new Point(e.X, e.Y));
                        }
                    }
                }
            };

            ret.DragLeave += (sender, e) => {
                /// feedbackを隠す
                Host.HideFeedback(new DropTextRequest(Rectangle.Empty));
            };

            ret.DragDrop += (sender, e) => {
                if (DragDropUtil.IsNone(e)) {
                    return;
                }

                var data = e.Data;
                var loc = new Point(e.X, e.Y);
                var focus = Host.Focus as StyledTextFocus;

                if (data.GetDataPresent(typeof(MemoInfo[]))) {
                    /// MemoInfo[]
                    if (DragDropUtil.IsLink(e)) {
                        Host.RequestSelect(SelectKind.True, true);
                        Host.RequestFocus(FocusKind.Begin, loc);
                        if (focus != null) {
                            var infos = (MemoInfo[]) data.GetData(typeof(MemoInfo[]));
                            foreach (var info in infos) {
                                var text = info.Title;
                                var url = UriUtil.GetUri(info);
                                var charIndex = focus.Referer.CaretIndex;
                                focus.InsertText(text, false);

                                /// set link
                                focus.Selection.Range = new Range(charIndex, text.Length);
                                focus.SetLink(url, null);
                                focus.Selection.Range = Range.Empty;
                            }
                        }
                        Host.HideFeedback(new DropTextRequest(Rectangle.Empty));
                        Host.Site.EditorCanvas.Select();
                    }

                } else if (data.GetDataPresent(StyledTextConsts.BlocksAndInlinesFormat.Name)) {
                    /// StyledText flows
                    if (DragDropUtil.IsCopy(e) || DragDropUtil.IsMove(e)) {
                        Host.RequestSelect(SelectKind.True, true);
                        Host.RequestFocus(FocusKind.Begin, loc);
                        if (focus != null) {
                            var flows = (IEnumerable<Flow>) data.GetData(StyledTextConsts.BlocksAndInlinesFormat.Name);
                            focus.InsertBlocksAndInlines(flows);
                        }
                        Host.HideFeedback(new DropTextRequest(Rectangle.Empty));
                        Host.Site.EditorCanvas.Select();
                    }

                } else if (data.GetDataPresent("UniformResourceLocator") && data.GetDataPresent(DataFormats.UnicodeText)) {
                    /// URL
                    if (DragDropUtil.IsLink(e)) {
                        Host.RequestSelect(SelectKind.True, true);
                        Host.RequestFocus(FocusKind.Begin, loc);
                        if (focus != null) {
                            var charIndex = focus.Referer.CaretIndex;
                            var url = (string) data.GetData(DataFormats.UnicodeText);
                            var len = url.Length;
                            focus.InsertText(url, false);

                            /// set link
                            focus.Selection.Range = new Range(charIndex, len);
                            focus.SetLink(url, null);
                            focus.Selection.Range = Range.Empty;
                        }
                        Host.HideFeedback(new DropTextRequest(Rectangle.Empty));
                        Host.Site.EditorCanvas.Select();
                    }

                } else if (data.GetDataPresent(DataFormats.UnicodeText)) {
                    /// Text
                    if (DragDropUtil.IsCopy(e) | DragDropUtil.IsMove(e)) {
                        Host.RequestSelect(SelectKind.True, true);
                        Host.RequestFocus(FocusKind.Begin, loc);
                        if (focus != null) {
                            focus.InsertText((string) data.GetData(DataFormats.UnicodeText), false);
                        }
                        Host.HideFeedback(new DropTextRequest(Rectangle.Empty));
                        Host.Site.EditorCanvas.Select();
                    }
                }
            };

            return ret;
        }

    
        protected IDragTarget CreateFocusFigureDragTarget() {
            var ret = MouseOperatableFactory.CreateDragTarget();

            ret.DragOver += (sender, e) => {
                e.Effect = DragDropEffects.None;

                var data = e.Data;

                if (data.GetDataPresent(typeof(MemoInfo[]))) {
                    if (DragDropUtil.IsLinkAllowed(e)) {
                        e.Effect = DragDropEffects.Link;
                        ShowFocusFigureDropTextFeedback(new Point(e.X, e.Y));
                    }

                } else if (data.GetDataPresent(StyledTextConsts.BlocksAndInlinesFormat.Name)) {
                    if (DragDropUtil.IsControlPressed(e)) {
                        if (DragDropUtil.IsCopyAllowed(e)) {
                            e.Effect = DragDropEffects.Copy;
                            ShowFocusFigureDropTextFeedback(new Point(e.X, e.Y));
                        } else {
                            e.Effect = DragDropEffects.Move;
                            ShowFocusFigureDropTextFeedback(new Point(e.X, e.Y));
                        }
                    } else {
                        if (DragDropUtil.IsMoveAllowed(e)) {
                            e.Effect = DragDropEffects.Move;
                            ShowFocusFigureDropTextFeedback(new Point(e.X, e.Y));
                        } else if (DragDropUtil.IsCopyAllowed(e)) {
                            e.Effect = DragDropEffects.Copy;
                            ShowFocusFigureDropTextFeedback(new Point(e.X, e.Y));
                        }
                    }

                } if (data.GetDataPresent("UniformResourceLocator") && data.GetDataPresent(DataFormats.UnicodeText)) {
                    if (DragDropUtil.IsLinkAllowed(e)) {
                        e.Effect = DragDropEffects.Link;
                        ShowFocusFigureDropTextFeedback(new Point(e.X, e.Y));
                    }

                } else if (data.GetDataPresent(DataFormats.UnicodeText)) {
                    if (DragDropUtil.IsControlPressed(e)) {
                        if (DragDropUtil.IsCopyAllowed(e)) {
                            e.Effect = DragDropEffects.Copy;
                            ShowFocusFigureDropTextFeedback(new Point(e.X, e.Y));
                        } else {
                            e.Effect = DragDropEffects.Move;
                            ShowFocusFigureDropTextFeedback(new Point(e.X, e.Y));
                        }
                    } else {
                        if (DragDropUtil.IsMoveAllowed(e)) {
                            e.Effect = DragDropEffects.Move;
                            ShowFocusFigureDropTextFeedback(new Point(e.X, e.Y));
                        } else if (DragDropUtil.IsCopyAllowed(e)) {
                            e.Effect = DragDropEffects.Copy;
                            ShowFocusFigureDropTextFeedback(new Point(e.X, e.Y));
                        }
                    }
                }
            };

            ret.DragLeave += (sender, e) => {
                /// feedbackを隠す
                Host.HideFeedback(new DropTextRequest(Rectangle.Empty));
            };

            ret.DragDrop += (sender, e) => {
                if (DragDropUtil.IsNone(e)) {
                    return;
                }

                var data = e.Data;
                var loc = new Point(e.X, e.Y);
                var focus = Host.Focus as StyledTextFocus;

                if (data.GetDataPresent(typeof(MemoInfo[]))) {
                    if (DragDropUtil.IsLink(e)) {
                        if (focus != null && Host.Figure.Root != null) {
                            var infos = (MemoInfo[]) data.GetData(typeof(MemoInfo[]));
                            focus.Selection.Range = Range.Empty;

                            var charIndex = focus.Figure.GetCharIndexAt(loc);
                            foreach (var info in infos) {
                                var text = info.Title;
                                var url = UriUtil.GetUri(info);
                                focus.Referer.CaretIndex = charIndex;
                                focus.InsertText(text, false);

                                /// set link
                                focus.Selection.Range = new Range(charIndex, text.Length);
                                focus.SetLink(url, null);
                                focus.Selection.Range = Range.Empty;
                            }
                        }
                        Host.HideFeedback(new DropTextRequest(Rectangle.Empty));
                        Host.Site.EditorCanvas.Select();
                    }

                } else if (data.GetDataPresent(StyledTextConsts.BlocksAndInlinesFormat.Name)) {
                    /// StyledText flows
                    if (DragDropUtil.IsCopy(e) || DragDropUtil.IsMove(e)) {
                        if (focus != null && Host.Figure.Root != null) {

                            var charIndex = focus.Figure.GetCharIndexAt(loc);

                            if (!_selectionRangeOnDragStart.Contains(charIndex)) {

                                /// Moveの場合はこのタイミングでremoveしてしまう。
                                /// DragFinishでやろうとすると大変。
                                if (!_selectionRangeOnDragStart.IsEmpty && DragDropUtil.IsMove(e)) {
                                    focus.RemoveForward();
                                }

                                focus.Selection.Range = Range.Empty;

                                if (charIndex >= _selectionRangeOnDragStart.End) {
                                    charIndex -= _selectionRangeOnDragStart.Length;
                                }
                                focus.Referer.CaretIndex = charIndex;

                                var flows = (IEnumerable<Flow>) data.GetData(StyledTextConsts.BlocksAndInlinesFormat.Name);
                                focus.InsertBlocksAndInlines(flows);
                            }
                        }
                        Host.HideFeedback(new DropTextRequest(Rectangle.Empty));
                        Host.Site.EditorCanvas.Select();
                    }

                } else if (data.GetDataPresent("UniformResourceLocator") && data.GetDataPresent(DataFormats.UnicodeText)) {
                    if (DragDropUtil.IsLink(e)) {
                        if (focus != null && Host.Figure.Root != null) {
                            focus.Selection.Range = Range.Empty;

                            var url = (string) data.GetData(DataFormats.UnicodeText);
                            var len = url.Length;
                            var charIndex = focus.Figure.GetCharIndexAt(loc);
                            focus.Referer.CaretIndex = charIndex;
                            focus.InsertText(url, false);

                            /// set link
                            focus.Selection.Range = new Range(charIndex, len);
                            focus.SetLink(url, null);
                            focus.Selection.Range = Range.Empty;
                        }
                        Host.HideFeedback(new DropTextRequest(Rectangle.Empty));
                        Host.Site.EditorCanvas.Select();
                    }

                } else if (data.GetDataPresent(DataFormats.UnicodeText)) {
                    if (DragDropUtil.IsCopy(e) || DragDropUtil.IsMove(e)) {
                        if (focus != null && Host.Figure.Root != null) {
                            focus.Selection.Range = Range.Empty;

                            var charIndex = focus.Figure.GetCharIndexAt(loc);
                            focus.Referer.CaretIndex = charIndex;
                            var text = (string) data.GetData(DataFormats.UnicodeText);
                            focus.InsertText(text, false);
                        }
                        Host.HideFeedback(new DropTextRequest(Rectangle.Empty));
                        Host.Site.EditorCanvas.Select();
                    }
                }
            };

            return ret;
        }

        protected IDragSource CreateFocusFigureDragSource() {
            var ret = MouseOperatableFactory.CreateDragSource(DragDropEffects.Copy | DragDropEffects.Move);

            ret.JudgeDragStart += (sender, e) => {
                e.DoIt = false;
                if (e.Button == MouseButtons.Left && e.Clicks == 1) {
                    var loc = new Point(e.X, e.Y);
                    var focus = Host.Focus as StyledTextFocus;
                    if (focus.Figure.IsInSelection(loc)) {
                        _selectionRangeOnDragStart = focus.Selection.Range;
                        e.DoIt = true;
                    }
                }
            };

            //ret.DragStart += delegate {
            //    var focus = Host.Focus as StyledTextFocus;
            //    _selectionRangeOnDragStart = focus.Selection.Range;
            //};
            ret.DragSetData += (sender, e) => {
                var focus = Host.Focus as StyledTextFocus;
                var flows = focus.Referer.Target.CopyBlocksAndInlines(focus.Selection.Range);
                var data = new DataObject();
                //data.SetData(StyledTextConsts.BlocksAndInlinesFormat.Name, flows);
                data.SetData(StyledTextConsts.BlocksAndInlinesFormat.Name, false, flows);

                /// plain text
                {
                    var writer = new PlainTextWriter();
                    var text = writer.ToPlainText(flows);
                    data.SetText(text, TextDataFormat.UnicodeText);
                }

                var app = MemopadApplication.Instance;

                /// html
                {
                    var writer = new HtmlWriter();
                    var html = writer.ToHtml(flows);
                    data.SetData(DataFormats.Html, ClipboardUtil.GetCFHtmlMemoryStream(html));
                }

                e.DataObject = new DataObject(data);
            };
            ret.DragFinish += (sender, e) => {
                if (Host.IsFocused) {
                    /// このEditorにdropされた場合，
                    /// Moveの処理もDragTargetのDragDropで処理されるので何もしない

                } else {
                    /// 他のEditorにdropされた
                    if (e.Effects == DragDropEffects.Move) {
                        var memoStyledText = (MemoStyledText) Host.Model;
                        var node = (INode) Host.Figure;

                        BeginUpdate();
                        using (node.BeginUpdateStyledText()) {
                            var cmd = new RemoveCommand(
                                memoStyledText.StyledText,
                                _selectionRangeOnDragStart.Offset,
                                _selectionRangeOnDragStart.Length
                            );
                            Host.Site.CommandExecutor.Execute(cmd);

                        }
                        EndUpdate();

                        /// コメント線の処理
                        DisconnectRemovedAnchor(node, node);

                        /// StyledText.IsEmptyになったら削除
                        var model = Host.Model as MemoStyledText;
                        if (model != null && model.StyledText.IsEmpty) {
                            Host.RequestRemove();
                        }
                    }
                }
                _selectionRangeOnDragStart = Range.Empty;
            };

            return ret;
        }
        // ------------------------------
        // private
        // ------------------------------
        private void ShowHostFigureDropTextFeedback(Point loc) {
            var node = Host.Figure as INode;
            if (node != null && Host.Figure.Root != null) {
                var charIndex = node.GetCharIndexAt(loc);
                var charRect = node.GetCharRect(charIndex);
                var feedbackRect = new Rectangle(charRect.Location, new Size(2, charRect.Height));
                Host.ShowFeedback(new DropTextRequest(feedbackRect));
            }
        }

        private void ShowFocusFigureDropTextFeedback(Point loc) {
            var focus = Host.Focus as StyledTextFocus;
            if (focus != null && Host.Figure.Root != null) {
                var charIndex = focus.Figure.GetCharIndexAt(loc);
                var charRect = focus.Figure.GetCharRect(charIndex);
                var feedbackRect = new Rectangle(charRect.Location, new Size(2, charRect.Height));
                Host.ShowFeedback(new DropTextRequest(feedbackRect));
            }
        }

    }
}
