/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Common.Win32.Core;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Internal.Utils;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class ToolStripForm: Form {
        // ========================================
        // field
        // ========================================
        private FusenForm _targetForm;

        // ========================================
        // constructor
        // ========================================
        public ToolStripForm() {
            InitializeComponent();

            StartPosition = FormStartPosition.Manual;
            ClientSize = new Size(ClientSize.Width, _mainToolStrip.Height + _editToolStrip.Height);

            _mainToolStrip.MouseEnter += (se, ev) => Activate();
            _editToolStrip.MouseEnter += (se, ev) => Activate();
        }

        // ========================================
        // property
        // ========================================
        internal FusenForm TargetForm {
            set { _targetForm = value; }
        }

        // ========================================
        // method
        // ========================================
        // ------------------------------
        // protected
        // ------------------------------
        protected override bool ShowWithoutActivation {
            get { return true; }
        }

        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case (int) WindowMessage.NCACTIVATE: {
                    /// タイトルバーを常にアクティブ色で描画する
                    m.WParam = (IntPtr) 1;
                    break;
                }
                case (int) WindowMessage.ACTIVATEAPP: {
                    /// アプリケーション自体のフォーカスが変わればNCACTIVE
                    m.Msg = (int) WindowMessage.NCACTIVATE;
                    break;
                }
            }

            base.WndProc(ref m);
        }

        protected override CreateParams CreateParams {
            get {
                /// 閉じるボタンを押せなくする
                const int CS_NOCLOSE = 0x200;
                var cp = base.CreateParams;
                cp.ClassStyle = cp.ClassStyle | CS_NOCLOSE;
                return cp;
            }
        }

        protected override void OnDeactivate(EventArgs e) {
            base.OnDeactivate(e);
            if (Form.ActiveForm != this && Form.ActiveForm != _targetForm) {
                Hide();
            }
        }

        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            Activate();
        }

        // ------------------------------
        // private
        // ------------------------------
        // --- handler ---
        private void _topMostToolStripButton_Click(object sender, EventArgs e) {
            _targetForm.TopMost = !_targetForm.TopMost;
            _targetForm.UpdateMainToolStrip();
        }

        private void _showInfoToolStripButton_Click(object sender, EventArgs e) {
            _targetForm.PageContent.IsCompact = !_targetForm.PageContent.IsCompact;
            _targetForm.UpdateMainToolStrip();
        }

        private void _deleteMemoToolStripButton_Click(object sender, EventArgs e) {
            var info = _targetForm.PageContent.MemoInfo;
            if (!MessageUtil.ConfirmMemoRemoval(new [] { info })) {
                return;
            }
            var app = MemopadApplication.Instance;
            app.RemoveMemo(info);
        }

        private void _showInMainFormToolStripButton_Click(object sender, EventArgs e) {
            var app = MemopadApplication.Instance;
            var info = _targetForm.PageContent.MemoInfo;
            app.CloseMemo(info);
            app.LoadMemo(info, false);
        }
    }
}
