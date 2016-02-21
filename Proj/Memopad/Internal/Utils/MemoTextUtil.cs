/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Diagnostics;
using Mkamo.Editor.Core;
using Mkamo.Common.String;
using Mkamo.Model.Memo;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Core;
using Mkamo.StyledText.Writer;
using Mkamo.Common.Core;

namespace Mkamo.Memopad.Internal.Utils {
    internal static class MemoTextUtil {
        // ========================================
        // static field
        // ========================================
        private const int MaxLineCount = 5;

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
        public static string LoadOrSaveAndLoadSummaryText(MemoInfo info) {
            var ret = LoadSummaryText(info);
            if (ret == null) {
                SaveSummaryText(info);
                return LoadSummaryText(info);
            } else {
                return ret;
            }
        }

        public static void SaveSummaryText(MemoInfo info) {
            var facade = MemopadApplication.Instance;
            var container = facade.Container;
            var memo = container.Find<Memo>(info.MemoId);

            var summary = GetSummaryText(info);

            facade.Container.SaveExtendedTextData(memo, "SummaryText", summary);
        }

        public static string LoadSummaryText(MemoInfo info) {
            var facade = MemopadApplication.Instance;
            var container = facade.Container;
            var memo = container.Find<Memo>(info.MemoId);

            return container.LoadExtendedTextData(memo, "SummaryText");
        }

        public static string GetSummaryText(MemoInfo info) {
            var canvas = new EditorCanvas();
            MemoSerializeUtil.LoadEditor(canvas, info.MementoId);

            try {
                return GetSummaryText(canvas);
            } finally {
                canvas.Dispose();
            }
        }
        
        public static string GetSummaryText(EditorCanvas target) {
            return GetText(target, MaxLineCount, true);
        }

        public static string GetText(EditorCanvas target, int? maxLineCount, bool removeEmptyLine) {
            var buf = new StringBuilder();

            var lineCount = 0;
            var children = target.RootEditor.Children.First().GetChildrenByPosition();
            foreach (var child in children) {
                child.Accept(
                    editor => {
                        var memoText = editor.Model as MemoText;
                        if (memoText != null) {
                            var settings = new PlainTextWriterSettings();
                            settings.UnorderedListBullet = "  ・";
                            var text = memoText.StyledText.ToPlainText(settings);
                            if (!string.IsNullOrEmpty(text)) {
                                var lines = removeEmptyLine ?
                                    text.Split(new[] { '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries):
                                    StringUtil.SplitLines(text);

                                foreach (var line in lines) {
                                    if (!StringUtil.IsNullOrWhitespace(line)) {
                                        buf.AppendLine(line);
                                        ++lineCount;
                                    }
                                    if (maxLineCount.HasValue && lineCount >= maxLineCount) {
                                        return true;
                                    }
                                }
                                if (!removeEmptyLine) {
                                    buf.AppendLine();
                                }
                            }
                        }
                        return false;
                    }
                );
                if (maxLineCount.HasValue && lineCount >= maxLineCount) {
                    break;
                }
            }

            return buf.ToString();
        }


    }
}
