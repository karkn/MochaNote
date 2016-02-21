/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Focuses;
using Mkamo.Common.Core;

namespace Mkamo.Editor.Internal.Core {
    internal class AbbrevWordProvider: IAbbrevWordProvider {
        // ========================================
        // static field
        // ========================================
        private static readonly string[] EmptyStringArray = new string[0];

        // ========================================
        // field
        // ========================================
        private EditorCanvas _owner;

        // ========================================
        // constructor
        // ========================================
        public AbbrevWordProvider(EditorCanvas owner) {
            _owner = owner;
        }

        // ========================================
        // property
        // ========================================
        public Func<IEnumerable<string>> AdditionalWordsProvider { get; set; }

        // ========================================
        // method
        // ========================================
        public IEnumerable<string> GetCandidates(string inputting) {
            var ret = new List<string>();

            /// additional
            if (AdditionalWordsProvider != null) {
                foreach (var word in AdditionalWordsProvider()) {
                    if (word.StartsWith(inputting) && !string.Equals(word, inputting, StringComparison.Ordinal)) {
                        ret.Add(word);
                    }
                }
            }
            
            /// in editor
            var text = GetText();
            var words = SplitToWords(text);

            if (words == null || words.Count() < 1) {
                return EmptyStringArray;
            }

            if (StringUtil.IsNullOrWhitespace(inputting)) {
                return EmptyStringArray;
            }

            foreach (var word in words) {
                if (word.StartsWith(inputting) && !string.Equals(word, inputting, StringComparison.Ordinal)) {
                    ret.Add(word);
                }
            }
            return ret;
        }

        // ------------------------------
        // private
        // ------------------------------
        private IEnumerable<string> SplitToWords(string s) {
            var ret = new List<string>();

            var i = 0;
            var len = s.Length;
            while (i < len) {
                var chKind = StringUtil.GetCharKind(s[i]);
                var wordLast = StringUtil.GetForwardWordBound(s, i);
                if (i != wordLast && IsValidWordCharKind(chKind)) {
                    var item = s.Substring(i, wordLast - i + 1);
                    if (!ret.Contains(item)) {
                        ret.Add(item);
                    }
                }
                i = wordLast + 1;
            }

            return ret;
        }

        private bool IsValidWordCharKind(CharKind chKind) {
            return !(chKind == CharKind.Control || chKind == CharKind.Whitespace || chKind == CharKind.Punctuation || chKind == CharKind.Others);
        }

        private string GetText() {
            var buf = new StringBuilder();
            _owner.RootEditor.Accept(
                editor => {
                    var text = string.Empty;
                    if (editor.IsFocused) {
                        var stfocus = editor.Focus as StyledTextFocus;
                        text = stfocus == null ? string.Empty : stfocus.StyledText.Text;
                    } else {
                        text = editor.Controller.GetText();
                    }
                    if (!string.IsNullOrEmpty(text)) {
                        buf.AppendLine(text);
                    }
                    return false;
                }
            );
            return buf.ToString();
        }
    }
}
