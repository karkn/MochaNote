/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Controls;
using Mkamo.Common.Forms.KeyMap;

namespace Mkamo.Memopad.Internal.Core {
    internal interface IKeyScheme {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================
        //KeyBinder<EditorCanvas> NoSelectionKeyBinder { get; }
        IKeyMap<EditorCanvas> NoSelectionKeyMap { get; }
        //KeyBinder<IEditor> MemoEditorKeyBinder { get; }
        IKeyMap<IEditor> MemoEditorKeyMap { get; }
        //KeyBinder<IFocus> MemoContentFocusKeyBinder { get; }
        IKeyMap<IFocus> MemoContentFocusKeyMap { get; }
        //KeyBinder<IFocus> MemoContentSingleLineFocusKeyBinder { get; }
        IKeyMap<IFocus> MemoContentSingleLineFocusKeyMap { get; }
        //KeyBinder<IEditor> MemoContentEditorKeyBinder { get; }
        IKeyMap<IEditor> MemoContentEditorKeyMap { get; }
        //KeyBinder<IEditor> MemoTableCellEditorKeyBinder { get; }
        IKeyMap<IEditor> MemoTableCellEditorKeyMap { get; }
        //KeyBinder<IFocus> MemoTableCellFocusKeyBinder { get; }
        IKeyMap<IFocus> MemoTableCellFocusKeyMap { get; }
        //KeyBinder<IEditor> UmlFeatureEditorKeyBinder { get; }
        IKeyMap<IEditor> UmlFeatureEditorKeyMap { get; }

        KeyBinder<TextBox> TextBoxKeyBinder { get; }
        KeyBinder<TreeView> TreeViewKeyBinder { get; }
        KeyBinder<ComboBox> ComboBoxKeyBinder { get; }
        KeyBinder<MemoListView> MemoListViewKeyBinder { get; }
        KeyBinder<TextBox> PageContentTitleTextBoxKeyBinder { get; }

        // ========================================
        // method
        // ========================================
    }
}
