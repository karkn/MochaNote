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
    /// 二つのIConnectableを接続する図形．
    /// </summary>
    public interface IConnection: IFigure {
        // ========================================
        // event
        // ========================================
        event EventHandler<ConnectableChangedEventArgs> ConnectableChanged;

        // ========================================
        // property
        // ========================================
        IConnectable Source { get; set; }
        IConnectable Target { get; set; }
        bool IsSourceConnected { get; }
        bool IsTargetConnected { get; }

        /// <summary>
        /// SourcePoint経由で始点を設定した場合，Route()は呼ばれない
        /// </summary>
        Point SourcePoint { get; set; }
        /// <summary>
        /// TargetPoint経由で始点を設定した場合，Route()は呼ばれない
        /// </summary>
        Point TargetPoint { get; set; }

        /// <summary>
        /// SourceAnchor.Location経由で始点を設定した場合，Route()が呼ばれる
        /// </summary>
        IAnchor SourceAnchor { get; }
        /// <summary>
        /// TargetAnchor.Location経由で始点を設定した場合，Route()が呼ばれる
        /// </summary>
        IAnchor TargetAnchor { get; }
    }

    public static class IConnectionProperty {
        public static readonly string Source = "Source";
        public static readonly string Target = "Target";
    }
}
