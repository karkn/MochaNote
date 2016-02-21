/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Mkamo.Common.Win32.Core;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using Mkamo.Common.String;
using Mkamo.Common.Core;

namespace Mkamo.Common.Win32.User32 {
    public static class User32Util {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        /// <summary>
        /// windowをアクティブにする．
        /// windowClassName，windowTitleは前方一致，moduleFileNameは後方一致．
        /// nullを渡すとそれぞれ常に適合．
        /// </summary>
        public static void ActivateWindow(string windowClassName, string windowTitle, string moduleFileName) {
            var targetHWnd = IntPtr.Zero;

            User32PI.EnumWindows(
                (hWnd, lParam) => {
                    var bufSize = 2048;
                    var buf = new StringBuilder(bufSize);

                    User32PI.GetClassName(hWnd, buf, buf.Capacity);
                    var className = buf.ToString();

                    User32PI.GetWindowText(hWnd, buf, buf.Capacity);
                    var title = buf.ToString();

                    var validClassName =
                        string.IsNullOrEmpty(windowClassName) ||
                        (!string.IsNullOrEmpty(className) && className.StartsWith(windowClassName));
                    var validTitle =
                        string.IsNullOrEmpty(windowTitle) ||
                        (!string.IsNullOrEmpty(title) && title.StartsWith(windowTitle));
                    if (validClassName && validTitle) {
                        if (string.IsNullOrEmpty(moduleFileName)) {
                            targetHWnd = hWnd;
                            return false;
                        } else {
                            var pid = default(int);
                            User32PI.GetWindowThreadProcessId(hWnd, out pid);
                            var process = Process.GetProcessById(pid);
                            var fileName = process.MainModule.FileName;

                            if (!string.IsNullOrEmpty(fileName) && fileName.EndsWith(moduleFileName)) {
                                targetHWnd = hWnd;
                                return false;
                            }
                        }

                    }

                    return true;
                },
                IntPtr.Zero
            );

            if (targetHWnd != IntPtr.Zero) {
                RestoreWindow(targetHWnd);
            }
        }

        public static void ActivateWindow(IntPtr handle) {
            var pid = default(int);
            var fore = User32PI.GetWindowThreadProcessId(User32PI.GetForegroundWindow(), out pid);
            var target = User32PI.GetWindowThreadProcessId(handle, out pid);
            User32PI.AttachThreadInput(target, fore, true);
            User32PI.SetForegroundWindow(handle);
            User32PI.AttachThreadInput(target, fore, false);
            User32PI.SetActiveWindow(handle);
        }

        public static void GetWindows() {
            User32PI.EnumWindows(
                (hWnd, lParam) => {
                    if (User32PI.IsWindowVisible(hWnd)) {
                        var className = GetWindowClassName(hWnd);
                        var title = GetWindowText(hWnd);
                        //Console.WriteLine(title + "," + className);
                    }
                    return true;
                },
                IntPtr.Zero
            );
        }

        public static string GetWindowClassName(IntPtr handle) {
            var bufSize = 2048;
            var buf = new StringBuilder(bufSize);

            User32PI.GetClassName(handle, buf, buf.Capacity);
            return  buf.ToString();
        }

        public static string GetWindowText(IntPtr handle) {
            var bufSize = 2048;
            var buf = new StringBuilder(bufSize);

            User32PI.GetWindowText(handle, buf, buf.Capacity);
            return buf.ToString();
        }

        public static IntPtr GetNextWindow(IntPtr handle, Predicate<IntPtr> pred) {
            var ret = handle;
            do {
                ret = User32PI.GetNextWindow(ret, GetNextWindowCommand.Next);
            } while (!pred(ret) && ret != IntPtr.Zero);
            return ret;
        }

        public static bool IsOwnedWindow(IntPtr handle) {
            return User32PI.GetWindow(handle, GetWindowCmd.GW_OWNER) != IntPtr.Zero;
        }

        public static void RestoreWindow(IntPtr hWnd) {
            if (hWnd != IntPtr.Zero) {
                if (User32PI.IsIconic(hWnd)) {
                    User32PI.ShowWindowAsync(hWnd, WindowShowStyle.Restore);
                }
                User32PI.SetForegroundWindow(hWnd);
                //ActivateWindow(hWnd);
            }
        }

        public static void ResetMouseHover(IntPtr windowTrackingMouseHandle, int hoverTime) {
            // Set up the parameter collection for the API call so that the appropriate
            // control fires the event
            TRACKMOUSEEVENT parameterBag = new TRACKMOUSEEVENT(windowTrackingMouseHandle, hoverTime);

            // The actual API call
            User32PI.TrackMouseEvent(ref parameterBag);
        }

        public static string GetActiveWindowText() {
            var hwnd = User32PI.GetForegroundWindow();
            if (hwnd == IntPtr.Zero) {
                return null;
            } else {
                var buf = new StringBuilder(2048);
                User32PI.GetWindowText(hwnd, buf, buf.Capacity);
                return buf.ToString();
            }
        }

            //var dummy = 0;
            //var foreThread = User32PI.GetWindowThreadProcessId(User32PI.GetForegroundWindow(), out dummy);
            //var thisThread = Kernel32PI.GetCurrentThreadId();

            //if (User32PI.AttachThreadInput(thisThread, foreThread, true)) {
            //    var hwnd = User32PI.GetFocus();
                //User32Util.SendCopy(hwnd);
            //    User32PI.AttachThreadInput(thisThread, foreThread, false);
            //}

        //public static void SendCopy(IntPtr targetHandle) {
        //    User32PI.SendMessage(targetHandle, (int) WindowMessages.COPY, IntPtr.Zero, IntPtr.Zero);
        //}


        public static void SendCtrlC(string hotKeyStr) {
            var inputs = new List<INPUT>();

            var vks = GetKeyCodes(hotKeyStr);
            inputs.AddRange(vks.Select(vk => GetUpInput(vk)));

            inputs.Add(GetDownInput(Win32Consts.VK_CONTROL));
            inputs.Add(GetDownInput(Win32Consts.VK_C));
            inputs.Add(GetUpInput(Win32Consts.VK_C));
            inputs.Add(GetUpInput(Win32Consts.VK_CONTROL));

            User32PI.SendInput(inputs.Count, inputs.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }


        public static void LockWindowUpdate(Control c) {
            if (c.IsHandleCreated) {
                User32PI.LockWindowUpdate(c.Handle.ToInt64());
            }
        }
        public static void UnLockWindowUpdate(Control c) {
            if (c.IsHandleCreated) {
                User32PI.LockWindowUpdate(0);
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private static ushort[] GetKeyCodes(string keysStr) {
            if (StringUtil.IsNullOrWhitespace(keysStr)) {
                return new ushort[0];
            }

            var keys = Keys.None;
            var items = keysStr.Split(new [] { '+' });

            foreach (var item in items) {
                keys |= (Keys) System.Enum.Parse(typeof(Keys), item.Trim());
            }

            var ret = new List<ushort>();

            if ((keys & Keys.Shift) == Keys.Shift) {
                ret.Add(Win32Consts.VK_SHIFT);
            }
            if ((keys & Keys.Control) == Keys.Control) {
                ret.Add(Win32Consts.VK_CONTROL);
            }
            if ((keys & Keys.Alt) == Keys.Alt) {
                ret.Add(Win32Consts.VK_MENU);
            }
            if ((keys & Keys.LWin) == Keys.LWin) {
                ret.Add(Win32Consts.VK_LWIN);
            }
            if ((keys & Keys.RWin) == Keys.RWin) {
                ret.Add(Win32Consts.VK_RWIN);
            }
            ret.Add((ushort) (keys & Keys.KeyCode));

            return ret.ToArray();
        }

        private static INPUT GetDownInput(ushort key) {
            var ret = new INPUT();
            ret.type = Win32Consts.INPUT_KEYBOARD;
            ret.ki.dwFlags = 0;
            ret.ki.wVk = key;
            return ret;
        }

        private static INPUT GetUpInput(ushort key) {
            var ret = new INPUT();
            ret.type = Win32Consts.INPUT_KEYBOARD;
            ret.ki.dwFlags = Win32Consts.KEYEVENTF_KEYUP;
            ret.ki.wVk = key;
            return ret;
        }

    }
}
