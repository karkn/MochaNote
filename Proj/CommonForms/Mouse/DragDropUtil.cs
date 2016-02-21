/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Common.Core;

namespace Mkamo.Common.Forms.Mouse {
    public static class DragDropUtil {
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
        // --- DragEventArgs ---
        public static bool IsCopyAllowed(DragEventArgs e) {
            return (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy;
        }

        public static bool IsMoveAllowed(DragEventArgs e) {
            return (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move;
        }

        public static bool IsLinkAllowed(DragEventArgs e) {
            return (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link;
        }

        public static bool IsCopy(DragEventArgs e) {
            return (e.Effect & DragDropEffects.Copy) == DragDropEffects.Copy;
        }

        public static bool IsMove(DragEventArgs e) {
            return (e.Effect & DragDropEffects.Move) == DragDropEffects.Move;
        }

        public static bool IsLink(DragEventArgs e) {
            return (e.Effect & DragDropEffects.Link) == DragDropEffects.Link;
        }

        public static bool IsNone(DragEventArgs e) {
            return e.Effect == DragDropEffects.None;
        }


        public static bool IsControlPressed(DragEventArgs e) {
            return (EnumUtil.HasAllFlags((int) e.KeyState, (int) DragEventKeyStates.Ctrl));
        }

        public static bool IsShiftPressed(DragEventArgs e) {
            return (EnumUtil.HasAllFlags((int) e.KeyState, (int) DragEventKeyStates.Shift));
        }

        public static bool IsAltPressed(DragEventArgs e) {
            return (EnumUtil.HasAllFlags((int) e.KeyState, (int) DragEventKeyStates.Alt));
        }

        public static bool IsLeftButtonPressed(DragEventArgs e) {
            return (EnumUtil.HasAllFlags((int) e.KeyState, (int) DragEventKeyStates.LeftButton));
        }

        public static bool IsRightButtonPressed(DragEventArgs e) {
            return (EnumUtil.HasAllFlags((int) e.KeyState, (int) DragEventKeyStates.RightButton));
        }

        public static bool IsMiddleButtonPressed(DragEventArgs e) {
            return (EnumUtil.HasAllFlags((int) e.KeyState, (int) DragEventKeyStates.MiddleButton));
        }

        // --- QueryContinueDragEventArgs ---
        public static bool IsControlPressed(QueryContinueDragEventArgs e) {
            return (EnumUtil.HasAllFlags((int) e.KeyState, (int) DragEventKeyStates.Ctrl));
        }

        public static bool IsShiftPressed(QueryContinueDragEventArgs e) {
            return (EnumUtil.HasAllFlags((int) e.KeyState, (int) DragEventKeyStates.Shift));
        }

        public static bool IsAltPressed(QueryContinueDragEventArgs e) {
            return (EnumUtil.HasAllFlags((int) e.KeyState, (int) DragEventKeyStates.Alt));
        }

        public static bool IsLeftButtonPressed(QueryContinueDragEventArgs e) {
            return (EnumUtil.HasAllFlags((int) e.KeyState, (int) DragEventKeyStates.LeftButton));
        }

        public static bool IsRightButtonPressed(QueryContinueDragEventArgs e) {
            return (EnumUtil.HasAllFlags((int) e.KeyState, (int) DragEventKeyStates.RightButton));
        }

        public static bool IsMiddleButtonPressed(QueryContinueDragEventArgs e) {
            return (EnumUtil.HasAllFlags((int) e.KeyState, (int) DragEventKeyStates.MiddleButton));
        }

    }
}
