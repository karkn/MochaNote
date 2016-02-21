/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Editor.Core {
    public interface IEditorDataAggregatorManager {
        // ========================================
        // property
        // ========================================
        IEnumerable<string> Formats { get; }

        // ========================================
        // method
        // ========================================
        bool HasAggretator(string format);
        EditorDataAggregator FindAggregator(string format);
        void RegisterAggregator(string format, EditorDataAggregator aggregator);
    }
}
#endif
