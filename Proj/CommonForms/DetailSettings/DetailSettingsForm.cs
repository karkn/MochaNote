/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Common.Diagnostics;
using Mkamo.Common.Command;
using Mkamo.Common.Forms.Utils;
using Mkamo.Common.Forms.Themes;

namespace Mkamo.Common.Forms.DetailSettings {
    public partial class DetailSettingsForm: Form {
        // ========================================
        // static field
        // ========================================
        private int ItemSpace = 8;

        private int ItemPadding = 3;
        
        // ========================================
        // field
        // ========================================
        private IDetailSettingsPage _currentPage;
        private Dictionary<string, IDetailSettingsPage> _pages;

        private ITheme _theme;

        // ========================================
        // constructor
        // ========================================
        public DetailSettingsForm() {
            _pages = new Dictionary<string, IDetailSettingsPage>();

            InitializeComponent();

            _pageSelectorListBox.MouseClick += HandlePageSelectorMouseClick;
        }

        // ========================================
        // property
        // ========================================
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITheme Theme {
            get { return _theme; }
            set {
                if (value == _theme) {
                    return;
                }
                _theme = value;

                var captionFont = value.CaptionFont;
                Font = captionFont;
                _okButton.Font = captionFont;
                _cancelButton.Font = captionFont;
                _pageSelectorListBox.Font = captionFont;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int PageSelectorListBoxWidth {
            get { return _pageSelectorListBox.Width; }
            set { _pageSelectorListBox.Width = value; }
        }

        // ------------------------------
        // private
        // ------------------------------
        private IDetailSettingsPage _CurrentPage {
            get { return _currentPage; }
            set {
                if (value == _currentPage) {
                    return;
                }
                if (_currentPage != null) {
                    _currentPage.PageControl.Hide();
                }
                _currentPage = value;
                if (value != null) {
                    if (_theme != null) {
                        _currentPage.Theme = _theme;
                    }
                    _currentPage.PageControl.Show();
                }
            }
        }


        // ========================================
        // method
        // ========================================
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            Arrange();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            ControlPaintUtil.DrawBorder(
                e.Graphics, _pageSelectorListBox, SystemColors.ControlDark, _pageSelectorListBox.BackColor, ItemPadding
            );

            if (_currentPage == null || _currentPage.NeedBorder) {
                ControlPaint.DrawBorder(
                    e.Graphics, GetPageBounds(), SystemColors.ControlDark, ButtonBorderStyle.Solid
                );
            }
        }

        // === DetailForm ==========
        public void RegisterPage(string pageName, IDetailSettingsPage page) {
            Contract.Requires(pageName != null);
            Contract.Requires(page != null);

            var firstPage = !_pages.Any();

            _pages[pageName] = page;
            page.PageControl.Hide();
            Controls.Add(page.PageControl);
            _pageSelectorListBox.Items.Add(pageName);

            if (firstPage) {
                _CurrentPage = page;
                _pageSelectorListBox.SelectedIndex = 0;
            }

            Arrange();
        }

        public ICommand GetUpdateCommand() {
            var ret = default(ICommand);

            var found = false;
            foreach (var page in _pages.Values) {
                if (page.IsModified) {
                    var cmd = page.GetUpdateCommand();
                    if (cmd != null) {
                        if (!found) {
                            found = true;
                            ret = new CompositeCommand();
                        }
                        ret.Chain(cmd);
                    }
                }
            }

            return ret;
        }


        // ------------------------------
        // private
        // ------------------------------
        private void Arrange() {
            SuspendLayout();

            var cliSize = ClientSize;
            _cancelButton.Location = new Point(
                cliSize.Width - _cancelButton.Width - ItemSpace,
                cliSize.Height - _cancelButton.Height - ItemSpace
            );

            _okButton.Location = new Point(
                _cancelButton.Left - _okButton.Width - ItemSpace,
                _cancelButton.Top
            );

            _pageSelectorListBox.Left = ItemSpace + ItemPadding;
            _pageSelectorListBox.Top = ItemSpace + ItemPadding;
            _pageSelectorListBox.Height = _cancelButton.Top - (ItemPadding + _pageSelectorListBox.Top + ItemSpace + ItemPadding);

            var pageBounds = GetPageBounds();
            foreach (var page in _pages.Values) {
                if (page != null && page.PageControl != null) {
                    var r = pageBounds;
                    if (page.NeedBorder) {
                        r.Inflate(-1, -1);
                    }
                    page.PageControl.Bounds = r;
                }
            }

            ResumeLayout();
        }

        private Rectangle GetPageBounds() {
            var pageLeft = _pageSelectorListBox.Right + ItemPadding + ItemSpace;
            var pageTop = _pageSelectorListBox.Top - ItemPadding;
            var pageHeight = _pageSelectorListBox.Height + ItemPadding + ItemPadding;
            return new Rectangle(
                pageLeft,
                pageTop,
                ClientSize.Width - pageLeft - ItemSpace,
                pageHeight
            );
        }

        private void HandlePageSelectorMouseClick(object sender, MouseEventArgs e) {
            var selected = _pageSelectorListBox.SelectedItem as string;
            _CurrentPage = _pages[selected];
        }

        private void _okButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void _cancelButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
