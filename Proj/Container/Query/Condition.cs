/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Structure;
using Mkamo.Common.Visitor;
using System.Collections.ObjectModel;

namespace Mkamo.Container.Query {

    [Serializable]
    public class Condition {
        public virtual bool IsComposite {
            get { return true; }
        }
    }

    [Serializable]
    public class CompositeCondition: Condition, IComposite<Condition, CompositeCondition>, IVisitable<Condition> {
        // ========================================
        // field
        // ========================================
        private CompositeSupport<Condition, CompositeCondition> _composite;

        // ========================================
        // constructor
        // ========================================
        public CompositeCondition() {
            _composite = new CompositeSupport<Condition, CompositeCondition>(this);
        }

        // ========================================
        // property
        // ========================================
        public CompositeCondition Parent {
            get { return _composite.Parent; }
            set { _composite.Parent = value; }
        }

        public Collection<Condition> Children {
            get { return _composite.Children; }
        }

        public override bool IsComposite {
            get { return false; }
        }

        public void Accept(IVisitor<Condition> visitor) {
            _composite.Accept(visitor);
        }

        public void Accept(IVisitor<Condition> visitor, NextVisitOrder order) {
            _composite.Accept(visitor, order);
        }

        public void Accept(Predicate<Condition> visitPred) {
            _composite.Accept(visitPred);
        }

        public void Accept(Predicate<Condition> visitPred, Action<Condition> endVisitAction, NextVisitOrder order) {
            _composite.Accept(visitPred, endVisitAction, order);
        }
    }

    [Serializable]
    public class Not: CompositeCondition {
    }

    [Serializable]
    public class And: CompositeCondition {
    }

    [Serializable]
    public class Or: CompositeCondition {
    }

    [Serializable]
    public class IdEqual: Condition {
        public string Id;
    }

    [Serializable]
    public class PropertyMatch<TValue>: Condition {
        public string Name;
        public Predicate<TValue> Matcher;
    }

    [Serializable]
    public class PropertyEqual<TValue>: Condition {
        public string Name;
        public TValue Value;
    }

    [Serializable]
    public class PropertyContains<TValue>: Condition {
        public string Name;
        public TValue Value;
    }

    [Serializable]
    public class PropertyGreater<TValue>: Condition {
        public string Name;
        public TValue Value;
        public bool ContainsBoundary;
    }

    [Serializable]
    public class PropertyLess<TValue>: Condition {
        public string Name;
        public TValue Value;
        public bool ContainsBoundary;
    }
}
