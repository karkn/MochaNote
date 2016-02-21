/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Model.Memo;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Editor.Core;
using System.IO;
using Mkamo.Memopad.Properties;

namespace Mkamo.Memopad.Internal.Utils {
    internal static class MemoMarkUtil {
        private static List<MemoMarkDefinition> MarkDefinitions;

        static MemoMarkUtil() {
            MarkDefinitions = new List<MemoMarkDefinition>();
            var def = default(MemoMarkDefinition);

            def = new MemoMarkDefinition(MemoMarkKind.Important, "重要", Resources.star, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.NeedConfirm, "要確認", Resources.exclamation, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Unconfirmed, "未確認", Resources.question, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Completed, "完了", Resources.check, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Cancel, "キャンセル", Resources.cross, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Idea, "アイデア", Resources.light_bulb, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Task, "タスク", Resources.task, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Reading, "読書", Resources.book, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Report, "レポート", Resources.report, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Web, "ウェブ", Resources.web, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Article, "記事", Resources.article, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Mail, "メール", Resources.email, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Telephone, "電話", Resources.telephone, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Fax, "FAX", Resources.telephone_fax, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Post, "郵送", Resources.envelope, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Map, "地図", Resources.map, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Calendar, "カレンダー", Resources.calendar, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Meeting, "打ち合わせ", Resources.users, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.BusinessTrip, "出張", Resources.briefcase, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Visitor, "来客", Resources.user_business, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Shopping, "買い物", Resources.cart, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Cup, "カップ", Resources.cup, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Glass, "グラス", Resources.glass, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Currency, "お金", Resources.currency, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

            def = new MemoMarkDefinition(MemoMarkKind.Game, "ゲーム", Resources.game, MemoMarkLocation.LeftTop);
            MarkDefinitions.Add(def);

        }


        // ------------------------------
        // public
        // ------------------------------
        public static IEnumerable<MemoMarkDefinition> GetMemoMarkDefinitions() {
            return MarkDefinitions;
        }

        public static MemoMarkDefinition GetMarkDefinition(MemoMark mark) {
            foreach (var def in MarkDefinitions) {
                if (mark.Kind == def.Kind) {
                    return def;
                }
            }
            return null;
        }

        /// <summary>
        /// memoに関連付けたMemoMarkのIdとしてidsをキャッシュする。
        /// </summary>
        public static void SaveMarkIdsCache(Memo memo, IEnumerable<string> ids) {
            var buf = new StringBuilder();
            foreach (var id in ids) {
                buf.AppendLine(id);
            }

            var container = MemopadApplication.Instance.Container;
            container.SaveExtendedTextData(memo, "MarkIds", buf.ToString());
        }

        /// <summary>
        /// キャッシュしたMemoMarkのIdを返す。
        /// キャッシュが作成されていない場合はnullを返す。
        /// </summary>
        public static IEnumerable<string> LoadMarkIdsCache(Memo memo) {
            var container = MemopadApplication.Instance.Container;
            var data = container.LoadExtendedTextData(memo, "MarkIds");
            if (data == null) {
                return null;
            }

            var ret = new List<string>();

            using (var reader = new StringReader(data)) {
                var line = default(string);
                while ((line = reader.ReadLine()) != null) {
                    if (!string.IsNullOrEmpty(line)) {
                        ret.Add(line);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// canvasに含まれるMemoMarkのIdを返す。
        /// </summary>
        //public static IEnumerable<string> GetMemoMarkIds(EditorCanvas canvas) {
        //    var ret = new List<string>();

        //    var container = MemopadFacade.Instance.Container;
        //    canvas.RootEditor.Accept(
        //        editor => {
        //            var content = editor.Model as MemoContent;
        //            if (content != null && content.IsMarkable) {
        //                foreach (var mark in content.Marks) {
        //                    ret.Add(container.GetId(mark));
        //                }
        //            }
        //            return false;
        //        }
        //    );

        //    return ret;
        //}


        public static IEnumerable<string> GetMemoMarkIds(Memo memo) {
            var ret = new List<string>();

            var container = MemopadApplication.Instance.Container;
            foreach (var mark in memo.Marks) {
                ret.Add(container.GetId(mark));
            }

            //foreach (var elem in memo.Contents.Items) {
            //    var content = elem as MemoContent;
            //    if (content != null && content.IsMarkable) {
            //        foreach (var mark in content.Marks) {
            //            ret.Add(container.GetId(mark));
            //        }
            //    }
            //}

            return ret;
        }

        public static IEnumerable<MemoMark> GetMarks(IEnumerable<string> markIds) {
            var ret = new List<MemoMark>();
            
            if (markIds == null) {
                return ret;
            }

            var container = MemopadApplication.Instance.Container;
            foreach (var id in markIds) {
                var mark = container.Find<MemoMark>(id);
                if (mark != null) {
                    ret.Add(mark);
                }
            }

            return ret;
        }
    }
}
