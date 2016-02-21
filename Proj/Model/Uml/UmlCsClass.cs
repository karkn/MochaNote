/*
 * Copyright (c) 2007-2011, Masahiko Kamo (mkamo@mkamo.com).
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
    [Externalizable(Type = typeof(UmlCsClass), FactoryMethodType = typeof(UmlFactory), FactoryMethod = "CreateCsClass")]
    public class UmlCsClass: UmlClass {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        protected internal UmlCsClass() {
        }

        // ========================================
        // property
        // ========================================


        // ========================================
        // method
        // ========================================
    }
}
