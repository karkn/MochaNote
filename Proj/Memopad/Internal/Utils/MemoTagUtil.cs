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

namespace Mkamo.Memopad.Internal.Utils {
    internal static class MemoTagUtil {
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
        public static IEnumerable<MemoTag> GetTags(IEnumerable<string> ids) {
            var facade = MemopadApplication.Instance;
            return ids.Select(id => facade.Container.Find<MemoTag>(id)).ToArray();
        }

        public static IEnumerable<string> GetIds(IEnumerable<MemoTag> tags) {
            var facade = MemopadApplication.Instance;
            return tags.Select(tag => facade.Container.GetId(tag)).ToArray();
        }
    }
}
