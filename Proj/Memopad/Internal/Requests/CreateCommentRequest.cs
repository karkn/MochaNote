/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using System.Drawing;
using Mkamo.Editor.Requests;

namespace Mkamo.Memopad.Internal.Requests {
    internal class CreateCommentRequest: AbstractRequest {
        // ========================================
        // field
        // ========================================
        public IModelFactory ModelFactory;
        public Point StartPoint;
        public Point EndPoint;

        public IEditor EdgeSourceEditor;
        public IEditor EdgeTargetEditor;

        // ========================================
        // property
        // ========================================
        public override string Id {
            get { return RequestIds.CreateEdge; }
        }
    }
}
