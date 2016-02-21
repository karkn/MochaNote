/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Editor.Core;
using Mkamo.Figure.Figures;
using Mkamo.Common.Collection;

namespace Mkamo.Editor.Controllers {
    public class RootController: AbstractController, IContainerController {
        // ========================================
        // property
        // ========================================
        public IEnumerable<object> Children {
            get { return EmptyList<object>.Empty; }
        }

        public int ChildCount {
            get { return 0; }
        }

        public bool SyncChildEditors {
            get { return false; }
        }

        // ========================================
        // method
        // ========================================
        public override void Activate() {

        }

        public override void Deactivate() {

        }

        public override IFigure CreateFigure(object model) {
            /// RootFigureはCanvasが作るのでこのメソッドが呼ばれることはない
            throw new InvalidOperationException("This method must not be called");
        }

        public override void RefreshEditor(RefreshContext context, IFigure figure, object model) {
            
        }

        public override void ConfigureEditor(IEditor editor) {
            
        }

        public override Mkamo.Common.Externalize.IMemento GetModelMemento() {
            return null;
        }

        public bool CanContainChild(IModelDescriptor descriptor) {
            return true;
        }

        public void InsertChild(object child, int index) {

        }

        public void RemoveChild(object child) {

        }

        public override string GetText() {
            return null;
        }

    }
}
