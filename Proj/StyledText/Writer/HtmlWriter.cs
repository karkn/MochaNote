/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Writer {
    public class HtmlWriter {
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
        public string ToHtml(IEnumerable<Flow> blocksAndInlines) {
            var ret = new StringBuilder();

            ret.Append("<html>");
            ret.Append("<body>");

            ret.Append(ToHtmlBodyContent(blocksAndInlines));

            ret.Append("</body>");
            ret.Append("</html>");

            return ret.ToString();
        }

        public string ToHtmlBodyContent(IEnumerable<Flow> blocksAndInlines) {
            var ret = new StringBuilder();

            var context = new Stack<Paragraph>();
            var prevPara = default(Paragraph);
            var needBlock = true;
            foreach (var flow in blocksAndInlines) {
                if (flow is Inline) {
                    if (needBlock) {
                        needBlock = false;
                        ret.Append("<p>");
                    }
                    ret.Append(flow.ToHtmlText());
                    prevPara = null;

                } else if (flow is Block) {
                    if (!needBlock) {
                        ret.Append("</p>");
                        needBlock = true;
                    }

                    if (flow is Paragraph) {
                        var para = flow as Paragraph;

                        /// consume context and close
                        if (context.Count > 0) {
                            /// paraよりインデントが深いParagraphをclose

                            var consumeds = new List<Paragraph>();
                            var peek = context.Peek();
                            while (peek.ListLevel > para.ListLevel) {
                                consumeds.Add(context.Pop());

                                if (context.Count == 0) {
                                    break;
                                }
                                peek = context.Peek();
                            }

                            if (consumeds.Count > 0) {
                                for (int i = 0; i < consumeds.Count - 1; ++i) {
                                    var cur = consumeds[i];
                                    var next = consumeds[i + 1];
                                    ret.Append(CloseList(cur.ListKind, cur.ListLevel - next.ListLevel));
                                }

                                var last = consumeds.Last();
                                ret.Append(CloseList(last.ListKind, last.ListLevel - para.ListLevel));
                            }
                        }

                        /// para
                        if (context.Count > 0) {
                            var peek = context.Peek();
                            if (peek.ListLevel == para.ListLevel && peek.ListKind != para.ListKind) {
                                ret.Append(CloseList(peek.ListKind, 1));
                                ret.Append(OpenList(para.ListKind, 1));
                            } else if (peek.ListLevel < para.ListLevel) {
                                ret.Append(OpenList(para.ListKind, para.ListLevel - peek.ListLevel));
                            }

                            ret.Append(para.ToHtmlText());

                        } else {
                            if (prevPara == null || prevPara.ListKind == ListKind.None) {
                                ret.Append(OpenList(para.ListKind, para.ListLevel + 1));
                            }
                            ret.Append(para.ToHtmlText());
                        }

                        /// push para into context
                        if (context.Count > 0) {
                            var peek = context.Peek();
                            if (para.ListLevel == peek.ListLevel) {
                                context.Pop();
                                context.Push(para);
                            } else {
                                context.Push(para);
                            }

                        } else {
                            if (para.ListKind != ListKind.None) {
                                context.Push(para);
                            }
                        }

                        prevPara = para;
                    }
                }
            }

            if (context.Count > 0) {
                for (int i = 0; i < context.Count - 1; ++i) {
                    var cur = context.ElementAt(i);
                    var next = context.ElementAt(i + 1);
                    ret.Append(CloseList(cur.ListKind, cur.ListLevel - next.ListLevel));
                }

                var last = context.Last();
                ret.Append(CloseList(last.ListKind, last.ListLevel + 1));
            }

            return ret.ToString();
        }

        private string OpenList(ListKind listKind, int count) {
            if (listKind == ListKind.None) {
                return string.Empty;
            }

            var ret = new StringBuilder();
            for (int i = 0; i < count; ++i) {
                switch (listKind) {
                    case ListKind.Unordered:
                    case ListKind.Star:
                    case ListKind.LeftArrow:
                    case ListKind.RightArrow:
                    case ListKind.CheckBox:
                    case ListKind.TriStateCheckBox:
                        ret.Append("<ul>");
                        break;
                    case ListKind.Ordered:
                        ret.Append("<ol>");
                        break;
                }
            }
            return ret.ToString();
        }

        private string CloseList(ListKind listKind, int count) {
            if (listKind == ListKind.None) {
                return string.Empty;
            }

            var ret = new StringBuilder();
            for (int i = 0; i < count; ++i) {
                switch (listKind) {
                    case ListKind.Unordered:
                    case ListKind.Star:
                    case ListKind.LeftArrow:
                    case ListKind.RightArrow:
                    case ListKind.CheckBox:
                    case ListKind.TriStateCheckBox:
                        ret.Append("</ul>");
                        break;
                    case ListKind.Ordered:
                        ret.Append("</ol>");
                        break;
                }
            }
            return ret.ToString();
        }

    }
}
