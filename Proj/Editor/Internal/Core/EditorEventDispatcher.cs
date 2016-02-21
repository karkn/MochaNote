/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Editor.Core;
using System.Windows.Forms;
using System.Drawing;
using Mkamo.Common.Core;

namespace Mkamo.Editor.Internal.Core {
    /// <summary>
    /// Editorとしての動作のためにToolやコンテキストメニューなどへの
    /// イベントディスパッチを割り込ませたEventDispatcher．
    /// </summary>
    internal class EditorEventDispatcher: EventDispatcher {
        // ========================================
        // static field
        // ========================================
        private const int UISpace = 4;

        // ========================================
        // field
        // ========================================
        private EditorCanvas _canvas;
        private ContextMenuStrip _contextMenu;
        private ToolStripDropDown _miniToolBar;

        private bool _isMouseOnMiniToolBar;
        private bool _needCloseMiniToolBarOnMouseLeave;

        // ========================================
        // constructor
        // ========================================
        public EditorEventDispatcher(EditorCanvas canvas, ContextMenuStrip contextMenu, ToolStripDropDown miniToolBar): base(canvas) {
            _canvas = canvas;
            _contextMenu = contextMenu;
            _miniToolBar = miniToolBar;

            _miniToolBar.MouseEnter += HandleMiniToolBarMouseEnter;
            _miniToolBar.MouseLeave += HandleMiniToolBarMouseLeave;

            _miniToolBar.Opened += HandleMiniToolBarOpened;
            _miniToolBar.Closed += HandleMiniToolBarClosed;
            _contextMenu.Closed += HandleContextMenuClosed;

            // これでクリックしても閉じないようにできる
            //_miniToolBar.Closing += (s, e) => {
            //    if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked) {
            //        e.Cancel = true;
            //    }
            //};
        }


        // ========================================
        // property
        // ========================================
        // ------------------------------
        // protected
        // ------------------------------
        protected ITool _Tool {
            get { return _canvas.Tool; }
        }

        protected IFocusManager _FocusManager {
            get { return _canvas.FocusManager; }
        }

        protected ISelectionManager _SelectionManager {
            get { return _canvas.SelectionManager; }
        }

        // ========================================
        // method
        // ========================================
        // --- mouse ---
        public override void HandleMouseDown(MouseEventArgs e) {
            if (_Tool != null && _Tool.HandleMouseDown(e)) {
                return;
            }
            base.HandleMouseDown(e);
        }

        public override void HandleMouseMove(MouseEventArgs e) {
            if (_Tool != null && _Tool.HandleMouseMove(e)) {
                return;
            }
            base.HandleMouseMove(e);
        }

        public override void HandleMouseUp(MouseEventArgs e) {
            if (_Tool != null && _Tool.HandleMouseUp(e)) {
                return;
            }

            if (e.Button == MouseButtons.Right) {
                /// コンテキストメニューの表示

                _contextMenu.Items.Clear();
                _miniToolBar.Items.Clear();

                var fig = Target as IFigure;
                if (fig != null && fig.GetRole() == EditorConsts.FocusFigureFigureRole) {
                    /// FocusのFigureならば
                    var focus = fig.GetFocus();
                    if (focus != null) {
                        var menu = focus.GetContextMenuProvider().GetContextMenu(e);
                        ToolStripManager.Merge(menu, _contextMenu);
                    }

                } else {
                    var defaultMenu = _canvas.ContextMenuProvider.GetContextMenu(e);
                    if (Target != null) {
                        if (fig != null && fig.GetEditor() != null) {
                            /// EditorFigureかEditorFigureのPartsであればGetEditor()でEditorが取れる
                            var targetUI = fig.GetEditor().Controller.UIProvider;
                            if (targetUI != null) {
                                var targetMenu = targetUI.GetContextMenu(e);
                                if (targetMenu != null) {
                                    ToolStripManager.Merge(targetMenu, _contextMenu);
                                    if (_contextMenu.Items.Count > 0 && defaultMenu.Items.Count > 0) {
                                        _contextMenu.Items.Add(new ToolStripSeparator());
                                    }
                                }

                                var targetMiniToolBar = targetUI.GetMiniToolBar(e);
                                if (targetMiniToolBar != null && targetMiniToolBar.Items.Count > 0) {
                                    PrepareMiniToolBar(targetMiniToolBar);
                                    ToolStripManager.Merge(targetMiniToolBar, _miniToolBar);
                                }
                            }
                        }
                    }

                    ToolStripManager.Merge(defaultMenu, _contextMenu);
                }
                
                if (_miniToolBar.Items.Count > 0) {

                    /// ツールバー・メニュー位置の調整
                    var loc = _canvas.PointToScreen(_canvas.TranslateToControlPoint(e.Location));
                    var screenPt = Point.Empty;
                    var area = Screen.GetWorkingArea(e.Location);
                    if (loc.Y + _contextMenu.Height < area.Bottom) {
                        /// context menuが下にはみ出ない
                        screenPt = new Point(loc.X, loc.Y - _miniToolBar.Height - UISpace);

                    } else {
                        /// context menuが下にはみ出る

                        if (loc.Y + _miniToolBar.Height < area.Bottom) {
                            /// mini tool barは下にはみ出ない
                            screenPt = new Point(loc.X, loc.Y + UISpace);

                        } else {
                            /// mini tool barも下にはみ出る
                            screenPt = new Point(loc.X, loc.Y - _contextMenu.Height - UISpace - _miniToolBar.Height);
                        }
                    }

                    _isMouseOnMiniToolBar = false;
                    _needCloseMiniToolBarOnMouseLeave = false;
                    _miniToolBar.Show(screenPt);

                } else {
                    _contextMenu.Show(_canvas, _canvas.TranslateToControlPoint(e.Location));
                }
            }

            base.HandleMouseUp(e);
        }

        public override void HandleDragStart(MouseEventArgs e) {
            if (_Tool != null && _Tool.HandleDragStart(e)) {
                return;
            }
            base.HandleDragStart(e);
        }
        public override void HandleDragMove(MouseEventArgs e) {
            if (_Tool != null && _Tool.HandleDragMove(e)) {
                return;
            }
            base.HandleDragMove(e);
        }
        public override void HandleDragFinish(MouseEventArgs e) {
            if (_Tool != null && _Tool.HandleDragFinish(e)) {
                return;
            }

            base.HandleDragFinish(e);
        }
        public override void HandleDragCancel(EventArgs e) {
            if (_Tool != null && _Tool.HandleDragCancel(e)) {
                return;
            }
            base.HandleDragCancel(e);
        }

        public override void HandleMouseClick(MouseEventArgs e) {
            if (_Tool != null && _Tool.HandleMouseClick(e)) {
                return;
            }
            base.HandleMouseClick(e);
        }

        public override void HandleMouseDoubleClick(MouseEventArgs e) {
            if (_Tool != null && _Tool.HandleMouseDoubleClick(e)) {
                return;
            }
            base.HandleMouseDoubleClick(e);
        }

        public override void HandleMouseEnter(EventArgs e) {
            if (_Tool != null && _Tool.HandleMouseEnter(e)) {
                return;
            }
            base.HandleMouseEnter(e);
        }

        public override void HandleMouseLeave(EventArgs e) {
            if (_Tool != null && _Tool.HandleMouseLeave(e)) {
                return;
            }
            base.HandleMouseLeave(e);
        }

        public override void HandleMouseHover(EventArgs e) {
            if (_Tool != null && _Tool.HandleMouseHover(e)) {
                return;
            }
            base.HandleMouseHover(e);
        }


        // --- dnd ---
        //public override void HandleDragOver(DragEventArgs e) {
        //    base.HandleDragOver(e);
        //}
        
        //public override void HandleDragDrop(DragEventArgs e) {
        //    base.HandleDragDrop(e);
        //}

        //public override void HandleDragEnter(DragEventArgs e) {
        //    base.HandleDragEnter(e);
        //}

        //public override void HandleDragLeave(EventArgs e) {
        //    base.HandleDragLeave(e);
        //}

        //public override void HandleQueryContinueDrag(QueryContinueDragEventArgs e) {
        //    base.HandleQueryContinueDrag(e);
        //}

        // --- key ---
        public virtual void HandleShortcutKeyProcess(ShortcutKeyProcessEventArgs e) {
            //if (_Tool != null && _Tool.HandleKeyUp(e)) {
            //    return;
            //}

            if (_FocusManager.IsEditorFocused) {
                _FocusManager.Focus.Figure.HandleShortcutKeyProcess(e);
            } else {
                var selectedEditors = _SelectionManager.SelectedEditors.ToArray();
                foreach (var selected in selectedEditors) {
                    selected.Figure.HandleShortcutKeyProcess(e);
                }
            }
        }


        public virtual void HandleKeyDown(KeyEventArgs e) {
            if (_Tool != null && _Tool.HandleKeyDown(e)) {
                return;
            }

            if (_FocusManager.IsEditorFocused) {
                _FocusManager.Focus.Figure.HandleKeyDown(e);

            } else {
                var selectedEditors = _SelectionManager.SelectedEditors.ToArray();

                if (selectedEditors.Length == 0) {
                    var keyMap = _canvas.NoSelectionKeyMap;
                    if (keyMap != null) {
                        var keyData = e.KeyData;
                        if (keyMap.IsDefined(keyData)) {
                            var action = keyMap.GetAction(keyData);
                            if (action != null) {
                                action(_canvas);

                            }
                        }
                    }

                } else {
                    foreach (var selected in selectedEditors) {
                        selected.Figure.HandleKeyDown(e);
                    }
                }
            }
        }

        public virtual void HandleKeyUp(KeyEventArgs e) {
            if (_Tool != null && _Tool.HandleKeyUp(e)) {
                return;
            }

            if (_FocusManager.IsEditorFocused) {
                _FocusManager.Focus.Figure.HandleKeyUp(e);
            } else {
                var selectedEditors = _SelectionManager.SelectedEditors.ToArray();
                foreach (var selected in selectedEditors) {
                    selected.Figure.HandleKeyUp(e);
                }
            }
        }

        public virtual void HandleKeyPress(KeyPressEventArgs e) {
            if (_Tool != null && _Tool.HandleKeyPress(e)) {
                return;
            }

            if (_FocusManager.IsEditorFocused) {
                _FocusManager.Focus.Figure.HandleKeyPress(e);
            } else {
                var selectedEditors = _SelectionManager.SelectedEditors.ToArray();
                foreach (var selected in selectedEditors) {
                    selected.Figure.HandleKeyPress(e);
                }
            }
        }

        public virtual void HandlePreviewKeyDown(PreviewKeyDownEventArgs e) {
            if (_Tool != null && _Tool.HandlePreviewKeyDown(e)) {
                return;
            }

            if (_FocusManager.IsEditorFocused) {
                _FocusManager.Focus.Figure.HandlePreviewKeyDown(e);
            } else {
                var selectedEditors = _SelectionManager.SelectedEditors.ToArray();
                foreach (var selected in selectedEditors) {
                    selected.Figure.HandlePreviewKeyDown(e);
                }
            }
        }


        // ------------------------------
        // private
        // ------------------------------
        private void PrepareMiniToolBar(ToolStripDropDown miniToolBar) {
            foreach (ToolStripItem item in miniToolBar.Items) {
                var dd = item as ToolStripDropDownItem;
                if (dd != null) {
                    dd.DropDownDirection = ToolStripDropDownDirection.BelowRight;
                    dd.DropDownOpening += HandleMiniToolBarDropDownItemOpening;
                    dd.DropDownClosed += HandleMiniToolBarDropDownItemClosed;
                }
            }
        }

        private void CleanMiniToolBar(ToolStripDropDown miniToolBar) {
            foreach (ToolStripItem item in miniToolBar.Items) {
                var dd = item as ToolStripDropDownItem;
                if (dd != null) {
                    dd.DropDownOpening -= HandleMiniToolBarDropDownItemOpening;
                    dd.DropDownClosed -= HandleMiniToolBarDropDownItemClosed;
                }
            }
        }

        // --- event handler ---
        private void HandleMiniToolBarDropDownItemOpening(object sender, EventArgs e) {
            var dd = (ToolStripDropDownItem) sender;
            _needCloseMiniToolBarOnMouseLeave = false;
        }

        private void HandleMiniToolBarDropDownItemClosed(object sender, EventArgs e) {
            var dd = (ToolStripDropDownItem) sender;
            if (_isMouseOnMiniToolBar) {
                _needCloseMiniToolBarOnMouseLeave = true;
            } else {
                _miniToolBar.Close();
            }
        }

        private void HandleMiniToolBarMouseEnter(object sender, EventArgs e) {
            _isMouseOnMiniToolBar = true;
        }

        private void HandleMiniToolBarMouseLeave(object sender, EventArgs e) {
            _isMouseOnMiniToolBar = false;
            if (_needCloseMiniToolBarOnMouseLeave) {
                _needCloseMiniToolBarOnMouseLeave = false;
                _miniToolBar.Close();
            }
        }

        private void HandleMiniToolBarOpened(object sender, EventArgs e) {
            if (_contextMenu != null && _contextMenu.Items.Count > 0) {
                
                var loc = Point.Empty;
                var area = Screen.GetWorkingArea(loc);
                if (_miniToolBar.Bottom + UISpace + _contextMenu.Height < area.Bottom) {
                    /// はみ出ない
                    loc = new Point(_miniToolBar.Left, _miniToolBar.Bottom + UISpace);

                } else {
                    /// コンテキストメニューがはみ出る
                    loc = new Point(_miniToolBar.Left, _miniToolBar.Top - _contextMenu.Height - UISpace);
                }

                _contextMenu.Show(loc);
            }
        }

        private void HandleMiniToolBarClosed(object sender, EventArgs e) {
            CleanMiniToolBar(_miniToolBar);
        }

        private void HandleContextMenuClosed(object sender, ToolStripDropDownClosedEventArgs e) {
            if (_miniToolBar != null) {
                /// マウスがmini toolbar上にあり，かつ，
                /// 閉じる理由がなんらかのConfidante上のマウスクリックであるときのみ
                /// Closeの予約だけにする
                if (_isMouseOnMiniToolBar && e.CloseReason == ToolStripDropDownCloseReason.AppClicked) {
                    //_needCloseMiniToolBarOnMouseLeave = true;
                } else {
                    _miniToolBar.Close();
                }
            }
        }

    }
}
