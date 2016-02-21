/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Mkamo.Common.Event;
using Mkamo.Common.Collection;

namespace Mkamo.Editor.Utils.CategorizedListBox.Models {
    public class Category: DetailedNotifyPropertyChangedBase {
        // ========================================
        // field
        // ========================================
        private string _name;
        private NotifyChangeList<ListItem> _listItems;

        // ========================================
        // constructor
        // ========================================
        public Category(): this(string.Empty) {
        }

        public Category(string name) {
            _name = name;
            _listItems = new NotifyChangeList<ListItem>(new List<ListItem>());
            _listItems.EventSender = this;
            _listItems.EventPropertyName = "ListItems";
            _listItems.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);
        }

        // ========================================
        // property
        // ========================================
        public string Name {
            get { return _name; }
            set {
                if (value == _name) {
                    return;
                }
                string old = _name;
                _name = value;
                OnPropertySet(this, "Name", old, _name);
            }
        }

        public IList<ListItem> ListItems {
            get { return _listItems; }
        }
    }
}
