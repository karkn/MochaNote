/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Editor.Controllers;
using Mkamo.Editor.Internal.Core;
using Mkamo.Common.Collection;
using System.Windows.Forms;

namespace Mkamo.Editor.Core {
    using ICommandExecutor = Mkamo.Common.Command.ICommandExecutor;
using System.Drawing;

    public static class EditorConsts {
        // ------------------------------
        // public
        // ------------------------------
        /// <summary>
        /// editorをクリップボードに格納するときのフォーマット．
        /// </summary>
        public static readonly DataFormats.Format DataEditorFormat = DataFormats.GetFormat("Mkamo.Editor.DataEditorFormat");

        /// <summary>
        /// editorのModelDescriptorをクリップボードに格納するときのフォーマット．
        /// </summary>
        public static readonly DataFormats.Format DataDescriptorFormat =
            DataFormats.GetFormat("Mkamo.Editor.DataDescriptorFormat");

        /// <summary>
        /// editorの接続情報をクリップボードに格納するときのフォーマット．
        /// </summary>
        public static readonly DataFormats.Format DataConnectionFormat =
            DataFormats.GetFormat("Mkamo.Editor.DataConnectionFormat");

        /// <summary>
        /// 空のIEditorのリスト．複数オブジェクトで共用・再利用可能．
        /// </summary>
        public static readonly IList<IEditor> EmptyEditorList = new EmptyList<IEditor>();

        /// <summary>
        /// IFigureからIEditorを取得するための参照を設定するIFigure#TransientDataのキー．
        /// </summary>
        public const string FigureEditorKey = "Mkamo.Editor.Core.IEditor.Editor";

        /// <summary>
        /// Focusに関連付けられたIFigureからIFocusを取得するための参照を設定するIFigure#TransientDataのキー．
        /// </summary>
        public const string FigureFocusKey = "Mkamo.Editor.Core.IEditor.Focus";



        /// <summary>
        /// EditorFactory.RestoreEditorStructure()でmodelを復元するときに
        /// ExternalizeContextのExtendedDataに親modelを格納するキー．
        /// </summary>
        public const string RestoreEditorStructureParentModel = "Mkamo.Editor.RestoreEditorStructure.ParentModel";

        /// <summary>
        /// PasteRequestで文字列を貼りつけるときに行区切りを改段落でなく改行にするためのDescription
        /// </summary>
        public const string InBlockPasteDescription = "inblock";


        // ------------------------------
        // internal
        // ------------------------------
        /// <summary>
        /// IFigureからEditorでの役割を取得するための参照を設定するIFigure#PersistentDataのキー．
        /// </summary>
        internal const string FigureRoleKey = "Mkamo.Editor.Core.IEditor.FigureRole";
        /// <summary>
        /// FigureがEditorに対応づいたFigureであることを示す．
        /// primaryLayer下でこのRoleが設定されていないFigureはEditorFigureのPartsである．
        /// </summary>
        internal const string EditorFigureFigureRole = "Mkamo.Editor.Core.IEditor.FigureRole.EditorFigure";
        /// <summary>
        /// FigureがFocusに対応づいたFigureであることを示す．
        /// </summary>
        internal const string FocusFigureFigureRole = "Mkamo.Editor.Core.IEditor.FigureRole.FocusFigure";

        /// <summary>
        /// PersistContextからModelPersisterを取得するための参照を設定するPersistContext#ExtendedDataのキー．
        /// </summary>
        internal const string ModelSerializerKey = "Mkamo.Editor.Core.EditorCanvas.ModelSerializer";

        /// <summary>
        /// PersistContextからEditorSiteを取得するための参照を設定するPersistContext#ExtendedDataのキー．
        /// </summary>
        internal const string EditorSiteKey = "Mkamo.Editor.Core.EditorCanvas.EditorSite";

        /// <summary>
        /// 詳細ページの標準サイズ。
        /// </summary>
        internal static readonly Size DefaultDetailFormSize = new Size(520, 480);

        // --- null object ---
        internal static readonly IEditorSite NullEditorSite = new NullEditorSite();
        internal static readonly IController NullController = new NullController();
        internal static readonly ICommandExecutor NullCommandExecutor = new NullCommandExecutor();
        internal static readonly IControllerFactory NullControllerFactory = new NullControllerFactory();
        internal static readonly ISelectionManager NullSelectionManager= new NullSelectionManager();
        internal static readonly IFocusManager NullFocusManager= new NullFocusManager();

    }
}
