/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.DataType;
using DataType = Mkamo.Common.DataType;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.String;
using Mkamo.StyledText.Writer;
using System.Runtime.Serialization;
using Mkamo.Common.Core;

namespace Mkamo.StyledText.Core {
    /// <summary>
    /// 段落．
    /// 最後にBlockBreakが必ず付く．
    /// ListKindとIndentLevelでBulletの表示文字が変わる．
    /// インデント幅はPaddingで設定する．
    /// </summary>
    [Serializable]
    [DataContract]
    public class Paragraph: Block {
        // ========================================
        // field
        // ========================================
        [DataMember]
        private ParagraphKind _paragraphKind;
        [DataMember]
        private ListKind _listKind;
        [DataMember]
        private int _listLevel = 0;
        [DataMember]
        private ListStateKind _listState = ListStateKind.Unchecked;

        // ========================================
        // constructor
        // ========================================
        public Paragraph(ListKind listKind) {
            _listKind = listKind;
        }

        public Paragraph(): this(ListKind.None) {
        }

        public Paragraph(Run run): this(ListKind.None) {
            InsertAfter(run);
        }

        public Paragraph(ListKind listKind, Run run): this(listKind) {
            InsertAfter(run);
        }

        //[OnDeserialized]
        //private void OnDeserialized(StreamingContext context) {
        //    /// Paddingを変更したための措置
        //    // todo: 設定可能にしたときに削除する
        //    var def = GetDefaultPadding(ParagraphKind);
        //    var cur = Padding;
        //    Padding = new Insets(cur.Left, def.Top, cur.Right, def.Bottom);
        //}

        // ========================================
        // property
        // ========================================
        // === Paragraph ==========
        public ParagraphKind ParagraphKind {
            get { return _paragraphKind; }
            set {
                if (value == _paragraphKind) {
                    return;
                }

                _paragraphKind = value;
                //Padding = GetDefaultPadding(value);

                OnContentsChanged(this);
            }
        }

        public ListKind ListKind {
            get { return _listKind; }
            set {
                if (value == _listKind) {
                    return;
                }
                _listKind = value;
                OnContentsChanged(this);
            }
        }

        public int ListLevel {
            get { return _listLevel; }
            set {
                if (value == _listLevel) {
                    return;
                }
                _listLevel = value;
                OnContentsChanged(this);
            }
        }

        public ListStateKind ListState {
            get { return _listState; }
            set {
                if (_listState == value) {
                    return;
                }
                _listState = value;
                OnContentsChanged(this);
            }
        }

        /// <summary>
        /// 0開始のインデックス．
        /// ただしIsEmpty == trueの場合，-1を返す．
        /// </summary>
        public int ListIndex {
            get {
                if (IsEmpty) {
                    return -1;
                } else if (!HasPrevSibling) {
                    return 0;
                } else {
                    var prevPara = PrevSibling as Paragraph;
                    if (prevPara == null) {
                        return 0;
                    } else {
                        if (prevPara.ListLevel > _listLevel) {
                            /// 同じレベルのリストまでスキップ
                            var prev = prevPara;
                            while (prev.HasPrevSibling && prev.ListLevel > _listLevel) {
                                prev = prev.PrevSibling as Paragraph;
                                if (prev == null) {
                                    return 0;
                                } else {
                                    if (prev.ListLevel < _listLevel) {
                                        return 0;
                                    } else if (prev.ListLevel == _listLevel) {
                                        if (prev.ListKind == _listKind) {
                                            return prev.ListIndex + 1;
                                        } else {
                                            return 0;
                                        }
                                    }
                                }
                            }
                            return 0;

                        } else if (prevPara.ListLevel < _listLevel){
                            return 0;
                        } else {
                            if (prevPara.ListKind == _listKind) {
                                return prevPara.ListIndex + 1;
                            } else {
                                return 0;
                            }
                        }
                    }
                }
            }
        }


        // ========================================
        // method
        // ========================================
        public override void Transfer(Flow flow) {
            base.Transfer(flow);

            var para = flow as Paragraph;
            if (para == null) {
                return;
            }
            para._paragraphKind = _paragraphKind;
            para._listKind = _listKind;
            para._listLevel = _listLevel;
            para._listState = _listState;
        }

        public override string ToPlainText(PlainTextWriterSettings settings) {
            var ret = new StringBuilder();

            var indent = StringUtil.Repeat("    ", _listLevel);

            var first = true;
            switch (_listKind) {
                case ListKind.None:
                    foreach (var line in LineSegments) {
                        ret.Append(indent + line.ToPlainText(settings));
                    }
                    break;
                case ListKind.Unordered:
                case ListKind.CheckBox:
                case ListKind.TriStateCheckBox:
                case ListKind.Star:
                case ListKind.LeftArrow:
                case ListKind.RightArrow:
                    {
                        var bullet = string.Empty;
                        switch (_listKind) {
                            case ListKind.Unordered:
                                bullet = settings.UnorderedListBullet;
                                break;
                            case ListKind.Star:
                                bullet = settings.StarListBullet;
                                break;
                            case ListKind.LeftArrow:
                                bullet = settings.LeftArrowListBullet;
                                break;
                            case ListKind.RightArrow:
                                bullet = settings.RightArrowListBullet;
                                break;
                            case ListKind.CheckBox:
                            case ListKind.TriStateCheckBox:
                                if (_listState == ListStateKind.Checked) {
                                    bullet = settings.CheckBoxCheckedListBullet;
                                } else if (_listState == ListStateKind.Indeterminate) {
                                    bullet = settings.CheckBoxIndeterminateListBullet;
                                } else {
                                    bullet = settings.CheckBoxUncheckedListBullet;
                                }
                                break;

                        }
                        var space = StringUtil.Repeat(" ", bullet.Length);
                        foreach (var line in LineSegments) {
                            if (first && !IsEmpty) {
                                ret.Append(indent + bullet + line.ToPlainText(settings));
                                first = false;
                            } else {
                                ret.Append(indent + space + line.ToPlainText(settings));
                            }
                        }
                    }
                    break;
                case ListKind.Ordered:
                    {
                        /// BlocksAndInlines形式だとListIndexが必ず0になってしまう
                        var bullet = string.Empty;
                        var nobullet = string.Empty;
                        if (Root != null) {
                            bullet = "  " + (ListIndex + 1) + ".";
                            nobullet = StringUtil.Repeat(" ", bullet.Length);
                        } else {
                            bullet = " # ";
                            nobullet = "   ";
                        }
                        foreach (var line in LineSegments) {
                            if (first && !IsEmpty) {
                                ret.Append(indent + bullet + line.ToPlainText(settings));
                                first = false;
                            } else {
                                ret.Append(indent + nobullet + line.ToPlainText(settings));
                            }
                        }
                    }
                    break;
            }

            return ret.ToString();
        }

        private string GetHtmlText(string startTag, string endTag) {
            var ret = new StringBuilder();
            ret.Append(startTag);
            foreach (var line in LineSegments) {
                ret.Append(line.ToHtmlText());
            }
            ret.Append(endTag);
            return ret.ToString();
        }
    
        public override string ToHtmlText() {
            var ret = new StringBuilder();

            switch (_paragraphKind) {
                case ParagraphKind.Heading1:
                    ret.Append(GetHtmlText("<h1>", "</h1>"));
                    break;
                case ParagraphKind.Heading2:
                    ret.Append(GetHtmlText("<h2>", "</h2>"));
                    break;
                case ParagraphKind.Heading3:
                    ret.Append(GetHtmlText("<h3>", "</h3>"));
                    break;
                case ParagraphKind.Heading4:
                    ret.Append(GetHtmlText("<h4>", "</h4>"));
                    break;
                case ParagraphKind.Heading5:
                    ret.Append(GetHtmlText("<h5>", "</h5>"));
                    break;
                case ParagraphKind.Heading6:
                    ret.Append(GetHtmlText("<h6>", "</h6>"));
                    break;
            }

            if (_paragraphKind == ParagraphKind.Normal) {
                switch (_listKind) {
                    case ListKind.None:
                        ret.Append(GetHtmlText("<p>", "</p>"));
                        break;
                    case ListKind.Unordered:
                    case ListKind.Ordered:
                    case ListKind.Star:
                    case ListKind.LeftArrow:
                    case ListKind.RightArrow:
                    case ListKind.CheckBox:
                    case ListKind.TriStateCheckBox:
                        if (IsEmpty) {
                            ret.Append("<li style=\"list-style-type: none;\">");
                            ret.Append("</li>");
                        } else {
                            ret.Append("<li>");
                            foreach (var line in LineSegments) {
                                ret.Append(line.ToHtmlText());
                            }
                            ret.Append("</li>");
                        }
                        break;
                }
            }

            return ret.ToString();
        }

        public ListStateKind GetNextListState() {
            if (_listKind != ListKind.CheckBox && _listKind != ListKind.TriStateCheckBox) {
                return ListStateKind.Unchecked;
            }

            switch (_listState) {
                case ListStateKind.Unchecked:
                    return _listKind == ListKind.TriStateCheckBox ? ListStateKind.Indeterminate : ListStateKind.Checked;
                case ListStateKind.Indeterminate:
                    return ListStateKind.Checked;
                case ListStateKind.Checked:
                    return ListStateKind.Unchecked;
                default:
                    return ListStateKind.Unchecked;
            }
        }


        // ------------------------------
        // internal
        // ------------------------------
        internal Insets GetDefaultPadding(ParagraphKind paraKind) {
            switch (paraKind) {
                case ParagraphKind.Heading1:
                case ParagraphKind.Heading2:
                case ParagraphKind.Heading3:
                case ParagraphKind.Heading4:
                case ParagraphKind.Heading5:
                case ParagraphKind.Heading6:
                    return new Insets(2, 8, 2, 6);
                    //return new Insets(2, 6, 2, 6);
                case ParagraphKind.Normal:
                default:
                    return new Insets(2);
            }
        }

    }
}
