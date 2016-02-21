/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Figure.Core {
    [Serializable]
    public enum AutoAdjustExceptionKind {
        None,
        ExceptThis,
        ExceptThisAndChildren,
    }

    /// <summary>
    /// AbstractEdgeが接続点を算出する方法．
    /// </summary>
    [Serializable]
    public enum ConnectionMethodKind {
        Intersect,
        Nearest,
        Center,
        Comment,
        SideMidpointOfOpposite,
        UpperSideMidpoint,
        LowerSideMidpoint,
        LeftSideMidpoint,
        RightSideMidpoint,
        SideMidpointOfNearest,
    }

    /// <summary>
    /// ハイライトする範囲。
    /// </summary>
    [Serializable]
    public enum HighlightRange {
        Keyword, /// マッチしたキーワードのみ
        //LineContainsKeyword, /// 行全体
        //LineAfterKeyword, /// マッチ個所以降行末まで
        //EncloseSingleline, /// 開始～終了，1行内
        //EncloseMultiline, /// 開始～終了，複数行
    }

    [Serializable]
    public class EdgeBehaviorOptions {
        private bool _routeOnMoved = false;
        private bool _routeOnNodeMaxSizeChanged = false;

        public bool RouteOnMoved {
            get { return _routeOnMoved; }
            set { _routeOnMoved = value; }
        }

        public bool RouteOnNodeMaxSizeChanged {
            get { return _routeOnNodeMaxSizeChanged; }
            set { _routeOnNodeMaxSizeChanged = value; }
        }
    }
}
