/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Focuses;
using System.Windows.Forms;
using Mkamo.Memopad.Internal.Forms;
using Mkamo.StyledText.Core;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Memopad.Core;
using System.Diagnostics;
using Mkamo.StyledText.Commands;
using Mkamo.Editor.Core;
using Mkamo.Common.Win32.Gdi32;
using System.Drawing;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Common.DataType;
using Mkamo.Memopad.Internal.Commands;
using Mkamo.Memopad.Internal.Requests;
using Mkamo.Model.Core;
using Mkamo.Model.Memo;
using Mkamo.Editor.Commands;

namespace Mkamo.Memopad.Internal.Focuses {
    internal class MemoStyledTextFocusContextMenuProvider: StyledTextFocusContextMenuProvider {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private MemopadApplication _app;

        private StyledTextFocus _focus;
        private IEditor _editor;
        private bool _canSplit;

        private ToolStripMenuItem _splitThisParagraph;
        private ToolStripMenuItem _splitParagraphs;

        private ToolStripMenuItem _cutInNewMemo;

        private ToolStripMenuItem _openLink;
        private ToolStripMenuItem _setLink;
        private ToolStripMenuItem _removeLink;

        private ToolStripMenuItem _addComment;

        // ========================================
        // constructor
        // ========================================
        public MemoStyledTextFocusContextMenuProvider(StyledTextFocus focus, IEditor editor, bool canSplit): base(focus) {
            _app = MemopadApplication.Instance;
            _focus = focus;
            _editor = editor;
            _canSplit = canSplit;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override ContextMenuStrip GetContextMenu(MouseEventArgs e) {
            var ret = base.GetContextMenu(e);

            if (_splitThisParagraph == null) {
                InitMenuItems();
            }

            var isSelectionEmpty = Owner.Selection.IsEmpty;
            if (!isSelectionEmpty) {
                ret.Items.Add(_Separator1);
                ret.Items.Add(_cutInNewMemo);
            }

            if (_canSplit) {
                ret.Items.Add(_Separator2);

                ret.Items.Add(_splitThisParagraph);
                {
                    var lineIndex = Owner.Referer.Target.GetLineIndex(Owner.Referer.CaretIndex);
                    _splitThisParagraph.Enabled = lineIndex > 0;
                }

                ret.Items.Add(_splitParagraphs);
            }


            var inline = Owner.GetInlineAtCaretIndex();
            var run = inline as Run;
            var isLinkedRun = run != null && run.Link != null;

            if (!Owner.Selection.IsEmpty || isLinkedRun) {
                ret.Items.Add(_Separator3);
            }

            if (isLinkedRun) {
                ret.Items.Add(_openLink);
            }

            if (isLinkedRun) {
                ret.Items.Add(_removeLink);
            }

            if (!Owner.Selection.IsEmpty || isLinkedRun) {
                ret.Items.Add(_setLink);
            }

            if (Owner.Host.Model is MemoText && Owner.Selection.IsEmpty) {
                ret.Items.Add(_Separator4);
                ret.Items.Add(_addComment);
            }

            return ret;
        }

        private void InitMenuItems() {
            _cutInNewMemo = new ToolStripMenuItem("切り出す(&C)");
            _cutInNewMemo.Click += (sender, e) => {
                using (var form = new CreateMemoForm()) {
                    form.Font = _app.Theme.CaptionFont;
                    form.MemoTitle = "新しいノート";
                    if (form.ShowDialog() == DialogResult.OK) {
                        var copied = Owner.Referer.Target.CopyBlocksAndInlines(Owner.Referer.Selection.Range);
                        
                        _app.ShowMainForm();
                        _app.ActivateMainForm();
                        var info = _app.CreateMemo(form.MemoTitle);

                        if (form.OriginalModification == CreateMemoForm.OriginalModificationKind.Remove) {
                            Owner.RemoveForward();
                        } else if (form.OriginalModification == CreateMemoForm.OriginalModificationKind.ReplaceWithLink) {
                            Owner.RemoveForward();
                            Owner.InsertLink(info.Title, new Link(UriUtil.GetUri(info)));
                            if (copied.Last() is BlockBreak) {
                                Owner.InsertBlockBreak();
                            }
                        }

                        var pageContent = _app.MainForm.FindPageContent(info);
                        var canvas = pageContent.EditorCanvas;
                        var caret = canvas.Caret;
                        var loc = CaretUtil.GetExpectedMemoTextPosition(caret.Position);
                        MemoEditorHelper.AddBlocksAndInlines(canvas.RootEditor.Children.First(), loc, copied, false);
                    }
                }
            };


            _splitThisParagraph = new ToolStripMenuItem("この行で分割(&S)");
            _splitThisParagraph.Click += (sender, e) => {
                using (_editor.Site.CommandExecutor.BeginChain()) {

                    var referer = Owner.Referer;
                    var stext = referer.Target;

                    var lineIndex = stext.GetLineIndex(referer.CaretIndex);
                    var lineStart = stext.GetLineStartCharIndex(lineIndex);
                    var flows = stext.CopyBlocksAndInlines(lineStart);

                    var rect = Rectangle.Empty;
                    rect = _focus.Figure.GetCharRect(lineStart);

                    Owner.Selection.Range = Range.FromStartAndEnd(lineStart - 1, stext.Length - 2);
                    Owner.RemoveForward();
                    _editor.RequestFocus(FocusKind.Commit, null);

                    var loc = new Point(_editor.Figure.Left, rect.Bottom + 16);
                    var memoEditor = _editor.Parent;
                    var created = MemoEditorHelper.AddBlocksAndInlines(memoEditor, loc, flows, true);
                    created.RequestFocusCommit(true);
                }
            };

            _splitParagraphs = new ToolStripMenuItem("段落ごとに分割(&P)");
            _splitParagraphs.Click += (sender, e) => {
                using (_editor.Figure.Root.DirtManager.BeginDirty())
                using (_editor.Site.CommandExecutor.BeginChain()) {
                    var referer = Owner.Referer;
                    var stext = referer.Target;
                    var memoEditor = _editor.Parent;

                    var created = default(IEditor);
                    var createds = new List<IEditor>();
                    var cTop = _editor.Figure.Top;
                    foreach (var block in stext.Blocks) {
                        var flows = new Flow[] { block.CloneDeeply() as Block, };
                        var loc = new Point(_editor.Figure.Left, cTop);
                        created = MemoEditorHelper.AddBlocksAndInlines(memoEditor, loc, flows, true);
                        createds.Add(created);
                        cTop += created.Figure.Height + 16;
                    }
                    created.RequestFocusCommit(true);

                    _editor.RequestRemove();

                    var cmd = new SelectMultiCommand(createds, SelectKind.True, true);
                    cmd.Execute();
                }
            };


            _openLink = new ToolStripMenuItem("リンクを開く(&O)");
            _openLink.Click += (sender, e) => {
                var run = Owner.GetInlineAtCaretIndex() as Run;
                if (run != null && run.HasLink) {
                    LinkUtil.GoLink(run.Link);
                }
            };

            _removeLink = new ToolStripMenuItem("リンクを削除(&R)");
            _removeLink.Click += (sender, e) => {
                Owner.UnsetLink();
            };

            _setLink = new ToolStripMenuItem("リンクを設定(&L)");
            _setLink.Click += (sender, e) => {

                if (Owner.Selection.IsEmpty) {
                    var run = Owner.GetInlineAtCaretIndex() as Run;
                    if (run != null) {
                        var oldUri = default(string);
                        if (run != null && run.HasLink) {
                            oldUri = run.Link.Uri;
                        }

                        var oldText = run.Text;
                        var dialog = new LinkSelectForm();
                        if (dialog.ShowDialog(_app.MainForm, oldUri, run.Text) == DialogResult.OK) {
                            var uri = dialog.Uri;
                            var newText = dialog.TitleText;
                            if (newText != oldText) {
                                Owner.SetRunText(run, newText);
                            }
                            if (uri != oldUri) {
                                Owner.SetLink(uri, null);
                            }
                        }
                    }

                } else {
                    var range = Owner.Selection.Range;
                    var oldText = Owner.Referer.Target.Text.Substring(range.Offset, range.Length);

                    var dialog = new LinkSelectForm();
                    dialog.TitleTextTextBoxEnabled = false;
                    if (dialog.ShowDialog(_app.MainForm, null, oldText) == DialogResult.OK) {
                        Owner.SetLink(dialog.Uri, null);
                    }
                }
            };


            _addComment = new ToolStripMenuItem("コメントを追加(&C)");
            _addComment.Click += (sender, e) => {
                MemoEditorHelper.AddCommentForMemoText(Owner);
            };
        }

    }
}
