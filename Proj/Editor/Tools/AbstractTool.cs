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
using Mkamo.Figure.Core;

namespace Mkamo.Editor.Tools {
    public abstract class AbstractTool: ITool {
        // ========================================
        // field
        // ========================================
        private EditorCanvas _host;
        private IFigure _customFeedback;

        // ========================================
        // constructor
        // ========================================
        protected AbstractTool(IFigure customFeedback) {
            _customFeedback = customFeedback;
        }

        protected AbstractTool(): this(null) {
        }

        // ========================================
        // property
        // ========================================
        public IFigure CustomFeedback {
            get { return _customFeedback; }
            set { _customFeedback = value; }
        }

        protected EditorCanvas _Host {
            get { return _host; }
        }

        // ========================================
        // method
        // ========================================
        public void Installed(EditorCanvas host) {
            _host = host;
        }

        public void Uninstalled(EditorCanvas host) {
            _host = null;
        }

        public abstract bool HandleMouseDown(MouseEventArgs e);
        public abstract bool HandleMouseMove(MouseEventArgs e);
        public abstract bool HandleMouseUp(MouseEventArgs e);

        public abstract bool HandleDragStart(MouseEventArgs e);
        public abstract bool HandleDragMove(MouseEventArgs e);
        public abstract bool HandleDragFinish(MouseEventArgs e);
        public abstract bool HandleDragCancel(EventArgs e);

        public abstract bool HandleMouseClick(MouseEventArgs e);
        public abstract bool HandleMouseDoubleClick(MouseEventArgs e);

        public abstract bool HandleMouseEnter(EventArgs e);
        public abstract bool HandleMouseLeave(EventArgs e);
        public abstract bool HandleMouseHover(EventArgs e);

        public abstract bool HandleKeyDown(KeyEventArgs e);
        public abstract bool HandleKeyUp(KeyEventArgs e);
        public abstract bool HandleKeyPress(KeyPressEventArgs e);
        public abstract bool HandlePreviewKeyDown(PreviewKeyDownEventArgs e);
    }
}
