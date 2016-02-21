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

namespace Mkamo.Control.SelectorDropDown {
    [ToolboxItem(false)]
    public class SelectorDropDown: ToolStripDropDown {
        // ========================================
        // constructor
        // ========================================
        public SelectorDropDown() {
        }

        // ========================================
        // property
        // ========================================
        public override System.Drawing.Font Font {
            get { return base.Font; }
            set {
                base.Font = value;
                foreach (ToolStripControlHost item in Items) {
                    var c = item.Control as SelectorCategory;
                    c.Font = value;
                }
            }
        }

        // ========================================
        // method
        // ========================================
        public void AddCategory(SelectorCategory cate) {
            cate.Dock = DockStyle.Top;
            var item = new ToolStripControlHost(cate);
            item.Margin = Padding.Empty;
            Items.Add(item);
        }
    }
}
