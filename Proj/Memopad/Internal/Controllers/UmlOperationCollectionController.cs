/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Controllers;
using Mkamo.Figure.Figures;
using Mkamo.Editor.Core;
using System.Collections.ObjectModel;
using Mkamo.Common.Core;
using Mkamo.Figure.Core;
using Mkamo.Figure.Layouts;
using System.Drawing;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Forms.Descriptions;
using System.Windows.Forms;
using Mkamo.Editor.Roles;
using Mkamo.Editor.Utils;
using Mkamo.Model.Uml;
using Mkamo.Model.Core;
using Mkamo.Container.Core;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Memopad.Internal.Figures;
using Mkamo.Common.Externalize;
using System.Reflection;

namespace Mkamo.Memopad.Internal.Controllers {
    internal class UmlOperationCollectionController: AbstractController, IContainerController  {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        public UmlOperationCollectionController() {
        }

        // ========================================
        // property
        // ========================================
        public IEnumerable<object> Children {
            get { return _UmlOperationCollection.As<UmlOperation, object>(); }
        }

        public int ChildCount {
            get { return _UmlOperationCollection.Count; }
        }

        public bool SyncChildEditors {
            get { return true; }
        }

        protected UmlOperationCollection _UmlOperationCollection {
            get { return (UmlOperationCollection) Model; }
        }

        // ========================================
        // method
        // ========================================
        public override void ConfigureEditor(IEditor editor) {
            editor.InstallRole(new ContainerRole());
        }

        public override IFigure CreateFigure(object model) {
            return new UmlClassifierStructureFigure() {
                Foreground = Color.DimGray,
                IsBackgroundEnabled = false,
                AutoSizeKinds = AutoSizeKinds.GrowBoth,
                Layout = new ListLayout() {
                    AdjustItemWidth = false,
                    EmptyPreferredSize = new Size(2, 10),
                },
            };
        }

        public override void RefreshEditor(RefreshContext context, IFigure figure, object model) {
            //figure.IsVisible = _UmlOperationCollection.Count > 0;
            Host.Parent.RebuildChildren();
            Host.Parent.Figure.InvalidateLayout();
            //((INode) Host.Parent.Figure).AdjustSize();
        }

        public override IMemento GetModelMemento() {
            var externalizer = new Externalizer();
            return externalizer.Save(new UmlOperationCollectionReplacingExternalizable(), null);
        }

        public bool CanContainChild(IModelDescriptor descriptor) {
            return typeof(UmlOperation).IsAssignableFrom(descriptor.ModelType);
        }

        public void InsertChild(object child, int index) {
            _UmlOperationCollection.Owner.InsertOperation(index, (UmlOperation) child);
        }

        public void RemoveChild(object child) {
            _UmlOperationCollection.Owner.RemoveOperation((UmlOperation) child);
        }

        public override string GetText() {
            return "";
        }

        // ========================================
        // class
        // ========================================
        [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = false)]
        protected class UmlOperationCollectionReplacingExternalizable: IReplacingExternalizable {
            private UmlClassifier _parent;

            public UmlOperationCollectionReplacingExternalizable() {
            }

            public void WriteExternal(IMemento memento, ExternalizeContext context) {
            }

            public void ReadExternal(IMemento memento, ExternalizeContext context) {
                _parent = context.ExtendedData[EditorConsts.RestoreEditorStructureParentModel] as UmlClassifier;
            }

            public object GetReplaced() {
                return _parent.Operations;
            }
        }
    }
}
