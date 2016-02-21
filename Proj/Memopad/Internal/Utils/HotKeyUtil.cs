/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Win32.User32;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Internal.Helpers;
using System.Windows.Forms;
using Mkamo.Common.Forms.ScreenCapture;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Mkamo.Common.Forms.Clipboard;
using Mkamo.Memopad.Core;
using Mkamo.Common.String;
using Mkamo.Model.Memo;
using System.Threading;
using Mkamo.Common.Util;

namespace Mkamo.Memopad.Internal.Utils {
    internal static class HotKeyUtil {
        // ========================================
        // static method
        // ========================================
        // ------------------------------
        // internal
        // ------------------------------
        public static void ActivateHotKeyPressed() {
            var facade = MemopadApplication.Instance;
            facade.ShowMainForm();
            facade.ActivateMainForm();
        }

        public static void CreateMemoHotKeyPressed() {
            var facade = MemopadApplication.Instance;
            facade.ShowMainForm();
            facade.ActivateMainForm();
            facade.CreateMemo();
        }

        public static void CreateMemoFromClipboardHotKeyPressed() {
            var facade = MemopadApplication.Instance;
            facade.ShowMainForm();
            facade.ActivateMainForm();
            facade.CreateMemo();

            var form = facade.MainForm;
            if (form.CurrentEditorCanvas != null) {
                MemoEditorHelper.Paste(form.CurrentEditorCanvas.RootEditor.Children.First(), false);
            }
        }

        public static void ClipMemoHotKeyPressed() {
            var facade = MemopadApplication.Instance;
            facade.ClipAndCreateMemo();
        }

        public static void CaptureScreenHotKeyPressed() {
            var facade = MemopadApplication.Instance;
            facade.CaptureAndCreateMemo();
        }
    }
}
