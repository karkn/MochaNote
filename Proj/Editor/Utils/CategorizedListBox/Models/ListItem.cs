/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Mkamo.Common.Event;
using System.Drawing;

namespace Mkamo.Editor.Utils.CategorizedListBox.Models {
    public class ListItem: DetailedNotifyPropertyChangedBase {
        // ========================================
        // field
        // ========================================
        private string _text;
        private Image _icon;
        private object _data;

        // ========================================
        // constructor
        // ========================================
        public ListItem(): this(null, null) {
        }

        public ListItem(string text): this(text, null) {
        }

        public ListItem(string text, Image icon): base() {
            _text = text;
            _icon = icon;
        }

        // ========================================
        // property
        // ========================================
        public string Text {
            get { return _text; }
            set {
                if (value == _text) {
                    return;
                }
                string old = _text;
                _text = value;
                OnPropertySet(this, "Text", old, value);
            }
        }

        public Image Icon {
            get { return _icon; }
            set {
                if (value == _icon) {
                    return;
                }
                Image old = _icon;
                _icon = value;
                OnPropertySet(this, "Icon", old, value);
            }
        }

        public object Data {
            get { return _data; }
            set {
                if (value == _data) {
                    return;
                }
                object old = _data;
                _data = value;
                OnPropertySet(this, "Data", old, value);
            }
        }
    }
}
