/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Container.Core {
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class PersistAttribute: Attribute {
        // ========================================
        // field
        // ========================================
        private bool _enabled;
        private bool _cascade;
        private string _add;
        private Type _composite;
        private bool _allowNull;

        // ========================================
        // constructor
        // ========================================
        public PersistAttribute(): this(true, false, null) {
        }

        public PersistAttribute(bool enabled, bool cascade, string elementAdder) {
            _enabled = enabled;
            _cascade = cascade;
            _add = elementAdder;

            _composite = null;
            _allowNull = false;
        }

        // ========================================
        // property
        // ========================================
        /// <summary>
        /// 親クラスでPersistされたpropertyをpersist対象外にするため．
        /// </summary>
        public bool Enabled {
            get { return _enabled; }
            set { _enabled = value; }
        }

        /// <summary>
        /// このプロパティを持つエンティティが削除されたときに
        /// このプロパティに格納されているプロパティも削除するかどうか
        /// </summary>
        public bool Cascade {
            get { return _cascade; }
            set { _cascade = value; }
        }

        public string Add {
            get { return _add; }
            set { _add = value; }
        }

        /// <summary>
        /// このプロパティを持つクラスのインスタンスが生成された時点で
        /// このプロパティのインスタンスを自動生成するかどうか
        /// </summary>
        public Type Composite {
            get { return _composite; }
            set { _composite = value; }
        }

        /// <summary>
        /// このプロパティにnullを格納することができるかどうか。
        /// IEnumerable，ICollectionのときのみ有効。
        /// null格納時にエラーにするのではなく，
        /// Load時にnullの項目を無視するだけ。
        /// </summary>
        public bool AllowNull {
            get { return _allowNull; }
            set { _allowNull = value; }
        }

    }
}
