/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.StyledText.Core;

namespace Mkamo.Editor.Core {
    public class LinkClickedEventArgs: EventArgs {
        private Link _link;
        public LinkClickedEventArgs(Link link) {
            _link = link;
        }
        public Link Link {
            get { return _link; }
        }
    }
}
