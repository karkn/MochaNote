/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Utils.CategorizedListBox.Controllers;
using Mkamo.Editor.Utils.CategorizedListBox.Models;
using Mkamo.Figure.Layouts;
using System.ComponentModel;
using Mkamo.Common.Forms.Drawing;
using System.Drawing;

namespace Mkamo.Editor.Utils.CategorizedListBox {
    [ToolboxItem(false)]
    public class CategorizedListBox: EditorCanvas {
        // ========================================
        // field
        // ========================================
        private CategorizedList _categorizedList;

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CategorizedList CategorizedList {
            get { return _categorizedList; }
        }

        // ========================================
        // method
        // ========================================
        protected override void ConfigureEditorCanvas() {
            base.ConfigureEditorCanvas();
            EnableAutoScroller = false;
            EnableAutoAdjustRootFigureSize = false;
            BackColor = SystemColors.Control;
            //CanvasPadding = Insets.Empty;
            ControllerFactory = new CategorizedListBoxControllerFactory();
            EditorContent = _categorizedList = new CategorizedList();
            MultiSelect = false;
        }
    }
}
