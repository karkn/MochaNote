/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class ConfirmDataConversionForm: Form {
        public ConfirmDataConversionForm() {
            InitializeComponent();
        }

        public bool DontShow {
            get { return _dontShowCheckBox.Checked; }
        }
    }
}
