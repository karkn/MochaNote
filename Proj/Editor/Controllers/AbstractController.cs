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
using Mkamo.Common.Diagnostics;
using Mkamo.Common.Collection;
using System.Windows.Forms;
using Mkamo.Common.Externalize;

namespace Mkamo.Editor.Controllers {
    /// <summary>
    /// Controllerの基底クラス．Hostのインストールを行う．
    /// </summary>
    public abstract class AbstractController: IController {
        // ========================================
        // field
        // ========================================
        private IEditor _host;

        // ========================================
        // property
        // ========================================
        public virtual IModelDescriptor ModelDescriptor {
            get { return new DefaultModelDescriptor(Model); }
        }

        public virtual IUIProvider UIProvider {
            get { return null; }
        }

        public virtual bool NeedAdjustSizeOnPrint {
            get { return false; }
        }

        public IEditor Host {
            get { return _host; }
        }

        public object Model {
            get { return _host.Model; }
        }

        public IFigure Figure {
            get { return _host.Figure; }
        }

        // ========================================
        // method
        // ========================================
        // --- install ---
        public virtual void Installed(IEditor host) {
            Contract.Requires(host != null);
            _host = host;
        }
        public virtual void Uninstalled(IEditor host) {
            _host = null;
        }

        // --- editor ---
        public abstract void ConfigureEditor(IEditor editor);

        // --- figure ---
        public abstract IFigure CreateFigure(object model);
        public abstract void RefreshEditor(RefreshContext context, IFigure figure, object model);

        // --- externalize ---
        public abstract IMemento GetModelMemento();
        public virtual TransferInitializer GetTransferInitializer() {
            return null;
        }
        public virtual object GetTranserInitArgs() {
            return null;
        }

        // --- text ---
        public abstract string GetText();

        // --- activate ---
        public virtual void Activate() {
        }
        public virtual void Deactivate() {
        }

        // --- model lifecycle ---
        public virtual void DisposeModel(object model) {
        }

        public virtual void RestoreModel(object model) {
        }

    }

    public abstract class AbstractController<TModel, TFigure>: AbstractController
        where TModel: class
        where TFigure: class, IFigure
    {
        // ========================================
        // field
        // ========================================
        
        // ========================================
        // property
        // ========================================
        public new TModel Model {
            get { return Host == null ? null : Host.Model as TModel; }
        }

        public new TFigure Figure {
            get { return Host == null ? null : Host.Figure as TFigure; }
        }

        // ========================================
        // method
        // ========================================
        public override void Installed(IEditor host) {
            Contract.Requires(host != null);

            Contract.Requires(host.Model is TModel);
            Contract.Requires(host.Figure is TFigure);

            base.Installed(host);
        }

        public override sealed IFigure CreateFigure(object model) {
            return CreateFigure((TModel) model);
        }

        public override sealed void RefreshEditor(RefreshContext context, IFigure figure, object model) {
            RefreshEditor(context, (TFigure) figure, (TModel) model);
        }

        protected abstract TFigure CreateFigure(TModel model);
        protected abstract void RefreshEditor(RefreshContext context, TFigure figure, TModel model);
    }
}
