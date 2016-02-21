/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;

namespace Mkamo.Editor.Core {
    public interface IRequest {
        string Id { get; }
        IFigure CustomFeedback { get; set; }
    }
}
