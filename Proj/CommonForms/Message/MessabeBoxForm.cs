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

namespace Mkamo.Common.Forms.Message {
    internal partial class MessabeBoxForm: Form {
        public MessabeBoxForm() {
            InitializeComponent();
        }

        public string Message {
            get { return _messageLabel.Text; }
            set { _messageLabel.Text = value; }
        }
        
        public void Run(Form owner, Action action) {
            if (owner == null) {
                var bounds = Screen.PrimaryScreen.WorkingArea;
                Location = new Point(
                    bounds.Left + (bounds.Width - Width) / 2,
                    bounds.Top + (bounds.Height - Height) / 2
                );
            } else {
                var bounds = owner.Bounds;
                Location = new Point(
                    bounds.Left + (bounds.Width - Width) / 2,
                    bounds.Top + (bounds.Height - Height) / 2
                );
            }
            Show(owner);
            Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            action();
            Cursor = Cursors.Default;
            Close();
        }
    }
}
