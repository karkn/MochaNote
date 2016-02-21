/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using System.Windows.Forms;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Commands;
using Mkamo.Common.Command;
using System.Drawing;
using Mkamo.Editor.Utils;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Editor.Controllers;
using Mkamo.Figure.Layouts;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Editor.Internal.Core {
    internal class DefaultContextMenuProvider: AbstractContextMenuProvider {
        // ========================================
        // static field
        // ========================================
        private static readonly Size CloneMoveDelta = new Size(20, 20);

        // ========================================
        // field
        // ========================================
        private EditorCanvas _owner;

        private MouseEventArgs _currentEvent;

        private ToolStripMenuItem _remove;
        private ToolStripMenuItem _clone;

        private ToolStripMenuItem _cut;
        private ToolStripMenuItem _copy;
        private ToolStripMenuItem _copyAsImage;
        private ToolStripMenuItem _paste;
        private ToolStripMenuItem _pasteInBlock;

        private ToolStripMenuItem _front;
        private ToolStripMenuItem _frontMost;
        private ToolStripMenuItem _back;
        private ToolStripMenuItem _backMost;

        private ToolStripMenuItem _arrangeHLeft;
        private ToolStripMenuItem _arrangeHCenter;
        private ToolStripMenuItem _arrangeHRight;

        private ToolStripMenuItem _arrangeVTop;
        private ToolStripMenuItem _arrangeVMiddle;
        private ToolStripMenuItem _arrangeVBottom;

        private ToolStripMenuItem _detailForm;
        private Size _detailFormSize;

        // ========================================
        // constructor
        // ========================================
        internal DefaultContextMenuProvider(EditorCanvas owner) {
            _owner = owner;
            _detailFormSize = EditorConsts.DefaultDetailFormSize;
        }

        // ========================================
        // property
        // ========================================
        public EditorCanvas Owner {
            get { return _owner; }
            set { _owner = value; }
        }

        public Size DetailFormSize {
            get { return _detailFormSize; }
            set { _detailFormSize = value; }
        }

        // ========================================
        // method
        // ========================================
        public override void Dispose() {
            if (_remove != null) {
                _remove.Dispose();
            }
            if (_clone != null) {
                _clone.Dispose();
            }

            if (_cut != null) {
                _cut.Dispose();
            }
            if (_copy != null) {
                _copy.Dispose();
            }
            if (_copyAsImage != null) {
                _copyAsImage.Dispose();
            }
            if (_paste != null) {
                _paste.Dispose();
            }
            if (_pasteInBlock != null) {
                _pasteInBlock.Dispose();
            }

            if (_front != null) {
                _front.Dispose();
            }
            if (_frontMost != null) {
                _frontMost.Dispose();
            }
            if (_back != null) {
                _back.Dispose();
            }
            if (_backMost != null) {
                _backMost.Dispose();
            }

            if (_detailForm != null) {
                _detailForm.Dispose();
            }

            if (_arrangeHLeft != null) {
                _arrangeHLeft.Dispose();
            }
            if (_arrangeHCenter != null) {
                _arrangeHCenter.Dispose();
            }
            if (_arrangeHRight != null) {
                _arrangeHRight.Dispose();
            }

            if (_arrangeVTop != null) {
                _arrangeVTop.Dispose();
            }
            if (_arrangeVMiddle != null) {
                _arrangeVMiddle.Dispose();
            }
            if (_arrangeVBottom != null) {
                _arrangeVBottom.Dispose();
            }


            base.Dispose();
        }

        public override ContextMenuStrip GetContextMenu(MouseEventArgs e) {
            if (_remove == null) {
                InitItems();
            }

            _currentEvent = e;
            _ContextMenu.Items.Clear();

            var selectedCount = _owner.SelectionManager.SelectedEditors.Count();

            var target = _owner.RootEditor.FindEnabledEditor(e.Location);
            if (target == null) {
                return _ContextMenu;
            }

            var sepaIndex = 0;
            var itemCount = 0;

            /// 削除，複製
            if (UpdateItem(_remove, target, () => new RemoveRequest(), false)) {
                ++itemCount;
            }
            if (UpdateItem(_clone, target.Parent, () => new CloneRequest(_owner.SelectionManager.SelectedEditors) { MoveDelta = CloneMoveDelta }, false)) {
                ++itemCount;
            }

            /// separate
            sepaIndex = itemCount;

            /// クリップボード
            if (UpdateItem(_cut, target, GetCutCommand(), true)) {
                ++itemCount;
            }
            {
                var copyReq = new CopyRequest(_owner.SelectionManager.SelectedEditors);
                var copyBundle = new EditorBundle(copyReq.TargetEditors);
                var canCopy = copyBundle.CanUnderstandAll(copyReq);
                _copy.Enabled = canCopy;
                _ContextMenu.Items.Add(_copy);
                ++itemCount;
            }
            {
                _copyAsImage.Enabled = true;
                _ContextMenu.Items.Add(_copyAsImage);
                ++itemCount;
            }
            if (UpdateItem(_paste, target, () => new PasteRequest(), true)) {
                ++itemCount;
            }
            if (Clipboard.ContainsText()) {
                if (UpdateItem(_pasteInBlock, target, () => new PasteRequest(), false)) {
                    ++itemCount;
                }
            }

            /// separate
            if (itemCount > sepaIndex && sepaIndex > 0) {
                 _ContextMenu.Items.Insert(sepaIndex, new ToolStripSeparator());
                ++itemCount;
                sepaIndex = itemCount;
            }

            /// 前面，背面移動
            if (selectedCount == 1 && !(target.Parent.Figure.Layout is ListLayout)) {
                /// Layoutが有効な場合は前面，背面移動の意味が違うことがあるので表示しない
                if (UpdateItem(_front, target, () => new ReorderRequest() { Kind = ReorderKind.Front }, false)) {
                    ++itemCount;
                }
                if (UpdateItem(_frontMost, target, () => new ReorderRequest() { Kind = ReorderKind.FrontMost },     false)) {
                    ++itemCount;
                }
                if (UpdateItem(_back, target, () => new ReorderRequest() { Kind = ReorderKind.Back}, false)) {
                    ++itemCount;
                }
                if (UpdateItem(_backMost, target, () => new ReorderRequest() { Kind = ReorderKind.BackMost }, false))     {
                    ++itemCount;
                }
    
                /// separate
                if (itemCount > sepaIndex && sepaIndex > 0) {
                     _ContextMenu.Items.Insert(sepaIndex, new ToolStripSeparator());
                    ++itemCount;
                    sepaIndex = itemCount;
                }
            }


            var canMove = target.CanUnderstand(new ChangeBoundsRequest());
            if (selectedCount > 1 && canMove) {
                /// 左揃え
                _ContextMenu.Items.Add(_arrangeHLeft);
                ++itemCount;
 
                /// 左右中央揃え
                _ContextMenu.Items.Add(_arrangeHCenter);
                ++itemCount;

                /// 右揃え
                _ContextMenu.Items.Add(_arrangeHRight);
                ++itemCount;

                /// separate
                _ContextMenu.Items.Insert(sepaIndex, new ToolStripSeparator());
                ++itemCount;
                sepaIndex = itemCount;

                /// 上揃え
                _ContextMenu.Items.Add(_arrangeVTop);
                ++itemCount;

                /// 上下中央揃え
                _ContextMenu.Items.Add(_arrangeVMiddle);
                ++itemCount;

                /// 下揃え
                _ContextMenu.Items.Add(_arrangeVBottom);
                ++itemCount;

                /// separate
                _ContextMenu.Items.Insert(sepaIndex, new ToolStripSeparator());
                ++itemCount;
                sepaIndex = itemCount;
            }

            /// 設定
            if (selectedCount == 1 && target.Controller.UIProvider != null && target.Controller.UIProvider.SupportDetailForm) {
                var enabled = target.Controller.UIProvider.SupportDetailForm;
                _detailForm.Enabled = enabled;
                _ContextMenu.Items.Add(_detailForm);
                ++itemCount;
            }

            /// separate
            if (itemCount > sepaIndex && sepaIndex > 0) {
                 _ContextMenu.Items.Insert(sepaIndex, new ToolStripSeparator());
                ++itemCount;
                sepaIndex = itemCount;
            }

            return _ContextMenu;
        }


        // ------------------------------
        // private
        // ------------------------------
        private bool UpdateItem(
            ToolStripMenuItem item, IEditor target, Func<IRequest> requestProvider, bool forceAdding
        ) {
            /// CanUnderstandなら表示，さらにCanExecuteならEnabled
            var req = requestProvider();
            if (forceAdding || target.CanUnderstand(req)) {
                var cmd = target.GetCommand(req);
                item.Enabled = cmd != null && cmd.CanExecute;
                _ContextMenu.Items.Add(item);
                return true;
            }
            return false;
        }

        private bool UpdateItem(ToolStripMenuItem item, IEditor target, ICommand command, bool forceAdding) {
            /// command != nullなら表示，さらにCanExecuteならEnabled
            if (forceAdding || command != null) {
                item.Enabled = command != null && command.CanExecute;
                _ContextMenu.Items.Add(item);
                return true;
            }
            return false;
        }

        private ICommand GetCutCommand() {
            var targets = _owner.SelectionManager.SelectedEditors;
            var req = new CopyRequest(targets);
            var list = new EditorBundle(targets);
            var copy = list.GetGroupCommand(req);
            if (copy == null) {
                return null;
            }
            var remove = list.GetCompositeCommand(new RemoveRequest());
            if (remove == null) {
                return null;
            }
            return copy.Chain(remove);
        }

        private void InitItems() {
            _remove = new ToolStripMenuItem();
            _remove.Text = "削除(&R)";
            _remove.Click += (sender, e) => {
                using (_owner.CommandExecutor.BeginChain()) {
                    var req = new RemoveRequest();
                    var cmds = new CompositeCommand();

                    var parent = default(IEditor);
                    foreach (var editor in _owner.SelectionManager.SelectedEditors) {
                        if (parent == null) {
                            parent = editor.Parent;
                        }
                        if (editor.CanUnderstand(req)) {
                            cmds.Children.Add(editor.GetCommand(req));
                        }
                    }
                    _owner.CommandExecutor.Execute(cmds);
                    if (parent != null) {
                        parent.RequestSelect(SelectKind.True, true);
                    }
                }
            };
 
            _clone = new ToolStripMenuItem();
            _clone.Text = "複製(&C)";
            _clone.Click += (sender, e) => {
                var cloning = new List<IEditor>(_owner.SelectionManager.SelectedEditors);
                var req = new CloneRequest(cloning);
                req.MoveDelta = CloneMoveDelta;

                if (cloning.Any()) {
                    var target = cloning.First().Parent;
                    var cmd = target.PerformRequest(req) as CloneCommand;
                    if (cmd != null && cmd.ClonedEditors != null) {
                        _owner.SelectionManager.DeselectAll();
                        foreach (var cloned in cmd.ClonedEditors) {
                            cloned.RequestSelect(SelectKind.True, false);
                        }
                    }
                }
            };

            _cut = new ToolStripMenuItem();
            _cut.Text = "切り取り(&X)";
            _cut.Click += (sender, e) => {
                var cmd = GetCutCommand();
                _owner.CommandExecutor.Execute(cmd);
            };

            _copy = new ToolStripMenuItem();
            _copy.Text = "コピー(&C)";
            _copy.Click += (sender, e) => {
                var targets = _owner.SelectionManager.SelectedEditors;
                var req = new CopyRequest(targets);
                var list = new EditorBundle(targets);
                list.PerformGroupRequest(req, _owner.CommandExecutor);
            };

            _copyAsImage = new ToolStripMenuItem();
            _copyAsImage.Text = "画像としてコピー(&I)";
            _copyAsImage.Click += (sender, e) => {
                _owner.CopySelectedAsImage();
            };

            _paste = new ToolStripMenuItem();
            _paste.Text = "貼り付け(&P)";
            _paste.Click += (sender, e) => {
                var found = _owner.RootEditor.FindEnabledEditor(
                    _currentEvent.Location
                );
                if (found != null) {
                    var cmd = found.RequestPaste(_currentEvent.Location, null) as PasteCommand;
                    if (cmd != null && cmd.PastedEditors != null) {
                        _owner.SelectionManager.DeselectAll();
                        foreach (var editor in cmd.PastedEditors) {
                            editor.RequestSelect(SelectKind.True, false);
                        }
                    }
                }

            };

            _pasteInBlock = new ToolStripMenuItem();
            _pasteInBlock.Text = "段落内に貼り付け(&P)";
            _pasteInBlock.Click += (sender, e) => {
                var found = _owner.RootEditor.FindEnabledEditor(
                    _currentEvent.Location
                );
                if (found != null) {
                    var cmd = found.RequestPaste(
                        _currentEvent.Location, EditorConsts.InBlockPasteDescription
                    ) as PasteCommand;
                    if (cmd != null && cmd.PastedEditors != null) {
                        _owner.SelectionManager.DeselectAll();
                        foreach (var editor in cmd.PastedEditors) {
                            editor.RequestSelect(SelectKind.True, false);
                        }
                    }
                }

            };


            _front = new ToolStripMenuItem();
            _front.Text = "前面に移動(&F)";
            _front.Click += (sender, e) => {
                var found = _owner.RootEditor.FindEnabledEditor(
                    _currentEvent.Location
                );
                if (found != null) {
                    found.RequestReorder(ReorderKind.Front);
                }
            };

            _frontMost = new ToolStripMenuItem();
            _frontMost.Text = "最前面に移動(&F)";
            _frontMost.Click += (sender, e) => {
                var found = _owner.RootEditor.FindEnabledEditor(
                    _currentEvent.Location
                );
                if (found != null) {
                    found.RequestReorder(ReorderKind.FrontMost);
                }
            };

            _back = new ToolStripMenuItem();
            _back.Text = "背面に移動(&B)";
            _back.Click += (sender, e) => {
                var found = _owner.RootEditor.FindEnabledEditor(
                    _currentEvent.Location
                );
                if (found != null) {
                    found.RequestReorder(ReorderKind.Back);
                }
            };

            _backMost = new ToolStripMenuItem();
            _backMost.Text = "最背面に移動(&B)";
            _backMost.Click += (sender, e) => {
                var found = _owner.RootEditor.FindEnabledEditor(
                    _currentEvent.Location
                );
                if (found != null) {
                    found.RequestReorder(ReorderKind.BackMost);
                }
            };

            _arrangeHLeft = new ToolStripMenuItem("左揃え(&L)");
            _arrangeHLeft.Click += (sender, e) => {
                var editors = _owner.SelectionManager.SelectedEditors;
                var rect = editors.Select(ed => ed.Figure.Bounds).Aggregate((r1, r2) => Rectangle.Union(r1, r2));
                using (_owner.CommandExecutor.BeginChain()) {
                    foreach (var edi in editors) {
                        var left = rect.Left;
                        edi.RequestMove(new Size(left - edi.Figure.Left, 0));
                    }
                }
            };

            _arrangeHCenter = new ToolStripMenuItem("左右中央揃え(&C)");
            _arrangeHCenter.Click += (sender, e) => {
                var editors = _owner.SelectionManager.SelectedEditors;
                var rect = editors.Select(ed => ed.Figure.Bounds).Aggregate((r1, r2) => Rectangle.Union(r1, r2));
                var center = RectUtil.GetCenter(rect);
                using (_owner.CommandExecutor.BeginChain()) {
                    foreach (var edi in editors) {
                        var left = center.X - edi.Figure.Width / 2;
                        edi.RequestMove(new Size(left - edi.Figure.Left , 0));
                    }
                }
            };

            _arrangeHRight = new ToolStripMenuItem("右揃え(&R)");
            _arrangeHRight.Click += (sender, e) => {
                var editors = _owner.SelectionManager.SelectedEditors;
                var rect = editors.Select(ed => ed.Figure.Bounds).Aggregate((r1, r2) => Rectangle.Union(r1, r2));
                using (_owner.CommandExecutor.BeginChain()) {
                    foreach (var edi in editors) {
                        var left = rect.Right - edi.Figure.Width;
                        edi.RequestMove(new Size(left - edi.Figure.Left, 0));
                    }
                }
            };

            _arrangeVTop = new ToolStripMenuItem("上揃え(&T)");
            _arrangeVTop.Click += (sender, e) => {
                var editors = _owner.SelectionManager.SelectedEditors;
                var rect = editors.Select(ed => ed.Figure.Bounds).Aggregate((r1, r2) => Rectangle.Union(r1, r2));
                using (_owner.CommandExecutor.BeginChain()) {
                    foreach (var edi in editors) {
                        var top = rect.Top;
                        edi.RequestMove(new Size(0, top - edi.Figure.Top));
                    }
                }
            };

            _arrangeVMiddle = new ToolStripMenuItem("上下中央揃え(&M)");
            _arrangeVMiddle.Click += (sender, e) => {
                var editors = _owner.SelectionManager.SelectedEditors;
                var rect = editors.Select(ed => ed.Figure.Bounds).Aggregate((r1, r2) => Rectangle.Union(r1, r2));
                var center = RectUtil.GetCenter(rect);
                using (_owner.CommandExecutor.BeginChain()) {
                    foreach (var edi in editors) {
                        var top = center.Y - edi.Figure.Height / 2;
                        edi.RequestMove(new Size(0, top - edi.Figure.Top));
                    }
                }
            };

            _arrangeVBottom = new ToolStripMenuItem("下揃え(&B)");
            _arrangeVBottom.Click += (sender, e) => {
                var editors = _owner.SelectionManager.SelectedEditors;
                var rect = editors.Select(ed => ed.Figure.Bounds).Aggregate((r1, r2) => Rectangle.Union(r1, r2));
                using (_owner.CommandExecutor.BeginChain()) {
                    foreach (var edi in editors) {
                        var top = rect.Bottom - edi.Figure.Height;
                        edi.RequestMove(new Size(0, top - edi.Figure.Top));
                    }
                }
            };


            _detailForm = new ToolStripMenuItem();
            _detailForm.Text = "設定(&S)";
            _detailForm.Click += (sender, e) => {
                var found = _owner.RootEditor.FindEnabledEditor(
                    _currentEvent.Location
                );
                if (found != null) {
                    if (found.Controller != null && found.Controller.UIProvider != null) {
                        var ui = found.Controller.UIProvider;
                        var form = new DetailSettingsForm();
                        try {
                            form.Size = _detailFormSize;
                            form.Theme = _owner.Theme;
                            ui.ConfigureDetailForm(form);
                            if (form.ShowDialog() == DialogResult.OK) {
                                var cmd = form.GetUpdateCommand();
                                if (cmd != null) {
                                    _owner.CommandExecutor.Execute(cmd);
                                }
                            }
                        } finally {
                            form.Dispose();
                        }
                    }
                }
            };

        }
    }
}
