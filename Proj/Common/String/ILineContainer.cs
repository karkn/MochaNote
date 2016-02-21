/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.String {
    public interface ILineContainer {
        /// <summary>
        /// 各行を格納したstringの配列を返す．
        /// 行末の改行文字は含まない．
        /// </summary>
        string[] Lines { get; }

        int GetLineIndex(int index);
        int GetColumnIndex(int index);
        int GetLineStartCharIndex(int lineIndex);
        int GetCharIndex(int lineIndex, int columnIndex);
        int GetColumnCount(int lineIndex);
    }
}
