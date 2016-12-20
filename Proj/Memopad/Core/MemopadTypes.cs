/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Memopad.Core {
    [Serializable]
    public enum KeySchemeKind {
        Default,
        Emacs,
    }

    [Serializable]
    public enum ThemeKind {
        Default,
        Blue,
        Silver,
        Black,
    }

    [Serializable]
    public enum MemoListBoxDisplayItem {
        Title,
        CreatedDate,
        ModifiedDate,
        AccessedDate,
        Tag,
        SummaryText,
    }

    [Serializable]
    internal enum TimeSpanTargetKind {
        None,
        Modified,
        Created,
        Accessed,
    }

    [Serializable]
    internal enum MemoListTargetKind {
        None,
        QueryBuilder,
        SmartFolder,
        Tag,
        Folder,
        TrashBox,
        OpenMemos,
        AllMemos,
    }

    [Serializable]
    public enum NotifyIconActionKind {
        ShowMainForm,
        ShowFusenForms,
        CreateMemo,
        CreateFusenMemo,
    }
}
