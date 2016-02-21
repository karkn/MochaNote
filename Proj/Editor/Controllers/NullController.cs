/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using Mkamo.Figure.Figures;
using System.Windows.Forms;
using Mkamo.Common.Externalize;

namespace Mkamo.Editor.Controllers {
    public class NullController: IController {
        // ========================================
        // static field
        // ========================================
        private static readonly object[] EmptyArray = new object[0];

        // ========================================
        // property
        // ========================================
        public bool CanClone {
            get { return false; }
        }

        public IModelDescriptor ModelDescriptor {
            get { return null; }
        }

        public IUIProvider UIProvider {
            get { return null; }
        }

        public bool NeedAdjustSizeOnPrint {
            get { return false; }
        }

        // ========================================
        // method
        // ========================================
        public void Installed(IEditor host) {
            
        }

        public void Uninstalled(IEditor host) {
            
        }

        public void Activate() {

        }

        public void Deactivate() {

        }

        public IFigure CreateFigure(object model) {
            throw new NotSupportedException();
        }

        public void RefreshEditor(RefreshContext context, IFigure figure, object model) {

        }

        public void ConfigureEditor(IEditor editor) {

        }

        public object CloneModel() {
            return null;
        }

        public ContextMenuStrip CreateContextMenu(MouseEventArgs e) {
            return null;
        }


        public IUIProvider GetUIProvider() {
            return null;
        }

        public IMemento GetModelMemento() {
            throw new NotImplementedException();
        }

        public TransferInitializer GetTransferInitializer() {
            throw new NotImplementedException();
        }

        public virtual object GetTranserInitArgs() {
            throw new NotImplementedException();
        }


        public void DisposeModel(object model) {
            throw new NotImplementedException();
        }

        public void RestoreModel(object model) {
            throw new NotImplementedException();
        }


        public string GetText() {
            throw new NotImplementedException();
        }

    }
}
