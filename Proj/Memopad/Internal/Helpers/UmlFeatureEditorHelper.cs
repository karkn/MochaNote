/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using System.Windows.Forms;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Core;
using Mkamo.Model.Uml;
using Mkamo.Editor.Focuses;
using Mkamo.StyledText.Core;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.DataType;

namespace Mkamo.Memopad.Internal.Helpers {
    using StyledText = Mkamo.StyledText.Core.StyledText;
    using Mkamo.Common.Diagnostics;
    using System.Text.RegularExpressions;
    using Mkamo.Model.Utils;
    
    internal class UmlFeatureEditorHelper {

        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        protected UmlFeatureEditorHelper() {
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public static object InitFeatureFocus(IFocus focus, object model) {
            var facade = MemopadApplication.Instance;
            //((StyledTextFocus) focus).IsEmacsEdit = facade.Settings.KeyScheme == KeySchemeKind.Emacs;

            var elem = model as UmlNamedElement;
            if (elem != null) {
                var para = new Paragraph(new Run(elem.Name)) {
                    HorizontalAlignment = Mkamo.Common.DataType.HorizontalAlignment.Left,
                    Padding = Insets.Empty,
                };
                return new StyledText(para) {
                    Font = MemopadApplication.Instance.Settings.GetDefaultUmlFont(),
                    VerticalAlignment = VerticalAlignment.Top,
                };
            } else {
                return new StyledText();
            }
        }

        public static FocusUndoer CommitNamedElementFocus(IFocus focus, object model, object value, out bool canceled) {
            var elem = (UmlNamedElement) model;
            var oldName = elem.Name;

            var strs = ((StyledText) value).Lines;
            var newName = strs.Any() ? strs[0] : "";

            canceled = false;
            if (newName == oldName) {
                return null;
            } else {
                elem.Name = newName;
                return (f, m) => {
                    ((UmlNamedElement) m).Name = oldName;
                };
            }
        }

        public static object InitUmlOperationFocus(IFocus focus, object model) {
            var facade = MemopadApplication.Instance;
            //((StyledTextFocus) focus).IsEmacsEdit = facade.Settings.KeyScheme == KeySchemeKind.Emacs;
            focus.Figure.Padding = MemopadConsts.UmlFeaturePadding;

            var ope = model as UmlOperation;
            if (ope != null) {
                var visText = UmlTextUtil.GetVisibilityText(ope.Visibility);
                var visibility = string.IsNullOrEmpty(visText)? "-": visText;
                var paras = string.IsNullOrEmpty(ope.Parameters)? "": ope.Parameters;
                var typeName = string.IsNullOrEmpty(ope.TypeName)? "": ope.TypeName;
                var text = visibility + " " + ope.Name + "(" + paras + ")" + ": " + typeName;

                var para = new Paragraph(new Run(text)) {
                    HorizontalAlignment = Mkamo.Common.DataType.HorizontalAlignment.Left,
                    Padding = Insets.Empty,
                };
                return new StyledText(para) {
                    Font = MemopadApplication.Instance.Settings.GetDefaultUmlFont(),
                    VerticalAlignment = VerticalAlignment.Top,
                };
            } else {
                return new StyledText();
            }
        }

        public static FocusUndoer CommitUmlOperationFocus(IFocus focus, object model, object value, bool isRedo, out bool isCancelled) {
            var ope = (UmlOperation) model;

            var oldName = ope.Name;
            var oldVisibility = ope.Visibility;
            var oldParams = ope.Parameters;
            var oldTypeName = ope.TypeName;

            var strs = ((StyledText) value).Lines;
            var newValue = strs.Any() ? strs[0] : "";

            string newVisibilityText, newName, newParams, newTypeName;
            isCancelled = !UmlTextUtil.ParseUmlOperation(newValue, out newVisibilityText, out newName, out newParams, out newTypeName);
            if (isCancelled) {
                return null;
            }
            var newVisibility = UmlTextUtil.GetVisibility(newVisibilityText);

            if (newName == oldName && newVisibility == oldVisibility && newParams == oldParams && newTypeName == oldTypeName) {
                return null;

            } else {
                ope.Name = newName;
                ope.Visibility = UmlTextUtil.GetVisibility(newVisibilityText);
                ope.Parameters = newParams;
                ope.TypeName = newTypeName;
                return (f, m) => {
                    var o = (UmlOperation) m;
                    o.Name = oldName;
                    o.Visibility = oldVisibility;
                    o.Parameters = oldParams;
                    o.TypeName = oldTypeName;
                };
            }
        }

        public static object InitUmlPropertyFocus(IFocus focus, object model) {
            var facade = MemopadApplication.Instance;
            //((StyledTextFocus) focus).IsEmacsEdit = facade.Settings.KeyScheme == KeySchemeKind.Emacs;
            focus.Figure.Padding = MemopadConsts.UmlFeaturePadding;

            var elem = model as UmlProperty;
            if (elem != null) {
                var visText = UmlTextUtil.GetVisibilityText(elem.Visibility);
                var visibility = string.IsNullOrEmpty(visText)? "-": visText;
                var typeName = string.IsNullOrEmpty(elem.TypeName)? "": elem.TypeName;
                var text = visibility + " " + elem.Name + ": " + typeName;

                var para = new Paragraph(new Run(text)) {
                    HorizontalAlignment = Mkamo.Common.DataType.HorizontalAlignment.Left,
                    Padding = Insets.Empty,
                };
                return new StyledText(para) {
                    Font = MemopadApplication.Instance.Settings.GetDefaultUmlFont(),
                    VerticalAlignment = VerticalAlignment.Top,
                };
            } else {
                return new StyledText();
            }
        }

        public static FocusUndoer CommitUmlPropertyFocus(IFocus focus, object model, object value, bool isRedo, out bool isCancelled) {
            var prop = (UmlProperty) model;
            var oldName = prop.Name;
            var oldVisibility = prop.Visibility;
            var oldTypeName = prop.TypeName;

            var strs = ((StyledText) value).Lines;
            var newValue = strs.Any() ? strs[0] : "";

            string newVisibilityText, newName, newTypeName;
            isCancelled = !UmlTextUtil.ParseUmlProperty(newValue, out newVisibilityText, out newName, out newTypeName);
            if (isCancelled) {
                return null;
            }
            var newVisibility = UmlTextUtil.GetVisibility(newVisibilityText);

            if (newName == oldName && newVisibility == oldVisibility && newTypeName == oldTypeName) {
                return null;

            } else {
                prop.Name = newName;
                prop.Visibility = UmlTextUtil.GetVisibility(newVisibilityText);
                prop.TypeName = newTypeName;

                return (f, m) => {
                    var p = (UmlProperty) m;
                    p.Name = oldName;
                    p.Visibility = oldVisibility;
                    p.TypeName = oldTypeName;
                };
            }
        }

    }
}
