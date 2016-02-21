/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.StyledText.Core;
using Mkamo.Editor.Focuses;
using Mkamo.Model.Uml;
using Mkamo.Common.DataType;
using Mkamo.Memopad.Properties;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Core;

namespace Mkamo.Memopad.Internal.Helpers {
    using StyledText = Mkamo.StyledText.Core.StyledText;
    using Mkamo.Common.Forms.Drawing;
    using Mkamo.Model.Utils;
    using System.Text.RegularExpressions;

    internal static class UmlClassifierHelper {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public static object InitUmlClassifierFocus(IFocus focus, object model) {
            var facade = MemopadApplication.Instance;
            //((StyledTextFocus) focus).IsEmacsEdit = facade.Settings.KeyScheme == KeySchemeKind.Emacs;

            var elem = (UmlClassifier) model;
            if (elem != null) {
                var stereotypeRun = string.IsNullOrEmpty(elem.Stereotype)? null: new Run(UmlTextUtil.GetStereotypeText(elem));
                var nameRun = new Run(elem.Name);
                var para = default(Paragraph);
                if (stereotypeRun == null) {
                    para = new Paragraph(nameRun);
                } else {
                    para = new Paragraph(stereotypeRun);
                    var line = new LineSegment();
                    line.InsertAfter(nameRun);
                    para.InsertAfter(line);
                }
                para.HorizontalAlignment = HorizontalAlignment.Center;
                para.Padding = Insets.Empty;
                return new StyledText(para) {
                    Font = MemopadApplication.Instance.Settings.GetDefaultUmlFont(),
                    VerticalAlignment = VerticalAlignment.Top,
                };
            } else {
                return new StyledText();
            }
        }

        public static FocusUndoer CommitClassifierFocus(IFocus focus, object model, object value, bool isRedo, out bool isCancelled) {
            var elem = (UmlClassifier) model;
            var oldName = elem.Name;
            var oldStereotype = elem.Stereotype;

            var strs = ((StyledText) value).Lines;
            var newName = "";
            var newStereotype = "";
            if (strs.Any()) {
                var stereotypes = new HashSet<string>();
                foreach (var str in strs) {
                    if (!string.IsNullOrEmpty(str)) {
                        var parsed = default(string[]);
                        if (UmlTextUtil.ParseStereotype(str, out parsed)) {
                            foreach (var s in parsed) {
                                stereotypes.Add(s);
                            }
                        } else {
                            newName = str.Trim();
                        }
                    }
                }
                var arr = stereotypes.ToArray();
                Array.Sort(arr);
                newStereotype = string.Join(", ", arr);
            }

            isCancelled = false;
            if (newName == oldName && newStereotype == oldStereotype) {
                return null;
            } else {
                elem.Name = newName;
                elem.Stereotype = newStereotype;
                return (f, m) => {
                    var cls = (UmlClassifier) m;
                    cls.Name = oldName;
                    cls.Stereotype = oldStereotype;
                };
            }
        }

        public static object InitUmlInterfaceFocus(IFocus focus, object model) {
            var facade = MemopadApplication.Instance;
            //((StyledTextFocus) focus).IsEmacsEdit = facade.Settings.KeyScheme == KeySchemeKind.Emacs;

            var elem = (UmlClassifier) model;
            if (elem != null) {
                var stereotypeRun = new Run(UmlTextUtil.GetStereotypeText(elem, "interface"));
                var nameRun = new Run(elem.Name);
                var para = new Paragraph(stereotypeRun);
                var line = new LineSegment();
                line.InsertAfter(nameRun);
                para.InsertAfter(line);
                para.HorizontalAlignment = HorizontalAlignment.Center;
                para.Padding = Insets.Empty;
                return new StyledText(para) {
                    Font = MemopadApplication.Instance.Settings.GetDefaultUmlFont(),
                    VerticalAlignment = VerticalAlignment.Top,
                };
            } else {
                return new StyledText();
            }
        }

        public static FocusUndoer CommitInterfaceFocus(IFocus focus, object model, object value, bool isRedo, out bool isCancelled) {
            var elem = (UmlClassifier) model;
            var oldName = elem.Name;
            var oldStereotype = elem.Stereotype;

            var strs = ((StyledText) value).Lines;
            var newName = "";
            var newStereotype = "";
            if (strs.Any()) {
                var stereotypes = new HashSet<string>();
                foreach (var str in strs) {
                    if (!string.IsNullOrEmpty(str)) {
                        var parsed = default(string[]);
                        if (UmlTextUtil.ParseStereotype(str, out parsed)) {
                            foreach (var s in parsed) {
                                if (!s.Equals("interface", StringComparison.OrdinalIgnoreCase)) {
                                    stereotypes.Add(s);
                                }
                            }
                        } else {
                            newName = str.Trim();
                        }
                    }
                }
                var arr = stereotypes.ToArray();
                Array.Sort(arr);
                newStereotype = string.Join(", ", arr);
            }

            isCancelled = false;
            if (newName == oldName && newStereotype == oldStereotype) {
                return null;
            } else {
                elem.Name = newName;
                elem.Stereotype = newStereotype;
                return (f, m) => {
                    var cls = (UmlClassifier) m;
                    cls.Name = oldName;
                    cls.Stereotype = oldStereotype;
                };
            }
        }
    }
}
