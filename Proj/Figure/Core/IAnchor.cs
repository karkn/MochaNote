/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mkamo.Figure.Core {
    /// <summary>
    /// IConnectableとIConnectionの接点を表す．
    /// </summary>
    public interface IAnchor {
        // ========================================
        // event
        // ========================================
        event EventHandler<AnchorMovedEventArgs> AnchorMoved;

        // ========================================
        // property
        // ========================================
        IConnection Owner { get; }
        ConnectionAnchorKind Kind { get; }

        IConnectable Connectable { get; }

        Point Location { get; set; }
        bool IsConnected { get; }

        /// <summary>
        /// 接続点に関する補足情報．
        /// 例えば接続されたのがIConnectableの上下左右のどの辺上にあるか，など
        /// </summary>
        //object Hint { get; set; }

        // ========================================
        // method
        // ========================================
        void Connect(IConnectable connectable);
        void Disconnect();
    }

    [Serializable]
    public enum ConnectionAnchorKind {
        Source,
        Target,
    }
}
