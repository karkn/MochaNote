/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Remote;

namespace Mkamo.Memopad.Internal.Core {
    internal class MemopadRemoteFacade: IMemopadRemoteFacade {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;

        // ========================================
        // constructor
        // ========================================
        public MemopadRemoteFacade() {
            _facade = MemopadApplication.Instance;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public void Ping() {
        }

        public string CreateMemo(string title) {
            var ret = _facade.MainForm.Invoke(
                (Func<string>)(() => {
                    var info = _facade.CreateMemo(title);
                    return info.MemoId;
                })
            );
            return ret as string;
        }

        public bool LoadMemo(string memoId) {
            var ret = _facade.MainForm.Invoke(
                (Func<bool>)(() => {
                    var info = _facade.FindMemoInfoByMemoId(memoId);
                    return _facade.LoadMemo(info);
                })
            );

            if (ret is bool) {
                return (bool) ret;
            } else {
                return false;
            }
        }

        public bool RemoveMemo(string memoId) {
            var ret = _facade.MainForm.Invoke(
                (Func<bool>)(() => {
                    var info = _facade.FindMemoInfoByMemoId(memoId);
                    return _facade.RemoveMemo(info);
                })
            );

            if (ret is bool) {
                return (bool) ret;
            } else {
                return false;
            }
        }

        public bool ExistsMemo(string memoId) {
            var ret = _facade.MainForm.Invoke(
                (Func<bool>)(() => {
                    var info = _facade.FindMemoInfoByMemoId(memoId);
                    return info != null;
                })
            );

            if (ret is bool) {
                return (bool) ret;
            } else {
                return false;
            }
        }

        public string GetMemoTitle(string memoId) {
            var ret = _facade.MainForm.Invoke(
                (Func<string>)(() => {
                    var info = _facade.FindMemoInfoByMemoId(memoId);
                    return info == null? null: info.Title;
                })
            );
            return ret as string;
        }

        public string[] GetMemoIds() {
            var ret = _facade.MainForm.Invoke(
                (Func<string[]>)(() => {
                    var infos = _facade.MemoInfos;
                    return infos.Select(info => info.MemoId).ToArray();
                })
            );
            return ret as string[];
        }


    }
}
