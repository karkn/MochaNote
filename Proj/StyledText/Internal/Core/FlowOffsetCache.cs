/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Internal.Core {
    internal class FlowOffsetCache<T> where T: Flow {
        // ========================================
        // field
        // ========================================
        private IList<T> _flows;
        private List<CacheItem> _items = new List<CacheItem>();

        // ========================================
        // constructor
        // ========================================
        internal FlowOffsetCache(IList<T> flows) {
            _flows = flows;
        }

        // ========================================
        // property
        // ========================================
        public int Length {
            get {
                CacheAll();
                return _items.Sum(item => item.Length);
            }
        }

        public List<CacheItem> Items {
            get {
                CacheAll();
                return _items;
            }
        }

        // ========================================
        // method
        // ========================================
        public int GetOffset(T flow) {
            return GetItem(flow).Offset;
        }

        public int GetOffset(int index) {
            return GetItem(index).Offset;
        }

        public int GetLength(T flow) {
            return GetItem(flow).Length;
        }

        public int GetLength(int index) {
            return GetItem(index).Length;
        }

        public void Invalidate(int index) {
            for (int i = _items.Count - 1; i >= index; --i) {
                _items.RemoveAt(i);
            }
        }

        public void Clear() {
            _items.Clear();
        }

        public void Invalidate(T flow) {
            Invalidate(_flows.IndexOf(flow));
        }

        public CacheItem GetItem(T flow) {
            var index = _flows.IndexOf(flow);
            if (index < 0) {
                throw new ArgumentException("flow");
            }

            return GetItem(index);
        }

        public CacheItem GetItem(int index) {
            CacheAll();

            if (index < 0 || index > _items.Count - 1) {
                throw new ArgumentOutOfRangeException("index");
            }

            return _items[index];
        }

        
        public void CacheAll() {
            if (_flows.Count > _items.Count) {
                var cOffset = 0;
                if (_items.Count > 0) {
                    var prevItem = _items[_items.Count - 1];
                    cOffset = prevItem.Offset + prevItem.Length;
                }

                for (int i = _items.Count, cnt = _flows.Count; i < cnt; ++i) {
                    var len = _flows[i].Length;
                    var item = new CacheItem(cOffset, len);
                    _items.Add(item);
                    cOffset += len;
                }
            }
        }




        // ========================================
        // class
        // ========================================
        internal struct CacheItem: IEquatable<CacheItem> {
            // ========================================
            // static method
            // ========================================
            public static bool operator ==(CacheItem a, CacheItem b) {
                return a.Equals(b);
            }

            public static bool operator !=(CacheItem a, CacheItem b) {
                return !a.Equals(b);
            }

            // ========================================
            // field
            // ========================================
            public int _offset;
            public int _length;

            // ========================================
            // constructor
            // ========================================
            public CacheItem(int offset, int length) {
                _offset = offset;
                _length = length;
            }

            // ========================================
            // property
            // ========================================
            public int Offset {
                get { return _offset; }
            }
            public int Length {
                get { return _length; }
            }

            // ========================================
            // method
            // ========================================
            public override bool Equals(object obj) {
                if (obj is CacheItem) {
                    return Equals((CacheItem) obj);
                } else {
                    return false;
                }
            }

            public override int GetHashCode() {
                return _offset ^ _length;
            }

            public bool Equals(CacheItem other) {
                return _offset == other._offset && _length == other._length;
            }
        }
    }
}
