/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace Mkamo.Control.HotKey {
    /// <summary>
    /// Utility class for Hotkey Handler
    /// </summary>
    internal sealed class HotKeyUtils
    {
       
        /// <summary>
        /// Check whether specified Hotkey modifier available or not from given HotKey
        /// </summary>
        /// <param name="key">HotKey</param>
        /// <param name="keyModifier">Key Modifier from Keys enum</param>
        /// <param name="hotKeyModifier">Hotkey modifier</param>
        /// <returns>Matched Hotkeymodifier</returns>
        private static HotKeyModifiers CheckModifier(Keys key, Keys keyModifier, HotKeyModifiers hotKeyModifier) {
            if ((key & keyModifier) == keyModifier) {
                return hotKeyModifier;
            }
            return HotKeyModifiers.None;
        }

        /// <summary>
        /// Parse and register the suppiled Hotkey
        /// </summary>
        /// <param name="winHandle">Handle for hotkey event receiver</param>
        /// <param name="hotKeyId">Global atom id for given hotkey</param>
        /// <param name="strKey">HotKey</param>
        /// <returns>success flag of register action</returns>
        public static bool RegisterKey(System.Windows.Forms.Control winHandle, int hotKeyId, string strKey) {
            if (strKey == null) {
                strKey = "";
            }
            Keys keyNone = Keys.None;
            try {
                if (string.IsNullOrEmpty(strKey))
                    throw new Exception("Hot key is empty");
                else {
                    string[] strKeys = strKey.Split(new char[] { '+' });
                    bool isFirst = true;
                    foreach (string strKeyItem in strKeys) {
                        if (string.IsNullOrEmpty(strKeyItem))
                            throw new Exception("Invalid Hotkey");

                        if (isFirst) {
                            keyNone = (Keys) Enum.Parse(typeof(Keys), strKeyItem.Trim());
                            isFirst = false;
                        } else
                            keyNone |= (Keys) Enum.Parse(typeof(Keys), strKeyItem.Trim());
                    }
                }
            } catch (Exception exception) {
                throw exception;
            }
            if (keyNone == Keys.None) {
                return false;
            }

            return RegisterKey(winHandle, hotKeyId, keyNone);
        }

        /// <summary>
        /// Register the suppiled Hotkey
        /// </summary>
        /// <param name="winHandle">Handle for hotkey event receiver</param>
        /// <param name="id">Global atom id for given hotkey</param>
        /// <param name="key">HotKey</param>
        /// <returns>success flag of register action</returns>
        public static bool RegisterKey(System.Windows.Forms.Control winHandle, int id, Keys key) {
            HotKeyModifiers keyModifierNone = HotKeyModifiers.None;
            keyModifierNone |= CheckModifier(key, Keys.Control, HotKeyModifiers.Control);
            keyModifierNone |= CheckModifier(key, Keys.Alt, HotKeyModifiers.Alt);
            keyModifierNone |= CheckModifier(key, Keys.Shift, HotKeyModifiers.Shift);
            keyModifierNone |= CheckModifier(key, Keys.LWin, HotKeyModifiers.Windows);
            keyModifierNone |= CheckModifier(key, Keys.RWin, HotKeyModifiers.Windows);
            return RegisterHotKey(winHandle.Handle, id, keyModifierNone, key & Keys.KeyCode);
        }
        /// <summary>
        /// Unregister specified hotkey
        /// </summary>
        /// <param name="winHandle">Hotkey event receiver</param>
        /// <param name="hotKeyId">Global atom id for given hotkey</param>
        /// <returns>success flag of unregister action</returns>
        public static bool UnregisterKey(System.Windows.Forms.Control winHandle, int hotKeyId) {
            return UnregisterHotKey(winHandle.Handle, hotKeyId);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GlobalAddAtom(string lpString);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern ushort GlobalDeleteAtom(int nAtom);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int keyId, HotKeyModifiers fsModifiers, Keys vk);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Keymodifers for hotkey
        /// </summary>
        [Flags]
        internal enum HotKeyModifiers {
            Alt = 1,
            Control = 2,
            None = 0,
            Shift = 4,
            Windows = 8
        }
    }
    
}
