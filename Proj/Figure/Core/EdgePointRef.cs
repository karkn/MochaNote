/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;

namespace Mkamo.Figure.Core {
    /// <summary>
    /// IEdgeが持つEdgePointへの参照を表す．
    /// IEdgeが持つEdgePointが追加・削除されてindexがずれても，
    /// 常に同じEdgePointへの参照を保持するために利用．
    /// </summary>
    public class EdgePointRef {
        // ========================================
        // field
        // ========================================
        private IEdge _owner;
        private int _index;

        // ========================================
        // constructor
        // ========================================
        public EdgePointRef(IEdge owner, int index) {
            _owner = owner;
            _index = index;
        }

        // ========================================
        // property
        // ========================================
        public Point EdgePoint {
            get { return _owner[_index]; }
            set { _owner[_index] = value; }
        }
        public int Index {
            get { return _index; }
            set { _index = value; }
        }

        public bool IsFirst {
            get { return _index == 0; }
        }

        public bool IsLast {
            get { return _index == _owner.EdgePointCount - 1; }
        }

        public bool HasPrevEdgePointReference {
            get { return _index > 0; }
        }

        public bool HasNextEdgePointReference {
            get { return _index < _owner.EdgePointCount - 1; }
        }

        public EdgePointRef Prev {
            get {
                if (_index <= 0) {
                    throw new ArgumentException("前のEdgePointRefはありません");
                }
                return _owner.EdgePointRefs.ElementAt(_index - 1);
            }
        }

        public EdgePointRef Next {
            get {
                if (_index >= _owner.EdgePointRefs.Count() - 1) {
                    throw new ArgumentException("次のEdgePointRefはありません");
                }
                return _owner.EdgePointRefs.ElementAt(_index + 1);
            }
        }
    }
}
