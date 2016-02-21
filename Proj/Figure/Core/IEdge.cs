/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mkamo.Common.Event;
using System.Drawing.Drawing2D;

namespace Mkamo.Figure.Core {
    /// <summary>
    /// 複数の点をつなぎ合わせた線で表現される図形．
    /// </summary>
    public interface IEdge: IConnection {
        // ========================================
        // event
        // ========================================
        event EventHandler<DetailedPropertyChangedEventArgs> EdgePointsChanged;

        // ========================================
        // property
        // ========================================
        IEnumerable<Point> EdgePoints { get; }
        Point this[int index] { get; set; }
        int EdgePointCount { get; }
        Point First { get; set; }
        Point Last { get; set; }

        IEnumerable<Point> BendPoints { get; }

        EdgePointRef FirstRef { get; }
        EdgePointRef LastRef { get; }
        IEnumerable<EdgePointRef> EdgePointRefs { get; }

        int HitMargin { get; set; }
        Color LineColor { get; set; }
        int LineWidth { get; set; }
        DashStyle LineDashStyle { get; set; }

        IRouter Router { get; set; }
        ConnectionMethodKind ConnectionMethod { get; set; }
        
        object SourceConnectionOption { get; set; }
        object TargetConnectionOption { get; set; }

        EdgeBehaviorOptions EdgeBehaviorOptions { get; set; }

        // ========================================
        // method
        // ========================================
        void AddBendPoint(Point pt);

        /// <summary>
        /// indexはEdgePointでのindex．
        /// </summary>
        void InsertBendPoint(int index, Point pt);

        /// <summary>
        /// indexはEdgePointでのindex．
        /// </summary>
        void RemoveBendPoint(int index);
        void ClearBendPoints();

        void SetEdgePoints(IEnumerable<Point> edgePoints);

        void Route();
        Point GetConnectionPoint(IAnchor anchor, INode node, Point location);
    }

    public static class IEdgeProperty {
        public static readonly string EdgePoints = "EdgePoints";
    }
}
