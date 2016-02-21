/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Common.Event;
using System.Drawing;
using Mkamo.Common.Association;
using Mkamo.Common.Externalize;

namespace Mkamo.Figure.Figures {
    public abstract class AbstractConnection: AbstractWrappingFigure, IConnection {
        // ========================================
        // field
        // ========================================
        private IConnectable _source;
        private IConnectable _target;

        // ========================================
        // constructor
        // ========================================
        public AbstractConnection(): base() {
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<ConnectableChangedEventArgs> ConnectableChanged;

        // ========================================
        // property
        // ========================================
        public virtual IConnectable Source {
            get { return _source; }
            set {
                if (_source == value) {
                    return;
                }

                var old = _source;

                AssociationUtil.EnsureAssociation(
                    _source,
                    value,
                    node => _source = node,
                    node => node.Outgoings.Add(this),
                    node => node.Outgoings.Remove(this)
                );

                _source = value;

                OnConnectableChanged(
                    new ConnectableChangedEventArgs(ConnectionAnchorKind.Source, old, value)
                );
            }
        }

        public virtual IConnectable Target {
            get { return _target; }
            set {
                if (_target == value) {
                    return;
                }

                var old = _target;

                AssociationUtil.EnsureAssociation(
                    _target,
                    value,
                    node => _target = node,
                    node => node.Incomings.Add(this),
                    node => node.Incomings.Remove(this)
                );

                OnConnectableChanged(
                    new ConnectableChangedEventArgs(ConnectionAnchorKind.Target, old, value)
                );
            }
        }

        public virtual bool IsSourceConnected {
            get { return _source != null; }
        }

        public virtual bool IsTargetConnected {
            get { return _target != null; }
        }

        public virtual IAnchor SourceAnchor {
            get { return _SourceAnchor; }
        }

        public virtual IAnchor TargetAnchor {
            get { return _TargetAnchor; }
        }

        public virtual Point SourcePoint {
            get { return _SourcePoint; }
            set { _SourcePoint = value; }
        }

        public virtual Point TargetPoint {
            get { return _TargetPoint; }
            set { _TargetPoint = value; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected abstract ConnectionAnchor _SourceAnchor { get; }
        protected abstract ConnectionAnchor _TargetAnchor { get; }

        protected abstract Point _SourcePoint { get; set; }
        protected abstract Point _TargetPoint { get; set; }
        
        // ========================================
        // method
        // ========================================
        // === IPersistable ==========
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);

            memento.WriteExternalizable("Source", _source);
            memento.WriteExternalizable("Target", _target);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);

            Source = memento.ReadExternalizable("Source") as IConnectable;
            Target = memento.ReadExternalizable("Target") as IConnectable;
        }

        // === IFigure ===
        protected override IEnumerable<string> GetNonCompositeExternalizableKeys() {
            /// Source/Targetは所有するものではないので除く
            return new[] { "Source", "Target" };
        }

        // ------------------------------
        // protected
        // ------------------------------
        // === AbstractConnection ==========
        // --- event ---
        protected virtual void OnConnectableChanged(ConnectableChangedEventArgs e) {
            var handler = ConnectableChanged;
            if (handler != null) {
                handler(this, e);
            }
        }

        // ========================================
        // class
        // ========================================
        [Serializable]
        protected class ConnectionAnchor: IAnchor {
            // ========================================
            // field
            // ========================================
            [NonSerialized]
            private AbstractConnection _owner;

            private ConnectionAnchorKind _kind;
    
            [NonSerialized]
            private object _hint;
    
            // ========================================
            // constructor
            // ========================================
            public ConnectionAnchor(AbstractConnection owner, ConnectionAnchorKind kind) {
                _owner = owner;
                _kind = kind;
            }

            // ========================================
            // event
            // ========================================
            [field:NonSerialized]
            public event EventHandler<AnchorMovedEventArgs> AnchorMoved;

            // ========================================
            // property
            // ========================================
            public IConnection Owner {
                get { return _owner; }
            }
    
            public ConnectionAnchorKind Kind {
                get { return _kind; }
            }
    
            public IConnectable Connectable {
                get {
                    return _kind == ConnectionAnchorKind.Source?
                        _owner.Source:
                        _owner.Target;
                }
            }

            public virtual Point Location {
                get { return Kind == ConnectionAnchorKind.Source? _owner._SourcePoint: _owner._TargetPoint; }
                set {
                    if (Kind == ConnectionAnchorKind.Source) {
                        if (value == _owner._SourcePoint) {
                            return;
                        }
                        _owner._SourcePoint = value;
                    } else {
                        if (value == _owner._TargetPoint) {
                            return;
                        }
                        _owner._TargetPoint = value;
                    }
                }
            }
    
            public bool IsConnected {
                get {
                    return _kind == ConnectionAnchorKind.Source?
                        _owner.IsSourceConnected:
                        _owner.IsTargetConnected;
                }
            }

            public object Hint {
                get { return _hint; }
                set { _hint = value; }
            }

            // ========================================
            // method
            // ========================================
            public void Connect(IConnectable connectable) {
                if (_kind == ConnectionAnchorKind.Source) {
                    _owner.Source = connectable;
                } else {
                    _owner.Target = connectable;
                }
            }

            public void Disconnect() {
                if (_kind == ConnectionAnchorKind.Source) {
                    _owner.Source = null;
                } else {
                    _owner.Target = null;
                }
            }

            // ------------------------------
            // protected
            // ------------------------------
            protected internal virtual void OnAnchorMoved(Point oldLocation, Point newLocation) {
                var handler = AnchorMoved;
                if (handler != null) {
                    handler(this, new AnchorMovedEventArgs(_kind, oldLocation, newLocation));
                }
            }

        }

    }
}
