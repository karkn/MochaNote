/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Editor.Core {
    public interface ISelectionManager {
        // ========================================
        // property
        // ========================================
        IEnumerable<IEditor> SelectedEditors { get; }
        bool MultiSelect { get; set; }

        // ========================================
        // event
        // ========================================
        event EventHandler<EventArgs> SelectionChanged;

        // ========================================
        // method
        // ========================================
        void Select(IEditor selected);
        void SelectMulti(IEnumerable<IEditor> selecteds, bool toggle);
        void Deselect(IEditor selected);
        void DeselectAll();
    }
}
