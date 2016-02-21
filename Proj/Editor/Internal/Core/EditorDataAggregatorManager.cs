/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.DataType;
using Mkamo.Editor.Core;

namespace Mkamo.Editor.Internal.Core {
    using FormatAndAggregator = Tuple<string, EditorDataAggregator>;

    internal class EditorDataAggregatorManager: IEditorDataAggregatorManager {
        // ========================================
        // field
        // ========================================
        private List<FormatAndAggregator> _formatAndAggregators;

        // ========================================
        // constructor
        // ========================================
        public EditorDataAggregatorManager() {
            _formatAndAggregators = new List<FormatAndAggregator>();
        }

        // ========================================
        // property
        // ========================================
        public IEnumerable<string> Formats {
            get {
                foreach (var item in _formatAndAggregators) {
                    yield return item.Item1;
                }
            }
        }

        // ========================================
        // method
        // ========================================
        public bool HasAggretator(string format) {
            foreach (var item in _formatAndAggregators) {
                if (item.Item1 == format) {
                    return true;
                }
            }
            return false;
        }

        public EditorDataAggregator FindAggregator(string format) {
            foreach (var item in _formatAndAggregators) {
                if (item.Item1 == format) {
                    return item.Item2;
                }
            }
            return null;
        }

        public void RegisterAggregator(string format, EditorDataAggregator aggregator) {
            if (!_formatAndAggregators.Any(item => item.Item1 == format)) {
                _formatAndAggregators.Add(Tuple.Create(format, aggregator));
            }
        }
    }
}
#endif
