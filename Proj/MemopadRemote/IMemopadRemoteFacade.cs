/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Memopad.Remote {
    public interface IMemopadRemoteFacade {
        // ========================================
        // method
        // ========================================
        void Ping();
        string CreateMemo(string title);
        bool LoadMemo(string memoId);
        bool RemoveMemo(string memoId);
        bool ExistsMemo(string memoId);
        string GetMemoTitle(string memoId);
        string[] GetMemoIds();
    }
}
