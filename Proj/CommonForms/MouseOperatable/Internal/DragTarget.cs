/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Common.Forms.MouseOperatable.Internal {
    internal class DragTarget: IDragTarget {
        // ========================================
        // field
        // ========================================
        //private string[] _supportedFormats;

        // ========================================
        // constructor
        // ========================================
        //public DragTarget(string[] supportedFormats) {
        //    _supportedFormats = supportedFormats;
        //}
        public DragTarget() {
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<DragEventArgs> DragEnter;
        public event EventHandler<DragEventArgs> DragDrop;
        public event EventHandler<DragEventArgs> DragOver;
        public event EventHandler<EventArgs> DragLeave;

        // ========================================
        // property
        // ========================================
        //public string[] SupportedFormats {
        //    get { return _supportedFormats; }
        //}

        // ========================================
        // method
        // ========================================
        public void HandleDragOver(object sender, DragEventArgs e) {
            var handler = DragOver;
            if (handler != null) {
                handler(sender, e);
            }
        }

        public void HandleDragDrop(object sender, DragEventArgs e) {
            var handler = DragDrop;
            if (handler != null) {
                handler(sender, e);
            }
        }

        public void HandleDragEnter(object sender, DragEventArgs e) {
            var handler = DragEnter;
            if (handler != null) {
                handler(sender, e);
            }
        }

        public void HandleDragLeave(object sender, EventArgs e) {
            var handler = DragLeave;
            if (handler != null) {
                handler(sender, e);
            }
        }

    }
}
