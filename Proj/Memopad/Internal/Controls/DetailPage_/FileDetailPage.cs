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
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Common.Forms.Themes;
using Mkamo.Common.Command;
using Mkamo.Editor.Core;
using Mkamo.Model.Memo;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal partial class FileDetailPage: UserControl, IDetailSettingsPage {
        // ========================================
        // field
        // ========================================
        private IEditor _target;

        private ITheme _theme;
        private bool _isModified;

        // ========================================
        // constructor
        // ========================================
        public FileDetailPage(IEditor target) {
            InitializeComponent();

            BackColor = Color.White;

            _target = target;
            _isModified = false;

            var memoFile = _target.Model as MemoFile;
            if (memoFile != null) {
                if (memoFile.IsEmbedded) {
                    _pathLabel.Enabled = false;
                    _pathTextBox.Enabled = false;
                    _pathButton.Enabled = false;
                }
            }

            _titleTextBox.TextChanged += HandleTextBoxChanged;
            _pathTextBox.TextChanged += HandleTextBoxChanged;
        }

        // ========================================
        // property
        // ========================================
        public System.Windows.Forms.Control PageControl {
            get { return this; }
        }

        public bool NeedBorder {
            get { return true; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITheme Theme {
            get { return _theme; }
            set {
                if (value == _theme) {
                    return;
                }
                _theme = value;

                var captionFont = value.CaptionFont;
                _titleLabel.Font = captionFont;
                _pathLabel.Font = captionFont;

                var inputFont = value.InputFont;
                _titleTextBox.Font = inputFont;
                _pathTextBox.Font = inputFont;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsModified {
            get { return _isModified; }
            set { _isModified = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title {
            get { return _titleTextBox.Text; }
            set {
                if (value == _titleTextBox.Text) {
                    return;
                }
                _titleTextBox.Text = value;
                _isModified = true;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Path {
            get { return _pathTextBox.Text; }
            set {
                if (value == _pathTextBox.Text) {
                    return;
                }
                _pathTextBox.Text = value;
                _isModified = true;
            }
        }


        // ========================================
        // method
        // ========================================
        public ICommand GetUpdateCommand() {
            var memoFile = _target.Model as MemoFile;
            if (memoFile != null) {
                var oldName = memoFile.Name;
                var oldPath = memoFile.Path;
                return new DelegatingCommand(
                    () => {
                        memoFile.Name = _titleTextBox.Text;
                        if (!memoFile.IsEmbedded) {
                            memoFile.Path = _pathTextBox.Text;
                        }
                    },
                    () => {
                        memoFile.Name = oldName;
                        if (!memoFile.IsEmbedded) {
                            memoFile.Path = oldPath;
                        }
                    }
                );
            }

            return null;
        }

        // ------------------------------
        // private
        // ------------------------------
        private void HandleTextBoxChanged(object sender, EventArgs e) {
            _isModified = true;
        }

        private void _pathButton_Click(object sender, EventArgs e) {
            var dialog = new OpenFileDialog();
            dialog.RestoreDirectory = true;
            dialog.ShowHelp = true;
            dialog.Filter = "All files (*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK) {
                _titleTextBox.Text = System.IO.Path.GetFileName(dialog.FileName);
                _pathTextBox.Text = dialog.FileName;
            }
        }
    }
}
