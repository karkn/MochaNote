/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Mkamo.Common.Event;
using Mkamo.Common.Visitor;
using Mkamo.Common.Collection;
using Mkamo.Common.Association;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Mkamo.Common.Structure {
    [Serializable]
    [DataContract]
    public class CompositeSupport<TComponent, TComposite>:
        IComposite<TComponent, TComposite>,
        IVisitable<TComponent>,
        IDetailedNotifyPropertyChanged,
        IDetailedNotifyPropertyChanging
        where TComponent: class
        where TComposite: class, TComponent, IComposite<TComponent, TComposite> {

        // ========================================
        // static field
        // ========================================
        private static TComponent[] EmptyChildren = new TComponent[0];

        // ========================================
        // field
        // ========================================
        [DataMember]
        private TComposite _self;
        [DataMember]
        private TComposite _parent;
        [DataMember]
        private AssociationCollection<TComponent> _children; /// lazy

        [DataMember]
        private VisitableSupport<TComponent> _visitable; /// lazy

        [NonSerialized]
        private DetailedNotifyPropertyChange _notifier = new DetailedNotifyPropertyChange();

        // ========================================
        // constructor
        // ========================================
        public CompositeSupport(TComposite self) {
            _self = self;
        }

        [OnDeserialized]
        internal void InitDeserializedCompositeSupport(StreamingContext context) {
            _notifier = new DetailedNotifyPropertyChange();
            if (_children != null) {
                _children.DetailedPropertyChanging += (sender, e) => _notifier.NotifyPropertyChanging(sender, e);
                _children.DetailedPropertyChanged += (sender, e) => _notifier.NotifyPropertyChanged(sender, e);
            }
        }

        // ========================================
        // event
        // ========================================
        public event PropertyChangingEventHandler PropertyChanging {
            add { _notifier.PropertyChanging += value;}
            remove { _notifier.PropertyChanging -= value;}
        }

        public event PropertyChangedEventHandler PropertyChanged {
            add { _notifier.PropertyChanged += value;}
            remove { _notifier.PropertyChanged -= value;}
        }

        public virtual event EventHandler<DetailedPropertyChangingEventArgs> DetailedPropertyChanging {
            add { _notifier.DetailedPropertyChanging += value; }
            remove { _notifier.DetailedPropertyChanging -= value; }
        }

        public virtual event EventHandler<DetailedPropertyChangedEventArgs> DetailedPropertyChanged {
            add { _notifier.DetailedPropertyChanged += value; }
            remove { _notifier.DetailedPropertyChanged -= value; }
        }

        // ========================================
        // property
        // ========================================
        public virtual TComposite Parent {
            get { return _parent; }
            set {
                TComponent oldParent = _parent;

                _notifier.NotifyPropertySetting(_self, ICompositeProperty.Parent, oldParent, value);

                AssociationUtil.EnsureResult result = AssociationUtil.EnsureAssociation(
                    _parent,
                    value,
                    parent => _parent = parent,
                    composite => composite.Children.Add(_self),
                    composite => composite.Children.Remove(_self)
                );
                if (result != AssociationUtil.EnsureResult.None) {
                    _notifier.NotifyPropertySet(_self, ICompositeProperty.Parent, oldParent, value);
                }
            }
        }

        public virtual Collection<TComponent> Children {
            get { return _Children; }
        }

        public virtual bool HasChildren {
            get { return _children != null && _children.Count > 0; }
        }

        public virtual IQualifiedEventHandlers<DetailedPropertyChangingEventArgs> NamedPropertyChanging {
            get { return _notifier.NamedPropertyChanging; }
        }

        public virtual IQualifiedEventHandlers<DetailedPropertyChangedEventArgs> NamedPropertyChanged {
            get { return _notifier.NamedPropertyChanged; }
        }


        public virtual bool HasNextSibling {
            get {
                if (Parent == null) {
                    return false;
                }
                return Parent.Children.IndexOf(_self) < Parent.Children.Count - 1;
            }
        }

        public virtual bool HasPreviousSibling {
            get {
                if (Parent == null) {
                    return false;
                }
                return Parent.Children.IndexOf(_self) > 0;
            }
        }

        public virtual TComponent NextSibling {
            get {
                if (Parent == null) {
                    return null;
                }
                int index = Parent.Children.IndexOf(_self);
                return index < Parent.Children.Count - 1? Parent.Children[index + 1]: null;
            }
        }

        public virtual TComponent PreviousSibling {
            get {
                if (Parent == null) {
                    return null;
                }
                int index = Parent.Children.IndexOf(_self);
                return index > 0? Parent.Children[index - 1]: null;
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected VisitableSupport<TComponent> _Visitable {
            get {
                if (_visitable == null) {
                    if (_children == null) {
                        _visitable = new VisitableSupport<TComponent>(_self, EmptyChildren);
                    } else {
                        _visitable = new VisitableSupport<TComponent>(_self, _Children);
                    }
                    _visitable = new VisitableSupport<TComponent>(
                        _self,
                        compo => _children == null ? (IEnumerable<TComponent>) EmptyChildren : _Children
                    );
                }
                return _visitable;
            }
        }

        protected AssociationCollection<TComponent> _Children {
            get {
                if (_children == null) {
                    _children = new AssociationCollection<TComponent>(
                        component => {
                            var composite = component as TComposite;
                            if (composite != null) {
                                composite.Parent = _self;
                            }
                        },
                        component => {
                            var composite = component as TComposite;
                            if (composite != null) {
                                composite.Parent = null;
                            }
                        }
                    );
                    _children.EventSender = _self;
                    _children.EventPropertyName = ICompositeProperty.Children;
        
                    _children.DetailedPropertyChanging += (sender, e) => _notifier.NotifyPropertyChanging(sender, e);
                    _children.DetailedPropertyChanged += (sender, e) => _notifier.NotifyPropertyChanged(sender, e);
                }
                return _children;
            }
        }

        // ========================================
        // method
        // ========================================
        // === IAcceptable ==========
        public virtual void Accept(IVisitor<TComponent> visitor) {
            _Visitable.Accept(visitor);
        }

        public virtual void Accept(IVisitor<TComponent> visitor, NextVisitOrder order) {
            _Visitable.Accept(visitor, order);
        }

        public virtual void Accept(Predicate<TComponent> visitPred) {
            _Visitable.Accept(visitPred);
        }

        public virtual void Accept(
            Predicate<TComponent> visitPred,
            Action<TComponent> endVisitAction,
            NextVisitOrder order
        ) {
            _Visitable.Accept(visitPred, endVisitAction, order);
        }

    }


}
