/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Common.Visitor {
    public interface IVisitor<TElem> {
        /// <summary>
        /// 子要素を走査する前に呼ばれる．
        /// 戻り値がtrueならば走査を中止，falseならば継続する
        /// </summary>
        bool Visit(TElem elem);


        /// <summary>
        /// 子要素を走査した後に呼ばれる．
        /// </summary>
        void EndVisit(TElem elem);
    }
}
