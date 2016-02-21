/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Controllers;
using Mkamo.Figure.Core;
using Mkamo.Editor.Utils.CategorizedListBox.Models;
using Mkamo.Figure.Figures;
using Mkamo.Editor.Core;
using Mkamo.Figure.Layouts;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using Mkamo.Common.Event;
using Mkamo.Common.Core;
using Mkamo.Editor.Utils.CategorizedListBox.Handles;
using System.ComponentModel;

namespace Mkamo.Editor.Utils.CategorizedListBox.Controllers {
    public class CategoryController: AbstractController, IContainerController {
        // ========================================
        // field
        // ========================================
        private SimpleRect _categoryLabel;

        // ========================================
        // constructor
        // ========================================
        public CategoryController() {
        }

        // ========================================
        // property
        // ========================================
        public IEnumerable<object> Children {
            get { return _Category.ListItems.As<ListItem, object>(); }
        }

        public int ChildCount {
            get { return _Category.ListItems.Count; }
        }

        public bool SyncChildEditors {
            get { return true; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected Category _Category {
            get { return Model as Category; }
        }


        // ========================================
        // method
        // ========================================
        public override IFigure CreateFigure(object model) {
            var catefig = new Layer();
            catefig.Layout = new ListLayout(Insets.Empty);
            
            _categoryLabel = new SimpleRect();
            _categoryLabel.Padding = new Insets(20, 2, 4, 2);
            _categoryLabel.Font = new FontDescription(_categoryLabel.Font, FontStyle.Bold);
            _categoryLabel.Foreground = SystemColors.ButtonShadow;
            _categoryLabel.Background = new GradientBrushDescription(
                SystemColors.ButtonShadow, SystemColors.ButtonFace
            );
            
            catefig.Children.Add(_categoryLabel);
            
            return catefig;
        }

        public override void RefreshEditor(RefreshContext context, IFigure figure, object model) {
            _categoryLabel.Text = _Category.Name;
        }

        public override void ConfigureEditor(IEditor editor) {
            editor.InstallHandle(new ChildrenFoldHandle(), HandleStickyKind.Always);
        }

        public override Mkamo.Common.Externalize.IMemento GetModelMemento() {
            throw new NotImplementedException();
        }

        public bool CanContainChild(IModelDescriptor descriptor) {
            return typeof(ListItem).IsAssignableFrom(descriptor.ModelType);
        }

        public void InsertChild(object child, int index) {
            _Category.ListItems.Insert(index, child as ListItem);
        }

        public void RemoveChild(object child) {
            _Category.ListItems.Remove(child as ListItem);
        }

        public override string GetText() {
            return _Category.Name;
        }
    }
}
