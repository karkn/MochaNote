/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Mkamo.Common.Collection;
using Mkamo.Common.Event;
using Mkamo.Common.Visitor;

namespace Mkamo.Common.Structure {
    [Serializable]
    public class StructuredSupport<TElem>:
        CompositeSupport<TElem, TElem>,
        IStructured<TElem>,
        IVisitable<TElem>,
        IDetailedNotifyPropertyChanged,
        IDetailedNotifyPropertyChanging
        where TElem: class, IComposite<TElem, TElem>
    {
        // ========================================
        // constructor
        // ========================================
        public StructuredSupport(TElem self): base(self) {
        }

    }
}
