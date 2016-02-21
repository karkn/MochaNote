/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Common.Forms.Clipboard;
using Mkamo.Common.Forms.KeyMap;
using System.Reflection;

namespace Mkamo.Memopad.Internal.KeyActions {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    internal static class TextBoxKeyActions {
        // ========================================
        // method
        // ========================================
        [KeyAction("")]
        public static void MoveBeginningOfLine(TextBox textBox) {
            textBox.DeselectAll();
            textBox.SelectionStart = 0;
        }

        [KeyAction("")]
        public static void MoveEndOfLine(TextBox textBox) {
            textBox.DeselectAll();
            textBox.SelectionStart = textBox.Text.Length;
        }

        [KeyAction("")]
        public static void MoveForward(TextBox textBox) {
            textBox.DeselectAll();
            var i = textBox.SelectionStart;
            if (i < textBox.Text.Length) {
                textBox.SelectionStart = i + 1;
            }
        }

        [KeyAction("")]
        public static void MoveBackward(TextBox textBox) {
            textBox.DeselectAll();
            var i = textBox.SelectionStart;
            if (i > 0) {
                textBox.SelectionStart = i - 1;
            }
        }

        [KeyAction("")]
        public static void RemoveForward(TextBox textBox) {
            if (textBox.SelectionLength > 0) {
                var offset = textBox.SelectionStart;
                var len = textBox.SelectionLength;
                textBox.DeselectAll();
                textBox.Text = textBox.Text.Remove(offset, len);
                textBox.SelectionStart = offset;
            } else {
                var i = textBox.SelectionStart;
                if (i < textBox.Text.Length) {
                    textBox.Text = textBox.Text.Remove(i, 1);
                    textBox.SelectionStart = i;
                }
            }
        }

        [KeyAction("")]
        public static void KillLine(TextBox textBox) {
            var index = textBox.SelectionStart;
            if (index < textBox.Text.Length) {
                var text = textBox.Text.Substring(index);
                ClipboardUtil.SetText(text);
                textBox.Text = textBox.Text.Remove(index);
                textBox.SelectionStart = index;
            }
        }

        [KeyAction("")]
        public static void Paste(TextBox textBox) {
            if (Clipboard.ContainsText()) {
                textBox.Paste();
            }
        }

        [KeyAction("")]
        public static void ClearText(TextBox textBox) {
            textBox.Clear();
        }

    }
}
