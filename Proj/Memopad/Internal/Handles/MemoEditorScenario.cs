/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Handles.Scenarios;
using Mkamo.Common.Forms.Input;
using Mkamo.Editor.Requests;
using System.Windows.Forms;
using Mkamo.Editor.Core;
using System.Drawing;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Diagnostics;
using Mkamo.Editor.Commands;
using Mkamo.Editor.Focuses;
using Mkamo.Model.Memo;
using Mkamo.Model.Core;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Internal.Controls;
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Figure.Core;
using Mkamo.Memopad.Internal.Controllers;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Memopad.Internal.Forms;

namespace Mkamo.Memopad.Internal.Handles {
    internal class MemoEditorScenario: AbstractScenario {
        // ========================================
        // field
        // ========================================
        private SelectRequest _selectRequest;
        private RubberbandRequest _rubberbandRequest;

        private Point _startPoint;
        private bool _toggle;
        private Cursor _oldCursor;

        private IEditorHandle _editorHandle;

        //private bool _isImeOn;

        // ========================================
        // constructor
        // ========================================
        internal MemoEditorScenario(IHandle handle): base(handle) {
            Contract.Requires(handle is IEditorHandle);
            _selectRequest = new SelectRequest();
            _rubberbandRequest = new RubberbandRequest();

            //_isImeOn = false;
        }

        // ========================================
        // property
        // ========================================
        protected Caret _Caret {
            get { return Handle.Host == null? null: Handle.Host.Site.EditorCanvas.Caret; }
        }

        protected IEditorHandle _EditorHandle {
            get { return _editorHandle?? (_editorHandle = (IEditorHandle) Handle); }
        }

        // ========================================
        // method
        // ========================================
        public override void Apply() {
            _editorHandle = Handle as IEditorHandle;
            Contract.Requires(_editorHandle != null);

            Handle.MouseDoubleClick += HandleMouseDoubleClick;

            Handle.MouseDown += HandleMouseDown;
            Handle.MouseClick += HandleMouseClick;
            Handle.DragStart += HandleDragStart;
            Handle.DragMove += HandleDragMove;
            Handle.DragFinish += HandleDragFinish;
            Handle.DragCancel += HandleDragCancel;

            _editorHandle.KeyPress += HandleKeyPress;
            
            Handle.Installed += HandleHandleInstalled;
            Handle.Uninstalling += HandleHandleUninstalling;
        }

        protected virtual void HandleMouseDown(object sender, MouseEventArgs e) {
            /// 他のEditorがFocusされていたらCommitしておく
            var prevFocused = Handle.Host.Site.FocusManager.FocusedEditor;
            if (prevFocused != null && prevFocused != Handle.Host) {
                prevFocused.RequestFocusCommit(true);
            }

            _toggle = KeyUtil.IsControlPressed();
            if (!_toggle) {
                Handle.Host.RequestSelect(SelectKind.True, true);
            }

            if (_Caret != null) {
                var expectedTextPos = CaretUtil.GetExpectedMemoTextPosition(new Point(e.X, e.Y - _Caret.Height / 2));
                _Caret.Position = CaretUtil.GetExpectedCaretPosition(expectedTextPos, Handle.Host.Site.GridService);
            }
        }

        protected virtual void HandleMouseClick(object sender, MouseEventArgs e) {
            /// 他のEditorがFocusされていたらCommitしておく
            var prevFocused = Handle.Host.Site.FocusManager.FocusedEditor;
            if (prevFocused != null && prevFocused != Handle.Host) {
                prevFocused.RequestFocusCommit(true);
            }
        }

        protected virtual void HandleDragStart(object sender, MouseEventArgs e) {
            _startPoint = e.Location;
            _rubberbandRequest.Toggle = _toggle;
            _rubberbandRequest.Bounds = new Rectangle(_startPoint, Size.Empty);
            Handle.Host.ShowFeedback(_rubberbandRequest);
            _oldCursor = Host.Site.EditorCanvas.Cursor;
            Host.Site.EditorCanvas.Cursor = Cursors.Default;
        }

        protected virtual void HandleDragMove(object sender, MouseEventArgs e) {
            _rubberbandRequest.Toggle = _toggle;
            _rubberbandRequest.Bounds = RectUtil.GetRectangleFromDiagonalPoints(_startPoint, e.Location);
            Handle.Host.ShowFeedback(_rubberbandRequest);
        }

        protected virtual void HandleDragFinish(object sender, MouseEventArgs e) {
            _rubberbandRequest.Toggle = _toggle;
            _rubberbandRequest.Bounds = RectUtil.GetRectangleFromDiagonalPoints(_startPoint, e.Location);
            Handle.Host.HideFeedback(_rubberbandRequest);
            Handle.Host.PerformRequest(_rubberbandRequest);
            Host.Site.EditorCanvas.Cursor = _oldCursor;
        }

        protected virtual void HandleDragCancel(object sender, EventArgs e) {
            Handle.Host.HideFeedback(_rubberbandRequest);
            Host.Site.EditorCanvas.Cursor = _oldCursor;
        }

        protected virtual void HandleKeyPress(object sender, KeyPressEventArgs e) {
            /// caret位置にテキストを作成
            if (!Char.IsControl(e.KeyChar)) {
                CreateMemoText(e.KeyChar.ToString());
            }
        }

        protected virtual void HandleMouseDoubleClick(object sender, MouseEventArgs e) {
            if (Host.Children.Any()) {
                var cmd = new SelectMultiCommand(Host.Children, SelectKind.True, true);
                cmd.Execute();
            }
            

            /// デフォルト図形の作成
            //var facade = MemopadApplication.Instance;
            //var settings = facade.Settings;
            //var shapeId = settings.DefaultShapeId;

            //var form = (MemopadFormBase) Host.Site.EditorCanvas.TransientData[MemopadConsts.FormEditorTransientDataKey];
            //var reg = form.ToolRegistry;

            //var tool = reg.GetCreateNodeTool(shapeId);
            //tool.CreateNode(Handle.Host, e.Location);
        }

        /// KeyDownで処理するとIME ONで入力したときに，
        /// IMEウィンドウが開く前にMemoTextが生成される．
        //protected virtual void HandleKeyDown(object sender, KeyEventArgs e) {
        //    /// IMEの状態が変わるキーならばreturn
        //    var curImeOn = Handle.Host.Site.EditorCanvas.IsImeOpen();
        //    if (curImeOn != _isImeOn) {
        //        _isImeOn = curImeOn;
        //        return;
        //    }
        //    _isImeOn = curImeOn;

        //    /// なんらかの処理(KeyMapの処理など)がされていたら何もしない
        //    if (e.SuppressKeyPress) {
        //        return;
        //    }

        //    /// 制御用のキーならば何もしない
        //    if (char.IsControl((char)e.KeyValue)) {
        //        return;
        //    }

        //    /// 文字入力用のキー入力でない場合は何もしない
        //    var mod = e.KeyData & Keys.Modifiers;
        //    if (mod != Keys.None && !(mod == Keys.Shift && e.KeyCode != Keys.ShiftKey)) {
        //        return;
        //    }

        //    /// caret位置にテキストを作成
        //    CreateMemoText();
        //}


        // ------------------------------
        // private
        // ------------------------------
        private void CreateMemoText(string s) {
            var loc = CaretUtil.GetExpectedMemoTextPosition(_Caret.Position);
            MemoEditorHelper.AddString(Handle.Host, loc, s);
        }

        // --- event handler ---
        private void HandleHandleInstalled(object sender, EventArgs e) {
            Handle.Host.SelectionChanged += HandleEditorSelectionChanged;
        }

        private void HandleHandleUninstalling(object sender, EventArgs e) {
            Handle.Host.SelectionChanged -= HandleEditorSelectionChanged;
        }

        private void HandleEditorSelectionChanged(object sender, EventArgs e) {
            if (Handle.Host.IsSelected) {
                _Caret.Height = MemopadApplication.Instance.Settings.GetDefaultMemoTextFontHeight();
                _Caret.Show();
            } else {
                _Caret.Hide();
            }
            //_isImeOn = Handle.Host.Site.EditorCanvas.IsImeOpen();
        }

    }
}
