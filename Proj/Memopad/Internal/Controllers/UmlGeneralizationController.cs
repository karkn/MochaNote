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
using Mkamo.Figure.Figures;
using Mkamo.Editor.Handles;
using Mkamo.Editor.Roles;
using Mkamo.Editor.Roles.Edge;
using System.Drawing;
using System.Windows.Forms;
using Mkamo.Figure.Figures.EdgeDecorations;
using Mkamo.Common.Diagnostics;
using Mkamo.Model.Uml;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Common.Externalize;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Figure.Routers;

namespace Mkamo.Memopad.Internal.Controllers {
    internal class UmlGeneralizationController:
        AbstractModelController<UmlGeneralization, LineEdge>, IConnectionController {

        // ========================================
        // property
        // ========================================
        public bool CanDisconnectSource {
            get { return true; }
        }

        public bool CanDisconnectTarget {
            get { return true; }
        }

        public object SourceModel {
            get { return Model.Specific; }
        }

        public object TargetModel {
            get { return Model.General; }
        }

        
        // ========================================
        // method
        // ========================================
        protected override LineEdge CreateFigure(UmlGeneralization model) {
            Contract.Requires(model != null);

            var ret = new LineEdge() {
                LineColor = Color.DimGray,
                LineWidth = 1,
                TargetDecoration = new TriangleEdgeDecoration() {
                    Foreground = Color.DimGray,
                    Background = Color.White,
                },
            };

            return ret;
        }

        protected override void RefreshEditor(RefreshContext context, LineEdge figure, UmlGeneralization model) {
        }

        public override void ConfigureEditor(IEditor editor) {
            var editorHandle = new MoveEditorHandle() { Cursor = Cursors.SizeAll };
            var facade = MemopadApplication.Instance;
            editorHandle.KeyMap = facade.KeySchema.MemoContentEditorKeyMap;
            editor.InstallEditorHandle(editorHandle);

            editor.InstallHandle(new CompositeEdgePointAndAnchorHandle() { Cursor = Cursors.Cross });

            Func<bool> orth = () =>
                Figure.Router != null &&
                (Figure.Router.GetType() == typeof(OrthogonalRouter) || Figure.Router.GetType() == typeof(OrthogonalMidpointRouter));
            editor.InstallHandle(new CompositeNewEdgePointHandle(orth) { Cursor = Cursors.Cross });

            editor.InstallRole(new EdgeRole());
            editor.InstallRole(new SelectRole());
            editor.InstallRole(new RemoveRole());
            editor.InstallRole(new CopyRole());
            editor.InstallRole(new MoveEdgeRole());
            editor.InstallRole(new ConnectRole());
            editor.InstallRole(new ReorderRole());
        }

        public override IMemento GetModelMemento() {
            var externalizer = new Externalizer();
            return externalizer.Save(Model, (key, obj) => false);
        }

        // === IEdgeCnotroller ==========
        public bool CanConnectSource(object connected) {
            return connected is UmlClassifier;
        }

        public bool CanConnectTarget(object connected) {
            return connected is UmlClassifier;
        }

        public void ConnectSource(object connected) {
            Model.Specific = connected as UmlClassifier;
        }

        public void ConnectTarget(object connected) {
            Model.General = connected as UmlClassifier;
        }

        public void DisconnectSource() {
            Model.Specific = null;
        }

        public void DisconnectTarget() {
            Model.General = null;
        }

        public override string GetText() {
            var ret = "";
            if (Model.Keywords != null) {
                ret = Model.Keywords;
            }
            return ret;
        }
    }
}
