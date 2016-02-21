/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComponentFactory.Krypton.Toolkit;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace Mkamo.Control.ToolStrip {
    [ToolStripItemDesignerAvailability(
        ToolStripItemDesignerAvailability.ToolStrip |
        ToolStripItemDesignerAvailability.StatusStrip)]
    public class KryptonDropButtonToolStripItem: ToolStripControlHost {
        private KryptonDropButton _button;

        public KryptonDropButtonToolStripItem(): base(new KryptonDropButton()) {
            _button = Control as KryptonDropButton;
            _button.Text = "Button";
            _button.ButtonStyle = ButtonStyle.LowProfile;
        }


        [RefreshProperties(RefreshProperties.All),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public KryptonDropButton KryptonDropButtonControl {
            get { return Control as KryptonDropButton; }
        }

        public override Size GetPreferredSize(Size constrainingSize) {
            return KryptonDropButtonControl.GetPreferredSize(constrainingSize);
        }

        //protected override void OnSubscribeControlEvents(System.Windows.Forms.Control control) {
        //    base.OnSubscribeControlEvents(control);
        //}

        //protected override void OnUnsubscribeControlEvents(System.Windows.Forms.Control control) {
        //    base.OnUnsubscribeControlEvents(control);
        //}
    }
}
