/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;

namespace Mkamo.Editor.Requests {
    public class CombineRequest: AbstractRequest {
        public override string Id {
            get { return RequestIds.Combine; }
        }

        public IEnumerable<IEditor> Combineds {
            get; set;
        }
    }
}
