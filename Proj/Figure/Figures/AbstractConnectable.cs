/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Externalize;
using Mkamo.Figure.Core;
using Mkamo.Common.Event;
using Mkamo.Common.Association;
using System.Collections.ObjectModel;
using System.Drawing;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Core;
using Mkamo.Common.DataType;

namespace Mkamo.Figure.Figures {
    public abstract class AbstractConnectable: AbstractBoundingFigure, IConnectable {
        // ========================================
        // field
        // ========================================
        private AssociationCollection<IConnection> _outgoings;
        private AssociationCollection<IConnection> _incomings;

        // ========================================
        // constructor
        // ========================================
        public AbstractConnectable(): base() {
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<DetailedPropertyChangedEventArgs> OutgoingsChanged;
        public event EventHandler<DetailedPropertyChangedEventArgs> IncomingsChanged;

        // ========================================
        // property
        // ========================================
        // === IConnectable ==========
        public virtual Collection<IConnection> Outgoings {
            get { return _Outgoings; }
        }

        public virtual Collection<IConnection> Incomings {
            get { return _Incomings; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual Collection<IConnection> _Outgoings {
            get {
                if (_outgoings == null) {
                    _outgoings = new AssociationCollection<IConnection>(
                        edge => edge.Source = this,
                        edge => edge.Source = null
                    );
                    _outgoings.EventSender = this;
                    _outgoings.EventPropertyName = IConnectableProperty.Outgoings;
                    _outgoings.DetailedPropertyChanged += (sender, e) => OnOutgoingsChanged(e);
                }
                return _outgoings;
            }
        }

        protected virtual Collection<IConnection> _Incomings {
            get {
                if (_incomings == null) {
                    _incomings = new AssociationCollection<IConnection>(
                        edge => edge.Target = this,
                        edge => edge.Target = null
                    );
                    _incomings.EventSender = this;
                    _incomings.EventPropertyName = IConnectableProperty.Incomings;
                    _incomings.DetailedPropertyChanged += (sender, e) => OnIncomingsChanged(e);
                }
                return _incomings;
            }
        }

        // ========================================
        // method
        // ========================================
        // === IPersistable ==========
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);


            if (_outgoings != null) {
                memento.WriteExternalizables("Outgoings", _outgoings.As<IConnection, object>());
            }
            if (_incomings != null) {
                memento.WriteExternalizables("Incomings", _incomings.As<IConnection, object>());
            }
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);

            if (memento.Contains("Outgoings")) {
                foreach (var ex in memento.ReadExternalizables("Outgoings")) {
                    var edge = ex as IConnection;
                    if (edge != null) {
                        _Outgoings.Add(edge);
                    }
                }
            }

            if (memento.Contains("Incomings")) {
                foreach (var ex in memento.ReadExternalizables("Incomings")) {
                    var edge = ex as IConnection;
                    if (edge != null) {
                        _Incomings.Add(edge);
                    }
                }
            }
        }

        // === IFigure ==========
        //public override IFigure CloneFigure() {
        //    /// Outgoings/Incomingsは所有するものではないので除く
        //    return CloneFigure((key, ext) => key != "Outgoings" && key != "Incomings");
        //}

        protected override IEnumerable<string> GetNonCompositeExternalizableKeys() {
            /// Outgoings/Incomingsは所有するものではないので除く
            return new[] { "Outgoings", "Incomings" };
        }

        // ------------------------------
        // protected
        // ------------------------------
        // === AbstractFigure ==========
        protected override void OnBoundsChanged(Rectangle oldBounds, Rectangle newBounds, IEnumerable<IFigure> movingFigures) {
            foreach (var outgoing in Outgoings) {
                //outgoing.SourceAnchor.Location =
                outgoing.SourcePoint =
                    GetExpectedConnectLocationForConnectedAnchor(outgoing.SourceAnchor, oldBounds, newBounds);
            }

            foreach (var incoming in Incomings) {
                //incoming.TargetAnchor.Location =
                incoming.TargetPoint =
                    GetExpectedConnectLocationForConnectedAnchor(incoming.TargetAnchor, oldBounds, newBounds);
            }

            base.OnBoundsChanged(oldBounds, newBounds, movingFigures);
        }

        // === INode ==========
        protected virtual void OnOutgoingsChanged(DetailedPropertyChangedEventArgs e) {
            var handler = OutgoingsChanged;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnIncomingsChanged(DetailedPropertyChangedEventArgs e) {
            var handler = IncomingsChanged;
            if (handler != null) {
                handler(this, e);
            }
        }

        // === AbstractConnectableNode ==========
        protected virtual Point GetExpectedConnectLocationForConnectedAnchor(
            IAnchor anchor, Rectangle oldBounds, Rectangle newBounds
        ) {
            var locDelta = newBounds.Location - (Size) oldBounds.Location;
            
            var newAnchorX = anchor.Location.X + locDelta.X;
            newAnchorX = Math.Min(newAnchorX, newBounds.Right - 1);

            var newAnchorY = anchor.Location.Y + locDelta.Y;
            newAnchorY = Math.Min(newAnchorY, newBounds.Bottom - 1);

            var dir = RectUtil.GetNearestLineDirection(oldBounds, anchor.Location);

            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.Left)) {
                return new Point(newBounds.Left, newAnchorY);
            }
            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.Right)) {
                return new Point(newBounds.Right - 1, newAnchorY);
            }
            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.Up)) {
                return new Point(newAnchorX, newBounds.Top);
            }
            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.Down)) {
                return new Point(newAnchorX, newBounds.Bottom - 1);
            }
            
            throw new ArgumentException();
        }
    }
}
