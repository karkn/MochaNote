/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Common.Structure;
using Mkamo.Common.Visitor;
using System.Drawing;
using Mkamo.Common.Externalize;
using Mkamo.Common.Command;

namespace Mkamo.Editor.Core {
    /// <summary>
    /// - manage model, view, controller
    ///   - maintain figure's structure
    ///   - notify structural changes to controller
    /// - manage decoration
    ///
    /// </summary>
    public interface IEditor: IVisitable<IEditor>, IExternalizable {
        // ========================================
        // event
        // ========================================
        event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        event EventHandler<EventArgs> FocusChanged;

        // ========================================
        // property
        // ========================================
        // --- structure ---
        IEditor Parent { get; }
        IEnumerable<IEditor> Children { get; }
        bool HasNextSibling { get; }
        bool HasPreviousSibling { get; }
        IEditor NextSibling { get; }
        IEditor PreviousSibling { get; }

        // --- core ---
        /// <summary>
        /// Editorに設定されたFigureやModelなどを管理するコントローラ．
        /// </summary>
        IController Controller { get; }

        /// <summary>
        /// Editorを表示するためのFigure．
        /// すでにFigureが設定されている状態で他のFigureをsetするには，
        /// どちらのFigureもINodeを実装しているか，
        /// どちらのFigureもIEdgeを実装している必要がある．
        /// </summary>
        IFigure Figure { get; set; }

        /// <summary>
        /// Editorで表示・編集する対象となるモデル．
        /// </summary>
        object Model { get; set; }

        // --- controller and figure type ---
        bool IsContainer { get; }
        bool IsConnectable { get; }
        bool IsConnection { get; }

        // --- role ---
        IList<IRole> Roles { get; }

        // --- structure ---
        IRootEditor Root { get; }
        bool HasRoot { get; }
        bool IsRoot { get; }

        // --- service ---
        IEditorSite Site { get; }

        // --- event receiver ---
        IList<IEditorHandle> EditorHandles { get; }
        IList<IAuxiliaryHandle> AuxiliaryHandles { get; }
        IFocus Focus { get; }

        // --- selection ---
        bool IsSelected { get; set; }
        bool CanSelect { get; set; }

        // --- focus ---
        bool IsFocused { get; set; }
        bool CanFocus { get; set; }

        // --- mouse entered ---
        bool IsMouseEntered { get; }

        // --- activity ---
        bool IsEnabled { get; set; }

        // --- ui ---
        //IMenuProvider MenuProvider { get; set; }

        // --- misc ---
        /// <summary>
        /// Mementoには保存しない拡張データ．
        /// </summary>
        IDictionary<string, object> TransientData { get; }

        // ========================================
        // method
        // ========================================
        // --- enumerable ---
        IEnumerable<IEditor> GetChildrenByPosition();

        // --- enable ---
        void Enable();
        void Disable();

        /// <summary>
        /// 終了処理時などFigureに反映させる必要のない場合にonlyControllerにtrueを渡して呼び出す．
        /// Figureへの反映処理がない分処理が速い．
        /// </summary>
        void Disable(bool onlyController);

        // --- request ---
        void InstallRole(IRole role);
        bool CanUnderstand(IRequest request);
        ICommand GetCommand(IRequest request);
        ICommand PerformRequest(IRequest request);

        bool CanUnderstandAll(IEnumerable<IRequest> requests);
        ICommand GetChainedCommand(IEnumerable<IRequest> requests);

        void ShowFeedback(IRequest request);
        void HideFeedback(IRequest request);
        void HideFeedback(IRequest request, bool disposeFeedback);

        // --- decoreation ---
        void InstallEditorHandle(IEditorHandle handle);
        void InstallHandle(IAuxiliaryHandle handle);
        void InstallHandle(IAuxiliaryHandle handle, HandleStickyKind stickyKind);
        HandleStickyKind GetStickyKind(IAuxiliaryHandle handle);
        void InstallFocus(IFocus focus);

        // --- find ---
        IList<IEditor> GetChildEditorsFor(object model);
        IEditor FindEditor(Predicate<IEditor> finder);
        IEditor FindEditor(Point pt, Predicate<IEditor> finder);
        IEditor FindEnabledEditor(Point pt);
        IEditor FindEditorFor(object model);
        IList<IEditor> FindEditors(Predicate<IEditor> finder);
        IList<IEditor> FindEditors(Point pt, Predicate<IEditor> finder);
        IList<IEditor> FindEditorsFor(object model);

        // --- refresh ---
        /// <summary>
        /// Figureや子Editorを更新する．
        /// </summary>
        void Refresh(RefreshContext context);

        /// <summary>
        /// Figureを更新する．
        /// </summary>
        void RefreshFigure(RefreshContext context);

        /// <summary>
        /// 子Editorを更新する
        /// </summary>
        void RebuildChildren();
    }

}
