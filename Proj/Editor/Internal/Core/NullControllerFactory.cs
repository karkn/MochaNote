/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Controllers;

namespace Mkamo.Editor.Internal.Core {
    internal class NullControllerFactory: IControllerFactory {
        public IController CreateController(object model) {
            return EditorConsts.NullController;
        }
    }
}
