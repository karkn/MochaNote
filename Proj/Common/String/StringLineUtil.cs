/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Common.String {
    // text末尾にはEndOfTextとして'\0'があるものとする
    public static class StringLineUtil {
        [Flags, Serializable]
        public enum LineEndKinds {
            NotLineEnd = 0x0000,

            CarrigeReturn = 0x0001,
            LineFeed = 0x0002,
            EndOfText = 0x0004,

            SingleByte = 0x0010,
            DoubleByte = 0x0020,

            CarrigeReturnOfSingleByte = CarrigeReturn | SingleByte,
            LineFeedOfSingleByte = LineFeed | SingleByte,
            FirstOfDoubleByte = CarrigeReturn | DoubleByte,
            SecondOfDoubleByte = LineFeed | DoubleByte,
        }

        public static string EnsureEndOfText(string text) {
            return text[text.Length - 1] == '\0'? text: text + '\0';
        }

        // todo: EndOfTextを返すのをやめる
        public static LineEndKinds GetLineEndKind(string text, int charIndex) {
            text = EnsureEndOfText(text);

            if (charIndex < 0 | charIndex > text.Length - 1) {
                throw new ArgumentOutOfRangeException("charIndex");
            }

            if (charIndex == text.Length - 1) {
                return LineEndKinds.EndOfText | LineEndKinds.SingleByte;
            }

            switch (text[charIndex]) {
                case '\r': {
                    if (charIndex == text.Length - 2) {
                        return LineEndKinds.CarrigeReturn | LineEndKinds.SingleByte;
                    } else if (text[charIndex + 1] == '\n') {
                        return LineEndKinds.CarrigeReturn | LineEndKinds.DoubleByte;
                    } else {
                        return LineEndKinds.CarrigeReturn | LineEndKinds.SingleByte;
                    }
                }
                case '\n': {
                    if (charIndex == 0) {
                        return LineEndKinds.LineFeed | LineEndKinds.SingleByte;
                    } else if (text[charIndex - 1] == '\r') {
                        return LineEndKinds.LineFeed | LineEndKinds.DoubleByte;
                    } else {
                        return LineEndKinds.LineFeed | LineEndKinds.SingleByte;
                    }
                }
                default: {
                    return LineEndKinds.NotLineEnd;
                }
            }
        }

        public static bool IsSingleCarrigeReturnLineEnd(LineEndKinds kind) {
            return (kind & LineEndKinds.CarrigeReturnOfSingleByte) == LineEndKinds.CarrigeReturnOfSingleByte;
        }

        public static bool IsLineEndSecondOfDoubleByte(LineEndKinds kind) {
            return (kind & LineEndKinds.SecondOfDoubleByte) == LineEndKinds.SecondOfDoubleByte;
        }

        public static bool IsLineEndSecondOfDoubleByte(string text, int charIndex) {
            return IsLineEndSecondOfDoubleByte(GetLineEndKind(text, charIndex));
        }

        public static bool IsLineEnd(string text, int charIndex) {
            text = EnsureEndOfText(text);
            LineEndKinds kind = GetLineEndKind(text, charIndex);
            return
                ((kind & LineEndKinds.EndOfText) == LineEndKinds.EndOfText) ||
                ((kind & LineEndKinds.CarrigeReturnOfSingleByte) == LineEndKinds.CarrigeReturnOfSingleByte) ||
                ((kind & LineEndKinds.LineFeed) == LineEndKinds.LineFeed);
        }

        public static int GetLineIndexOf(string text, int charIndex) {
            text = EnsureEndOfText(text);
            int currentLine = 0;
            for (int i = 0; i < charIndex; ++i) {
                if (IsLineEnd(text, i)) {
                    ++currentLine;
                }
            }
            return currentLine;
        }

        public static int GetColumnIndexOf(string text, int charIndex) {
            text = EnsureEndOfText(text);
            int currentColumn = 0;
            for (int i = 0; i < charIndex; ++i) {
                if (IsLineEnd(text, i)) {
                    currentColumn = 0;
                } else {
                    ++currentColumn;
                }
            }
            return currentColumn;
        }

        public static int GetLineStartCharacterIndexOf(string text, int lineIndex) {
            text = EnsureEndOfText(text);
            int currentLine = 0;
            for (int i = 0; i < text.Length; ++i) {
                if (currentLine == lineIndex) {
                    return i;
                }
                if (IsLineEnd(text, i)) {
                    ++currentLine;
                }
            }
            throw new ArgumentOutOfRangeException("lineIndex");
        }

        public static int GetLineEndCharacterIndexOf(string text, int lineIndex) {
            text = EnsureEndOfText(text);
            int currentLine = 0;
            for (int i = 0; i < text.Length; ++i) {
                if (IsLineEnd(text, i)) {
                    if (currentLine == lineIndex) {
                        return i;
                    }
                    ++currentLine;
                }
            }
            throw new ArgumentOutOfRangeException("lineIndex");
        }

        public static int GetLineEndFirstCharacterIndexOf(string text, int lineIndex) {
            int lineEndIndex = GetLineEndCharacterIndexOf(text, lineIndex);
            if ((StringLineUtil.GetLineEndKind(text, lineEndIndex) & StringLineUtil.LineEndKinds.SecondOfDoubleByte) ==
                StringLineUtil.LineEndKinds.SecondOfDoubleByte
            ) {
                --lineEndIndex;
            }
            return lineEndIndex;
        }

        public static int GetCharacterIndexOf(string text, int lineIndex, int columnIndex) {
            text = EnsureEndOfText(text);
            if (columnIndex > GetColumnCount(text, lineIndex) - 1) {
                throw new ArgumentOutOfRangeException("columnIndex");
            }

            int bol = GetLineStartCharacterIndexOf(text, lineIndex);
            return bol + columnIndex;
        }

        public static int GetLineCount(string text) {
            text = EnsureEndOfText(text);
            int currentLine = 0;
            for (int i = 0; i < text.Length; ++i) {
                if (IsLineEnd(text, i)) {
                    ++currentLine;
                }
            }
            return currentLine;
        }

        public static int GetColumnCount(string text, int lineIndex) {
            text = EnsureEndOfText(text);
            return GetLineEndCharacterIndexOf(text, lineIndex) - GetLineStartCharacterIndexOf(text, lineIndex) + 1;
        }

        public static string GetLine(string text, int lineIndex) {
            text = EnsureEndOfText(text);
            return text.Substring(GetLineStartCharacterIndexOf(text, lineIndex), GetColumnCount(text, lineIndex));
        }

        public static string[] SplitLines(string text, int startLineIndex, int lineCount) {
            text = EnsureEndOfText(text);
            if (startLineIndex < 0) {
                throw new ArgumentOutOfRangeException("startLineIndex");
            }
            if (lineCount < 0) {
                throw new ArgumentOutOfRangeException("lineCount");
            }

            string[] ret = new string[lineCount];

            int currentLine = 0;
            int currentColumn = 0;
            int currentLineStartIndex = 0;
            for (int i = 0; i < text.Length; ++i) {
                if (currentColumn == 0) {
                    currentLineStartIndex = i;
                }

                if (IsLineEnd(text, i)) {
                    if (currentLine >= startLineIndex) {
                        ret[currentLine - startLineIndex] = text.Substring(currentLineStartIndex, i - currentLineStartIndex + 1);
                    }
                    ++currentLine;
                    currentColumn = 0;
                    if (currentLine >= startLineIndex + lineCount) {
                        return ret;
                    }
                } else {
                    ++currentColumn;
                }
            }

            throw new ArgumentOutOfRangeException("startLineIndex + lineCount");
        }

        public static string[] SplitLines(string text) {
            text = EnsureEndOfText(text);
            List<string> ret = new List<string>();

            int currentLine = 0;
            int currentColumn = 0;
            int currentLineStartIndex = 0;
            for (int i = 0; i < text.Length; ++i) {
                if (currentColumn == 0) {
                    currentLineStartIndex = i;
                }

                if (IsLineEnd(text, i)) {
                    ret.Add(text.Substring(currentLineStartIndex, i - currentLineStartIndex + 1));
                    ++currentLine;
                    currentColumn = 0;
                } else {
                    ++currentColumn;
                }
            }
            return ret.ToArray();
        }
    }
}
