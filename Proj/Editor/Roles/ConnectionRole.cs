/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Roles.Edge;

namespace Mkamo.Editor.Roles {
    public class ConnectionRole: CompositeRole {
        // ========================================
        // constructor
        // ========================================
        public ConnectionRole() {
            Children.Add(new ConnectRole());
        }
    }
}
