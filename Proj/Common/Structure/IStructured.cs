/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Common.Structure {
    public interface IStructured<TElem>: IComposite<TElem, TElem>
        where TElem: class, IComposite<TElem, TElem>
    {

    }

    public static class IStructuredProperty {
        public static readonly string Parent = "Parent";
        public static readonly string Children = "Children";
    }
}
