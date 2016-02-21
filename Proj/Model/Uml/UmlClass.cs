/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Mkamo.Common.Event;
using System.ComponentModel;
using Mkamo.Container.Core;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;

namespace Mkamo.Model.Uml {
    [Externalizable(Type = typeof(UmlClass), FactoryMethodType = typeof(UmlFactory), FactoryMethod = "CreateClass")]
    public class UmlClass: UmlClassifier {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        protected internal UmlClass() {
        }

        // ========================================
        // property
        // ========================================
        public override bool IsMarkable {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
    }
}
