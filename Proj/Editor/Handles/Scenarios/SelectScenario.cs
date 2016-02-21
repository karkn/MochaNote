/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Common.Forms.Input;
using Mkamo.Editor.Requests;
using System.Windows.Forms;

namespace Mkamo.Editor.Handles.Scenarios {
    public class SelectScenario: AbstractScenario {
        // ========================================
        // field
        // ========================================
        private bool _isSelectedOnMouseDown;
        private bool _toggle;
        private bool _needClearSelectionOnMouseDown;

        // ========================================
        // constructor
        // ========================================
        public SelectScenario(IHandle handle, bool needClearSelectionOnMouseDown): base(handle) {
            _isSelectedOnMouseDown = false;
            _toggle = false;
            _needClearSelectionOnMouseDown = needClearSelectionOnMouseDown;
        }

        public SelectScenario(IHandle handle): this(handle, false) {
        }

        // ========================================
        // property
        // ========================================

        // ------------------------------
        // protected
        // ------------------------------
        protected bool _IsSelectedOnMouseDown {
            get { return _isSelectedOnMouseDown; }
            set { _isSelectedOnMouseDown = value; }
        }

        protected bool _Toggle {
            get { return _toggle; }
            set { _toggle = value; }
        }

        // ========================================
        // method
        // ========================================
        public override void Apply() {
            Handle.MouseDown += HandleMouseDown;
            Handle.MouseUp += HandleMouseUp;
            Handle.MouseDoubleClick += HandleMouseDoubleClick;
            Handle.MouseEnter += HandleMouseEnter;
            Handle.MouseLeave += HandleMouseLeave;
        }

        // ------------------------------
        // protected
        // ------------------------------
        public virtual void HandleMouseDown(object sender, MouseEventArgs e) {
            _isSelectedOnMouseDown = Handle.Host.IsSelected;
            _toggle = KeyUtil.IsControlPressed();

            /// EditorがFocusされていたらCommitしておく
            var focused = Handle.Host.Site.FocusManager.FocusedEditor;
            if (focused != null) {
                focused.RequestFocusCommit(true);
            }

            Handle.Host.RequestSelect(
                SelectKind.True,
                _needClearSelectionOnMouseDown || (!_toggle && !_isSelectedOnMouseDown)
            );
        }

        public virtual void HandleMouseUp(object sender, MouseEventArgs e) {
            if (_toggle && _isSelectedOnMouseDown) {
                Handle.Host.RequestSelect(SelectKind.False, false);
            }
        }

        public virtual void HandleMouseDoubleClick(object sender, MouseEventArgs e) {
            /// 他のEditorがFocusされていたらCommitしておく
            var prevFocused = Handle.Host.Site.FocusManager.FocusedEditor;
            if (prevFocused != null && prevFocused != Handle.Host) {
                prevFocused.RequestFocusCommit(true);
            }

            Handle.Host.RequestFocus(FocusKind.Begin, e.Location);
        }


        public virtual void HandleMouseEnter(object sender, EventArgs e) {
            Handle.Host.ShowFeedback(new HighlightRequest());
        }

        public virtual void HandleMouseLeave(object sender, EventArgs e) {
            Handle.Host.HideFeedback(new HighlightRequest());
        }
    }
}
