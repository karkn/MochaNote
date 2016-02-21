/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Forms.KeyMap {
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class KeyActionAttribute: Attribute {
        private string _id;
        private string _description;

        public KeyActionAttribute(string description) {
            _description = description;
        }

        public string Id {
            get { return _id; }
            set { _id = value; }
        }

        public string Description {
            get { return _description; }
            set { _description = value; }
        }
    }

}
