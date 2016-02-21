/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using Mkamo.Editor.Controllers;
using Mkamo.Figure.Figures;
using Mkamo.Editor.Utils.CategorizedListBox.Models;
using Mkamo.Common.Event;
using Mkamo.Editor.Handles;
using Mkamo.Editor.Roles;
using Mkamo.Common.Forms.Drawing;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Editor.Utils.CategorizedListBox.Handles;

namespace Mkamo.Editor.Utils.CategorizedListBox.Controllers {
    internal class ListItemController: AbstractController {
        // ========================================
        // property
        // ========================================
        // ------------------------------
        // protected
        // ------------------------------
        protected ListItem _ListItemModel {
            get { return Model as ListItem; }
        }

        // ========================================
        // method
        // ========================================
        public override void ConfigureEditor(IEditor editor) {
            editor.InstallEditorHandle(new SelectEditorHandle());
            editor.InstallHandle(new SelectionIndicatingHandle());
            editor.InstallRole(new SelectRole());
            editor.InstallRole(new HighlightRole());
        }

        public override IFigure CreateFigure(object model) {
            return new SimpleRect() {
                Padding = new Insets(8, 4, 4, 4),
                IsForegroundEnabled = false,
                IsBackgroundEnabled = false,
            };
        }

        public override void RefreshEditor(RefreshContext context, IFigure figure, object model) {
            var label = figure as SimpleRect;
            var listItem = model as ListItem;
            label.Text = listItem.Text;
        }

        public override Mkamo.Common.Externalize.IMemento GetModelMemento() {
            throw new NotImplementedException();
        }

        public override string GetText() {
            return _ListItemModel.Text;
        }
    }
}
