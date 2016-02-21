/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Utils.CategorizedListBox.Models;

namespace Mkamo.Editor.Utils.CategorizedListBox.Controllers {
    public class CategorizedListBoxControllerFactory: IControllerFactory {
        // ========================================
        // method
        // ========================================
        public IController CreateController(object model) {
            if (model is CategorizedList) {
                return new CategorizedListController();
            } else if (model is Category) {
                return new CategoryController();
            } else if (model is ListItem) {
                return new ListItemController();
            }
            throw new ArgumentException("model");
        }
    }
}
