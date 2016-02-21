/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Control.KeyBindTextBox {
    public class KeyBindTextBox: TextBox {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);

            var win = (e.KeyData & Keys.LWin) == Keys.LWin;

            var ctrl = e.KeyCode == Keys.ControlKey;
            var shift = e.KeyCode == Keys.ShiftKey;
            var alt = e.KeyCode == Keys.Menu;
            var modkey = ctrl || shift || alt;

            if (!modkey) {
                if (win) {
                    // winキーでうまく設定できないのでとりあえず無効にしておく
                    //Text = "Win+" + e.KeyCode;
                } else {
                    if (e.Shift && e.Alt) {
                        Text = "Control+Shift+Alt+" + e.KeyCode;
                    } else if (e.Shift) {
                        Text = "Control+Shift+" + e.KeyCode;
                    } else if (e.Alt) {
                        Text = "Control+Alt+" + e.KeyCode;
                    } else {
                        Text = "Control+Alt+" + e.KeyCode;
                    }
                }
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }
}
