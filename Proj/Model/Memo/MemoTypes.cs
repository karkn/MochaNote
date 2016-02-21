/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Model.Memo {
    [Serializable]
    public enum MemoEdgeKind {
        Normal = 0,
        Arrow, /// deprecated, MemoEdge#Kindで使われないように対応
        Orthogonal,
        OrthogonalArrow, /// deprecated, MemoEdge#Kindで使われないように対応
        Central,
        OrthogonalMidpoint,
    }

    //[Serializable]
    //public enum MemoEdgeDashStyle {
    //    Solid = 0,
    //    Dash = 1,
    //    Dot = 2,
    //    DashDot = 3,
    //    DashDotDot = 4,
    //    Custom = 5,
    //}

    [Serializable]
    public enum MemoEdgeCapKind {
        Normal = 0,
        Arrow,
        // Diamond,
        // Round,
        // Triangle,
    }

    [Serializable]
    public enum MemoShapeKind {
        Rect = 0,

        RoundRect,
        Triangle,
        Ellipse,
        Diamond,
        Parallelogram,
        Cylinder,
        Paper,

        RightArrow,
        LeftArrow,
        UpArrow,
        DownArrow,

        LeftRightArrow,
        UpDownArrow,

        Pentagon,
        Chevron,

        Equal,
        NotEqual,
        Plus,
        Minus,
        Times,
        Devide,
    }

    [Serializable]
    public enum MemoMarkLocation {
        LeftTop,
        RightTop,
        // LeftBottom,
        // RightBottom,
    }

    [Serializable]
    public enum MemoDateKind {
        Created,
        Modified,
        Accessed,
    }

    [Serializable]
    public enum MemoImportanceKind {
        Normal = 0,
        High = 1,
        Low = -1,
    }

    [Serializable]
    public enum MemoConditionCompoundKind {
        Any,
        All,
    }

    [Serializable]
    public enum MemoMarkKind {
        Important, /// 重要

        NeedConfirm,  /// 要確認
        Unconfirmed, /// 未確認
        Completed, /// 完了
        Cancel, /// キャンセル

        Task, /// タスク
        Idea, /// アイデア
        //Research, /// 調べ物
        Reading, /// 読書
        Report, /// レポート
        //Experience, /// 経験
        //Work, /// 作業
        Web, /// Web
        Article, /// 記事

        Telephone, /// 電話
        //Call, /// 訪問
        Mail, /// メール
        Fax, /// ファックス
        Post, /// 郵送
        Map, /// 地図

        //Address, /// 住所
        //Contact, /// 連絡先

        Meeting, /// 打ち合わせ
        //Conference, /// 会議
        BusinessTrip, /// 出張
        Visitor, /// 来客

        Shopping, /// 買い物

        Cup, /// カップ
        Glass, /// グラス
        Currency, /// お金
        Calendar, /// カレンダー
        Game, /// ゲーム

    }

    [Serializable]
    public struct MemoRecentTimeSpan {
        private MemoDateKind _dateKind;
        private int _day;
        private int _week;
        private int _month;

        public MemoDateKind DateKind {
            get { return _dateKind; }
            set { _dateKind = value; }
        }

        public int Day {
            get { return _day; }
            set { _day= value; }
        }

        public int Week {
            get { return _week; }
            set { _week = value; }
        }

        public int Month {
            get { return _month; }
            set { _month = value; }
        }

        public DateTime FromDate {
            get {
                var ret = DateTime.Today;

                if (_day > 0) {
                    ret = ret.AddDays(-_day);
                }
                if (_week > 0) {
                    ret = ret.AddDays(-_week * 7);
                }
                if (_month > 0) {
                    ret = ret.AddMonths(-_month);
                }

                return ret;
            }
        }

        public DateTime ToDate {
            get {
                var ret = DateTime.Today;
                ret = ret.AddDays(1);
                return ret;
            }
        }
    }

    [Serializable]
    public struct MemoTimeSpan {
        private MemoDateKind _dateKind;
        private DateTime _from;
        private DateTime _to;

        public MemoTimeSpan(MemoDateKind dateKind, DateTime from, DateTime to) {
            _dateKind = dateKind;
            _from = from;
            _to = to;
        }

        public MemoDateKind DateKind {
            get { return _dateKind; }
            set { _dateKind = value; }
        }

        public DateTime FromDate {
            get { return _from; }
            set { _from = value; }
        }

        public DateTime ToDate {
            get { return _to; }
            set { _to = value; }
        }

    }

}
