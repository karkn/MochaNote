/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Requests;
using Mkamo.Figure.Core;
using Mkamo.Figure.Figures;
using System.Drawing;
using Mkamo.Editor.Roles.Container;

namespace Mkamo.Editor.Roles {
    public class ContainerRole: CompositeRole{
        // ========================================
        // field
        // ========================================
        private PasteRole _pasteRole;

        // ========================================
        // constructor
        // ========================================
        public ContainerRole() {
            Children.Add(new SelectRole());
            //Children.Add(new FocusRole());
            Children.Add(new RubberbandRole());
            Children.Add(new CreateChildNodeRole());
            Children.Add(new CreateChildEdgeRole());
            Children.Add(new CloneRole());
            Children.Add(_pasteRole = new PasteRole());
        }


        // ========================================
        // property
        // ========================================
        public PasteRole PasteRole {
            get { return _pasteRole; }
        }
    }
}
