/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STCore = Mkamo.StyledText.Core;
using Mkamo.Editor.Focuses;
using System.Drawing;
using System.Windows.Forms;

namespace Mkamo.Editor.Core {
    public enum FocusKind {
        Begin,
        Commit,
        Rollback,
    }

    public enum FocusCommitResultKind {
        Noop, /// undoの不可
        Commited, /// 正常コミット
        Canceled, /// コミットキャンセル
    }

    public enum SelectKind {
        True,
        False,
        Toggle,
    }

    public enum ReorderKind {
        Front,
        FrontMost,
        Back,
        BackMost,
    }

    [Flags]
    public enum FontModificationKinds {
        Name = 0x01,
        Size = 0x02,
        Bold = 0x10,
        Italic = 0x20,
        Underline = 0x40,
        Strikeout = 0x80,
        Style = Bold | Italic | Underline | Strikeout,
        All = Name | Size | Style,
    }

    [Flags]
    public enum AlignmentModificationKinds {
        Horizontal = 0x01,
        Vertical = 0x02,
        All = Horizontal | Vertical,
    }

    public enum EditorRefreshKind {
        Loaded,
        Inserted,
        ModelSet,
        ModelUpdated,
    }

    /// <summary>
    /// Handleの表示オプション。
    /// </summary>
    public enum HandleStickyKind {
        Selected = 1,
        MouseEnter,
        Always,
        SelectedIncludingChildren,
        MouseOver,
    }

    public enum PrintFitKind {
        None,
        Horizontal,
        Vertical,
        Both,
    }

    // --- transfer ---
    /// <summary>
    /// originalを元に作成されたcreatedの初期化処理を行う。
    /// MemoFileをコピーした後に，ファイルをコピーしてプロパティを更新する，など。
    /// </summary>
    public delegate void TransferInitializer(IEditor transfered, object hint);

    // --- clipboard ---
    /// <summary>
    /// parentのlocationにクリップボードの内容を貼りつけたEditorを追加し，それらを返す．
    /// </summary>
    public delegate IEnumerable<IEditor> EditorPaster(IEditor parent, Point location, string description);


    //public delegate object EditorDataPicker(IEditor editor);
    //public delegate void EditorDataAggregator(IEnumerable<object> data);
    public delegate void EditorCopyExtender(IEnumerable<IEditor> editors, IDataObject dataObj);

    // --- export ---
    /// <summary>
    /// targetをoutputPathに指定された形式で出力する。
    /// </summary>
    public delegate void EditorExporter(IEditor target, string outputPath);
    
    // --- focus ---
    /// <summary>
    /// フォーカス時にmodelの情報などを元にfocusの初期値を返す．
    /// </summary>
    public delegate object FocusInitializer(IFocus focus, object model);

    /// <summary>
    /// focusのコミット時に編集結果のvalueをmodelに反映し，その反映を元に戻すためのFocusUndoerを返す．
    /// valueが正しくないため編集状態を継続する場合はisCancelledにtrueを代入する．
    /// 戻り値にnullを返すとundo不要すなわち編集が行われなかったものとする．
    /// </summary>
    public delegate FocusUndoer FocusCommiter(IFocus focus, object model, object value, bool isRedo, out bool isCancelled);

    /// <summary>
    /// Undo時にFocusでの編集結果をundoする．
    /// </summary>
    public delegate void FocusUndoer(IFocus focus, object model);

    // --- combine ---
    /// <summary>
    /// targetにcombinedを結合する。
    /// CombineRoleなどで使われる。
    /// </summary>
    public delegate EditorCombinatorUndoer EditorCombinator(IEditor target, IEnumerable<IEditor> combined);
    public delegate void EditorCombinatorUndoer(IEditor target);

    /// <summary>
    /// targetにcombinedを結合できるかどうかを返す。
    /// CombineRoleなどで使われる。
    /// </summary>
    public delegate bool EditorCombinationJudge(IEditor target, IEnumerable<IEditor> combined);
}
