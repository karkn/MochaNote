/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Controllers;
using Mkamo.Figure.Core;
using Mkamo.Figure.Figures;
using Mkamo.Figure.Layouts;
using Mkamo.Editor.Utils.CategorizedListBox.Models;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Event;
using Mkamo.Common.Core;
using System.ComponentModel;

namespace Mkamo.Editor.Utils.CategorizedListBox.Controllers {
    public class CategorizedListController: AbstractController, IContainerController {
        // ========================================
        // constructor
        // ========================================
        public CategorizedListController() {
        }

        // ========================================
        // property
        // ========================================
        public IEnumerable<object> Children {
            get { return _CategorizedList.Categories.As<Category, object>(); }
        }

        public int ChildCount {
            get { return _CategorizedList.Categories.Count; }
        }

        public bool SyncChildEditors {
            get { return true; }
        }

        protected CategorizedList _CategorizedList {
            get { return Model as CategorizedList; }
        }

        // ========================================
        // method
        // ========================================
        public override IFigure CreateFigure(object model) {
            return new Layer() { Layout = new ListLayout(Insets.Empty) };
        }

        public override void RefreshEditor(RefreshContext context, IFigure figure, object model) {
            
        }

        public override void ConfigureEditor(IEditor editor) {
            
        }

        public override Mkamo.Common.Externalize.IMemento GetModelMemento() {
            throw new NotImplementedException();
        }

        public bool CanContainChild(IModelDescriptor descriptor) {
            return typeof(Category).IsAssignableFrom(descriptor.ModelType);
        }

        public void InsertChild(object child, int index) {
            (Model as CategorizedList).Categories.Insert(index, child as Category);
        }

        public void RemoveChild(object child) {
            (Model as CategorizedList).Categories.Remove(child as Category);
        }

        public override string GetText() {
            return "";
        }
    }
}
