/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Mkamo.Common.Diagnostics;

namespace Mkamo.Common.Visitor {
    [Serializable]
    public class VisitableSupport<TElem>: IVisitable<TElem> {
        // ========================================
        // field
        // ========================================
        private TElem _self;

        private IEnumerable<TElem> _nextToVisit;
        private Func<TElem, IEnumerable<TElem>> _nextProvider;

        [NonSerialized]
        private bool _isAccepting;

        // ========================================
        // constructor
        // ========================================
        public VisitableSupport(TElem self, IEnumerable<TElem> nextToVisit) {
            Contract.Requires(self != null);
            Contract.Requires(nextToVisit != null);

            _self = self;
            _nextToVisit = nextToVisit;
            _nextProvider = ProvideNext;

            _isAccepting = false;
        }

        public VisitableSupport(TElem self, Func<TElem, IEnumerable<TElem>> nextProvider) {
            Contract.Requires(self != null);
            Contract.Requires(nextProvider != null);

            _self = self;
            _nextToVisit = null;
            _nextProvider = nextProvider;

            _isAccepting = false;
        }

        // ========================================
        // property
        // ========================================
        public bool IsAccepting {
            get { return _isAccepting; }
        }

        // ========================================
        // method
        // ========================================
        // === IVisitable ===
        public virtual void Accept(IVisitor<TElem> visitor, NextVisitOrder order) {
            Contract.Requires(visitor != null);

            _isAccepting = true;
            var next = _nextProvider(_self);
            try {
                if (!visitor.Visit(_self)) {
                    switch (order) {
                        case NextVisitOrder.PositiveOrder: {
                            foreach (var elem in next) {
                                VisitNext(elem, visitor, order);
                            }
                            break;
                        }
                        case NextVisitOrder.NegativeOrder: {
                            foreach (var elem in next.Reverse()) {
                                VisitNext(elem, visitor, order);
                            }
                            break;
                        }
                    }
                }
                visitor.EndVisit(_self);
            } finally {
                _isAccepting = false;
            }
        }

        public virtual void Accept(IVisitor<TElem> visitor) {
            Accept(visitor, NextVisitOrder.PositiveOrder);
        }

        public virtual void Accept(Predicate<TElem> visitPred) {
            Accept(new DelegatingVisitor<TElem>(visitPred));
        }

        public virtual void Accept(
            Predicate<TElem> visitPred,
            Action<TElem> endVisitAction,
            NextVisitOrder order
        ) {
            Accept(new DelegatingVisitor<TElem>(visitPred, endVisitAction), order);
        }

        // --- private ---
        private void VisitNext(TElem next, IVisitor<TElem> visitor, NextVisitOrder order) {
            var vnext = next as IVisitable<TElem>;
            if (vnext != null) {
                vnext.Accept(visitor, order);
            } else {
                visitor.Visit(next);
                visitor.EndVisit(next);
            }
        }

        private IEnumerable<TElem> ProvideNext(TElem elem) {
            return _nextToVisit;
        }

    }
}
