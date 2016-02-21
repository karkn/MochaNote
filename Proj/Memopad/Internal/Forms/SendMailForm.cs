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
using Mkamo.Common.String;
using System.Text.RegularExpressions;
using Mkamo.Common.Core;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class SendMailForm: Form {
        // ========================================
        // type
        // ========================================
        internal enum BodyKind {
            Text,
            Image,
            TextAndImage,
        }

        // ========================================
        // field
        // ========================================
        private int _smtpPort;
        private bool _enableAuth;
        private string _userName;
        private string _password;
        private bool _enableSsl;

        // ========================================
        // constructor
        // ========================================
        public SendMailForm() {
            InitializeComponent();

            _bodyComboBox.SelectedIndex = 0;
            
            _smtpServerTextBox.TextChanged += (se, ev) => UpdateUI();
            _toTextBox.TextChanged += (se, ev) => UpdateUI();
            _fromTextBox.TextChanged += (se, ev) => UpdateUI();
            _subjectTextBox.TextChanged += (se, ev) => UpdateUI();

            UpdateUI();
        }

        // ========================================
        // property
        // ========================================
        public string SmtpServer {
            get { return _smtpServerTextBox.Text; }
            set {
                _smtpServerTextBox.Text = value;
                UpdateUI();
            }
        }

        public string To {
            get { return _toTextBox.Text; }
            set {
                _toTextBox.Text = value;
                UpdateUI();
            }
        }

        public string From {
            get { return _fromTextBox.Text; }
            set {
                _fromTextBox.Text = value;
                UpdateUI();
            }
        }

        public string Subject {
            get { return _subjectTextBox.Text; }
            set {
                _subjectTextBox.Text = value;
                UpdateUI();
            }
        }


        public BodyKind Body {
            get {
                switch (_bodyComboBox.SelectedIndex) {
                    case 0:
                        return BodyKind.Text;
                    case 1:
                        return BodyKind.Image;
                    case 2:
                        return BodyKind.TextAndImage;
                    default:
                        return BodyKind.Text;
                }
            }
        }


        public int SmtpPort {
            get { return _smtpPort; }
            set { _smtpPort = value; }
        }

        public bool SmtpEnableAuth {
            get { return _enableAuth; }
            set { _enableAuth = value; }
        }

        public string SmtpUserName {
            get { return _userName; }
            set { _userName = value; }
        }

        public string SmtpPassword {
            get { return _password; }
            set { _password = value; }
        }

        public bool SmtpEnableSsl {
            get { return _enableSsl; }
            set { _enableSsl = value; }
        }


        // ========================================
        // method
        // ========================================
        private void UpdateUI() {
            _sendButton.Enabled =
                !StringUtil.IsNullOrWhitespace(_smtpServerTextBox.Text) &&
                !StringUtil.IsNullOrWhitespace(_toTextBox.Text) &&
                !StringUtil.IsNullOrWhitespace(_fromTextBox.Text) &&
                !StringUtil.IsNullOrWhitespace(_subjectTextBox.Text);
        }

        private void _smtpDetailButton_Click(object sender, EventArgs e) {
            using (var dialog = new SmtpDetailSettingsForm()) {
                dialog.Font = Font;
                dialog.Port = _smtpPort;
                dialog.EnableAuth = _enableAuth;
                dialog.UserName = _userName;
                dialog.Password = _password;
                dialog.EnableSsl = _enableSsl;

                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    _smtpPort = dialog.Port;
                    _enableAuth = dialog.EnableAuth;
                    _userName = dialog.UserName;
                    _password = dialog.Password;
                    _enableSsl = dialog.EnableSsl;
                }
            }
        }

    }
}
