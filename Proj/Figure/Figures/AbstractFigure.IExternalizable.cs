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
using Mkamo.Common.Core;

namespace Mkamo.Figure.Figures {
    partial class AbstractFigure {
        // ========================================
        // method
        // ========================================
        public virtual void WriteExternal(IMemento memento, ExternalizeContext context) {
            memento.WriteBool("IsVisible", _isVisible);
            memento.WriteExternalizable("Layout", _layout);

            if (_layoutConstraints != null) {
                var constraints = new Dictionary<IMemento, object>();
                foreach (var pair in _layoutConstraints) {
                    var childMem = context.GetMemento("Children", pair.Key);
                    if (childMem != null) {
                        constraints[childMem] = pair.Value;
                    }
                }
                memento.WriteSerializable("LayoutConstraints", constraints);
            }

            var data = new Dictionary<string, object>();
            if (_persistentData != null) {
                foreach (var pair in _persistentData) {
                    data[pair.Key] = pair.Value;
                }
            }
            memento.WriteSerializable("PersistentData", data);

            if (_structure.HasChildren) {
                memento.WriteExternalizables("Children", Children.As<IFigure, object>());
            }
        }

        public virtual void ReadExternal(IMemento memento, ExternalizeContext context) {
            _isVisible = memento.ReadBool("IsVisible");
            _layout = memento.ReadExternalizable("Layout") as ILayout;
            if (_layout != null) {
                _layout.Owner = this;
            }
            if (memento.Contains("LayoutConstraints")) {
                var constraints = memento.ReadSerializable("LayoutConstraints") as Dictionary<IMemento, object>;
                if (constraints.Count > 0) {
                    foreach (var pair in constraints) {
                        var child = context.GetExternalizable("Children", pair.Key) as IFigure;
                        LayoutConstraints[child] = pair.Value;
                    }
                }
            }
            if (memento.Contains("PersistentData")) {
                var data = memento.ReadSerializable("PersistentData") as Dictionary<string, object>;
                if (data.Count > 0) {
                    foreach (var pair in data) {
                        PersistentData[pair.Key] = pair.Value;
                    }
                }
            }

            if (memento.Contains("Children")) {
                var children = memento.ReadExternalizables("Children");
                foreach (var child in children) {
                    var childFig = child as IFigure;
                    if (childFig != null) {
                        Children.Add(childFig);
                    }
                }
            }
        }

    }
}
