/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Common.Visitor {
    public class DelegatingVisitor<TElem>: IVisitor<TElem> {
        // ========================================
        // field
        // ========================================
        private Predicate<TElem> _visitPred;
        private Action<TElem> _endVisitAction;

        // ========================================
        // constructor
        // ========================================
        public DelegatingVisitor(
            Predicate<TElem> visitPred,
            Action<TElem> endVisitAction
        ) {
            _visitPred = visitPred;
            _endVisitAction = endVisitAction;
        }

        public DelegatingVisitor(Predicate<TElem> visitorPred) {
            _visitPred = visitorPred;
        }

        // ========================================
        // method
        // ========================================
        public bool Visit(TElem elem) {
            return (_visitPred == null? false: _visitPred(elem));
        }

        public void EndVisit(TElem elem) {
            if (_endVisitAction != null) {
                _endVisitAction(elem);
            }
        }
    }
}
