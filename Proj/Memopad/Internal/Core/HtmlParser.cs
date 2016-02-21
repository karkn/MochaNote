/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Mkamo.StyledText.Core;
using Mkamo.Memopad.Internal.Core;
using Mkamo.StyledText.Commands;
using Mkamo.Common.DataType;
using Mkamo.Common.Forms.Clipboard;
using System.Net;
using System.IO;
using System.Drawing;
using Mkamo.Common.String;
using Mkamo.Common.Forms.Descriptions;
using System.Text.RegularExpressions;
using Mkamo.Model.Core;
using Mkamo.Model.Memo;
using Mkamo.Common.Core;

namespace Mkamo.Memopad.Internal.Core {
    internal class HtmlParser {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private List<object> _result;
        private string _source;

        private WebClient _webClient;

        private StyledText.Core.StyledText _currentStyledText;
        private Paragraph _currentParagraph;

        private Stack<ParserContext> _contexts;
        private bool _indent;
        private bool _inPre;

        private MemoTable _currentTable;
        private MemoTableRow _currentTableRow;
        private MemoTableCell _currentCell;
        private bool _isFirstRow;

        // ========================================
        // constructor
        // ========================================
        public HtmlParser() {
        }

        // ========================================
        // property
        // ========================================
        private WebClient _WebClient {
            get { return _webClient ?? (_webClient = new WebClient()); }
        }


        // ========================================
        // method
        // ========================================
        public IEnumerable<object> ParseCFHtml(string cfHtml) {
            _result = new List<object>();

            _contexts = new Stack<ParserContext>();
            _source = ClipboardUtil.GetHtmlSourceUrl(cfHtml);

            var html = Sanitize(cfHtml);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode;
            
            var bodies = doc.DocumentNode.SelectNodes(@"//html/body");
            if (bodies == null || bodies.Count == 0) {
                return _result;
            }
            var body = bodies[0];

            ParseFlows(body);

            if (_currentParagraph != null) {
                /// Blockに囲まれていないInlineで終わった場合
                if (!_currentParagraph.IsEmpty) {
                    if (_currentStyledText == null) {
                        _currentStyledText = CreateStyledText(_currentParagraph);
                    } else {
                        _currentStyledText.Add(_currentParagraph);
                    }
                }
                _currentParagraph = null;
            }

            if (_currentStyledText != null) {
                _result.Add(_currentStyledText);
            }

            return _result;
        }

        // ------------------------------
        // private
        // ------------------------------
        private void ParseFlows(HtmlNode node) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("flows: {0}, {1}", node.Name, node.InnerText));
            }

            foreach (var child in node.ChildNodes) {
                switch (child.Name.ToLowerInvariant()) {
                    case "#comment":
                        break;

                    case "p":
                    case "h1":
                    case "h2":
                    case "h3":
                    case "h4":
                    case "h5":
                    case "h6":
                    case "pre":
                    case "blockquote":
                    case "ul":
                    case "ol":
                    case "li":
                    case "dl":
                    case "dt":
                    case "dd":
                    case "div":
                    case "table":
                        ParseBlock(child);
                        break;

                    case "tr":
                        // todo: いきなりtrの場合，次のtrを探して，あれば列数を取得する，なければ現在のtrで列数確定
                        //ParseTable(node);
                        ParseText(GetInnerText(node), null, true);
                        return;
                        //break;

                    case "a":
                    case "img":
                    case "span":

                    case "br":

                    case "i":
                    case "em":
                    case "dfn":
                    case "var":
                    case "cite":

                    case "b":
                    case "strong":

                    case "u":
                    case "abbr":
                    case "acronym":

                    case "big":
                    case "small":

                    case "font":
                    case "tt":
                    case "code":
                    case "samp":
                    case "kbd":
                    case "sub":
                    case "sup":
                    case "q":
                    case "#text":
                        ParseInline(child);
                        break;
                }
            }
        }

        private void ParseBlock(HtmlNode node) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("block: {0}, {1}", node.Name, node.InnerText));
            }

            if (_currentParagraph != null) {
                /// Blockに囲まれていないInlineのため
                if (!_currentParagraph.IsEmpty) {
                    if (_currentStyledText == null) {
                        _currentStyledText = CreateStyledText(_currentParagraph);
                    } else {
                        _currentStyledText.Add(_currentParagraph);
                    }
                }
                _currentParagraph = null;
            }

            switch (node.Name.ToLowerInvariant()) {
                case "p":
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                    ParseParagraph(node);
                    break;
                case "pre":
                    _inPre = true;
                    ParseParagraph(node);
                    _inPre = false;
                    break;

                case "ul":
                case "ol":
                    ParseList(node);
                    break;
                case "li":
                    _contexts.Push(CreateListContext(node));
                    ParseListItem(node);
                    _contexts.Pop();
                    break;

                case "dl":
                    ParseDefList(node);
                    break;
                case "dt":
                    ParseDefListTerm(node);
                    break;
                case "dd":
                    ParseDefListDescription(node);
                    break;

                case "table":
                    ParseTable(node);
                    break;

                case "blockquote":
                    _indent = true;
                    ParseFlows(node);
                    _indent = false;
                    break;

                case "div":
                    ParseFlows(node);
                    break;
            }

            if (!IsDiv(node) && !IsBlockQuote(node)) {
                _currentParagraph = null;
            }
        }


        private void ParseInline(HtmlNode node) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("inline: {0}, {1}", node.Name, node.InnerText));
            }

            if (_currentParagraph == null) {
                _currentParagraph = CreateParagraph();
            }

            switch (node.Name.ToLowerInvariant()) {
                case "a":
                    ParseAnchor(node);
                    break;
                case "img":
                    ParseImage(node);
                    break;
                case "code":
                case "font":
                case "span":
                    ParseInlines(node);
                    break;
                case "br":
                    ParseBr(node);
                    break;

                case "i":
                case "em":
                case "dfn":
                case "var":
                case "cite":
                    ParseText(node, new FontDescription(GetDefaultFont(), FontStyle.Italic));
                    break;

                case "b":
                case "strong":
                    ParseText(node, new FontDescription(GetDefaultFont(), FontStyle.Bold));
                    break;

                case "u":
                case "abbr":
                case "acronym":
                    ParseText(node, new FontDescription(GetDefaultFont(), FontStyle.Underline));
                    break;

                case "big":
                    {
                        var font = GetDefaultFont();
                        ParseText(node, new FontDescription(font, font.Size + 1));
                    }
                    break;
                case "small":
                    {
                        var font = GetDefaultFont();
                        ParseText(node, new FontDescription(font, font.Size - 1));
                    }
                    break;

                //case "font":
                case "tt":
                //case "code":
                case "samp":
                case "kbd":
                case "sub":
                case "sup":
                case "q":
                case "#text":
                    ParseText(node, null);
                    break;
            }
        }

        private void ParseParagraph(HtmlNode node) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("paragraph: {0}, {1}", node.Name, node.InnerText));
            }

            _currentParagraph = CreateParagraph();

            ParseInlines(node);

            if (_currentParagraph == null || _currentParagraph.IsEmpty) {
                return;
            }

            if (_currentStyledText == null) {
                _currentStyledText = CreateStyledText(_currentParagraph);
            } else {
                _currentStyledText.Add(_currentParagraph);
            }

            SetParagraphKind(_currentParagraph, GetParagraphKind(node));
            if (IsPre(node)) {
                var indent = new IndentParagraphCommand(_currentParagraph);
                indent.Execute();
            }
        }

        private void ParseInlines(HtmlNode node) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("inlines: {0}, {1}", node.Name, node.InnerText));
            }

            foreach (var child in node.ChildNodes) {
                ParseInline(child);
            }
        }

        private void ParseTable(HtmlNode node) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("table: {0}, {1}", node.Name, node.InnerText));
            }

            /// 一時的に退避
            var para = _currentParagraph;
            var stext = _currentStyledText;
            var inBlockQuote = _indent;
            var contexts = new Stack<ParserContext>(_contexts);

            _currentStyledText = null;
            _currentParagraph = null;
            _indent = false;
            _contexts = new Stack<ParserContext>();

            var hasError = false;
            if (_currentTable == null) {
                _currentTable = MemoFactory.CreateTable();
                _isFirstRow = true;

                try {
                    ParseTableContent(node);
                } catch (Exception e) {
                    Logger.Warn("Parse table error.", e);
                    MemopadApplication.Instance.Container.Remove(_currentTable);
                    _currentTable = null;

                    hasError = true;
                }

            } else {
                /// 入れ子tableはInnerTextを貼りつけておくだけ
                ParseText(GetInnerText(node), null, true);
                return;
            }

            _currentParagraph = para;
            _currentStyledText = stext;
            _indent = inBlockQuote;
            _contexts = new Stack<ParserContext>(contexts);

            if (hasError) {
                /// 失敗したらInnerTextを貼りつけておくだけ
                ParseText(GetInnerText(node), null, true);
            }

            if (_currentTable != null && (_currentTable.RowCount == 0 || _currentTable.ColumnCount == 0)) {
                MemopadApplication.Instance.Container.Remove(_currentTable);
                return;
            }

            if (_currentParagraph != null && !_currentParagraph.IsEmpty) {
                if (_currentStyledText == null) {
                    _currentStyledText = CreateStyledText(_currentParagraph);
                } else {
                    _currentStyledText.Add(_currentParagraph);
                }
            }
            _currentParagraph = null;

            if (_currentStyledText != null) {
                _result.Add(_currentStyledText);
                _currentStyledText = null;
            }

            if (_currentTable != null) {
                _result.Add(_currentTable);
                _currentTable = null;
            }
        }



        private void ParseTableContent(HtmlNode node) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("table content: {0}, {1}", node.Name, node.InnerText));
            }

            _currentRowSpans = new Dictionary<int, int>(); /// col => rowspan
            foreach (var child in node.ChildNodes) {
                switch (child.Name.ToLowerInvariant()) {
                    case "tr":
                        ParseTR(child);
                        break;
                    case "thead":
                    case "tfoot":
                    case "tbody":
                        ParseTableContent(child);
                        break;
                }
            }
            _currentRowSpans = null;
        }

        private Dictionary<int, int> _currentRowSpans;

        private void ParseTR(HtmlNode node) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("tr: {0}, {1}", node.Name, node.InnerText));
            }

            _currentTableRow = _currentTable.AddRow();

            if (!node.ChildNodes.Any(child => IsTHOrTD(child))) {
                /// thやtdの内trのrowspan消費
                var newRowSpans = new Dictionary<int, int>();
                foreach (var key in _currentRowSpans.Keys) {
                    var rs = _currentRowSpans[key] - 1;
                    if (rs > 1) {
                        newRowSpans[key] = rs;
                    }
                }
                _currentRowSpans = newRowSpans;
            }

            var colspan = 1;
            var rowspan = 1;
            var col = 0;
            foreach (var child in node.ChildNodes) {
                switch (child.Name.ToLowerInvariant()) {
                    case "th":
                    case "td":
                        colspan = GetSpan(child, "colspan");
                        rowspan = GetSpan(child, "rowspan");

                        if (_isFirstRow) {
                            for (int i = 0; i < colspan; ++i) {
                                _currentTable.AddColumn();
                            }
                        }

                        if (_currentRowSpans.ContainsKey(col)) {
                            var c = col;
                            while (_currentRowSpans.ContainsKey(c)) {
                                _currentRowSpans[c] = _currentRowSpans[c] - 1;
                                if (_currentRowSpans[c] <= 1) {
                                    _currentRowSpans.Remove(c);
                                }
                                ++c;
                            }
                            col = c;
                        }

                        //if (col < _currentTableRow.Cells.Count()) {
                            _currentCell = _currentTableRow.Cells.ElementAt(col);
                            _currentCell.ColumnSpan = colspan;
                            _currentCell.RowSpan = rowspan;

                            ParseTHTD(child);
                        //}

                        if (rowspan > 1) {
                            for (int c = col, clen = col + colspan; c < clen; ++c) {
                                _currentRowSpans[c] = rowspan;
                            }
                        }
                        col += colspan;
                        break;
                }
            }

            /// trのth/tdより後ろにあるrowspan消費
            if (col < _currentTable.ColumnCount) {
                var newRowSpans = new Dictionary<int, int>();
                foreach (var key in _currentRowSpans.Keys) {
                    if (key >= col) {
                        var rs = _currentRowSpans[key] - 1;
                        if (rs > 1) {
                            newRowSpans[key] = rs;
                        }
                    }
                }
                _currentRowSpans = newRowSpans;
            }

            _currentTableRow = null;
            _isFirstRow = false;
        }

        private int GetSpan(HtmlNode node, string attrName) {
            var colspanAttr = node.Attributes[attrName];
            if (colspanAttr == null || string.IsNullOrEmpty(colspanAttr.Value)) {
                return 1;
            } else {
                try {
                    return int.Parse(colspanAttr.Value);
                } catch (Exception) {
                    return 1;
                }
            }
        }

        private void ParseTHTD(HtmlNode node) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("th: {0}, {1}", node.Name, node.InnerText));
            }

            _currentParagraph = CreateParagraph();

            if (IsTH(node)) {
                _currentParagraph.HorizontalAlignment = HorizontalAlignment.Center;
            }

            ParseFlows(node);

            if (_currentParagraph != null && !_currentParagraph.IsEmpty) {
                if (_currentStyledText == null) {
                    _currentStyledText = CreateStyledText(_currentParagraph);
                } else {
                    _currentStyledText.Add(_currentParagraph);
                }
            }
            if (_currentStyledText != null) {
                _currentCell.StyledText = _currentStyledText;
            }
            _currentParagraph = null;
            _currentStyledText = null;
        }

        private void ParseDefList(HtmlNode node) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("dl: {0}, {1}", node.Name, node.InnerText));
            }

            foreach (var child in node.ChildNodes) {
                switch (child.Name.ToLowerInvariant()) {
                    case "dt":
                        ParseDefListTerm(child);
                        break;
                    case "dd":
                        ParseDefListDescription(child);
                        break;
                }
            }
        }

        private void ParseDefListTerm(HtmlNode node) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("dt: {0}, {1}", node.Name, node.InnerText));
            }

            //_currentParagraph = CreateParagraph();
            _currentParagraph = new Paragraph();

            ParseInlines(node);

            if (_currentParagraph == null || _currentParagraph.IsEmpty) {
                return;
            }

            if (_currentStyledText == null) {
                _currentStyledText = CreateStyledText(_currentParagraph);
                //SetListItemProps(_currentParagraph, ListKind.Unordered, 0);
            } else {
                _currentStyledText.Add(_currentParagraph);
                //SetListItemProps(_currentParagraph, ListKind.Unordered, 0);
            }
        }

        private void ParseDefListDescription(HtmlNode node) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("dd: {0}, {1}", node.Name, node.InnerText));
            }

            _indent = true;

            _currentParagraph = CreateParagraph();

            ParseFlows(node);

            if (_currentParagraph == null || _currentParagraph.IsEmpty) {
                return;
            }

            if (_currentStyledText == null) {
                _currentStyledText = CreateStyledText(_currentParagraph);
                //SetListItemProps(_currentParagraph, ListKind.Unordered, 1);
            } else {
                _currentStyledText.Add(_currentParagraph);
                //SetListItemProps(_currentParagraph, ListKind.Unordered, 1);
            }

            _indent = false;
        }

        private void ParseList(HtmlNode node) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("list: {0}, {1}", node.Name, node.InnerText));
            }

            _contexts.Push(CreateListContext(node));

            foreach (var child in node.ChildNodes) {
                if (IsLi(child)) {
                    ParseListItem(child);
                }
            }

            _contexts.Pop();
        }

        private void ParseListItem(HtmlNode node) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("li: {0}, {1}", node.Name, node.InnerText));
            }

            _currentParagraph = CreateParagraph();

            ParseFlows(node);

            if (_currentParagraph == null || _currentParagraph.IsEmpty) {
                return;
            }

            if (_currentStyledText == null) {
                _currentStyledText = CreateStyledText(_currentParagraph);
            } else {
                _currentStyledText.Add(_currentParagraph);
            }

            _currentParagraph = null;
        }

        private void ParseText(HtmlNode node, FontDescription font) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("text: {0}, {1}", node.Name, node.InnerText));
            }

            ParseText(node.InnerText, font);
        }

        private void ParseText(string text, FontDescription font) {
            ParseText(text, font, false);
        }

        private void ParseText(string text, FontDescription font, bool forcePre) {
            if (_currentParagraph == null) {
                _currentParagraph = CreateParagraph();
            }

            if (!StringUtil.IsNullOrWhitespace(text) || _inPre || forcePre) {
                if (_inPre || forcePre) {
                    var lines = StringUtil.SplitLines(text);
                    for (int i = 0, len = lines.Length; i < len; ++i) {
                        var line = lines[i];
                        var untabified = line.Replace("\t", "    ");
                        var s = HtmlEntity.DeEntitize(untabified);
                        if (!string.IsNullOrEmpty(s)) {
                            var run = new Run(s);
                            if (font != null) {
                                run.Font = font;
                            }
                            _currentParagraph.LineSegments.Last().InsertAfter(run);
                        }
                        if (i != len - 1) {
                            _currentParagraph.InsertAfter(new LineSegment());
                        }
                    }

                } else {

                    var spaceMerged = Regex.Replace(text, "\\s+", " "); /// &nbsp;がマージされないようにDeEntitize()する前にスペースをマージ
                    spaceMerged = HtmlEntity.DeEntitize(spaceMerged);
                    var run = new Run(spaceMerged);
                    if (font != null) {
                        run.Font = font;
                    }
                    _currentParagraph.LineSegments.Last().InsertAfter(run);
                }
            }
        }

        private void ParseAnchor(HtmlNode node) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("a: {0}, {1}", node.Name, node.InnerText));
            }

            var hrefAttr = node.Attributes["href"];
            var href = hrefAttr == null ? null : hrefAttr.Value;

            foreach (var child in node.ChildNodes) {
                if (_currentParagraph == null) {
                    _currentParagraph = CreateParagraph();
                    //_currentParagraph = new Paragraph();
                }

                switch (child.Name.ToLowerInvariant()) {
                    case "img":
                        ParseImage(child);
                        break;
                    case "#text":
                        var text = child.InnerText;
                        if (!StringUtil.IsNullOrWhitespace(text)) {
                            var run = new Run(HtmlEntity.DeEntitize(text));
                            if (!StringUtil.IsNullOrWhitespace(href)) {
                                run.Link = new Link(href);
                            }
                            _currentParagraph.LineSegments.Last().InsertAfter(run);
                        }
                        break;
                }
            }
        }

        private void ParseBr(HtmlNode node) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("br: {0}, {1}", node.Name, node.InnerText));
            }

            if (_currentParagraph == null) {
                _currentParagraph = CreateParagraph();
                //_currentParagraph = new Paragraph();
            }
            _currentParagraph.InsertAfter(new LineSegment());
        }

        private void ParseImage(HtmlNode node) {
            if (Logger.IsDebugEnabled) {
                Logger.Debug(string.Format("img: {0}, {1}", node.Name, node.InnerText));
            }

            if (_currentTable != null) {
                /// table内は非対応
                // todo: alt
                return;
            }

            var srcAttr = node.Attributes["src"];
            if (srcAttr == null || StringUtil.IsNullOrWhitespace(srcAttr.Value)) {
                return;
            }

            var srcUri = new Uri(srcAttr.Value, UriKind.RelativeOrAbsolute);
            if (!srcUri.IsAbsoluteUri) {
                srcUri = new Uri(new Uri(_source), srcUri);
            }


            _WebClient.Headers["Referer"] = _source;
            var stream = default(Stream);
            var img = default(Image);
            try {
                stream = _WebClient.OpenRead(srcUri);
                img = Image.FromStream(stream);
            } catch (Exception e) {
                Logger.Warn("Can't load image.", e);
                return;
            } finally {
                if (stream != null) {
                    stream.Dispose();
                }
            }

            if (_currentParagraph != null) {
                if (!_currentParagraph.IsEmpty) {
                    if (_currentStyledText == null) {
                        _currentStyledText = CreateStyledText(_currentParagraph);
                    } else {
                        _currentStyledText.Add(_currentParagraph);
                    }
                }
                _currentParagraph = null;
            }
            if (_currentStyledText != null) {
                _result.Add(_currentStyledText);
                _currentStyledText = null;
            }

            _result.Add(img);
        }


        // --- util ---
        private void SetParagraphKind(Paragraph para, ParagraphKind kind) {
            var cmd = new SetParagraphKindOfParagraphCommand(para, kind);
            cmd.Execute();
        }

        private void SetListItemProps(Paragraph para, ListKind kind, int level) {
            var list = new SetListKindOfParagraphCommand(para, kind);
            list.Execute();

            if (para.ListLevel < level) {
                for (int i = para.ListLevel; i < level; ++i) {
                    var indent = new IndentParagraphCommand(para);
                    indent.Execute();
                }
            } else if (para.ListLevel > level) {
                for (int i = para.ListLevel; i > level; --i) {
                    var outdent = new IndentParagraphCommand(para, -1);
                    outdent.Execute();
                }
            }
        }

        private bool IsUL(HtmlNode node) {
            return node != null && node.Name != null && node.Name.ToLowerInvariant() == "ul";
        }

        private bool IsOL(HtmlNode node) {
            return node != null && node.Name != null && node.Name.ToLowerInvariant() == "ol";
        }

        private bool IsDD(HtmlNode node) {
            return node != null && node.Name != null && node.Name.ToLowerInvariant() == "dd";
        }

        private bool IsLi(HtmlNode node) {
            return node != null && node.Name != null && node.Name.ToLowerInvariant() == "li";
        }

        private bool IsPre(HtmlNode node) {
            return node != null && node.Name != null && node.Name.ToLowerInvariant() == "pre";
        }

        private bool IsBlockQuote(HtmlNode node) {
            return node != null && node.Name != null && node.Name.ToLowerInvariant() == "blockquote";
        }

        private bool IsDiv(HtmlNode node) {
            return node != null && node.Name != null && node.Name.ToLowerInvariant() == "div";
        }

        private bool IsTH(HtmlNode node) {
            return node != null && node.Name != null && node.Name.ToLowerInvariant() == "th";
        }

        private bool IsTHOrTD(HtmlNode node) {
            return node != null && node.Name != null && (node.Name.ToLowerInvariant() == "th" || node.Name.ToLowerInvariant() == "td");
        }

        private bool IsA(HtmlNode node) {
            return node != null && node.Name != null && node.Name.ToLowerInvariant() == "a";
        }

        private bool IsTextNode(HtmlNode node) {
            return node != null && node.NodeType == HtmlNodeType.Text;
        }

        private bool HasChildList(HtmlNode node) {
            foreach (var child in node.ChildNodes) {
                if (IsUL(child) || IsOL(child)) {
                    return true;
                }
            }
            return false;
        }

        private ParserContext CreateListContext(HtmlNode node) {
            var listKind = ListKind.Unordered;
            if (IsOL(node)) {
                listKind = ListKind.Ordered;
            }
            return new ParserContext(listKind, _contexts.Count == 0 ? 0 : _contexts.Peek().ListLevel + 1);
        }

        private ParagraphKind GetParagraphKind(HtmlNode node) {
            switch (node.Name.ToLowerInvariant()) {
                case "h1":
                    return ParagraphKind.Heading1;
                case "h2":
                    return ParagraphKind.Heading2;
                case "h3":
                    return ParagraphKind.Heading3;
                case "h4":
                    return ParagraphKind.Heading4;
                case "h5":
                    return ParagraphKind.Heading5;
                case "h6":
                    return ParagraphKind.Heading6;
                case "p":
                default:
                    return ParagraphKind.Normal;
            }
        }

        private StyledText.Core.StyledText CreateStyledText(Paragraph para) {
            var ret = new Mkamo.StyledText.Core.StyledText(para);
            ret.Font = GetDefaultFont();
            return ret;
        }

        private Paragraph CreateParagraph() {
            var ret = new Paragraph();
            if (_contexts.Count > 0) {
                var context = _contexts.Peek();
                SetListItemProps(ret, context.ListKind, context.ListLevel);
            }
            if (_indent) {
                var indent = new IndentParagraphCommand(ret);
                indent.Execute();
            }
            return ret;
        }

        private FontDescription GetDefaultFont() {
            return MemopadApplication.Instance.Settings.GetDefaultMemoTextFont();
        }

        private string Sanitize(string html) {
            html = Regex.Replace(
                html,
                ".*<!--StartFragment-->(.*)<!--EndFragment-->.*",
                "<html><body>$1</body></html>",
                RegexOptions.IgnoreCase | RegexOptions.Singleline
            );

            var matches = Regex.Matches(html, "<.+?>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            var needEnd = new List<Match>();
            var cLevel = 0;
            var startLILevel = -1;
            foreach (Match match in matches) {
                var tag = match.Value.ToLowerInvariant();
                if (tag.StartsWith("<ul") || tag.StartsWith("<ol")) {
                    ++cLevel;
                }
                if (tag.StartsWith("</ul") || tag.StartsWith("</ol")) {
                    --cLevel;
                }

                if (tag.StartsWith("<li")) {
                    if (startLILevel == cLevel) {
                        needEnd.Add(match);
                    }
                    startLILevel = cLevel;
                }
                if (tag.StartsWith("</li")) {
                    startLILevel = -1;
                }
            }

            var ret = html;
            for (int i = needEnd.Count - 1; i >=0; --i) {
                ret = ret.Insert(needEnd[i].Index, "</li>");
            }

            return ret;
            //return Regex.Replace(html, "(<li.*>)", "</li>$1", RegexOptions.IgnoreCase);
        }

        private string GetInnerText(HtmlNode node) {
            if (node.NodeType == HtmlNodeType.Text) {
                return ((HtmlTextNode) node).Text;
            }
            if (node.NodeType == HtmlNodeType.Comment) {
                return ((HtmlCommentNode) node).Comment;
            }
            if (!node.HasChildNodes) {
                if (node.Name.ToUpperInvariant() == "BR") {
                    return Environment.NewLine;
                }
                return string.Empty;
            }

            var buf = new StringBuilder();
            foreach (var c in node.ChildNodes) {
                buf.Append(GetInnerText(c));
            }
            return buf.ToString();
        }

        // ========================================
        // type
        // ========================================
        public class ParserContext {
            public ListKind ListKind;
            public int ListLevel;

            public ParserContext(ListKind kind, int level) {
                ListKind = kind;
                ListLevel = level;
            }
        }
    }
}
