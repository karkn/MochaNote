/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Container.Core;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;
using Mkamo.Common.DataType;
using System.Runtime.Serialization;

namespace Mkamo.Model.Memo {
    [Externalizable(Type = typeof(MemoShape), FactoryMethodType = typeof(MemoFactory), FactoryMethod = "CreateShape")]
    [DataContract, Serializable]
    public class MemoShape: MemoNode {
        // ========================================
        // field
        // ========================================
        [DataMember(Name = "Kind")]
        private MemoShapeKind _kind;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoShape(MemoShapeKind kind) {
            _kind = kind;
            StyledText.VerticalAlignment = VerticalAlignment.Center;
            if (StyledText.Blocks.Any()) {
                StyledText.Blocks.First().HorizontalAlignment = HorizontalAlignment.Center;
            }
        }

        protected internal MemoShape(): this(MemoShapeKind.Rect) {
        }

        // ========================================
        // property
        // ========================================
        [Persist, External]
        public virtual MemoShapeKind Kind {
            get { return _kind; }
            set {
                if (value == _kind) {
                    return;
                }
                var old = _kind;
                _kind = value;
                OnPropertySet(this, "Kind", old, value);
            }
        }

        // ========================================
        // method
        // ========================================
    }

}
