/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Mkamo.Common.Forms.Core {
    [Serializable]
    [TypeConverter(typeof(ShortcutKeyConverter))]
    public struct ShortcutKey {
        // ========================================
        // field
        // ========================================
        private Keys _prefix;
        private Keys _key;

        // ========================================
        // constructor
        // ========================================
        public ShortcutKey(Keys prefix, Keys key) {
            _prefix = prefix;
            _key = key;
        }

        public ShortcutKey(Keys key): this(Keys.None, key) {
        }

        // ========================================
        // property
        // ========================================
        public Keys Prefix {
            get { return _prefix; }
        }

        public Keys Key {
            get { return _key; }
        }

        // ========================================
        // method
        // ========================================
        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != GetType()) {
                return false;
            }

            var sk = (ShortcutKey) obj;
            return (_prefix == sk._prefix) && (_key == sk._key);
        }

        public override int GetHashCode() {
            return _prefix.GetHashCode() ^ _key.GetHashCode();
        }
    }

    public class ShortcutKeyConverter: TypeConverter {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
            var str = (string) value;
            var keys = str.Split(' ');
            switch (keys.Length) {
                case 1: {
                    var key = (Keys) Enum.Parse(typeof(Keys), keys[0]);
                    return new ShortcutKey(key);
                }
                case 2: {
                    var prefix = (Keys) Enum.Parse(typeof(Keys), keys[0]);
                    var key = (Keys) Enum.Parse(typeof(Keys), keys[1]);
                    return new ShortcutKey(prefix, key);
                }
                default:
                    return new ShortcutKey();

            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
            var sk = (ShortcutKey) value;
            if (sk.Prefix == Keys.None) {
                return sk.Key.ToString();
            } else {
                return sk.Prefix.ToString() + " " + sk.Key.ToString();
            }
        }
    }
}
