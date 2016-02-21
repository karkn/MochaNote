/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using System.Windows.Forms;

namespace Mkamo.Editor.Handles {
    public abstract class AbstractAuxiliaryHandle: AbstractHandle, IAuxiliaryHandle {
        // ========================================
        // field
        // ========================================
        private bool _hideOnFocus;

        // ========================================
        // constructor
        // ========================================
        protected AbstractAuxiliaryHandle(bool hideOnFocus) {
            _hideOnFocus = hideOnFocus;
        }

        protected AbstractAuxiliaryHandle(): this(true) {
        }

        // ========================================
        // property
        // ========================================
        public abstract IFigure Figure { get; }

        public virtual bool HideOnFocus {
            get { return _hideOnFocus; }
            set { _hideOnFocus = value; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected override IFigure _Figure {
            get { return Figure; }
        }

        // ========================================
        // method
        // ========================================
        public abstract void Relocate(IFigure hostFigure);

    }
}
