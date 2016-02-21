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
using System.Drawing;
using Mkamo.Editor.Requests;
#if false
namespace Mkamo.Memopad.Internal.Requests {
    /// <summary>
    /// disconnectするときはNewConnectableEditorにnullを設定する．
    /// </summary>
    public class ConnectCommentRequest: AbstractRequest {
        // ========================================
        // field
        // ========================================
        private IAnchor _connectingAnchor;
        private IEditor _newConnectableEditor;
        private Point _location;

        // ========================================
        // property
        // ========================================
        public override string Id {
            get { return RequestIds.Connect; }
        }

        public IAnchor ConnectingAnchor {
            get { return _connectingAnchor; }
            set { _connectingAnchor = value; }
        }

        public IEditor NewConnectableEditor {
            get { return _newConnectableEditor; }
            set { _newConnectableEditor = value; }
        }

        public Point NewLocation {
            get { return _location; }
            set { _location = value; }
        }

        public bool IsDisconnect {
            get { return _newConnectableEditor == null; }
        }

    }
}

#endif
