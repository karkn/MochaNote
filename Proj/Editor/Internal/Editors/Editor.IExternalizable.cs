/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Common.Externalize;
using Mkamo.Figure.Core;
using Mkamo.Common.Core;

namespace Mkamo.Editor.Internal.Editors {
    partial class Editor {
        // ========================================
        // method
        // ========================================
        private bool _inLoading;

        // === IPersistable ==========
        public void WriteExternal(IMemento memento, ExternalizeContext context) {
            var modelSerializer = context.ExtendedData[EditorConsts.ModelSerializerKey] as IModelSerializer;
            if (modelSerializer == null) {
                throw new ArgumentException("context.ExtendedData is invalid");
            }

            memento.WriteExternalizable("Figure",  Figure);
            memento.WriteSerializable("ModelHint", modelSerializer.Save(Model));
            memento.WriteExternalizables("Children", Children.As<Editor, object>());
        }

        public void ReadExternal(IMemento memento, ExternalizeContext context) {
            _inLoading = true;

            var modelSerializer = context.ExtendedData[EditorConsts.ModelSerializerKey] as IModelSerializer;
            var site = context.ExtendedData[EditorConsts.EditorSiteKey] as IEditorSite;
            if (modelSerializer == null || site == null) {
                throw new ArgumentException("context.ExtendedData is invalid");
            }

            var model = modelSerializer.Load(memento.ReadSerializable("ModelHint"));
            var fig = memento.ReadExternalizable("Figure") as IFigure;
            var ctrl = site.ControllerFactory.CreateController(model); /// Controllerは作り直す

            Model = model;
            _Figure = fig;
            _Controller = ctrl;

            using (Figure.DirtManager.BeginDirty()) {
                ctrl.ConfigureEditor(this);

                if (memento.Contains("Children")) {
                    foreach (var child in memento.ReadExternalizables("Children")) {
                        var childEditor = child as Editor;
                        if (childEditor != null) {
                            AddChildEditor(childEditor);
                        }
                    }
                }

                RelocateAllHandles();
                RelocateFocus();

                //Refresh();
                //Enable();
            }

            _inLoading = false;
        }

    }
}
