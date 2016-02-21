/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Figure.Internal.Core;
using Mkamo.Common.Externalize;
using System.Windows.Forms;
using System.Drawing;

namespace Mkamo.Figure.Figures {
    public class RootFigure: Layer, IDisposable {
        // ========================================
        // field
        // ========================================
        private Canvas _canvas;
        private IDirtManager _dirtyManager;
        private EventDispatcher _eventDispatcher;

        // ========================================
        // property
        // ========================================
        public override IDirtManager DirtManager {
            get { return _dirtyManager?? base.DirtManager; }
        }

        public override RootFigure Root {
            get { return this; }

        }

        public Canvas Canvas {
            get { return _canvas; }
            set { _canvas = value; }
        }

        public Rectangle Viewport {
            get { return _canvas.Viewport; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        // === AbstractFigure ==========
        protected override bool _ChildrenFollowOnBoundsChanged {
            get { return false; }
        }

        // ------------------------------
        // internal
        // ------------------------------
        internal IDirtManager _DirtManager {
            set { _dirtyManager = value; }
        }


        // ------------------------------
        // protected internal
        // ------------------------------
        protected internal override EventDispatcher _EventDispatcher {
            get { return _eventDispatcher; }
            set { _eventDispatcher = value; }
        }

        // ========================================
        // method
        // ========================================
        // === IDisposable ==========
        public void Dispose() {
            DisposeResourceCacheAll();
            GC.SuppressFinalize(this);
        }

    }
}
