/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Externalize {
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ExternalAttribute: Attribute {
        private string _add;
        private string _clone;

        public string Add {
            get { return _add; }
            set { _add = value; }
        }

        public string Clone {
            get { return _clone; }
            set { _clone = value; }
        }
    }
}
