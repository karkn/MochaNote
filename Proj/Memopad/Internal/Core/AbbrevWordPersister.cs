/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Mkamo.Common.Core;
using System.Text.RegularExpressions;

namespace Mkamo.Memopad.Internal.Core {
    internal class AbbrevWordPersister {
        // ========================================
        // field
        // ========================================
        private List<string> _words = new List<string>();

        // ========================================
        // constructor
        // ========================================
        public AbbrevWordPersister() {
        }

        // ========================================
        // method
        // ========================================
        public IEnumerable<string> GetWords() {
            return _words;
        }

        public void Load(string fileName) {
            if (!File.Exists(fileName)) {
                return;
            }

            var lines = File.ReadAllLines(fileName);
            SetWords(lines);
        }

        public void Save(string fileName) {
            File.WriteAllLines(fileName, _words.ToArray());
        }

        public string ToText() {
            var buf = new StringBuilder();

            foreach (var word in _words) {
                buf.AppendLine(word);
            }

            return buf.ToString();
        }

        public void FromText(string text) {
            var lines = StringUtil.SplitLines(text);
            SetWords(lines);
        }

        // ------------------------------
        // private
        // ------------------------------
        private void SetWords(string[] lines) {
            _words.Clear();
            foreach (var line in lines) {
                if (!StringUtil.IsNullOrWhitespace(line)) {
                    var sanitized = line.Replace("\t", "    ");
                    _words.Add(sanitized.Trim());
                }
            }
        }
    }
}
