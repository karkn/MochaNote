/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Common.Structure;
using Mkamo.Common.Visitor;
using Mkamo.Figure.Core;
using Mkamo.Common.Command;
using System.Collections.ObjectModel;

namespace Mkamo.Editor.Roles {
    public class CompositeRole: AbstractRole, IComposite<IRole, CompositeRole> {
        // ========================================
        // field
        // ========================================
        private CompositeSupport<IRole, CompositeRole> _composite;

        // ========================================
        // constructor
        // ========================================
        public CompositeRole(): base() {
            _composite = new CompositeSupport<IRole, CompositeRole>(this);
        }

        // ========================================
        // property
        // ========================================
        // === IComposite ==========
        public CompositeRole Parent {
            get { return _composite.Parent; }
            set { _composite.Parent = value; }
        }

        public Collection<IRole> Children {
            get { return _composite.Children; }
        }

        // ========================================
        // method
        // ========================================
        // === IRole ==========
        public override void Installed(IEditor owner) {
            base.Installed(owner);
            foreach (var child in Children) {
                child.Installed(owner);
            }
        }

        public override void Uninstalled(IEditor owner) {
            base.Uninstalled(owner);
            foreach (var child in Children) {
                child.Uninstalled(owner);
            }
        }

        public override bool CanUnderstand(IRequest request) {
            foreach (var child in Children) {
                if (child.CanUnderstand(request)) {
                    return true;
                }
            }
            return false;
        }

        public override ICommand CreateCommand(IRequest request) {
            foreach (var child in Children) {
                if (child.CanUnderstand(request)) {
                    return child.CreateCommand(request);
                }
            }
            return null;
        }

        public override IFigure CreateFeedback(IRequest request) {
            foreach (var child in Children) {
                if (child.CanUnderstand(request)) {
                    return child.CreateFeedback(request);
                }
            }
            return null;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
            foreach (var child in Children) {
                if (child.CanUnderstand(request)) {
                    child.UpdateFeedback(request, feedback);
                    return;
                }
            }
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {
            foreach (var child in Children) {
                if (child.CanUnderstand(request)) {
                    child.DisposeFeedback(request, feedback);
                    return;
                }
            }
        }
    }
}
