/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Model.Uml;
using Mkamo.Common.Forms.KeyMap;
using System.Reflection;

namespace Mkamo.Memopad.Internal.KeyActions {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    internal static class UmlFeatureEditorKeyActions {
        // ========================================
        // method
        // ========================================
        [KeyAction("")]
        public static void SelectNextItem(IEditor editor) {
            if (editor.HasNextSibling) {
                editor.NextSibling.RequestSelect(SelectKind.True, true);
            } else {
                var prop = editor.Model as UmlProperty;
                if (prop != null && editor.Parent != null && editor.Parent.Parent != null) {
                    var classifierEditor = editor.Parent.Parent;
                    var opeColEditor = classifierEditor.Children.Last();
                    var opeColCtrl = opeColEditor.Controller as IContainerController;
                    if (opeColCtrl != null && opeColCtrl.ChildCount > 0) {
                        opeColEditor.Children.First().RequestSelect(SelectKind.True, true);
                    }
                }
            }
        }

        [KeyAction("")]
        public static void SelectPreviousItem(IEditor editor) {
            if (editor.HasPreviousSibling) {
                editor.PreviousSibling.RequestSelect(SelectKind.True, true);
            } else {
                var ope = editor.Model as UmlOperation;
                if (ope != null && editor.Parent != null && editor.Parent.Parent != null) {
                    var classifierEditor = editor.Parent.Parent;
                    var attrColEditor = classifierEditor.Children.First();
                    var attrColCtrl = attrColEditor.Controller as IContainerController;
                    if (attrColCtrl != null && attrColCtrl.ChildCount > 0) {
                        attrColEditor.Children.Last().RequestSelect(SelectKind.True, true);
                    }
                }
            }
        }

        [KeyAction("")]
        public static void RemoveItem(IEditor editor) {
            var parent = editor.Parent;
            var classifier = parent == null? null: parent.Parent;

            editor.RequestRemove();

            if (classifier != null) {
                classifier.RequestSelect(SelectKind.True, true);
            }
        }

        // --- focus ---
        [KeyAction("")]
        public static void BeginFocus(IEditor editor) {
            editor.RequestFocus(FocusKind.Begin, null);
        }
    }
}
