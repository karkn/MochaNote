/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Internal.Controls;
using System.Windows.Forms;
using Mkamo.Memopad.Core;
using Mkamo.Editor.Focuses;
using Mkamo.Editor.Core;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using Mkamo.Common.DataType;
using Mkamo.Editor.Requests;
using Mkamo.Control.TabControlEx;
using System.ComponentModel;
using Mkamo.Model.Memo;
using Mkamo.Model.Core;
using Mkamo.Memopad.Properties;
using Mkamo.Control.TreeNodeEx;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Common.Win32.User32;
using Mkamo.Figure.Core;
using System.Text.RegularExpressions;
using Mkamo.Editor.Tools;
using System.IO;
using System.Reflection;
using Mkamo.Common.Command;
using Mkamo.Memopad.Internal.Core;

namespace Mkamo.Memopad.Internal.Forms {
    partial class MemopadForm {
        // ========================================
        // method
        // ========================================
        // --- event handler ---
        private void HandleTabControlControlAdded(object sender, EventArgs e) {
            if (_tabControl.TabCount == 1 && _EditorCanvas != null) {
                /// ひとつめのtabpageを作成したときはSelectingがおこらない
                _EditorCanvas.AutoScrollMinSizeChanged += HandleEditorCanvasAutoScrollMinSizeChanged;
                UpdateCanvasSizeDisplay();
            }
        }

        private void HandleTabControlSelecting(object sender, TabControlCancelEventArgs e) {
            if (_EditorCanvas != null) {
                _EditorCanvas.AutoScrollMinSizeChanged -= HandleEditorCanvasAutoScrollMinSizeChanged;
            }
        }

        private void HandleTabControlSelected(object sender, TabControlEventArgs e) {
            var page = e.TabPage;

            /// 最後のタブページを閉じられたときはpageにnullが渡される
            if (page != null && IsMemoTabPage(page)) {
                var pageContent = (PageContent) page.Tag;
                _CurrentEditorCanvas = pageContent.EditorCanvas;
                _EditorCanvas.AutoScrollMinSizeChanged += HandleEditorCanvasAutoScrollMinSizeChanged;
                _CurrentEditorCanvas.Select();
            } else {
                _CurrentEditorCanvas = null;
            }
            UpdateToolStrip();
            UpdateCanvasSizeDisplay();
        }

        private void HandleCurrentEditorCanvasToolChanged(object sender, EventArgs e) {
            EnsureFocusCommited(_tabControl.SelectedTab);

            var tool = _currentEditorCanvas.Tool;

            _selectToolToolStripButton.Checked = tool is SelectTool;
            _handToolToolStripButton.Checked = tool is HandTool;
            _adjustSpaceToolToolStripButton.Checked = tool is AdjustSpaceTool;
            
            if (tool is SelectTool) {
                _currentEditorCanvas.Cursor = Cursors.Default;
                if (
                    _currentEditorCanvas.SelectionManager.SelectedEditors.Count() == 1 &&
                    _currentEditorCanvas.SelectionManager.SelectedEditors.First().Model is Memo
                ) {
                    _currentEditorCanvas.Caret.Show();
                }
            } else if (tool is HandTool) {
                using (var stream = new MemoryStream(Resources.cursor_hand)) {
                    _currentEditorCanvas.Cursor = new Cursor(stream);
                }
                _currentEditorCanvas.Caret.Hide();
            } else if (tool is FreehandTool || tool is EraserTool || tool is DragSelectTool) {
                _currentEditorCanvas.Cursor = Cursors.Cross;
                _currentEditorCanvas.Caret.Hide();
            } else if (tool is AdjustSpaceTool) {
                _currentEditorCanvas.Cursor = Cursors.HSplit;
                _currentEditorCanvas.Caret.Hide();
            } else {
                _currentEditorCanvas.Cursor = Cursors.Default;
                _currentEditorCanvas.Caret.Hide();
            }
        }

        private void UpdateCanvasSizeDisplay() {
            if (_EditorCanvas == null) {
                _canvasSizeToolStripDropDownButton.Text = "";
                _canvasSizeToolStripDropDownButton.Enabled = false;
            } else {
                var size = _EditorCanvas.AutoScrollMinSize;
                _canvasSizeToolStripDropDownButton.Text = size.Width + " x " + size.Height;
                _canvasSizeToolStripDropDownButton.Enabled = true;
            }
        }

        private void HandleEditorCanvasAutoScrollMinSizeChanged(object sender, EventArgs e) {
            UpdateCanvasSizeDisplay();
        }

        private void HandleTabControlMouseDown(object sender, MouseEventArgs e) {
            for (int i = 0, len = _tabControl.TabCount; i < len; ++i) {
                var r = _tabControl.GetTabRect(i);
                if (r.Contains(e.Location)) {
                    if (e.Button == MouseButtons.Right) {
                        _tabControl.SelectTab(i);

                        /// tabContextMenuStripを_tabControl.ContextMenuStripに設定してしまうと
                        /// なぜかEditorCanvasを右ボタンでドラッグしたときに
                        /// DragFinish時にtabContextMenuStripが表示されてしまうので自分でShow()する
                        _tabControlContextMenuStrip.Show(_tabControl, e.Location);
                    }
                    return;
                }
            }
        }

        private void HandleTabControlMouseDoubleClick(object sender, MouseEventArgs e) {
            if (!_tabControl.IsInCloseButtonBounds(e.Location)) {
                ToggleEditorSizeMaximized();
            }
        }

        private void HandleTabControlDragStart(object sender, DragStartEventArgs e) {
            var selectedTab = _tabControl.SelectedTab;
            if (selectedTab != null) {
                if (IsMemoTabPage(selectedTab)) {
                    var pageContent = (PageContent) selectedTab.Tag;
                    var memoInfo = pageContent.MemoInfo;
                    e.DragDataObjects = new[] {
                        new[] { memoInfo },
                    };
                    e.AllowedEffect = DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.Link;
                }
            }
        }

        private void HandleTabControlCloseButtonPressed(object sender, CloseButtonPressedEventArgs e) {
            if (e.Closed != null){
                if (IsMemoTabPage(e.Closed)) {
                    var content = e.Closed.Tag as PageContent;
                    _app.CloseMemo(content.MemoInfo);
                } else if (IsStartPageTabPage(e.Closed)) {
                    CloseStartPage();
                }
            }
        }

        private void HandleMemoTitleChanged(object sender, EventArgs e) {
            var content = sender as PageContent;
            var page = _tabControl.SelectedTab;
            page.Text = content.Title;
        }

        private void HandleMemoPanelDoubleClick(object sender, EventArgs e) {
            if (_IsCompact) {
                /// コンパクトウィンドウ時は何もしない
                return;
            }

            if (IsFinderPaneCollapsed && IsMemoListPaneCollapsed) {
                ExpandFinderPane();
                ExpandMemoListPane();
            } else {
                CollapseWorkspacePane();
                CollapseMemoListPane();
            }
        }

        private void HandleAppActiveFolderChanged(object sender, EventArgs e) {
            if (_app.ActiveFolder == null) {
                _activeFolderToolStripDropDownButton.Image = Resources.clear_folder_horizontal;
            } else {
                _activeFolderToolStripDropDownButton.Image = Resources.clear_folder_horizontal_open;
            }
        }

        // --- workspace ---
        private void _finderFoldButtonSpecHeaderGroup_Click(object sender, EventArgs e) {
            if (IsFinderPaneCollapsed) {
                ExpandFinderPane();
            } else {
                CollapseWorkspacePane();
            }
        }

        private void _workspaceTagContextMenuStrip_Opening(object sender, CancelEventArgs e) {
            _showDescendantTagsMemoToolStripMenuItem.Checked =
                _workspaceView.WorkspaceTree.TagTreePresenter.FindMemoWithDescendants;
        }

        private void _showDescendantTagsMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceView.WorkspaceTree.TagTreePresenter.FindMemoWithDescendants =
                !_workspaceView.WorkspaceTree.TagTreePresenter.FindMemoWithDescendants;
            _mediator.UpdateMemoListBox(false);
        }

        // --- memo list view ---
        private void _memoListButtonSpecHeaderGroup_Click(object sender, EventArgs e) {
            if (IsMemoListPaneCollapsed) {
                ExpandMemoListPane();
            } else {
                CollapseMemoListPane();
            }
        }

        private void _memoListViewSmartFilterContextMenuStrip_Opening(object sender, CancelEventArgs e) {
            // --- clear old filter menu items ---
            var removings = new List<ToolStripItem>();
            foreach (ToolStripItem item in _memoListViewSmartFilterContextMenuStrip.Items) {
                if (item.Tag as MemoSmartFilter != null) {
                    removings.Add(item);
                }
            }

            foreach (var removing in removings) {
                removing.Click -= HandleSmartFilterClick;
                _memoListViewSmartFilterContextMenuStrip.Items.Remove(removing);
                removing.Dispose();
            }

            // --- create filter menu items ---
            var filters = _app.Container.FindAll<MemoSmartFilter>();
            foreach (var filter in filters.OrderBy(f => f.Name)) {
                var item = new ToolStripMenuItem(filter.Name);
                item.Tag = filter;
                item.Click += HandleSmartFilterClick;
                item.Checked = filter == _app.ActiveSmartFilter;
                item.Image = Resources.filter;
                _memoListViewSmartFilterContextMenuStrip.Items.Add(item);
            }

            _clearSmartFilterToolStripMenuItem.Checked = _app.ActiveSmartFilter == null;
        }

        //private void _createSmartFilterToolStripMenuItem_Click(object sender, EventArgs e) {
        //    using (var dialog = new QueryHolderEditForm()) {
        //        var filter = MemoFactory.CreateTransientSmartFilter();
        //        filter.Query = MemoFactory.CreateTransientQuery();
        //        filter.Name = "新しいスマートフィルタ";
        //        dialog.QueryHolder = filter;
        //        if (dialog.ShowDialog(_app.MainForm) == DialogResult.OK) {
        //            _app.Container.Persist(dialog.QueryHolder.Query);
        //            _app.Container.Persist(dialog.QueryHolder);

        //            //
        //        }
        //    }

        //}

        private void _memoListViewManageSmartFilterToolStripMenuItem_Click(object sender, EventArgs e) {
            using (var form = new SmartFilterManageForm()) {
                form.Font = _theme.CaptionFont;
                form.ShowDialog(this);
            }
        }

        private void _clearSmartFilterToolStripMenuItem_Click(object sender, EventArgs e) {
            _app.ActiveSmartFilter = null;
        }

        private void HandleSmartFilterClick(object sender, EventArgs e) {
            var item = sender as ToolStripMenuItem;
            if (item == null) {
                return;
            }

            var filter = item.Tag as MemoSmartFilter;
            if (filter == null) {
                return;
            }

            _app.ActiveSmartFilter = filter;
        }

        private void HandleAppActiveSmartFilterChanged(object sender, EventArgs e) {
            var filter = _app.ActiveSmartFilter;
            if (filter == null) {
                _memoListView.MemoListBox.Filter = null;
            } else {
                Func<IEnumerable<MemoInfo>, IEnumerable<MemoInfo>> func = infos => {
                    var finder = new MemoFinder();
                    return finder.Search(infos, filter.Query);
                };
                _memoListView.MemoListBox.Filter = func;
            }
            _mediator.UpdateMemoListBox(true);
            UpdateSmartFilterLabel();

            _memoListViewSmartFilterButtonSpecHeaderGroup.Checked =
                _app.ActiveSmartFilter == null ?
                ComponentFactory.Krypton.Toolkit.ButtonCheckState.Unchecked :
                ComponentFactory.Krypton.Toolkit.ButtonCheckState.Checked;
        }

        private void UpdateSmartFilterLabel() {
            var name = _app.ActiveSmartFilter == null ? "なし" : _app.ActiveSmartFilter.Name;
            _smartFilterToolStripStatusLabel.Text = "スマートフィルタ: " + name;
        }



        private void _memoListViewDisplayItemContextMenuStrip_Opening(object sender, CancelEventArgs e) {
            var items = _memoListView.MemoListBox.DisplayItems;

            _createdDateDisplayToolStripMenuItem.Checked = items.Contains(MemoListBoxDisplayItem.CreatedDate);
            _modifiedDateDisplayToolStripMenuItem.Checked = items.Contains(MemoListBoxDisplayItem.ModifiedDate);
            _accessedDateDisplayToolStripMenuItem.Checked = items.Contains(MemoListBoxDisplayItem.AccessedDate);
            _tagDisplayToolStripMenuItem.Checked = items.Contains(MemoListBoxDisplayItem.Tag);
            _summaryTextDisplayToolStripMenuItem.Checked = items.Contains(MemoListBoxDisplayItem.SummaryText);
        }

        private void ToggleMemoListViewDisplayItem(MemoListBoxDisplayItem item) {
            var items = _memoListView.MemoListBox.DisplayItems;
            var newItems = new List<MemoListBoxDisplayItem>(items);
            if (items.Contains(item)) {
                newItems.Remove(item);
            } else {
                newItems.Add(item);
            }
            newItems.Sort();
            _memoListView.MemoListBox.DisplayItems = newItems.ToArray();
        }

        private void _createdDateDisplayToolStripMenuItem_Click(object sender, EventArgs e) {
            ToggleMemoListViewDisplayItem(MemoListBoxDisplayItem.CreatedDate);
        }

        private void _modifiedDateDisplayToolStripMenuItem_Click(object sender, EventArgs e) {
            ToggleMemoListViewDisplayItem(MemoListBoxDisplayItem.ModifiedDate);
        }

        private void _accessedDateDisplayToolStripMenuItem_Click(object sender, EventArgs e) {
            ToggleMemoListViewDisplayItem(MemoListBoxDisplayItem.AccessedDate);
        }

        private void _tagDisplayToolStripMenuItem_Click(object sender, EventArgs e) {
            ToggleMemoListViewDisplayItem(MemoListBoxDisplayItem.Tag);
        }

        private void _summaryTextDisplayToolStripMenuItem_Click(object sender, EventArgs e) {
            ToggleMemoListViewDisplayItem(MemoListBoxDisplayItem.SummaryText);
        }


        private void _memoListViewSortContextMenuStrip_Opening(object sender, CancelEventArgs e) {
            _sortByTitleToolStripMenuItem.Checked = false;
            _sortByCreatedDateToolStripMenuItem.Checked = false;
            _sortByModifiedDateToolStripMenuItem.Checked = false;
            _sortByAccessedDateToolStripMenuItem.Checked = false;
            _sortByAscendingOrderToolStripMenuItem.Checked = false;
            _sortByDescendingOrderToolStripMenuItem.Checked = false;

            switch (_memoListView.MemoListBox.SortKey) {
                case MemoListBoxDisplayItem.Title: {
                    _sortByTitleToolStripMenuItem.Checked = true;
                    break;
                }
                case MemoListBoxDisplayItem.CreatedDate: {
                    _sortByCreatedDateToolStripMenuItem.Checked = true;
                    break;
                }
                case MemoListBoxDisplayItem.ModifiedDate: {
                    _sortByModifiedDateToolStripMenuItem.Checked = true;
                    break;
                }
                case MemoListBoxDisplayItem.AccessedDate: {
                    _sortByAccessedDateToolStripMenuItem.Checked = true;
                    break;
                }
            }

            if (_memoListView.MemoListBox.SortsAscendingOrder) {
                _sortByAscendingOrderToolStripMenuItem.Checked = true;
            } else {
                _sortByDescendingOrderToolStripMenuItem.Checked = true;
            }
            
            _sortByImortanceToolStripMenuItem.Checked = _MemoListView.MemoListBox.SortsImportanceOrder;
        }

        private void _sortByTitleToolStripMenuItem_Click(object sender, EventArgs e) {
            _memoListView.MemoListBox.SortKey = MemoListBoxDisplayItem.Title;
        }

        private void _sortByCreatedDateToolStripMenuItem_Click(object sender, EventArgs e) {
            _memoListView.MemoListBox.SortKey = MemoListBoxDisplayItem.CreatedDate;
        }

        private void _sortByModifiedDateToolStripMenuItem_Click(object sender, EventArgs e) {
            _memoListView.MemoListBox.SortKey = MemoListBoxDisplayItem.ModifiedDate;
        }

        private void _sortByAccessedDateToolStripMenuItem_Click(object sender, EventArgs e) {
            _memoListView.MemoListBox.SortKey = MemoListBoxDisplayItem.AccessedDate;
        }

        private void _sortByAscendingOrderToolStripMenuItem_Click(object sender, EventArgs e) {
            _memoListView.MemoListBox.SortsAscendingOrder = true;
        }

        private void _sortByDescendingOrderToolStripMenuItem_Click(object sender, EventArgs e) {
            _memoListView.MemoListBox.SortsAscendingOrder = false;
        }

        private void _sortByImortanceToolStripMenuItem_Click(object sender, EventArgs e) {
            _memoListView.MemoListBox.SortsImportanceOrder = !_memoListView.MemoListBox.SortsImportanceOrder;
        }

        // --- tab context menu ---
        private void _tabControlContextMenuStrip_Opening(object sender, CancelEventArgs e) {
            var isEditorMaximized = IsFinderPaneCollapsed && IsMemoListPaneCollapsed;
            _maximizeEditorSizeTabToolStripMenuItem.Visible = !isEditorMaximized;
            _restoreEditorSizeTabToolStripMenuItem.Visible = isEditorMaximized;

            if (_currentEditorCanvas == null) {
                _removeMemoTabToolStripMenuItem.Enabled = false;
                _showFusenTabToolStripMenuItem.Enabled = false;
                return;
            }

            var pageContent = (PageContent) _tabControl.SelectedTab.Tag;
            var memoInfo = pageContent.MemoInfo;
            var removedContained = !_app.RemovedMemoInfos.Contains(memoInfo);
            _removeMemoTabToolStripMenuItem.Enabled = removedContained;
            _showFusenTabToolStripMenuItem.Enabled = removedContained;
        }

        private void _closeMemoTabToolStripMenuItem_Click(object sender, EventArgs e) {
            var selected = _tabControl.SelectedTab;
            if (selected != null) {
                if (IsMemoTabPage(selected)) {
                    var pageContent = (PageContent) _tabControl.SelectedTab.Tag;
                    _app.CloseMemo(pageContent.MemoInfo);
                } else if (IsStartPageTabPage(selected)) {
                    CloseStartPage();
                }
            }
        }

        private void _closeOtherMemoTabsToolStripMenuItem_Click(object sender, EventArgs e) {
            _app.CloseOtherMemos();
        }

        private void _closeAllMemoTabsToolStripMenuItem_Click(object sender, EventArgs e) {
            _app.CloseAllMemos();
        }

        private void _removeMemoTabToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_currentEditorCanvas == null) {
                return;
            }

            var pageContent = (PageContent) _tabControl.SelectedTab.Tag;
            var memoInfo = pageContent.MemoInfo;

            if (!MessageUtil.ConfirmMemoRemoval(new[] { memoInfo } )) {
                return;
            }
            _app.RemoveMemo(memoInfo);
        }

        private void _showFusenTabToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_currentEditorCanvas == null) {
                return;
            }

            var pageContent = (PageContent) _tabControl.SelectedTab.Tag;
            var memoInfo = pageContent.MemoInfo;
            _app.CloseMemo(memoInfo);
            _app.LoadMemoAsFusen(memoInfo, false);
        }

        private void _maximizeEditorSizeTabToolStripMenuItem_Click(object sender, EventArgs e) {
            ToggleEditorSizeMaximized();
        }

        private void _restoreEditorSizeTabToolStripMenuItem_Click(object sender, EventArgs e) {
            ToggleEditorSizeMaximized();
        }


        // --- search text box ---
        private void _searchTextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                e.SuppressKeyPress = true;
                if (e.Control) {
                    /// 検索解除
                    ShowWorkspaceView();
                    _memoQueryBuilderView.Value.ClearConditions();
                    _memoQueryBuilderView.Value.UpdateQuery();

                    /// ハイライト解除
                    _GlobalHighlight = null;

                } else {
                    /// 検索
                    if (!_memoQueryBuilderView.Value.Visible) {
                        if (_workspaceView.WorkspaceTree.IsTagSelected) {
                            _memoQueryBuilderView.Value.CheckedTags = new[] { _workspaceView.WorkspaceTree.SelectedTag };
                        } else if (_workspaceView.WorkspaceTree.IsUntaggedSelected) {
                            _memoQueryBuilderView.Value.IsUntaggedChecked = true;
                        }
                    }
                    ShowMemoQueryBuilderView();
                    _memoQueryBuilderView.Value.UpdateQuery();
                    _conditionTextBox.Focus();

                    /// ハイライト
                    _GlobalHighlight = _conditionTextBox.Text;
                }
            }

        }

        private void _searchButtonSpec_Click(object sender, EventArgs e) {
            if (!_memoQueryBuilderView.Value.Visible) {
                if (_workspaceView.WorkspaceTree.IsTagSelected) {
                    _memoQueryBuilderView.Value.CheckedTags = new[] { _workspaceView.WorkspaceTree.SelectedTag };
                } else if (_workspaceView.WorkspaceTree.IsUntaggedSelected) {
                    _memoQueryBuilderView.Value.IsUntaggedChecked = true;
                }
            }

            ShowMemoQueryBuilderView();
            _memoQueryBuilderView.Value.UpdateQuery();
            _conditionTextBox.Focus();

            /// ハイライト
            _GlobalHighlight = _conditionTextBox.Text;
        }

        private void _cancelSearchButtonSpec_Click(object sender, EventArgs e) {
            CancelSearch();
        }

        // --- memoinfo ---
        private void HandleMemoInfoAdded(object sender, MemoInfoEventArgs e) {
            UpdateMemoCountLabel();
        }

        private void HandleMemoInfoRemoved(object sender, MemoInfoEventArgs e) {
            UpdateMemoCountLabel();
        }

        private void HandleMemoInfoRecovered(object sender, MemoInfoEventArgs e) {
            UpdateMemoCountLabel();
        }

        private void UpdateMemoCountLabel() {
            _memoCountStatusLabel.Text = "ノート数: " + _app.MemoInfoCount;
        }


        // --- ad ---
        private void _adLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            MessageUtil.IntroducePremiumLicense();
        }

    }
}
