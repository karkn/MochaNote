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
using Mkamo.Common.Event;

namespace Mkamo.Common.Command {
    public class CompositeCommand: AbstractCommand, ICommand, IComposite<ICommand, CompositeCommand> {
        // ========================================
        // field
        // ========================================
        private CompositeSupport<ICommand, CompositeCommand> _composite;

        // ========================================
        // constructor
        // ========================================
        public CompositeCommand() {
            _composite = new CompositeSupport<ICommand, CompositeCommand>(this);
            _composite.DetailedPropertyChanged += HandleChildrenChanged;
        }

        // ========================================
        // property
        // ========================================
        // === IComposite ==========
        public CompositeCommand Parent {
            get { return _composite.Parent; }
            set { _composite.Parent = value; }
        }

        public Collection<ICommand> Children {
            get { return _composite.Children; }
        }

        // === ICommand ==========
        public override bool CanExecute {
            get {
                foreach (var child in Children) {
                    if (!child.CanExecute) {
                        return false;
                    }
                }
                return true;
            }
        }

        public override bool CanUndo {
            get {
                foreach (var child in Children) {
                    if (child.CanUndo) {
                        return true;
                    }
                }
                return false;
            }
        }

        // ========================================
        // method
        // ========================================
        // === ICommand ==========
        public override void Execute() {
            foreach (var child in Children) {
                if (child.CanExecute) {
                    child.Execute();
                }
            }
        }

        public override void Undo() {
            for (var i = Children.Count - 1; i >= 0; --i) {
                if (Children[i].CanUndo) {
                    Children[i].Undo();
                }
            }
        }

        public override void Redo() {
            foreach (var child in Children) {
                if (child.CanExecute) {
                    child.Redo();
                }
            }
        }

        public override ICommand Chain(ICommand next) {
            Children.Add(next);
            return this;
        }

        private void HandleChildrenChanged(object sender, DetailedPropertyChangedEventArgs e) {
            if (e.PropertyName == "Children") {
                switch (e.Kind) {
                    case PropertyChangeKind.Add: {
                        var newChild = e.NewValue as AbstractCommand;
                        if (newChild != null && !(newChild is CompositeCommand)) {
                            newChild._Parent = this;
                        }
                        break;
                    }
                    case PropertyChangeKind.Remove: {
                        var oldChild = e.OldValue as AbstractCommand;
                        if (oldChild != null && !(oldChild is CompositeCommand)) {
                            oldChild._Parent = null;
                        }
                        break;
                    }
                    case PropertyChangeKind.Clear: {
                        var oldChildren = e.OldValue as AbstractCommand[];
                        if (oldChildren != null) {
                            foreach (var child in oldChildren) {
                                if (!(child is CompositeCommand)) {
                                    child._Parent = null;
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
}
