/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;
using Mkamo.Container.Core;
using System.Runtime.Serialization;

namespace Mkamo.Model.Memo {
    [Externalizable(Type = typeof(MemoElementCollection),
        FactoryMethodType = typeof(MemoFactory),
        FactoryMethod = "CreateMemoElementCollection")]
    [Serializable]
    [DataContract(Name = "Memo")]
    [KnownType(typeof(MemoAnchorReference))]
    [KnownType(typeof(MemoContent))]
    [KnownType(typeof(MemoEdge))]
    [KnownType(typeof(MemoFile))]
    [KnownType(typeof(MemoFreehand))]
    [KnownType(typeof(MemoImage))]
    [KnownType(typeof(MemoMarkableElement))]
    [KnownType(typeof(MemoNode))]
    [KnownType(typeof(MemoShape))]
    [KnownType(typeof(MemoStyledText))]
    [KnownType(typeof(MemoTable))]
    [KnownType(typeof(MemoTableCell))]
    [KnownType(typeof(MemoTableColumn))]
    [KnownType(typeof(MemoTableLine))]
    [KnownType(typeof(MemoTableRow))]
    [KnownType(typeof(MemoText))]
    public class MemoElementCollection: EntityCollection<MemoElement> {
        // ========================================
        // constructor
        // ========================================
        protected internal MemoElementCollection() {
        }
    }
}
