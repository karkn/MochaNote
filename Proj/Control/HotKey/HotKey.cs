/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Mkamo.Control.HotKey {
    /// <summary>
    /// Control for Hotkey event handler
    /// </summary>
    public class HotKey: System.Windows.Forms.Control {
        // ========================================
        // static field
        // ========================================
        private const int WM_HOTKEY = 0x312;

        // ========================================
        // field
        // ========================================
        private Dictionary<int, string> _idToHotKey;
        private Dictionary<string, Action> _hotKeyToActions;

        // ========================================
        // constructor
        // ========================================
        public HotKey() {
            _idToHotKey = new Dictionary<int, string>();
            _hotKeyToActions = new Dictionary<string, Action>();
        }

        // ========================================
        // event
        // ========================================
        //public event EventHandler<HotKeyEventArgs> HotKeyPressed;

        // ========================================
        // method
        // ========================================
        public bool AddHotKey(string hotKey, Action action) {
            if (string.IsNullOrEmpty(hotKey) || action == null) {
                return false;
            }
            if (!base.IsHandleCreated) {
                base.CreateControl();
            }
            var ret = Register(hotKey);
            if (ret) {
                _hotKeyToActions[hotKey] = action;
            }
            return ret;
        }
        
        public void RemoveHotKey(string strKey) {
            if (string.IsNullOrEmpty(strKey)) {
                return;
            }
            if (_hotKeyToActions.ContainsKey(strKey)) {
                _hotKeyToActions.Remove(strKey);
            }
            Unregister(strKey);
        }
       
        public void RemoveAllHotKeys() {
            _hotKeyToActions.Clear();
            UnregisterAll();
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected override void WndProc(ref Message m) {
            if (m.Msg == WM_HOTKEY) {
                var hotKey = _idToHotKey[m.WParam.ToInt32()];
                if (_hotKeyToActions.ContainsKey(hotKey)) {
                    var act = _hotKeyToActions[hotKey];
                    act();
                }
                
            } else {
                base.WndProc(ref m);
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        /// <summary>
        /// Rregister the given hotkey
        /// </summary>
        /// <param name="strHotKey">hotkey</param>
        /// <returns>success flag of register action</returns>
        private bool Register(string strHotKey) {
            Unregister(strHotKey);
            var hotKeyId = HotKeyUtils.GlobalAddAtom("RE:" + strHotKey);
            if (hotKeyId == 0) {
                throw new Exception(string.Format("Could not register atom for {0} hotkey", strHotKey));
            }

            if (HotKeyUtils.RegisterKey(this, hotKeyId, strHotKey)) {
                _idToHotKey.Add(hotKeyId, strHotKey);
                return true;
            }
            return false;
        }
      
        /// <summary>
        /// Unregister all registered hot keys
        /// </summary>
        private void UnregisterAll() {
            foreach (KeyValuePair<int, string> hotKey in _idToHotKey) {
                HotKeyUtils.UnregisterKey(this, hotKey.Key);
                HotKeyUtils.GlobalDeleteAtom(hotKey.Key);
            }
            _idToHotKey.Clear();
        }
        
        /// <summary>
        /// Unregister the specified registered hot keys
        /// </summary>
        /// <param name="strKey">registered hotkey</param>
        private void Unregister(string strKey) {
            int intKey = 0;
            foreach (KeyValuePair<int, string> hotKey in _idToHotKey) {
                if (hotKey.Value == strKey) {
                    intKey = hotKey.Key;
                    HotKeyUtils.UnregisterKey(this, hotKey.Key);
                    HotKeyUtils.GlobalDeleteAtom(hotKey.Key);
                    break;
                }
            }
            if (intKey > 0) {
                _idToHotKey.Remove(intKey);
            }
        }

      
        private void InitializeComponent() {
            this.SuspendLayout();
            // 
            // HotKey
            // 
            this.Visible = false;
            this.ResumeLayout(false);

        }
    }
    /// <summary>
    /// EventArgs class for HotKeyPressed event
    /// </summary>
    public class HotKeyEventArgs : EventArgs {
        public string HotKey { get; set; }
    }
}
