/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Controllers;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using Mkamo.Memopad.Internal.Figures;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using Mkamo.Editor.Handles;
using System.Windows.Forms;
using Mkamo.Editor.Roles;
using Mkamo.Common.Core;
using Mkamo.Common.Collection;
using Mkamo.Common.DataType;
using Mkamo.Editor.Focuses;
using Mkamo.StyledText.Core;
using System.Windows.Forms.VisualStyles;
using Mkamo.Editor.Utils;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Model.Uml;
using Mkamo.Model.Core;
using Mkamo.Memopad.Properties;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Common.Externalize;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Common.Command;
using Mkamo.Editor.Forms;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Memopad.Internal.Controllers.UIProviders;
using Mkamo.Model.Utils;
using Mkamo.Figure.Layouts;

namespace Mkamo.Memopad.Internal.Controllers {
    internal class UmlInterfaceController:
        AbstractMemoContentController<UmlInterface, UmlInterfaceFigure>, IConnectableController, IContainerController {

        // ========================================
        // field
        // ========================================
        private Lazy<IUIProvider> _uiProvider;
        private Lazy<object[]> _children;

        // ========================================
        // constructor
        // ========================================
        public UmlInterfaceController() {
            _uiProvider = new Lazy<IUIProvider>(() => new UmlInterfaceUIProvider(this));
            _children = new Lazy<object[]>(() => new object[] { Model.Attributes, Model.Operations });
        }

        // ========================================
        // property
        // ========================================
        public override IUIProvider UIProvider {
            get { return _uiProvider.Value; }
        }
    
        public IEnumerable<object> Children {
            get { return _children.Value; }
        }

        public int ChildCount {
            get { return 2; }
        }

        public bool SyncChildEditors {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void ConfigureEditor(IEditor editor) {
            var editorHandle = new MoveEditorHandle() { Cursor = Cursors.SizeAll };
            var facade = MemopadApplication.Instance;
            editorHandle.KeyMap = facade.KeySchema.MemoContentEditorKeyMap;
            editor.InstallEditorHandle(editorHandle);

            editor.InstallHandle(
                new ResizeHandle(Directions.Left) {
                    Cursor = Cursors.SizeWE
                }
            );
            editor.InstallHandle(
                new ResizeHandle(Directions.Up) {
                    Cursor = Cursors.SizeNS
                }
            );
            editor.InstallHandle(
                new ResizeHandle(Directions.Right) {
                    Cursor = Cursors.SizeWE
                }
            );
            editor.InstallHandle(
                new ResizeHandle(Directions.Down) {
                    Cursor = Cursors.SizeNS
                }
            );
            editor.InstallHandle(
                new ResizeHandle(Directions.UpLeft) {
                    Cursor = Cursors.SizeNWSE
                }
            );
            editor.InstallHandle(
                new ResizeHandle(Directions.UpRight) {
                    Cursor = Cursors.SizeNESW
                }
            );
            editor.InstallHandle(
                new ResizeHandle(Directions.DownLeft) {
                    Cursor = Cursors.SizeNESW
                }
            );
            editor.InstallHandle(
                new ResizeHandle(Directions.DownRight) {
                    Cursor = Cursors.SizeNWSE
                }
            );

            editor.InstallRole(new SelectRole());
            editor.InstallRole(new FocusRole(UmlClassifierHelper.InitUmlInterfaceFocus, UmlClassifierHelper.CommitInterfaceFocus));
            editor.InstallRole(new ResizeRole());
            editor.InstallRole(new RemoveRole());
            //editor.InstallRole(new CopyRole());
            editor.InstallRole(new ReorderRole());
            //editor.InstallRole(new SetPlainTextFontRole(FontModificationKinds.Name));

            var editorFocus = new StyledTextFocus();
            editorFocus.KeyMap = facade.KeySchema.MemoContentSingleLineFocusKeyMap;
            editorFocus.Figure.Padding = editorFocus.Figure.Padding.GetTopChanged(4);
            editor.InstallFocus(editorFocus);
        }

        protected override UmlInterfaceFigure CreateFigure(UmlInterface model) {
            return new UmlInterfaceFigure() {
                BorderWidth = 1,
                Background = new GradientBrushDescription(
                    Color.FromArgb(240, 255, 250), Color.FromArgb(220, 245, 230), 90
                ),
                Foreground = Color.DimGray,
                AutoSizeKinds = AutoSizeKinds.GrowBoth,
                Font = MemopadApplication.Instance.Settings.GetDefaultUmlFont(),
            };
        }

        protected override void RefreshEditor(RefreshContext context, UmlInterfaceFigure figure, UmlInterface model) {
            var stereotypeRun = new Run(UmlTextUtil.GetStereotypeText(model, "interface"));
            var nameRun = new Run(model.Name);

            var para = new Paragraph(stereotypeRun) {
                Padding = Insets.Empty,
                HorizontalAlignment = Mkamo.Common.DataType.HorizontalAlignment.Center,
            };
            var line = new LineSegment();
            line.InsertBefore(nameRun);
            para.InsertAfter(line);

            var st = new StyledText.Core.StyledText(para) {
                Font = MemopadApplication.Instance.Settings.GetDefaultUmlFont(),
                VerticalAlignment = Mkamo.Common.DataType.VerticalAlignment.Top,
            };

            if (model.IsAbstract) {
                nameRun.Font = new FontDescription(nameRun.Font, nameRun.Font.Style | FontStyle.Italic);
            }

            figure.StyledText = st;

            var layout = (ListLayout) figure.Layout;
            layout.Padding = new Insets(
                0,
                figure.StyledTextBounds.Height + figure.Padding.Height,
                0,
                0
            );
            figure.InvalidateLayout();

            figure.AdjustSize();

            UpdateMemoMarkHandles(model);
        }

        public override IMemento GetModelMemento() {
            var externalizer = new Externalizer();
            return externalizer.Save(Model, (key, obj) => false);
        }

        public bool CanContainChild(IModelDescriptor descriptor) {
            return
                typeof(UmlProperty).IsAssignableFrom(descriptor.ModelType) ||
                typeof(UmlOperation).IsAssignableFrom(descriptor.ModelType);
        }

        public void InsertChild(object child, int index) {
            var prop = child as UmlProperty;
            if (prop != null) {
                Model.AddAttribute(prop);
                return;
            }
            var ope = child as UmlOperation;
            if (ope != null) {
                Model.AddOperation(ope);
            }
        }

        public void RemoveChild(object child) {
            var prop = child as UmlProperty;
            if (prop != null) {
                Model.RemoveAttribute(prop);
                return;
            }
            var ope = child as UmlOperation;
            if (ope != null) {
                Model.RemoveOperation(ope);
            }
        }

        public override string GetText() {
            var ret = Model.Name;
            if (Model.Keywords != null) {
                ret = ret + Environment.NewLine + Model.Keywords;
            }
            return ret;
        }

        public void ConnectOutgoing(object connected) {
            var gene = connected as UmlGeneralization;
            if (gene != null) {
                Model.OutgoingGeneralizations.Add(gene);
                return;
            }

            var dep = connected as UmlDependency;
            if (dep != null) {
                Model.ClientDependencies.Add(dep);
                return;
            }
        }

        public void ConnectIncoming(object connected) {
            var gene = connected as UmlGeneralization;
            if (gene != null) {
                Model.IncomingGeneralizations.Add(gene);
                return;
            }

            var real = connected as UmlInterfaceRealization;
            if (real != null) {
                Model.SupplierDependencies.Add(real);
                return;
            }

            var dep = connected as UmlDependency;
            if (dep != null) {
                Model.SupplierDependencies.Add(dep);
                return;
            }
        }

        public void DisconnectOutgoing(object disconnected) {
            var gene = disconnected as UmlGeneralization;
            if (gene != null) {
                Model.OutgoingGeneralizations.Remove(gene);
                return;
            }

            var dep = disconnected as UmlDependency;
            if (dep != null) {
                Model.ClientDependencies.Remove(dep);
                return;
            }
        }

        public void DisconnectIncoming(object disconnected) {
            var gene = disconnected as UmlGeneralization;
            if (gene != null) {
                Model.IncomingGeneralizations.Remove(gene);
                return;
            }

            var real = disconnected as UmlInterfaceRealization;
            if (real != null) {
                Model.SupplierDependencies.Remove(real);
                return;
            }

            var dep = disconnected as UmlDependency;
            if (dep != null) {
                Model.SupplierDependencies.Remove(dep);
                return;
            }
        }


        // ------------------------------
        // private
        // ------------------------------
        //private void RelocateFocus(IFocus focus, INode focusFigure, IFigure hostFigure) {
        //    var classFig = (UmlInterfaceFigure) hostFigure;
        //    var r = focusFigure.GetStyledTextBoundsFor(classFig.Bounds);
        //    r = Rectangle.Union(r, classFig.NameRect);

        //    focusFigure.MinSize = classFig.NameRect.Size;
        //    focusFigure.Bounds = r;
        //}
    }
}
