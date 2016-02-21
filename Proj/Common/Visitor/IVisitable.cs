/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Common.Visitor {
    public interface IVisitable<TElem> {
        void Accept(IVisitor<TElem> visitor);
        void Accept(IVisitor<TElem> visitor, NextVisitOrder order);
        void Accept(Predicate<TElem> visitPred);
        void Accept(Predicate<TElem> visitPred, Action<TElem> endVisitAction, NextVisitOrder order);
    }

    /// <summary>
    /// 要素を走査する順番．
    /// </summary>
    [Serializable]
    public enum NextVisitOrder {
        /// <summary>
        /// 正順での走査
        /// </summary>
        PositiveOrder,
        /// <summary>
        /// 逆順での走査
        /// </summary>
        NegativeOrder,
    }
}
