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
    public class CategorizedList: DetailedNotifyPropertyChangedBase {
        private NotifyChangeList<Category> _categories;

        public CategorizedList() {
            _categories = new NotifyChangeList<Category>(new List<Category>());
            _categories.EventSender = this;
            _categories.EventPropertyName = "Categories";
            _categories.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);
        }

        public IList<Category> Categories {
            get { return _categories; }
        }
    }
}
