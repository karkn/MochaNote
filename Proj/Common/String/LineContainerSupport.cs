/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.String {
    public class LineContainerSupport: ILineContainer {

        // ========================================
        // field
        // ========================================
        private string[] _lineStrings;
        private Func<string[]> _lineStringsProvider;
        private string _lineBreak;
        private int _lineBreakLength;

        // ========================================
        // constructor
        // ========================================
        public LineContainerSupport(Func<string[]> lineStringsProvider, string lineBreak) {
            _lineStrings = null;
            _lineStringsProvider = lineStringsProvider;
            _lineBreak = lineBreak;
            _lineBreakLength = lineBreak.Length;
        }


        // ========================================
        // property
        // ========================================
        public string[] Lines {
            get {
                if (_lineStrings == null) {
                    _lineStrings = _lineStringsProvider();
                }
                return _lineStrings;
            }
        }

        // ========================================
        // method
        // ========================================
        /// <summary>
        /// index番目の文字の行数を返す．
        /// </summary>
        public int GetLineIndex(int index) {
            if (index < 0) {
                throw new ArgumentOutOfRangeException("charIndex");
            }

            var lines = Lines;
            int lineStartIndex = 0;
            int lineIndex;
            for (lineIndex = 0; lineIndex < lines.Length; ++lineIndex) {
                var line = lines[lineIndex];
                if (index < lineStartIndex + line.Length + _lineBreakLength) {
                    return lineIndex;
                }
                lineStartIndex += line.Length + _lineBreakLength;
            }

            throw new ArgumentOutOfRangeException("charIndex");
        }

        /// <summary>
        /// index番目の文字の列数を返す．
        /// </summary>
        public int GetColumnIndex(int index) {
            if (index < 0) {
                throw new ArgumentOutOfRangeException("charIndex");
            }

            var lines = Lines;
            int lineStartIndex = 0;
            int lineIndex;
            for (lineIndex = 0; lineIndex < lines.Length; ++lineIndex) {
                var line = lines[lineIndex];
                if (index < lineStartIndex + line.Length + _lineBreakLength) {
                    return index - lineStartIndex;
                }
                lineStartIndex += line.Length + _lineBreakLength;
            }

            throw new ArgumentOutOfRangeException("charIndex");
        }

        /// <summary>
        /// lineIndex番目の行の最初の文字のインデクスを返す．
        /// </summary>
        public int GetLineStartCharIndex(int lineIndex) {
            var lines = Lines;

            if (lineIndex < 0 || lineIndex > lines.Length - 1) {
                throw new ArgumentOutOfRangeException("lineIndex");
            }

            int charIndex = 0;
            for (int i = 0; i < lineIndex; ++i) {
                charIndex += lines[i].Length + _lineBreakLength;
            }
            return charIndex;
        }

        /// <summary>
        /// lineIndex行目，columnIndex列目の文字のインデクスを返す．
        /// </summary>
        public int GetCharIndex(int lineIndex, int columnIndex) {
            var lines = Lines;
            if (lineIndex < 0 || lineIndex >= lines.Length) {
                throw new ArgumentOutOfRangeException("lineIndex");
            }

            var line = lines[lineIndex];
            if (columnIndex < 0 || columnIndex > line.Length + _lineBreakLength) {
                throw new ArgumentOutOfRangeException("columnIndex");
            }

            return GetLineStartCharIndex(lineIndex) + columnIndex;
        }


        /// <summary>
        /// lineIndex番目の列数を返す．改行文字も含む．
        /// </summary>
        public int GetColumnCount(int lineIndex) {
            var lines = Lines;
            if (lineIndex < 0 || lineIndex > lines.Length) {
                throw new ArgumentOutOfRangeException("lineIndex");
            }
            return lines[lineIndex].Length + _lineBreakLength;
        }


        public void Dirty() {
            _lineStrings = null;
        }
    }
}
