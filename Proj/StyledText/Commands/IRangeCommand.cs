/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.DataType;

namespace Mkamo.StyledText.Commands {
    internal interface IRangeCommand {
        // ========================================
        // property
        // ========================================
        Range ExecutedRange { get; }
        Range UndoneRange { get; }

    }
}
