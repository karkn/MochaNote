/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Figures;
using System.Drawing;
using Mkamo.Figure.Core;

namespace Mkamo.Figure.Core {
    public class GraphicsUsingContext: IDisposable {
        // ========================================
        // field
        // ========================================
        Canvas _owner;
        Graphics _graphics = null;
        bool _created = false;

        // ========================================
        // constructor
        // ========================================
        public GraphicsUsingContext(Canvas owner) {
            _owner = owner;
        }

        // ========================================
        // destructor
        // ========================================
        public void Dispose() {
            if (_created) {
                _owner._CurrentGraphics = null;
                if (_graphics != null) {
                    _graphics.Dispose();
                }
            }
        }

        // ========================================
        // property
        // ========================================
        /// <summary>
        /// Rootがたどれない場合はnull
        /// </summary>
        public Graphics Graphics {
            get {
                if (_graphics == null) {
                    if (_owner != null) {
                        _graphics = _owner._CurrentGraphics;
                        if (_graphics == null) {
                            _graphics = _owner._CurrentGraphics = _owner.CreateGraphics();
                            _created = true;
                        }
                    }
                }
                return _graphics;
            }
        }
    }

}
