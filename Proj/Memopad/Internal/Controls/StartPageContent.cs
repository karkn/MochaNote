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
using Mkamo.Memopad.Internal.Core;
using ComponentFactory.Krypton.Toolkit;
using Mkamo.Memopad.Internal.Forms;
using System.Diagnostics;
using System.IO;
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Properties;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal partial class StartPageContent: UserControl {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;
        private Color _borderColor;

        // ========================================
        // constructor
        // ========================================
        public StartPageContent() {
            InitializeComponent();
            DoubleBuffered = true;

            _facade = MemopadApplication.Instance;
            var theme = _facade.Theme;
            Font = theme.CaptionFont;

            _titleLabel.Font = new Font(theme.CaptionFont.Name, 16, FontStyle.Bold);
            _recentLabel.Font = new Font(theme.CaptionFont.Name, 12);
            _borderColor = KryptonManager.CurrentGlobalPalette.GetBorderColor1(
                PaletteBorderStyle.ControlClient,
                PaletteState.Normal
            );

            _recentlyClosedListBox.ListBox.MouseDown += HandleRecentlyClosedListBoxMouseDown;

            _facade.RecentlyClosedMemoInfosChanged += HandleFacadeRecentlyClosedMemoInfosChanged;
        }

        // ========================================
        // destructor
        // ========================================
        private void CleanUp() {
            _facade.RecentlyClosedMemoInfosChanged -= HandleFacadeRecentlyClosedMemoInfosChanged;
        }

        // ========================================
        // method
        // ========================================
        // --- override ---
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, _borderColor, ButtonBorderStyle.Solid);
        }

        protected override void OnVisibleChanged(EventArgs e) {
            base.OnVisibleChanged(e);

            if (Visible) {
                var winSettings = _facade.WindowSettings;
                _showOnStartCheckBox.Checked = winSettings.ShowStartPageOnStart;

                UpdateRecentlyClosedListBox();
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private void UpdateRecentlyClosedListBox() {
            _recentlyClosedListBox.SuspendLayout();

            _recentlyClosedListBox.Items.Clear();
            foreach (var info in _facade.RecentlyClosedMemoInfos.Reverse()) {
                var item = new KryptonListItem();
                item.ShortText = info.Title;
                item.Image = Resources.sticky_note;
                item.Tag = info;

                _recentlyClosedListBox.Items.Add(item);
            }

            _recentlyClosedListBox.ResumeLayout();
        }

        // --- handler ---
        private void HandleFacadeRecentlyClosedMemoInfosChanged(object sender, EventArgs e) {
            UpdateRecentlyClosedListBox();
        }

        private void HandleRecentlyClosedListBoxMouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                var index = _recentlyClosedListBox.IndexFromPoint(e.Location);
                if (index > -1 && index < _recentlyClosedListBox.Items.Count) {
                    var rect = _recentlyClosedListBox.GetItemRectangle(index);
                    if (rect.Contains(e.Location)) {
                        /// 最後のitemより下の余白のマウス操作でない

                        var item = _recentlyClosedListBox.Items[index] as KryptonListItem;
                        if (item != null) {
                            var info = item.Tag as MemoInfo;
                            if (info != null) {
                                _facade.LoadMemo(info);
                            }
                        }
                    }
                }
            }
        }

        // --- generated handler ---
        private void _createMemoLinkLabel_LinkClicked(object sender, EventArgs e) {
            _facade.CreateMemo();
        }

        private void _manageTagsLinkLabel_LinkClicked(object sender, EventArgs e) {
            using (var form = new TagManageForm()) {
                form.Font = _facade.Theme.CaptionFont;
                form.ShowDialog(this);
            }
        }

        private void _showTutorialLinkLabel_LinkClicked(object sender, EventArgs e) {
            try {
                Process.Start(MemopadConsts.TutorialPath);
            } catch (Exception ex) {
                Logger.Warn("Can't show tutorial", ex);
            }
        }

        private void _checkForUpdatesLinkLabel_LinkClicked(object sender, EventArgs e) {
            // todo: show dialog
            //_facade.CheckForUpdates();
        }

        private void _showOnStartCheckBox_CheckedChanged(object sender, EventArgs e) {
            _facade.WindowSettings.ShowStartPageOnStart = _showOnStartCheckBox.Checked;
        }

        //private void _manageSmartFilterLinkLabel_LinkClicked(object sender, EventArgs e) {
        //    using (var form = new SmartFilterManageForm()) {
        //        form.Font = _facade.Theme.CaptionFont;
        //        form.ShowDialog(this);
        //    }
        //}

    }
}
