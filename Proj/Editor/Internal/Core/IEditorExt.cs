/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;

namespace Mkamo.Editor.Internal.Core {
    using Editor = Mkamo.Editor.Internal.Editors.Editor;

    /// <summary>
    /// IEditorで公開したくないが，持たせたいメソッドを定義する．
    /// </summary>
    public static class IEditorExt {

        // ========================================
        // static method
        // ========================================
        //internal static IEditor CreateChildEditor(this IEditor editor, object model) {
        //    return (editor as Editors.Editor).CreateEditor(model);
        //}

        public static IEditor AddChild(this IEditor editor, object model) {
            return ((Editor) editor).AddChild(model);
        }

        public static void AddChildEditor(this IEditor editor, IEditor child) {
            ((Editor) editor).AddChildEditor((Editor) child);
        }

        public static IEditor InsertChild(this IEditor editor, object model, int index) {
            return ((Editor) editor).InsertChild(model, index);
        }

        public static void InsertChildEditor(this IEditor editor, IEditor child, int index) {
            ((Editor) editor).InsertChildEditor((Editor) child, index);
        }

        public static bool RemoveChild(this IEditor editor, object model) {
            return ((Editor) editor).RemoveChild(model);
        }

        public static bool RemoveChildEditor(this IEditor editor, IEditor child) {
            return ((Editor) editor).RemoveChildEditor((Editor) child);
        }
    }
}
