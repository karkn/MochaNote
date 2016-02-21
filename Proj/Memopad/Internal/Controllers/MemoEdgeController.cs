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

namespace Mkamo.Memopad.Internal.Controllers {
    internal class MemoEdgeController:
        AbstractModelController<MemoEdge, LineEdge>, IConnectionController {

        // ========================================
        // field
        // ========================================
        private Lazy<IUIProvider> _uiProvider;

        // ========================================
        // constructor
        // ========================================
        internal MemoEdgeController() {
            _uiProvider = new Lazy<IUIProvider>(() => new MemoEdgeUIProvider(this));
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

            editor.InstallHandle(new CompositeEdgePointAndAnchorHandle() { Cursor = Cursors.Cross });

            Func<bool> orth = () => Model.Kind == MemoEdgeKind.Orthogonal || Model.Kind == MemoEdgeKind.OrthogonalMidpoint;
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

        protected override LineEdge CreateFigure(MemoEdge edge) {
            var ret = new LineEdge() {
                LineColor = Color.DimGray,
                LineWidth = 1,
            };

            //RefreshFigure(ret, edge);

            return ret;
        }

        protected override void RefreshEditor(RefreshContext context, LineEdge figure, MemoEdge model) {
            switch (model.Kind) {
                case MemoEdgeKind.Orthogonal: {
                    if (figure.Router == null || figure.Router.GetType() != typeof(OrthogonalRouter)) {
                        figure.Router = new OrthogonalRouter();
                        figure.ConnectionMethod = ConnectionMethodKind.Nearest;
                    }
                    break;
                }
                case MemoEdgeKind.OrthogonalMidpoint: {
                    if (figure.Router == null || figure.Router.GetType() != typeof(OrthogonalMidpointRouter)) {
                        figure.Router = new OrthogonalMidpointRouter();
                        figure.ConnectionMethod = ConnectionMethodKind.SideMidpointOfNearest;
                    }
                    break;
                }
                case MemoEdgeKind.Central: {
                    if (figure.Router == null || figure.Router.GetType() != typeof(CentralRouter)) {
                        figure.Router = new CentralRouter();
                        figure.ConnectionMethod = ConnectionMethodKind.Center;
                    }
                    break;
                }
                case MemoEdgeKind.Normal: {
                    if (figure.Router != null) {
                        figure.Router = null;
                        figure.ConnectionMethod = ConnectionMethodKind.Intersect;
                    }
                    break;
                }
            }

            switch (model.StartCapKind) {
                case MemoEdgeCapKind.Arrow: {
                    if (!(figure.SourceDecoration is ArrowEdgeDecoration)) {
                        figure.SourceDecoration = new ArrowEdgeDecoration() {
                            Foreground = Color.DimGray,
                        };
                    }
                    break;
                }
                case MemoEdgeCapKind.Normal: {
                    if (figure.SourceDecoration != null) {
                        figure.SourceDecoration = null;
                    }
                    break;
                }
            }
            switch (model.EndCapKind) {
                case MemoEdgeCapKind.Arrow: {
                    if (!(figure.TargetDecoration is ArrowEdgeDecoration)) {
                        figure.TargetDecoration = new ArrowEdgeDecoration() {
                            Foreground = Color.DimGray,
                        };
                    }
                    break;
                }
                case MemoEdgeCapKind.Normal: {
                    if (figure.TargetDecoration != null) {
                        figure.TargetDecoration = null;
                    }
                    break;
                }
            }
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
            return connected is MemoNode && (Model == null ? true: Model.CanConnectSource);
        }

        public bool CanConnectTarget(object connected) {
            return connected is MemoNode && (Model == null ? true: Model.CanConnectTarget);
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
