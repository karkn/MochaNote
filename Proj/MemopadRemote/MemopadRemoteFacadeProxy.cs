/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Memopad.Remote {
    public class MemopadRemoteFacadeProxy: MarshalByRefObject, IMemopadRemoteFacade {
        // ========================================
        // field
        // ========================================
        private IMemopadRemoteFacade _facade;

        // ========================================
        // constructor
        // ========================================
        public MemopadRemoteFacadeProxy(IMemopadRemoteFacade facade) {
            _facade = facade;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override object InitializeLifetimeService() {
            /// リース切れで自動的に廃棄されないようにする
            return null;
        }

        public void Ping() {
            _facade.Ping();
        }

        public string CreateMemo(string title) {
            return _facade.CreateMemo(title);
        }

        public bool LoadMemo(string memoId) {
            return _facade.LoadMemo(memoId);
        }

        public bool RemoveMemo(string memoId) {
            return _facade.RemoveMemo(memoId);
        }

        public bool ExistsMemo(string memoId) {
            return _facade.ExistsMemo(memoId);
        }

        public string GetMemoTitle(string memoId) {
            return _facade.GetMemoTitle(memoId);
        }

        public string[] GetMemoIds() {
            return _facade.GetMemoIds();
        }
    }
}
