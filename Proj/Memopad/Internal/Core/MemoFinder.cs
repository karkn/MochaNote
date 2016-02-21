/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Core;
using Mkamo.Container.Query;
using Mkamo.Common.Diagnostics;
using Mkamo.Common.String;
using Mkamo.Model.Memo;
using Mkamo.Common.Core;
using Mkamo.Memopad.Internal.Controls;
using Mkamo.Memopad.Internal.Utils;

namespace Mkamo.Memopad.Internal.Core {
    internal class MemoFinder {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;

        // ========================================
        // constructor
        // ========================================
        public MemoFinder() {
            _facade = MemopadApplication.Instance;
        }


        // ========================================
        // method
        // ========================================
        /// <summary>
        /// すべてのメモからqueryに適合するものだけを返す。
        /// </summary>
        public IEnumerable<MemoInfo> Search(MemoQuery query) {
            return Search(_facade.MemoInfos, query);
        }

        /// <summary>
        /// infos内からqueryに適合するものだけを返す。
        /// </summary>
        public IEnumerable<MemoInfo> Search(IEnumerable<MemoInfo> infos, MemoQuery query) {
            if (query == null || query.IsEmptyQuery) {
                return new List<MemoInfo>();
            }

            var matched = infos;

            /// タイトル
            if (query.NarrowByTitle) {
                matched = matched.Where(
                    info => info.Title.IndexOf(query.Title, StringComparison.OrdinalIgnoreCase) > -1
                );
            }

            /// タグ
            if (query.NarrowByTagIds) {
                var tags = GetTags(query);
                var noTag = query.NoTag;
                var compKind = query.TagCompoundKind;
                Func<MemoInfo, bool> tagFilter = info => {
                    var memo = _facade.Container.Find<Memo>(info.MemoId);
                    return
                        /// noTagかつ「allでタグを指定」を満たすことはあり得ないので，noTagは常にorでつなぐだけにしておく
                        (noTag && !memo.Tags.Any()) ||
                        (compKind == MemoConditionCompoundKind.Any && memo.Tags.ContainsAny(tags)) ||
                        (compKind == MemoConditionCompoundKind.All && memo.Tags.ContainsAll(tags));
                };
                matched = matched.Where(tagFilter);
            }

            /// 重要度
            if (query.NarrowByImportanceKind) {
                var importances = query.ImportanceKinds;
                Func<MemoInfo, bool> importanceFilter = info => {
                    var memo = _facade.Container.Find<Memo>(info.MemoId);
                    return importances.Contains(memo.Importance);
                };
                matched = matched.Where(importanceFilter);
            }


            /// recent
            if (query.NarrowByRecentTimeSpan) {
                var recent = query.RecentTimeSpan;
                var filter = default(Func<MemoInfo, bool>);
                switch (recent.DateKind) {
                    case MemoDateKind.Created: {
                        filter = info => {
                            var memo = _facade.Container.Find<Memo>(info.MemoId);
                            return
                                memo.CreatedDate >= recent.FromDate &&
                                memo.CreatedDate < recent.ToDate;
                        };
                        break;
                    }
                    case MemoDateKind.Modified: {
                        filter = info => {
                            var memo = _facade.Container.Find<Memo>(info.MemoId);
                            return
                                memo.ModifiedDate >= recent.FromDate &&
                                memo.ModifiedDate < recent.ToDate;
                        };
                        break;
                    }
                    case MemoDateKind.Accessed: {
                        filter = info => {
                            var memo = _facade.Container.Find<Memo>(info.MemoId);
                            return
                                memo.AccessedDate >= recent.FromDate &&
                                memo.AccessedDate < recent.ToDate;
                        };
                        break;
                    }
                }
                matched = matched.Where(filter);
            }

            /// 期間
            if (query.NarrowByTimeSpan) {
                var timeSpan = query.TimeSpan;
                var filter = default(Func<MemoInfo, bool>);
                switch (timeSpan.DateKind) {
                    case MemoDateKind.Created: {
                        filter = info => {
                            var memo = _facade.Container.Find<Memo>(info.MemoId);
                            return
                                memo.CreatedDate >= timeSpan.FromDate &&
                                memo.CreatedDate < timeSpan.ToDate;
                        };
                        break;
                    }
                    case MemoDateKind.Modified: {
                        filter = info => {
                            var memo = _facade.Container.Find<Memo>(info.MemoId);
                            return
                                memo.ModifiedDate >= timeSpan.FromDate &&
                                memo.ModifiedDate < timeSpan.ToDate;
                        };
                        break;
                    }
                    case MemoDateKind.Accessed: {
                        filter = info => {
                            var memo = _facade.Container.Find<Memo>(info.MemoId);
                            return
                                memo.AccessedDate >= timeSpan.FromDate &&
                                memo.AccessedDate < timeSpan.ToDate;
                        };
                        break;
                    }
                }
                matched = matched.Where(filter);
            }

            /// マーク
            if (query.NarrowByMarkKinds) {
                var noMarkKind = query.NoMarkKind;
                Func<MemoInfo, bool> markFilter = info => {
                    var memo = _facade.Container.Find<Memo>(info.MemoId);
                    var opened = _facade.FindPageContent(info);

                    var markIds = default(IEnumerable<string>);
                    if (opened == null) {
                        /// 開かれていなければキャッシュを見る
                        markIds = MemoMarkUtil.LoadMarkIdsCache(memo);
                        if (markIds == null) {
                            markIds = MemoMarkUtil.GetMemoMarkIds(memo);
                            MemoMarkUtil.SaveMarkIdsCache(memo, markIds);
                        }
                    } else {
                        markIds = MemoMarkUtil.GetMemoMarkIds(memo);
                    }

                    if (noMarkKind && !markIds.Any()) {
                        return true;
                    }
                    var marks = MemoMarkUtil.GetMarks(markIds);
                    var markKinds = marks.Select(mark => mark.Kind).ToArray();
                    var compKind = query.MarkCompoundKind;
                    return
                        (compKind == MemoConditionCompoundKind.Any && query.MarkKinds.Any(kind => markKinds.Contains(kind))) ||
                        (compKind == MemoConditionCompoundKind.All && query.MarkKinds.All(kind => markKinds.Contains(kind)));
                };
                matched = matched.Where(markFilter);
            }


            /// 全文検索
            if (!query.NarrowByKeywords) {
                return matched;

            } else {
                var ret = new List<MemoInfo>();

                var keywords = query.Condition.Split(' ');

                foreach (var info in matched.ToArray()) {
                    /// MemoInfoのFullTextを取得
                    var fullText = default(string);
                    var opened = _facade.FindPageContent(info);

                    if (opened == null) {
                        fullText = _facade.Container.LoadExtendedTextData(typeof(Memo), info.MemoId, "FullText");

                        if (fullText == null) {
                            /// 見つからなかった場合はノートをロードしてGetText()できるようにしておく
                            _facade.LoadMemo(info, true);
                            opened = _facade.FindPageContent(info);
                            if (opened != null) {
                                var canvas = opened.EditorCanvas;
                                _facade.Container.SaveExtendedTextData(
                                    canvas.EditorContent, "FullText", canvas.GetFullText()
                                );
                            }
                        }
                    }

                    if (opened != null && fullText == null) {
                        var canvas = opened.EditorCanvas;
                        fullText = canvas.GetFullText();
                    }

                    if (fullText != null) {
                        var matchFailed = false;
                        foreach (var keyword in keywords) {
                            //if (Regex.IsMatch(fullText, keyword, RegexOptions.IgnoreCase | RegexOptions.Multiline)) {
                            if (keyword.StartsWith("-")) {
                                /// "-"で始まる場合は除外キーワード
                                var realKeyword = keyword.Substring(1); /// "-"を除去
                                if (fullText.IndexOf(realKeyword, StringComparison.OrdinalIgnoreCase) > -1) {
                                    matchFailed = true;
                                    break;
                                }
                            } else {
                                /// "-"で始まらなければ普通のキーワード
                                if (fullText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) < 0) {
                                    matchFailed = true;
                                    break;
                                }
                            }
                        }
                        if (!matchFailed) {
                            ret.Add(info);
                        }
                    }
                }

                return ret;
            }
        }

        private IEnumerable<MemoTag> GetTags(MemoQuery query) {
            var ret = new List<MemoTag>();

            if (query.TagIds == null) {
                return ret;
            }

            foreach (var id in query.TagIds) {
                var tag = _facade.Container.Find<MemoTag>(id);
                if (tag != null) {
                    ret.Add(tag);
                }
            }

            return ret;
        }

    }
}
