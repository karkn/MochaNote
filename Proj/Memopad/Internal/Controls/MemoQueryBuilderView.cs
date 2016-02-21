/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Common.Forms.Themes;
using Mkamo.Memopad.Internal.Core;
using ComponentFactory.Krypton.Toolkit;
using Mkamo.Model.Memo;
using Mkamo.Model.Core;
using Mkamo.Memopad.Internal.Forms;
using Mkamo.Memopad.Core;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal partial class MemoQueryBuilderView: UserControl {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;

        private KryptonTextBox _searchTextBox;

        // ========================================
        // constructor
        // ========================================
        public MemoQueryBuilderView() {
            _facade = MemopadApplication.Instance;

            InitializeComponent();

            var theme = _facade.Theme;
            _titleLabel.Font = theme.CaptionFont;
            _tagLabel.Font = theme.CaptionFont;
            _markLabel.Font = theme.CaptionFont;
            _importanceLabel.Font = theme.CaptionFont;
            _timeSpanLabel.Font = theme.CaptionFont;
            _flowLayoutPanel.BackColor = theme.DarkBackColor;

            _tagTextBox.ValueChanged += (s, e) => OnQueryUpdated();
            _markTextBox.ValueChanged += (s, e) => OnQueryUpdated();
            _importanceTextBox.ValueChanged += (s, e) => OnQueryUpdated();
            _timeSpanTextBox.ValueChanged += (s, e) => OnQueryUpdated();
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            //if (!DesignMode) {
            //    _saveAsSmartFolderButton.Visible = false;
            //    _saveAsSmartFilterButton.Visible = false;
            //}
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler QueryUpdated;


        // ========================================
        // property
        // ========================================
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public KryptonTextBox SearchTextBox {
            get { return _searchTextBox; }
            set { _searchTextBox = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<MemoTag> CheckedTags {
            get { return _tagTextBox.CheckedTags; }
            set { _tagTextBox.CheckedTags = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsUntaggedChecked {
            get { return _tagTextBox.IsUntaggedChecked; }
            set { _tagTextBox.IsUntaggedChecked = value; }
        }

        // ========================================
        // method
        // ========================================
        public void ClearConditions() {
            _searchTextBox.Text = "";
            _titleTextBox.Text = "";
            _tagTextBox.ClearCondition();
            _markTextBox.ClearCondition();
            _importanceTextBox.ClearCondition();
            _timeSpanTextBox.ClearCondition();
        }

        public void UpdateQuery() {
            OnQueryUpdated();
        }

        public MemoQuery GetQuery() {
            var ret = MemoFactory.CreateTransientQuery();

            ret.Condition = _searchTextBox == null? "": _searchTextBox.Text;
            ret.Title = _titleTextBox.Text;

            if (
                !_tagTextBox.IsUntaggedChecked &&
                (_tagTextBox.CheckedTags == null || !_tagTextBox.CheckedTags.Any())
            ) {
                ret.NarrowByTagIds = false;
            } else {
                ret.NarrowByTagIds = true;
                ret.NoTag = _tagTextBox.IsUntaggedChecked;
                foreach (var tag in _tagTextBox.CheckedTags) {
                    ret.AddTagId(_facade.Container.GetId(tag));
                }
                ret.TagCompoundKind = _tagTextBox.IsAnyChecked ? MemoConditionCompoundKind.Any : MemoConditionCompoundKind.All;
            }

            if (_markTextBox.CheckedMarkKinds == null || !_markTextBox.CheckedMarkKinds.Any()) {
                ret.NarrowByMarkKinds = false;
            } else {
                ret.NarrowByMarkKinds = true;
                foreach (var kind in _markTextBox.CheckedMarkKinds) {
                    ret.AddMarkKind(kind);
                }
                ret.MarkCompoundKind = _markTextBox.IsAnyChecked ? MemoConditionCompoundKind.Any : MemoConditionCompoundKind.All;
            }

            if (_importanceTextBox.CheckedImportanceKinds == null || !_importanceTextBox.CheckedImportanceKinds.Any()) {
                ret.NarrowByImportanceKind = false;
            } else {
                ret.NarrowByImportanceKind = true;
                foreach (var kind in _importanceTextBox.CheckedImportanceKinds) {
                    ret.AddImportanceKind(kind);
                }
            }

            if (!_timeSpanTextBox.IsRecentTimeSpanSelected) {
                ret.NarrowByRecentTimeSpan = false;
            } else {
                ret.NarrowByRecentTimeSpan = true;
                ret.RecentTimeSpan = _timeSpanTextBox.RecentTimeSpan;
            }

            if (!_timeSpanTextBox.IsTimeSpanSelected) {
                ret.NarrowByTimeSpan = false;
            } else {
                ret.NarrowByTimeSpan = true;
                ret.TimeSpan = _timeSpanTextBox.TimeSpan;
            }

            return ret;
        }

        // --- protected ---
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            _titleTextBox.Width =
                _flowLayoutPanel.Width - _flowLayoutPanel.Padding.Size.Width - _titleTextBox.Margin.Size.Width;
        }

        protected virtual void OnQueryUpdated() {
            var handler = QueryUpdated;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }


        // --- handler ---
        private void _titleTextBox_TextChanged(object sender, EventArgs e) {
            OnQueryUpdated();
        }

        private void _saveAsSmartFolderButton_Click(object sender, EventArgs e) {
            using (var dialog = new QueryHolderEditForm()) {
                var smartFolder = MemoFactory.CreateTransientSmartFolder();
                smartFolder.Name = "新しいスマートフォルダ";
                smartFolder.Query = GetQuery();
                dialog.QueryHolder = smartFolder;
                if (dialog.ShowDialog(_facade.MainForm) == DialogResult.OK) {
                    /// これをやっておかないと一度もRootがExpand()されていない場合，
                    /// 2つノードが作成されてしまう。
                    /// 理由は未調査。
                    _facade.MainForm.WorkspaceView.WorkspaceTree.SmartFolderTreePresenter.Root.Expand();

                    _facade.Container.Persist(dialog.QueryHolder.Query);
                    _facade.Container.Persist(dialog.QueryHolder);

                    _facade.MainForm.ShowWorkspaceView();
                    ClearConditions();
                    UpdateQuery();
                    _facade.MainForm.WorkspaceView.WorkspaceTree.SmartFolderTreePresenter.SelectNode(dialog.QueryHolder);
                    _facade.MainForm.WorkspaceView.WorkspaceTree.Select();
                }
            }
        }

        private void _saveAsSmartFilterButton_Click(object sender, EventArgs e) {
            using (var dialog = new QueryHolderEditForm()) {
                var smartFilter = MemoFactory.CreateTransientSmartFilter();
                smartFilter.Name = "新しいスマートフィルタ";
                smartFilter.Query = GetQuery();
                dialog.QueryHolder = smartFilter;
                if (dialog.ShowDialog(_facade.MainForm) == DialogResult.OK) {
                    _facade.Container.Persist(dialog.QueryHolder.Query);
                    _facade.Container.Persist(dialog.QueryHolder);

                    ClearConditions();
                    UpdateQuery();
                }
            }
        }

        private void _returnButton_Click(object sender, EventArgs e) {
            _facade.MainForm.CancelSearch();
            _facade.MainForm.WorkspaceView.WorkspaceTree.Select();
        }

    }
}
