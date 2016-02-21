/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Common.Forms.MouseOperatable {
    public class DragSourceEventArgs: MouseEventArgs {
        // ========================================
        // field
        // ========================================
        private bool _doit;
        private IDataObject _dataObject;
        private DragDropEffects _effects;

        // ========================================
        // constructor
        // ========================================
        public DragSourceEventArgs(MouseEventArgs e)
            : base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
        {
            _doit = false;
        }

        // ========================================
        // property
        // ========================================
        public bool DoIt {
            get { return _doit; }
            set { _doit = value ; }
        }
        public IDataObject DataObject {
            get { return _dataObject; }
            set { _dataObject = value; }
        }
        public DragDropEffects Effects {
            get { return _effects; }
            set { _effects = value; }
        }
    }
}
