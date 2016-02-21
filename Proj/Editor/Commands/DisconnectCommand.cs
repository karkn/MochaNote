/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using Mkamo.Common.Command;

namespace Mkamo.Editor.Commands {
    /// <summary>
    /// ConnectionEditorのConnectingAnchor側をConnectableから切断する．
    /// </summary>
    //public class DisconnectCommand: AbstractCommand {
    //    // ========================================
    //    // field
    //    // ========================================
    //    private IEditor _target;
    //    private IAnchor _connectingAnchor;

    //    // ========================================
    //    // constructor
    //    // ========================================
    //    public DisconnectCommand(IEditor target, IAnchor connectingAnchor) {
    //        _target = target;
    //        _connectingAnchor = connectingAnchor;
    //    }

    //    // ========================================
    //    // property
    //    // ========================================
    //    public override bool CanExecute {
    //        get { return _target != null && _connectingAnchor != null && _connectingAnchor.IsConnected; }
    //    }

    //    public override bool CanUndo {
    //        get { return true; }
    //    }

    //    public IEditor ConnectionEditor {
    //        get { return _target; }
    //    }

    //    public IAnchor ConnectingAnchor {
    //        get { return _connectingAnchor; }
    //    }

    //    // ========================================
    //    // method
    //    // ========================================
    //    public override void Execute() {
    //        /// controllerに切断させる
    //        var oldConnectableEditor = _connectingAnchor.Connectable.GetEditor();
    //        var oldConnectableCtrl = oldConnectableEditor.Controller as IConnectableController;
    //        var connectionCtrl = _target.Controller as IConnectionController;

    //        switch (_connectingAnchor.Kind) {
    //            case ConnectionAnchorKind.Source: {
    //                oldConnectableCtrl.DisconnectOutgoing(_target.Model);
    //                connectionCtrl.DisconnectSource();
    //                break;
    //            }
    //            case ConnectionAnchorKind.Target: {
    //                oldConnectableCtrl.DisconnectIncoming(_target.Model);
    //                connectionCtrl.DisconnectTarget();
    //                break;
    //            }
    //        }

    //        /// figureに切断させる
    //        _connectingAnchor.Disconnect();
    //    }

    //    public override void Undo() {
    //    }
    //}
}
