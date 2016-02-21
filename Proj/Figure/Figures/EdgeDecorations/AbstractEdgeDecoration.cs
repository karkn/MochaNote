/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using System.Drawing;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Disposable;
using Mkamo.Common.Diagnostics;
using Mkamo.Common.Externalize;

namespace Mkamo.Figure.Figures.EdgeDecorations {
    [Externalizable]
    public abstract class AbstractEdgeDecoration: IEdgeDecoration {
        // ========================================
        // static field
        // ========================================
        public static readonly string PenResourceKey = "AbstractEdgeDecoration.Pen";
        public static readonly string BrushResourceKey = "AbstractEdgeDecoration.Brush";

        // ========================================
        // field
        // ========================================
        [NonSerialized]
        private ResourceCache _resourceCache; /// lazy

        [NonSerialized]
        private bool _isActive;

        private Line _target;
        private Color _foreground;
        private Color _background;
        private int _lineWidth;

        // ========================================
        // constructor
        // ========================================
        protected AbstractEdgeDecoration() {
            _lineWidth = 1;
        }

        // ========================================
        // event
        // ========================================
        [field:NonSerialized]
        public event EventHandler<EventArgs> TargetChanged;

        [field:NonSerialized]
        public event EventHandler<EventArgs> ForegroundChanged;

        [field:NonSerialized]
        public event EventHandler<EventArgs> BackgroundChanged;

        [field:NonSerialized]
        public event EventHandler<EventArgs> LineWidthChanged;

        // ========================================
        // property
        // ========================================
        [External]
        public Line Target {
            get { return _target; }
            set {
                if (value == _target) {
                    return;
                }
                _target = value;
                OnTargetChanged();
            }
        }

        [External]
        public Color Foreground {
            get { return _foreground; }
            set {
                if (value == _foreground) {
                    return;
                }
                _foreground = value;
                OnForegroundChanged();
            }
        }

        [External]
        public Color Background {
            get { return _background; }
            set {
                if (value == _background) {
                    return;
                }
                _background = value;
                OnBackgroundChanged();
            }
        }

        [External]
        public int LineWidth {
            get { return _lineWidth; }
            set {
                if (value == _lineWidth) {
                    return;
                }
                _lineWidth = value;
                OnLineWidthChanged();
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected ResourceCache _ResourceCache {
            get {
                if (_resourceCache == null) {
                    _resourceCache = new ResourceCache();
                    InitResourceCache();
                    _resourceCache.Enabled = _isActive;
                }
                return _resourceCache;
            }
        }

        protected Pen _PenResource {
            get { return (Pen) _ResourceCache.GetResource(PenResourceKey); }
        }

        protected Brush _BrushResource {
            get { return (Brush) _ResourceCache.GetResource(BrushResourceKey); }
        }

        // ========================================
        // method
        // ========================================
        public abstract void Paint(Graphics g);

        public virtual void Activate() {
            _isActive = true;
            _ResourceCache.Enabled = true;
        }

        public virtual void Deactivate() {
            DisposeResourceCache();
            _isActive = false;
            _ResourceCache.Enabled = false;
        }

        protected virtual void InitResourceCache() {
            _ResourceCache.RegisterResourceCreator(
                PenResourceKey,
                () => new Pen(Foreground, _lineWidth),
                ResourceDisposingPolicy.Explicit
            );
            
            _ResourceCache.RegisterResourceCreator(
                BrushResourceKey,
                () => new SolidBrush(Background),
                ResourceDisposingPolicy.Explicit
            );
        }

        protected virtual void DisposeResourceCache() {
            _ResourceCache.DisposeResource(PenResourceKey);
            _ResourceCache.DisposeResource(BrushResourceKey);
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnTargetChanged() {
            DisposeResourceCache();

            var handler = TargetChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnForegroundChanged() {
            _ResourceCache.DisposeResource(PenResourceKey);

            var handler = ForegroundChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnBackgroundChanged() {
            _ResourceCache.DisposeResource(BrushResourceKey);
            
            var handler = BackgroundChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnLineWidthChanged() {
            _ResourceCache.DisposeResource(PenResourceKey);

            var handler = LineWidthChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

    }
}
