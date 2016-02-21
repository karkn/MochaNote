/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using System.Collections;
using Mkamo.Common.Collection;
using Mkamo.Common.Core;
using Mkamo.Editor.Core;

namespace Mkamo.Editor.Internal.Core {
    /// <summary>
    /// EditorのFigureの構造を管理する．
    /// EditorのFigureは子として以下のFigureを持つ．
    ///   - EditorFigureParts: Editorの内容を表示するために使われているFigure
    ///   - ChildEditorFigures: 子EditorのFigure
    /// 子Editorの追加・削除でEditorFigurePartsが影響を受けないようにChildEditorFiguresを追加・削除する．
    /// </summary>
    internal class FigureStructureMaintainer {
        // ========================================
        // field
        // ========================================
        private readonly IEditor _owner;
        private IFigure _editorFigure;

        private RangedList<IFigure> _editorFigureParts;
        private RangedList<IFigure> _childEditorFigures;

        // ========================================
        // constructor
        // ========================================
        public FigureStructureMaintainer(IEditor owner) {
            _owner = owner;
        }

        // ========================================
        // property
        // ========================================
        public IFigure EditorFigure {
            get { return _editorFigure; }
            set {
                if (value == null || value == _editorFigure) {
                    return;
                }

                var oldEditorFigure = _editorFigure;
                if (oldEditorFigure != null) {
                    /// 以前のfigureの設定をすべて解除

                    oldEditorFigure.UnsetEditor();
                    oldEditorFigure.UnsetRole();

                    /// EditorFigureを探す
                    var childIndex = oldEditorFigure.Children.IndexOf(
                        fig => fig.GetRole() == EditorConsts.EditorFigureFigureRole
                    );
                    var hasChild = (childIndex != -1);

                    var partsCount = hasChild? childIndex: _editorFigure.Children.Count;
                    var oldParts = new RangedList<IFigure>(oldEditorFigure.Children, 0, partsCount);
                    foreach (var part in oldParts) {
                        part.StopForwardMouseEvents(oldEditorFigure);
                        part.UnsetEditor();
                    }
                }

                _editorFigure = value;

                if (_editorFigure != null) {
                    _editorFigure.SetEditor(_owner);
                    /// 渡されたfigure自身にはEditorFigureと印をつける
                    _editorFigure.SetRole(EditorConsts.EditorFigureFigureRole);

                    /// 渡されたfigureの子からEditorFigureを探す
                    var childIndex = _editorFigure.Children.IndexOf(
                        fig => fig.GetRole() == EditorConsts.EditorFigureFigureRole
                    );
                    var hasChild = (childIndex != -1);

                    /// 子のEditorFigureより前のFigureはparts
                    var partsCount = hasChild? childIndex: _editorFigure.Children.Count;
                    _editorFigureParts = new RangedList<IFigure>(_editorFigure.Children, 0, partsCount);
                    foreach (var part in _editorFigureParts) {
                        /// partsのイベントは_editorFigureにフォワード
                        part.ForwardMouseEvents(_editorFigure);
                        part.SetEditor(_owner);
                    }

                    /// 昔のchildがあればそれを新しいEditorFigureに差し込む
                    var oldChildEditorFigures = oldEditorFigure == null? null: _childEditorFigures;
                    if (oldChildEditorFigures != null) {
                        foreach (var fig in oldChildEditorFigures) {
                            _editorFigure.Children.Add(fig);
                        }
                    }

                    var childrenCount = hasChild? _editorFigure.Children.Count - childIndex: 0;
                    _childEditorFigures = new RangedList<IFigure>(
                        _editorFigure.Children,
                        _editorFigureParts.Count,
                        childrenCount
                    );
                }
            }
        }

        public IList<IFigure> EditorFigureParts {
            get { return _editorFigureParts; }
        }

        public IList<IFigure> ChildEditorFigures {
            get { return _childEditorFigures; }
        }
    
        // ========================================
        // method
        // ========================================
        public void AddChildEditorFigure(IFigure childEditorFigure) {
            _childEditorFigures.Add(childEditorFigure);
        }

        public void InsertChildEditorFigure(IFigure childEditorFigure, int index) {
            _childEditorFigures.Insert(index, childEditorFigure);
        }

        public void RemoveChildEditorFigure(IFigure childEditorFigure) {
            _childEditorFigures.Remove(childEditorFigure);
        }

        public void ClearChildEditorFigure() {
            _childEditorFigures.Clear();
        }
    }

}
