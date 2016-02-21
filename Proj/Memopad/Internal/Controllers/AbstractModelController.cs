/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Controllers;
using Mkamo.Figure.Core;
using Mkamo.Container.Core;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Editor.Core;

namespace Mkamo.Memopad.Internal.Controllers {
    internal abstract class AbstractModelController<TModel, TFigure>: AbstractController<TModel, TFigure>
        where TModel: class
        where TFigure: class, IFigure {

        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================
        public override IModelDescriptor ModelDescriptor {
            get { return new DefaultModelDescriptor(typeof(TModel)); }
        }

        // ========================================
        // method
        // ========================================
        public override void DisposeModel(object model) {
            base.DisposeModel(model);
            MemopadApplication.Instance.Container.Remove(model);
        }

        public override void RestoreModel(object model) {
            base.RestoreModel(model);
            MemopadApplication.Instance.Container.Persist(model);
        }
    }
}
