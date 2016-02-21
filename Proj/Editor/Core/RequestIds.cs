/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Editor.Core {
    public static class RequestIds {
        public const string Escalate = "Escalate";

        public const string Select = "Select";
        public const string Rubberband = "Rubberband";
        public const string Highlight = "Highlight";

        public const string Focus = "Focus";

        public const string Move = "Move";
        public const string Resize = "Resize";
        public const string ChangeBounds = "ChangeBounds";

        public const string Combine = "Combine";

        public const string MoveEdgePoint = "MoveEdgePoint";
        public const string NewEdgePoint = "NewEdgePoint";

        public const string Connect = "Connect";
        public const string Contain = "Contain";

        public const string CreateNode = "CreateNode";
        public const string CreateEdge = "CreateEdge";
        public const string Freehand = "Freehand";

        public const string Clone = "Clone";

        public const string Copy = "Copy";
        public const string Paste = "Paste";

        public const string Export = "Export";

        public const string Remove = "Remove";

        public const string Reorder = "Reorder";
        public const string ReorderModel = "ReorderModel";

        public const string SetPlainTextFont = "SetPlainTextFont";
        public const string SetStyledTextAlignment = "SetStyledTextAlignment";
        public const string SetStyledTextFont = "SetStyledTextFont";
        public const string SetStyledTextColor = "SetStyledTextColor";
        public const string SetStyledTextListKind = "SetStyledTextListKind ";

        public const string DropText = "DropText";
        public const string AdjustSpace = "AdjustSpace";
    }
}
