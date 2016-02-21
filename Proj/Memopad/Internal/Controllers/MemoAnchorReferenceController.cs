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
using Mkamo.Model.Memo;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Common.Externalize;
using Mkamo.Figure.Routers;
using Mkamo.Editor.Forms;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Memopad.Internal.Controllers.UIProviders;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Common.Core;
using Mkamo.Memopad.Internal.Roles;
using Mkamo.Memopad.Internal.Handles;

namespace Mkamo.Memopad.Internal.Controllers {
    internal class MemoAnchorReferenceController:
        AbstractModelController<MemoAnchorReference, LineEdge>, IConnectionController {

        // ========================================
        // field
        // ========================================
        private Lazy<IUIProvider> _uiProvider;

        // ========================================
        // constructor
        // ========================================
        internal MemoAnchorReferenceController() {
            _uiProvider = new Lazy<IUIProvider>(() => new MemoAnchorReferenceUIProvider(this));
        }

        // ========================================
        // property
        // ========================================
        public override IUIProvider UIProvider {
            get { return _uiProvider.Value; }
        }

        public bool CanDisconnectSource {
            get { return true; }
        }

        public bool CanDisconnectTarget {
            get { return true; }
        }

        public object SourceModel {
            get { return Model.Source; }
        }

        public object TargetModel {
            get { return Model.Target; }
        }

        // ========================================
        // method
        // ========================================
        public override void ConfigureEditor(IEditor editor) {
            var editorHandle = new MoveEditorHandle() { Cursor = Cursors.SizeAll };
            var facade = MemopadApplication.Instance;
            editorHandle.KeyMap = facade.KeySchema.MemoContentEditorKeyMap;
            editor.InstallEditorHandle(editorHandle);

            editor.InstallHandle(new CompositeCommentAnchorHandle() { Cursor = Cursors.Cross });
            //editor.InstallHandle(new CompositeEdgePointAndAnchorHandle() { Cursor = Cursors.Cross });
            //editor.InstallHandle(new CompositeNewEdgePointHandle() { Cursor = Cursors.Cross });

            editor.InstallRole(new EdgeRole());
            editor.InstallRole(new SelectRole());
            editor.InstallRole(new RemoveRole());
            editor.InstallRole(new CopyRole());
            editor.InstallRole(new MoveEdgeRole());
            //editor.InstallRole(new ConnectRole());
            editor.InstallRole(new ConnectCommentRole());
            editor.InstallRole(new ReorderRole());
        }

        public override IMemento GetModelMemento() {
            var externalizer = new Externalizer();
            return externalizer.Save(Model, (key, obj) => false);
        }

        protected override LineEdge CreateFigure(MemoAnchorReference edge) {
            var ret = new LineEdge() {
                LineColor = Color.DimGray,
                LineWidth = 1,
                Router = new CommentRouter(),
                ConnectionMethod = ConnectionMethodKind.Comment,
                LineDashStyle = System.Drawing.Drawing2D.DashStyle.Dash,
                //SourceDecoration = new CircleEdgeDecoration() {
                //    Background = Color.DimGray,
                //},
            };
            ret.EdgeBehaviorOptions.RouteOnMoved = true;
            ret.EdgeBehaviorOptions.RouteOnNodeMaxSizeChanged = true;

            //RefreshFigure(ret, edge);

            return ret;
        }

        protected override void RefreshEditor(RefreshContext context, LineEdge figure, MemoAnchorReference model) {
            //switch (model.StartCapKind) {
            //    case MemoAnchorReferenceCapKind.Arrow: {
            //        if (!(figure.SourceDecoration is ArrowEdgeDecoration)) {
            //            figure.SourceDecoration = new ArrowEdgeDecoration() {
            //                Foreground = Color.DimGray,
            //            };
            //        }
            //        break;
            //    }
            //    case MemoAnchorReferenceCapKind.Normal: {
            //        if (figure.SourceDecoration != null) {
            //            figure.SourceDecoration = null;
            //        }
            //        break;
            //    }
            //}
            //switch (model.EndCapKind) {
            //    case MemoAnchorReferenceCapKind.Arrow: {
            //        if (!(figure.TargetDecoration is ArrowEdgeDecoration)) {
            //            figure.TargetDecoration = new ArrowEdgeDecoration() {
            //                Foreground = Color.DimGray,
            //            };
            //        }
            //        break;
            //    }
            //    case MemoAnchorReferenceCapKind.Normal: {
            //        if (figure.TargetDecoration != null) {
            //            figure.TargetDecoration = null;
            //        }
            //        break;
            //    }
            //}
        }

        public override string GetText() {
            var ret = "";
            if (Model.Keywords != null) {
                ret = Model.Keywords;
            }
            return ret;
        }

        // === IConnectionController ==========
        public bool CanConnectSource(object connected) {
            return connected is MemoNode && (connected == null || Model == null || connected != Model.Target);
        }

        public bool CanConnectTarget(object connected) {
            return connected is MemoText && (connected == null || Model == null || connected != Model.Source);
        }

        public void ConnectSource(object connected) {
            Model.Source = connected as MemoNode;
        }

        public void ConnectTarget(object connected) {
            Model.Target = connected as MemoNode;
        }

        public void DisconnectSource() {
            Model.Source = null;
        }

        public void DisconnectTarget() {
            Model.Target = null;
        }
    }
}
