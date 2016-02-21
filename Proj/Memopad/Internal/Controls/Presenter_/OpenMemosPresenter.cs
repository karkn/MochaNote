/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Internal.Core;

namespace Mkamo.Memopad.Internal.Controls {
    internal class OpenMemosPresenter: IMemoInfoListProvider {
        // ========================================
        // field
        // ========================================
        private bool _isForControlRemoved = false;

        // ========================================
        // constructor
        // ========================================
        public OpenMemosPresenter() {

        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public IEnumerable<MemoInfo> MemoInfos {
            get {
                var app = MemopadApplication.Instance;
                if (!app.IsMainFormLoaded) {
                    //return new MemoInfo[0];
                    yield break;
                }

                //return _isForControlRemoved ? app.MainForm._OpenMemoInfosForControlRemoved : app.MainForm.OpenMemoInfos;

                if (_isForControlRemoved) {
                    foreach (var info in app.MainForm._OpenMemoInfosForControlRemoved) {
                        yield return info;
                    }
                } else {
                    foreach (var info in app.MainForm.OpenMemoInfos) {
                        yield return info;
                    }
                }

                foreach (var info in app.FusenManager.OpenMemoInfos) {
                    yield return info;
                }

            }
        }

        public bool IsForControlRemoved {
            get { return _isForControlRemoved; }
            set { _isForControlRemoved = value; }
        }
    }
}
