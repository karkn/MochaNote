/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Structure;
using Mkamo.Editor.Core;
using Mkamo.Common.Visitor;
using Mkamo.Figure.Core;
using Mkamo.Editor.Internal.Core;
using Mkamo.Common.Event;
using System.Collections.ObjectModel;
using Mkamo.StyledText.Core;
using System.Windows.Forms;
using Mkamo.Figure.Figures;

namespace Mkamo.Editor.Handles {
    public class CompositeHandle: AbstractAuxiliaryHandle, IComposite<IAuxiliaryHandle, CompositeHandle> { 
        // ========================================
        // field
        // ========================================
        private CompositeSupport<IAuxiliaryHandle, CompositeHandle> _composite;
        private FigureGroup _figure;

        // ========================================
        // constructor
        // ========================================
        public CompositeHandle(bool hideOnFocus): base(hideOnFocus) {
            _composite = new CompositeSupport<IAuxiliaryHandle, CompositeHandle>(this);
            _composite.DetailedPropertyChanged += (sender, e) => {
                if (e.PropertyName == ICompositeProperty.Children) {
                    OnChildrenChanged(e);
                }
            };

            _figure = new FigureGroup();
            _figure.VisibleChanged += (sender, e) => {
                foreach (var child in Children) {
                    child.Figure.IsVisible = _figure.IsVisible;
                }
            };
        }

        public CompositeHandle(): this(true) {
        }

        // ========================================
        // property
        // ========================================
        // === IComposite ==========
        public CompositeHandle Parent {
            get { return _composite.Parent; }
            set { _composite.Parent = value; }
        }

        public Collection<IAuxiliaryHandle> Children {
            get { return _composite.Children; }
        }

        // === IHandle ==========
        public override IFigure Figure {
            get { return _figure; }
        }

        public override Cursor Cursor {
            get { return base.Cursor; }
            set {
                base.Cursor = value;
                foreach (var child in Children) {
                    child.Cursor = Cursor;
                }
            }
        }

        // ========================================
        // method
        // ========================================
        // === IHandle ==========
        public override void Install(IEditor host) {
            base.Install(host);

            foreach (var child in Children) {
                child.Install(host);
            }
        }

        public override void Uninstall(IEditor host) {
            foreach (var child in Children) {
                child.Uninstall(host);
            }

            base.Uninstall(host);
        }

        public override void Relocate(IFigure hostFigure) {
            foreach (var child in Children) {
                child.Relocate(hostFigure);
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        // --- event ---
        protected virtual void OnChildrenChanged(DetailedPropertyChangedEventArgs e) {
            switch (e.Kind) {
                case PropertyChangeKind.Add: {
                    var newHandle = e.NewValue as IAuxiliaryHandle;
                    newHandle.Figure.IsVisible = _figure.IsVisible;
                    newHandle.Figure.ForwardMouseEvents(Figure);
                    _figure.Children.Insert(e.Index, newHandle.Figure);
                    break;
                }
                case PropertyChangeKind.Remove: {
                    var oldHandle = e.OldValue as IAuxiliaryHandle;
                    Figure.Children.Remove(oldHandle.Figure);
                    oldHandle.Figure.StopForwardMouseEvents(Figure);
                    break;
                }
                case PropertyChangeKind.Clear: {
                    var oldHandles = e.OldValue as IAuxiliaryHandle[];
                    foreach (var handle in oldHandles) {
                        handle.Figure.StopForwardMouseEvents(Figure);
                    }
                    _figure.Children.Clear();
                    break;
                }
                case PropertyChangeKind.Set: {
                    var oldHandle = e.OldValue as IAuxiliaryHandle;
                    var newHandle = e.NewValue as IAuxiliaryHandle;

                    oldHandle.Figure.StopForwardMouseEvents(Figure);

                    newHandle.Figure.IsVisible = Figure.IsVisible;
                    newHandle.Figure.ForwardMouseEvents(Figure);

                    _figure.Children[e.Index] = newHandle.Figure;
                    break;
                }
            }
        }
    }
}
