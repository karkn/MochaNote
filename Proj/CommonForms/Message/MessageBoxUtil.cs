/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Mkamo.Common.Forms.Message {
    public static class MessageBoxUtil {
        public static DialogResult Show(Form owner, Font font, string text, string caption, Action action) {
            using (var dialog = new MessabeBoxForm()) {
                dialog.Font = font;
                dialog.Text = caption;
                dialog.Message = text;
                dialog.Run(owner, action);
                return dialog.DialogResult;
            }
        }
    }
}
