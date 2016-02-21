/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Editor.Core {
    public interface IConnectionController: IController {
        // ========================================
        // property
        // ========================================
        bool CanDisconnectSource { get; }
        bool CanDisconnectTarget { get; }

        // 今のところ使っていないのでコメントアウト
        //object SourceModel { get; }
        //object TargetModel { get; }

        // ========================================
        // method
        // ========================================
        bool CanConnectSource(object connected);
        bool CanConnectTarget(object connected);

        void ConnectSource(object connected);
        void ConnectTarget(object connected);
        void DisconnectSource();
        void DisconnectTarget();
    }
}
