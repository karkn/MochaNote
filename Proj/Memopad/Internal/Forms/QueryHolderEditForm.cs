/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Model.Memo;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Memopad.Core;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class QueryHolderEditForm: Form {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;

        private MemoQueryHolder _queryHolder;

        // ========================================
        // constructor
        // ========================================
        public QueryHolderEditForm() {
            _facade = MemopadApplication.Instance;

            InitializeComponent();

            var theme = _facade.Theme;
            Font = theme.CaptionFont;

            _nameTextBox.TextChanged += HandleNameTextBoxTextChanged;
            ValidateInput();
        }

        // ========================================
        // property
        // ========================================
        public MemoQueryHolder QueryHolder {
            get { return _queryHolder; }
            set {
                ClearConditions();
                _queryHolder = value;
                if (_queryHolder != null) {
                    var query = _queryHolder.Query;
                    _nameTextBox.Text = _queryHolder.Name;
                    _searchTextBox.Text = query.Condition;
                    _titleTextBox.Text = query.Title;
                    _tagTextBox.IsUntaggedChecked = query.NoTag;
                    _tagTextBox.CheckedTags = MemoTagUtil.GetTags(query.TagIds);
                    _tagTextBox.IsAnyChecked = query.TagCompoundKind == MemoConditionCompoundKind.Any;
                    _markTextBox.CheckedMarkKinds = query.MarkKinds;
                    _markTextBox.IsAnyChecked = query.MarkCompoundKind == MemoConditionCompoundKind.Any;
                    _importanceTextBox.CheckedImportanceKinds = query.ImportanceKinds;
                    if (query.NarrowByRecentTimeSpan) {
                        var recentTimeSpan = query.RecentTimeSpan;
                        _timeSpanTextBox.TimeSpanTargetKind = ToTimeSpanTargetKind(recentTimeSpan.DateKind);
                        _timeSpanTextBox.IsRecentTimeSpanSelected = true;
                        _timeSpanTextBox.RecentTimeSpan = recentTimeSpan;

                    } else if (query.NarrowByTimeSpan) {
                        var timeSpan = query.TimeSpan;
                        _timeSpanTextBox.TimeSpanTargetKind = ToTimeSpanTargetKind(timeSpan.DateKind);
                        _timeSpanTextBox.IsTimeSpanSelected = true;
                        _timeSpanTextBox.TimeSpan = timeSpan;
                    }
                }

                if (value is MemoSmartFilter) {
                    Text = "スマートフィルタの設定";
                    _nameLabel.Text = "スマートフィルタ名(&N):";
                } else if (value is MemoSmartFolder) {
                    Text = "スマートフォルダの設定";
                    _nameLabel.Text = "スマートフォルダ名(&N):";
                }

                ValidateInput();
            }
        }

        // ========================================
        // method
        // ========================================
        public void ClearConditions() {
            _searchTextBox.Text = "";
            _titleTextBox.Text = "";
            _tagTextBox.ClearCondition();
            _importanceTextBox.ClearCondition();
            _markTextBox.ClearCondition();
            _timeSpanTextBox.ClearCondition();
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);

            _nameTextBox.Width =
                _flowLayoutPanel.Width - _flowLayoutPanel.Padding.Size.Width - _nameTextBox.Margin.Size.Width;
        }

        private void _okButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            if (_queryHolder != null && _queryHolder.Query != null) {
                var query = _queryHolder.Query;
                query.Clear();

                _queryHolder.Name = _nameTextBox.Text;
                query.Condition = _searchTextBox.Text;
                query.Title = _titleTextBox.Text;

                if (
                    !_tagTextBox.IsUntaggedChecked &&
                    (_tagTextBox.CheckedTags == null || !_tagTextBox.CheckedTags.Any())
                ) {
                    query.NarrowByTagIds = false;
                } else {
                    query.NarrowByTagIds = true;
                    query.NoTag = _tagTextBox.IsUntaggedChecked;
                    foreach (var tag in _tagTextBox.CheckedTags.ToArray()) {
                        var id = _facade.Container.GetId(tag);
                        query.AddTagId(id);
                    }
                    query.TagCompoundKind = _tagTextBox.IsAnyChecked ? MemoConditionCompoundKind.Any : MemoConditionCompoundKind.All;
                }

                if (_importanceTextBox.CheckedImportanceKinds == null || !_importanceTextBox.CheckedImportanceKinds.Any()) {
                    query.NarrowByImportanceKind = false;
                } else {
                    query.NarrowByImportanceKind = true;
                    foreach (var kind in _importanceTextBox.CheckedImportanceKinds.ToArray()) {
                        query.AddImportanceKind(kind);
                    }
                }

                if (_markTextBox.CheckedMarkKinds == null || !_markTextBox.CheckedMarkKinds.Any()) {
                    query.NarrowByMarkKinds = false;
                } else {
                    query.NarrowByMarkKinds = true;
                    foreach (var kind in _markTextBox.CheckedMarkKinds.ToArray()) {
                        query.AddMarkKind(kind);
                    }
                    query.MarkCompoundKind = _markTextBox.IsAnyChecked ? MemoConditionCompoundKind.Any : MemoConditionCompoundKind.All;
                }

                if (_timeSpanTextBox.IsRecentTimeSpanSelected) {
                    query.NarrowByRecentTimeSpan = true;
                    query.NarrowByTimeSpan = false;
                    query.RecentTimeSpan = _timeSpanTextBox.RecentTimeSpan;
                }
                if (_timeSpanTextBox.IsTimeSpanSelected) {
                    query.NarrowByRecentTimeSpan = false;
                    query.NarrowByTimeSpan = true;
                    query.TimeSpan = _timeSpanTextBox.TimeSpan;
                }
            }
        }

        private void _cancelButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }

        private TimeSpanTargetKind ToTimeSpanTargetKind(MemoDateKind kind) {
            switch (kind) {
                case MemoDateKind.Created: {
                    return TimeSpanTargetKind.Created;
                }
                case MemoDateKind.Modified: {
                    return TimeSpanTargetKind.Modified;
                }
                case MemoDateKind.Accessed: {
                    return TimeSpanTargetKind.Accessed;
                }
            }

            throw new ArgumentException("kind");
        }

        private void HandleNameTextBoxTextChanged(object sender, EventArgs e) {
            ValidateInput();
        }

        private void ValidateInput() {
            _okButton.Enabled = !string.IsNullOrEmpty(_nameTextBox.Text);
        }
    }
}
