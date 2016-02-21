/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;

namespace Mkamo.Figure.Core {
    public class ConnectableChangedEventArgs: EventArgs {
        private ConnectionAnchorKind _kind;
        private IConnectable _oldValue;
        private IConnectable _newValue;

        public ConnectableChangedEventArgs(ConnectionAnchorKind kind, IConnectable oldValue, IConnectable newValue) {
            _kind = kind;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public ConnectionAnchorKind Kind {
            get { return _kind; }
        }
        public IConnectable OldValue {
            get { return _oldValue; }
        }
        public IConnectable NewValue {
            get { return _newValue; }
        }
    }
}
