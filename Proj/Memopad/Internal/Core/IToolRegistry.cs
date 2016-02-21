/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Tools;
using System.Drawing;

namespace Mkamo.Memopad.Internal.Core {
    internal interface IToolRegistry {
        // ========================================
        // property
        // ========================================
        IEnumerable<string> CreateNodeToolIds { get; }
        IEnumerable<string> AddEdgeToolIds { get; }

        // ========================================
        // method
        // ========================================
        string GetCreateNodeToolText(string id);
        CreateNodeTool GetCreateNodeTool(string id);

        string GetAddEdgeToolText(string id);
        Image GetAddEdgeToolImage(string id);
        AddEdgeTool GetAddEdgeTool(string id);
    }
}
