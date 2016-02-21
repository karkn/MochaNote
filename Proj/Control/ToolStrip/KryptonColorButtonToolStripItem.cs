/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentFactory.Krypton.Toolkit;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.Design;

namespace Mkamo.Control.ToolStrip {
    [ToolStripItemDesignerAvailability(
        ToolStripItemDesignerAvailability.ToolStrip |
        ToolStripItemDesignerAvailability.StatusStrip)]
    public class KryptonColorButtonToolStripItem: ToolStripControlHost {
        private KryptonColorButton _button;

        public KryptonColorButtonToolStripItem(): base(new KryptonColorButton()) {
            _button = Control as KryptonColorButton;
            _button.Text = "";
            _button.ButtonStyle = ButtonStyle.LowProfile;
        }


        [RefreshProperties(RefreshProperties.All),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public KryptonColorButton KryptonColorButtonControl {
            get { return Control as KryptonColorButton; }
        }

        public override Size GetPreferredSize(Size constrainingSize) {
            return KryptonColorButtonControl.GetPreferredSize(constrainingSize);
        }

        //protected override void OnSubscribeControlEvents(System.Windows.Forms.Control control) {
        //    base.OnSubscribeControlEvents(control);
        //}

        //protected override void OnUnsubscribeControlEvents(System.Windows.Forms.Control control) {
        //    base.OnUnsubscribeControlEvents(control);
        //}
    }
}
