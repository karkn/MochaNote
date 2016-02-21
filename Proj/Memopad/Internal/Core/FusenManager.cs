/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Core;
using Mkamo.Common.Core;
using Mkamo.Memopad.Internal.Forms;
using Mkamo.Common.Win32.User32;
using System.Windows.Forms;
using System.Drawing;

namespace Mkamo.Memopad.Internal.Core {
    internal class FusenManager {
        // ========================================
        // field
        // ========================================
        private Dictionary<MemoInfo, FusenForm> _infoToForms;

        // ========================================
        // constructor
        // ========================================
        public FusenManager() {
            _infoToForms = new Dictionary<MemoInfo, FusenForm>();
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler Registered;
        public event EventHandler Unregistered;

        // ========================================
        // property
        // ========================================
        public IEnumerable<FusenForm> RegisteredForms {
            get { return _infoToForms.Values; }
        }

        public IEnumerable<MemoInfo> OpenMemoInfos {
            get { return _infoToForms.Values.Select(fusen => fusen.PageContent.MemoInfo); }
        }

        // ========================================
        // method
        // ========================================
        public void RegisterForm(MemoInfo info, FusenForm form) {
            _infoToForms[info] = form;
            form.Activated += HandleFusenFormActivated;
            OnRegistered();
        }
        
        public void UnregisterForm(MemoInfo info) {
            if (!_infoToForms.ContainsKey(info)) {
                return;
            }

            var form = _infoToForms[info];
            form.Activated -= HandleFusenFormActivated;
            _infoToForms.Remove(info);
            OnUnregistered();
        }

        public bool IsRegistered(MemoInfo info) {
            return _infoToForms.ContainsKey(info);
        }

        public FusenForm GetRegisteredForm(MemoInfo info) {
            if (!_infoToForms.ContainsKey(info)) {
                return null;
            }
            return _infoToForms[info];
        }

        public void HideAllToolStripForms() {
            foreach (var fusen in _infoToForms.Values) {
                fusen.HideToolStripForm();
            }
        }

        public void ShowAll(bool useDummy) {
            /// NotifyIconクリック時は一旦dummyをアクティブ化しないと
            /// Confidanteが非アクティブ時にShowWindow()でfusenが前に出てこない
            var dummy = default(Form);
            if (useDummy) {
                dummy = new Form();
                dummy.ShowInTaskbar = false;
                dummy.StartPosition = FormStartPosition.Manual;
                dummy.Location = new Point(-2000, -2000);
                dummy.Size = new Size();
                dummy.WindowState = FormWindowState.Minimized;
                dummy.Show();
                dummy.Activate();
            }
                 
            foreach (var fusen in _infoToForms.Values) {
                //User32PI.SetForegroundWindow(fusen.Handle);
                User32PI.ShowWindow(fusen.Handle, WindowShowStyle.ShowNoActivate);
            }

            if (dummy != null) {
                dummy.Dispose();
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected void OnRegistered() {
            var handler = Registered;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected void OnUnregistered() {
            var handler = Unregistered;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private void HandleFusenFormActivated(object sender, EventArgs e) {
            var focused = (FusenForm) sender;
            foreach (var fusen in _infoToForms.Values) {
                if (fusen != focused) {
                    fusen.HideToolStripForm();
                }
            }
        }
    }
}
