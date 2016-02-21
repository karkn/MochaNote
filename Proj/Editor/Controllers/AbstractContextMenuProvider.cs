/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using System.Windows.Forms;
using System.Drawing;
using Mkamo.Common.Core;

namespace Mkamo.Editor.Controllers {
    public abstract class AbstractContextMenuProvider: IContextMenuProvider {
        // ========================================
        // field
        // ========================================
        private ContextMenuStrip _contextMenu;

        private Lazy<ToolStripSeparator> _separator1;
        private Lazy<ToolStripSeparator> _separator2;
        private Lazy<ToolStripSeparator> _separator3;
        private Lazy<ToolStripSeparator> _separator4;
        private Lazy<ToolStripSeparator> _separator5;
        private Lazy<ToolStripSeparator> _separator6;
        private Lazy<ToolStripSeparator> _separator7;
        private Lazy<ToolStripSeparator> _separator8;
        private Lazy<ToolStripSeparator> _separator9;

        // ========================================
        // constructor
        // ========================================
        protected AbstractContextMenuProvider() {
            _separator1 = new Lazy<ToolStripSeparator>(() => new ToolStripSeparator());
            _separator2 = new Lazy<ToolStripSeparator>(() => new ToolStripSeparator());
            _separator3 = new Lazy<ToolStripSeparator>(() => new ToolStripSeparator());
            _separator4 = new Lazy<ToolStripSeparator>(() => new ToolStripSeparator());
            _separator5 = new Lazy<ToolStripSeparator>(() => new ToolStripSeparator());
            _separator6 = new Lazy<ToolStripSeparator>(() => new ToolStripSeparator());
            _separator7 = new Lazy<ToolStripSeparator>(() => new ToolStripSeparator());
            _separator8 = new Lazy<ToolStripSeparator>(() => new ToolStripSeparator());
            _separator9 = new Lazy<ToolStripSeparator>(() => new ToolStripSeparator());
        }

        // ========================================
        // property
        // ========================================
        protected virtual ContextMenuStrip _ContextMenu {
            get {
                if (_contextMenu == null) {
                    _contextMenu = new ContextMenuStrip();
                    _contextMenu.Font = SystemFonts.MenuFont;
                }
                return _contextMenu;
            }
        }

        protected ToolStripSeparator _Separator1 {
            get { return _separator1.Value; }
        }

        protected ToolStripSeparator _Separator2 {
            get { return _separator2.Value; }
        }

        protected ToolStripSeparator _Separator3 {
            get { return _separator3.Value; }
        }

        protected ToolStripSeparator _Separator4 {
            get { return _separator4.Value; }
        }

        protected ToolStripSeparator _Separator5 {
            get { return _separator5.Value; }
        }

        protected ToolStripSeparator _Separator6 {
            get { return _separator6.Value; }
        }

        protected ToolStripSeparator _Separator7 {
            get { return _separator7.Value; }
        }

        protected ToolStripSeparator _Separator8 {
            get { return _separator8.Value; }
        }

        protected ToolStripSeparator _Separator9 {
            get { return _separator9.Value; }
        }

        // ========================================
        // method
        // ========================================
        public virtual void Dispose() {
            if (_contextMenu != null) {
                _contextMenu.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        public abstract ContextMenuStrip GetContextMenu(MouseEventArgs e);
    }
}
