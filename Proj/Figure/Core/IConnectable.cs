/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Event;
using System.Drawing;
using System.Collections.ObjectModel;

namespace Mkamo.Figure.Core {
    /// <summary>
    /// IConnectionが接続可能な図形．
    /// </summary>
    public interface IConnectable: IFigure {
        // ========================================
        // event
        // ========================================
        event EventHandler<DetailedPropertyChangedEventArgs> OutgoingsChanged;
        event EventHandler<DetailedPropertyChangedEventArgs> IncomingsChanged;

        // ========================================
        // property
        // ========================================
        Collection<IConnection> Outgoings { get; }
        Collection<IConnection> Incomings { get; }

        // ========================================
        // method
        // ========================================
        /// <summary>
        /// Bounds内の点pointに対して，このIConnectableに接続したときの接続点を返す．
        /// </summary>
        //Point GetExpectedConnectLocation(Point point, Point nextPoint);

        /// <summary>
        /// 現在接続されているIConnectionのanchorに対して，
        /// このIConnectableのBoundsをnewBoundsにしたときの接続点を返す．
        /// </summary>
        //Point GetExpectedConnectLocationForConnectedAnchor(
        //    IConnectionAnchor anchor, Rectangle oldBounds, Rectangle newBounds
        //);
    }

    public static class IConnectableProperty {
        public static readonly string Outgoings = "Outgoings";
        public static readonly string Incomings = "Incomings";
    }
}
