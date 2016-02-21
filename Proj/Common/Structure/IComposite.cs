/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Mkamo.Common.Visitor;
using System.Collections.ObjectModel;

namespace Mkamo.Common.Structure {
    public interface IComposite<TComponent, TComposite>
        where TComponent: class
        where TComposite: class, TComponent, IComposite<TComponent, TComposite>
        /// IComposite<TComponent>: TComponentと書けないため代わりの制約
    {
        // ========================================
        // property
        // ========================================
        TComposite Parent { get; set; }
        Collection<TComponent> Children { get; }
    }

    public static class ICompositeProperty {
        public static readonly string Parent = "Parent";
        public static readonly string Children = "Children";
    }
}
