/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using Mkamo.Common.Win32.User32;

namespace Mkamo.Common.Forms.Clipboard {
    using Clipboard = global::System.Windows.Forms.Clipboard;
    using System.Threading;
using System.IO;
    using System.Text.RegularExpressions;

    public static class ClipboardUtil {
        // ========================================
        // static method
        // ========================================
        public static void SetText(string text) {
            if (text == null) {
                return;
            }
            try {
                Clipboard.SetData(DataFormats.UnicodeText, text);
                
                /// Clipboard.SetText(text, TextDataFormat.UnicodeText)を使うと
                /// 内部的にSetDataObject(text, true)が呼ばれて
                /// さらに内部的にOleFlushClipboard()が呼ばれる。
                /// VNCなどクリップボード監視ソフトが起動しているとExternalExceptionが起こる。

                /// Clipboard.SetText(text) を使うと，
                /// vncなどのクリップボードを監視しているソフトが起動していると
                /// ExternalExceptionが起こる．
                /// なんどか繰り返すと大丈夫になる？
                /// 
                /// Clipboard.SetDataObject(text)だと
                /// Wordに貼りつけるとエラーが起きる．

            } catch (ExternalException) {
                /// vncなどが起動していると例外が起きるがコピーできているので握りつぶす
                //var hWnd = User32PI.GetOpenClipboardWindow();
                //if (hWnd != IntPtr.Zero) {
                //    int pid = 0;
                //    int tid = User32PI.GetWindowThreadProcessId(hWnd, out pid);
                //    MessageBox.Show(
                //        "クリップボードを開けませんでした。以下のプログラムがクリップボードを使用中です。" +
                //        Environment.NewLine +
                //        Process.GetProcessById((int) pid).Modules[0].FileName,
                //        "クリップボードエラー",
                //        MessageBoxButtons.OK,
                //        MessageBoxIcon.Exclamation
                //    );
                //}
            }
        }

        public static bool ContainsText() {
            try {
                return Clipboard.ContainsText(TextDataFormat.UnicodeText);
            } catch (ExternalException) {
                return false;
            }
        }

        /// <summary>
        /// formatのデータが取れるまで最大millisecondsミリ秒待つ。
        /// </summary>
        public static void Wait(string format, int milliseconds) {
            var start = DateTime.Now;

            var data = default(object);
            while (data == null) {
                /// millisecondsミリ秒経過したらあきらめる
                var span = DateTime.Now - start;
                if (span.TotalMilliseconds > milliseconds) {
                    break;
                }

                Application.DoEvents();
                try {
                    data = Clipboard.GetData(format);
                } catch (ExternalException) {
                }
            }
        }

        public static string GetCFHtmlFromClipboard() {
            var data = Clipboard.GetData("Html Format");
            var mem = data as MemoryStream;
            if (mem == null) {
                return null;
            } else {
                return Encoding.UTF8.GetString(mem.ToArray());
            }
        }

        public static string GetCFHtmlFromDataObject(IDataObject dataObj) {
            var data = dataObj.GetData("Html Format");
            var mem = data as MemoryStream;
            if (mem == null) {
                return null;
            } else {
                return Encoding.UTF8.GetString(mem.ToArray());
            }
        }

        public static string GetHtmlSourceUrl(string cfHtml) {
            var match = Regex.Match(cfHtml, "^SourceURL:(.*)$", RegexOptions.Multiline);
            if (match.Success && match.Groups.Count == 2) {
                return match.Groups[1].Value;
            }
            return null;
        }

        public static string GetHtmlSourceUrlFromClipboard() {
            var data = Clipboard.GetData("Html Format");
            using (var mem = data as MemoryStream) {
                if (mem == null) {
                    return null;
                } else {
                    var html = Encoding.UTF8.GetString(mem.ToArray());
                    return GetHtmlSourceUrl(html);
                }
            }
        }

        public static MemoryStream GetCFHtmlMemoryStream(string html) {
            var bytes = Encoding.UTF8.GetBytes(GetCFHtml(html));
            return new MemoryStream(bytes);
        }

        private static string GetCFHtml(string html) {
            var size = Encoding.UTF8.GetByteCount(html) + 120;
            var ret =
                "Version:0.9\r\n" +

                //"StartHTML:00000000\r\n" +
                //"EndHTML:00000000}\r\n" +
                //"StartFragment:00000000\r\n" +
                //"EndFragment:00000000\r\n" +

                "StartHTML:00000097\r\n" +
                string.Format("EndHTML:{0:00000000}\r\n", size + 22) +
                "StartFragment:00000120\r\n" +
                string.Format("EndFragment:{0:00000000}\r\n", size) +

                "<!--StartFragment -->\r\n" +
                html + "\r\n" +
                "<!--EndFragment-->\r\n";
            return ret;
        }

        public static bool ContainsCsv() {
            return Clipboard.ContainsData(DataFormats.CommaSeparatedValue);
        }

        public static string GetCsvText() {
            var data = Clipboard.GetDataObject();
            if (data != null && data.GetDataPresent(DataFormats.CommaSeparatedValue)) {
                var ms = data.GetData(DataFormats.CommaSeparatedValue) as MemoryStream;
                if (ms != null) {
                    var reader = new StreamReader(ms, Encoding.Default);
                    return reader.ReadToEnd();
                }
            }
            return null;
        }

    }
}
