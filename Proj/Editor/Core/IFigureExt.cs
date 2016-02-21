/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;

namespace Mkamo.Editor.Core {
    public static class IFigureExt {
        // ========================================
        // static method
        // ========================================
        /// <summary>
        /// 関連付けられているEditorを返す．
        /// </summary>
        public static IEditor GetEditor(this IFigure figure) {
            return (figure.TransientData.ContainsKey(EditorConsts.FigureEditorKey)?
                figure.TransientData[EditorConsts.FigureEditorKey]: null) as IEditor;
        }

        /// <summary>
        /// Editorに関連付ける．
        /// </summary>
        public static void SetEditor(this IFigure figure, IEditor value) {
            figure.TransientData[EditorConsts.FigureEditorKey] = value;
        }

        /// <summary>
        /// Editorへの関連を解除する．
        /// </summary>
        public static void UnsetEditor(this IFigure figure) {
            if (figure.TransientData.ContainsKey(EditorConsts.FigureEditorKey)) {
                figure.TransientData.Remove(EditorConsts.FigureEditorKey);
            }
        }


        /// <summary>
        /// 関連付けられているFocusを返す．
        /// </summary>
        public static IFocus GetFocus(this IFigure figure) {
            return (figure.TransientData.ContainsKey(EditorConsts.FigureFocusKey)?
                figure.TransientData[EditorConsts.FigureFocusKey]: null) as IFocus;
        }

        /// <summary>
        /// Focusに関連付ける．
        /// </summary>
        public static void SetFocus(this IFigure figure, IFocus value) {
            figure.TransientData[EditorConsts.FigureFocusKey] = value;
        }

        /// <summary>
        /// Focusへの関連を解除する．
        /// </summary>
        public static void UnsetFocus(this IFigure figure) {
            if (figure.TransientData.ContainsKey(EditorConsts.FigureFocusKey)) {
                figure.TransientData.Remove(EditorConsts.FigureFocusKey);
            }
        }


        /// <summary>
        /// figureがEditorでどのように使われているかを返す．
        /// 値は FIGURE_ROLE_EDITOR_FIGURE．
        /// </summary>
        public static string GetRole(this IFigure figure) {
            return (figure.PersistentData.ContainsKey(EditorConsts.FigureRoleKey) ?
                figure.PersistentData[EditorConsts.FigureRoleKey]: null) as string;
        }

        /// <summary>
        /// figureがEditorでどのように使われているかを設定する．
        /// 値は FIGURE_ROLE_EDITOR_FIGURE．
        /// </summary>
        public static void SetRole(this IFigure figure, string value) {
            figure.PersistentData[EditorConsts.FigureRoleKey] = value;
        }

        /// <summary>
        /// figureがEditorでどのように使われているかの設定を解除する．
        /// 値は FIGURE_ROLE_EDITOR_FIGURE．
        /// </summary>
        public static void UnsetRole(this IFigure figure) {
            if (figure.PersistentData.ContainsKey(EditorConsts.FigureRoleKey)) {
                figure.PersistentData.Remove(EditorConsts.FigureRoleKey);
            }
        }
    }
}
