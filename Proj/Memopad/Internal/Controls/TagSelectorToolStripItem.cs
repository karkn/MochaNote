/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using Mkamo.Model.Memo;

namespace Mkamo.Memopad.Internal.Controls {
    internal class TagSelectorToolStripItem: ToolStripControlHost {
        // ========================================
        // field
        // ========================================
        private TagSelector _tagSelector;

        // ========================================
        // constructor
        // ========================================
        public TagSelectorToolStripItem(Memo memo): base(new TagSelector(memo)) {
            _tagSelector = Control as TagSelector;
            Margin = Padding.Empty;
            Padding = Padding.Empty;
        }


        // ========================================
        // property
        // ========================================
        [RefreshProperties(RefreshProperties.All),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TagSelector TagSelector {
            get { return _tagSelector; }
        }

        // ========================================
        // method
        // ========================================
        public override Size GetPreferredSize(Size constrainingSize) {
            return TagSelector.GetPreferredSize(constrainingSize);
        }

        /////
        ///// Subscribes events from the hosted control.
        /////
        ///// The control from which to subscribe events.
        //protected override void OnSubscribeControlEvents(System.Windows.Forms.Control control) {
        //    base.OnSubscribeControlEvents(control);

        //    //Add your code here to subsribe to Control Events
        //}

        /////
        ///// Unsubscribes events from the hosted control.
        /////
        ///// The control from which to unsubscribe events.
        //protected override void OnUnsubscribeControlEvents(System.Windows.Forms.Control control) {
        //    base.OnUnsubscribeControlEvents(control);
        //}
    }
}
