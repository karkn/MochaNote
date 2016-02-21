/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Model.Uml;
using System.Text.RegularExpressions;
using Mkamo.Common.Diagnostics;
using Mkamo.Common.DataType;

namespace Mkamo.Model.Utils {
    public static class UmlTextUtil {
        // ========================================
        // method
        // ========================================
        public static string GetAttributeText(UmlProperty attr) {
            var text = attr.Name;

            if (!string.IsNullOrEmpty(attr.Stereotype)) {
                text = GetStereotypeText(attr) + " " + text;
            }

            if (attr.Visibility != UmlVisibilityKind.None) {
                text = GetVisibilityText(attr.Visibility) + " " + text;
            }
            if (attr.IsDerived) {
                text = "/ " + text;
            }
            if (!string.IsNullOrEmpty(attr.TypeName)) {
                text += ": " + attr.TypeName;
            }

            if (!string.IsNullOrEmpty(attr.Default)) {
                text += " = " + attr.Default;
            }
            if (attr.Lower != 1 || attr.Upper != 1 || attr.IsUpperUnlimited) {
                text += " [" + GetMultiplicityText(attr) + "]";
            }
            if (attr.IsOrdered || attr.IsUnique) {
                text += " " + GetMultiplicityOptionText(attr);
            }
            if (attr.IsReadOnly) {
                text += " {readOnly}";
            }
            return text;
        }

        public static string GetOperationText(UmlOperation ope) {
            var text = ope.Name + "(" + ope.Parameters + ")";
            if (!string.IsNullOrEmpty(ope.Stereotype)) {
                text = GetStereotypeText(ope) + " " + text;
            }
            if (ope.Visibility != UmlVisibilityKind.None) {
                text = GetVisibilityText(ope.Visibility) + " " + text;
            }
            if (!string.IsNullOrEmpty(ope.TypeName)) {
                text += ": " + ope.TypeName;
            }
            return text;
        }

        public static string GetStereotypeText(UmlElement elem) {
            return string.IsNullOrEmpty(elem.Stereotype)? "": "<<" + elem.Stereotype + ">>";
        }

        public static string GetStereotypeText(UmlElement elem, string defaultStereotype) {
            return string.IsNullOrEmpty(elem.Stereotype)?
                "<<" + defaultStereotype + ">>":
                "<<" + defaultStereotype + ", " + elem.Stereotype + ">>";
        }

        /// <summary>
        /// []は含まない．
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        public static string GetMultiplicityText(UmlMultiplicityElement elem) {
            if (elem.Lower != 1 || elem.Upper != 1 || elem.IsUpperUnlimited) {
                if (elem.Lower == 0 && elem.IsUpperUnlimited) {
                    return "*";
                } else {
                    if (elem.IsUpperUnlimited) {
                        return elem.Lower + "..*";
                    } else {
                        if (elem.Lower == elem.Upper) {
                            return elem.Lower.ToString();
                        } else {
                            return elem.Lower + ".." + elem.Upper;
                        }
                    }
                }
            }
            return "";
        }

        public static string GetMultiplicityOptionText(UmlMultiplicityElement elem) {
            if (elem.IsOrdered && elem.IsUnique) {
                return "{ordered, unique}";
            } else if (elem.IsOrdered) {
                return "{ordered}";
            } else if (elem.IsUnique) {
                return "{unique}";
            }
            return "";
        }

        public static string GetVisibilityText(UmlVisibilityKind visibility) {
            switch (visibility) {
                case UmlVisibilityKind.Public: {
                    return "+";
                }
                case UmlVisibilityKind.Private: {
                    return "-";
                }
                case UmlVisibilityKind.Protected: {
                    return "#";
                }
                case UmlVisibilityKind.Package: {
                    return "~";
                }

                case UmlVisibilityKind.None:
                default: {
                    return "";
                }
            }
        }

        public static UmlVisibilityKind GetVisibility(string text) {
            switch (text) {
                case "+": {
                    return UmlVisibilityKind.Public;
                }
                case "-": {
                    return UmlVisibilityKind.Private;
                }
                case "#": {
                    return UmlVisibilityKind.Protected;
                }
                case "~": {
                    return UmlVisibilityKind.Package;
                }
                default: {
                    return UmlVisibilityKind.None;
                }
            }
        }

        public static bool ParseStereotype(string str, out string[] stereotypes) {
            var regex = new Regex(@"<<([^<>]*)>>");
            var match = regex.Match(str);
            if (match.Success && match.Groups[1].Success) {
                var parsed = match.Groups[1].Value;
                stereotypes = parsed.Split(',');
                for (int i = 0, len = stereotypes.Length; i < len; ++i) {
                    stereotypes[i] = stereotypes[i].Trim();
                }
                return stereotypes.Length > 0;
            } else {
                stereotypes = new string[0];
                return false;
            }
        }

        public static bool ParseUmlProperty(
            string text, out string visibility, out string name, out string typeName
        ) {
            Contract.Requires(text != null);

            var regex = new Regex(@"([\+\-\#\~])?([^:]+)(:.*)?");
            var match = regex.Match(text);
            var groups = match.Groups;

            if (!groups[2].Success) {
                visibility = "";
                name = "";
                typeName = "";
                return false;
            }

            visibility = groups[1].Success? groups[1].Value.Trim(): "";
            name = groups[2].Success? groups[2].Value.Trim(): "";
            typeName = groups[3].Success? groups[3].Value.Substring(1).Trim(): "";
            return true;
        }

        public static bool ParseUmlOperation(
            string text, out string visibility, out string name, out string paras, out string typeName
        ) {
            Contract.Requires(text != null);

            var regex = new Regex(@"([\+\-\#\~])?([^\(]+)\((.*)\)(:[^\,\:]*)?");
            var match = regex.Match(text);
            var groups = match.Groups;

            if (!groups[2].Success) {
                visibility = "";
                name = "";
                paras = "";
                typeName = "";
                return false;
            }

            visibility = groups[1].Success? groups[1].Value.Trim(): "";
            name = groups[2].Success? groups[2].Value.Trim(): "";
            typeName = groups[4].Success? groups[4].Value.Substring(1).Trim(): "";

            var parasText = groups[3].Success? groups[3].Value.Trim(): "";
            if (string.IsNullOrEmpty(parasText)) {
                paras = "";
            } else {
                var paramStrs = ParseUmlParameters(parasText);
                var buf = default(StringBuilder);
                foreach (var paramStr in paramStrs) {
                    var str = string.Empty;
                    if (string.IsNullOrEmpty(paramStr.Item2)) {
                        str = paramStr.Item1;
                    } else {
                        str = paramStr.Item1 + ": " + paramStr.Item2;
                    }
                    if (buf == null) {
                        buf = new StringBuilder(str);
                    } else {
                        buf.Append(", " + str);
                    }
                }
                paras = buf.ToString();
            }
            
            return true;
        }

        private static Tuple<string, string>[] ParseUmlParameters(string text) {
            var ret = new List<Tuple<string, string>>();

            var regex = new Regex(@"([^\:\,]+)\:?([^\:\,]*)");
            var matches = regex.Matches(text);
            if (matches.Count < 1) {
                return ret.ToArray();
            }

            foreach (Match match in matches) {
                if (match.Success) {
                    var name = match.Groups[1].Success? match.Groups[1].Value.Trim(): "";
                    var type = match.Groups[2].Success? match.Groups[2].Value.Trim(): "";
                    if (!string.IsNullOrEmpty(name)) {
                        ret.Add(Tuple.Create(name, type));
                    }
                }
            }
            return ret.ToArray();
        }
    }
}
