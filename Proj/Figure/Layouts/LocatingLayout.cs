/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using System.Drawing;
using Mkamo.Common.Externalize;
using Mkamo.Figure.Layouts.Locators;
using Mkamo.Common.DataType;

namespace Mkamo.Figure.Layouts {
    [Externalizable]
    public class LocatingLayout: AbstractLayout {
        // ========================================
        // field
        // ========================================
        private Dictionary<IFigure, Tuple<object[], ILocator>> _locatorCache;

        // ========================================
        // constructor
        // ========================================
        public LocatingLayout() {
            _locatorCache = new Dictionary<IFigure, Tuple<object[], ILocator>>();
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override void Arrange(IFigure parent) {
            foreach (var child in parent.Children) {
                var locator = GetLocator(child);
                if (locator != null) {
                    child.Size = child.PreferredSize;
                    locator.Relocate(child, parent);
                }
            }
        }

        public override Size Measure(IFigure parent, SizeConstraint constraint) {
            if (parent.Children.Any()) {
                var rects = parent.Children.Select(fig => fig.Bounds);
                return rects.Aggregate((unionized, r) => Rectangle.Union(unionized, r)).Size;
            } else {
                return Size.Empty;
            }
        }

        protected virtual ILocator GetLocator(IFigure child) {
            var constraint = _Constraints.ContainsKey(child) ? _Constraints[child] as object[]: null;
            if (constraint == null) {
                return null;
            }

            if (_locatorCache.ContainsKey(child)) {
                var tuple = _locatorCache[child];
                if (constraint.SequenceEqual(tuple.Item1)) {
                    return tuple.Item2;
                }
            }

            var locator = LocatorFactory.Instance.GetLocator(constraint);
            _locatorCache.Add(child, Tuple.Create(constraint, locator));
            return locator;
        }

        // ========================================
        // class
        // ========================================
        private class LocatorFactory {
            // ========================================
            // static field
            // ========================================
            public static LocatorFactory Instance = new LocatorFactory();

            // ========================================
            // method
            // ========================================
            public ILocator GetLocator(object[] constraint) {
                if (constraint == null || constraint.Length < 1) {
                    return null;
                }

                var kind = (string) constraint[0];
                switch (kind) {
                    case "EdgeFirstDistance": {
                        var boundsDist = (int) constraint[1];
                        var isDist = (int) constraint[2];
                        var dir = (LocateDirectionKind) constraint[3];
                        return new EdgeFirstDistanceLocator(boundsDist, isDist, dir);
                    }
                    case "EdgeLastDistance": {
                        var boundsDist = (int) constraint[1];
                        var isDist = (int) constraint[2];
                        var dir = (LocateDirectionKind) constraint[3];
                        return new EdgeLastDistanceLocator(boundsDist, isDist, dir);
                    }
                }

                return null;
            }
        }
    }
}
