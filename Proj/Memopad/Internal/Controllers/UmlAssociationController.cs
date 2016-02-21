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
using Mkamo.Common.String;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Figure.Layouts.Locators;
using Mkamo.Figure.Layouts;
using Mkamo.Model.Uml;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Common.Externalize;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Memopad.Internal.Controllers.UIProviders;
using Mkamo.StyledText.Core;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Model.Utils;
using System.ComponentModel;
using Mkamo.Common.Core;
using Mkamo.Figure.Routers;

namespace Mkamo.Memopad.Internal.Controllers {
    internal class UmlAssociationController: AbstractModelController<UmlAssociation, LineEdge>, IConnectionController {
        // ========================================
        // field
        // ========================================
        private Lazy<IUIProvider> _uiProvider;

        // ========================================
        // constructor
        // ========================================
        public UmlAssociationController() {
            _uiProvider = new Lazy<IUIProvider>(() => new UmlAssociationUIProvider(this));
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
            get { return Model.SourceEndType; }
        }

        public object TargetModel {
            get { return Model.TargetEndType; }
        }

        // ========================================
        // method
        // ========================================
        public override void Activate() {
            base.Activate();
            Model.SourceMemberEnd.PropertyChanged += HandleMemberEndPropertyChanged;
            Model.TargetMemberEnd.PropertyChanged += HandleMemberEndPropertyChanged;
        }

        public override void Deactivate() {
            base.Deactivate();
            Model.SourceMemberEnd.PropertyChanged -= HandleMemberEndPropertyChanged;
            Model.TargetMemberEnd.PropertyChanged -= HandleMemberEndPropertyChanged;
        }

        protected override LineEdge CreateFigure(UmlAssociation model) {
            var ret = new LineEdge() {
                LineColor = Color.DimGray,
                LineWidth = 1,
            };

            //RefreshFigure(ret, model);

            ret.Layout = new LocatingLayout();

            return ret;
        }

        protected override void RefreshEditor(RefreshContext context, LineEdge figure, UmlAssociation model) {
            /// source decoration
            if (model.SourceMemberEnd.IsComposite) {
                figure.SourceDecoration = new DiamondEdgeDecoration() {
                    Foreground = Color.DimGray,
                    Background = Color.DimGray,
                };
            } else if (model.SourceMemberEnd.Aggregation == UmlAggregationKind.Shared) {
                figure.SourceDecoration = new DiamondEdgeDecoration() {
                    Foreground = Color.DimGray,
                    Background = Color.White,
                };
            } else if (model.IsSourceNavigable && !model.IsTargetNavigable) {
                figure.SourceDecoration = new ArrowEdgeDecoration() {
                    Foreground = Color.DimGray,
                };
            } else {
                figure.SourceDecoration = null;
            }

            /// target decoration
            if (model.TargetMemberEnd.IsComposite) {
                figure.TargetDecoration = new DiamondEdgeDecoration() {
                    Foreground = Color.DimGray,
                    Background = Color.DimGray,
                };
            } else if (model.TargetMemberEnd.Aggregation == UmlAggregationKind.Shared) {
                figure.TargetDecoration = new DiamondEdgeDecoration() {
                    Foreground = Color.DimGray,
                    Background = Color.White,
                };
            } else if (model.IsTargetNavigable && !model.IsSourceNavigable) {
                figure.TargetDecoration = new ArrowEdgeDecoration() {
                    Foreground = Color.DimGray,
                };
            } else {
                figure.TargetDecoration = null;
            }

            /// labels
            var srcEnd = model.SourceMemberEnd;
            var tgtEnd = model.TargetMemberEnd;
            UpdateMemberEndNameLabel(figure, srcEnd, true);
            UpdateMemberEndMultiplicityLabel(figure, srcEnd, true);
            UpdateMemberEndNameLabel(figure, tgtEnd, false);
            UpdateMemberEndMultiplicityLabel(figure, tgtEnd, false);

            if (figure.Children.Any()) {
                figure.InvalidateLayout();
            }
        }

        public override void ConfigureEditor(IEditor editor) {
            var editorHandle = new MoveEditorHandle() { Cursor = Cursors.SizeAll };
            var facade = MemopadApplication.Instance;
            editorHandle.KeyMap = facade.KeySchema.MemoContentEditorKeyMap;
            editor.InstallEditorHandle(editorHandle);

            editor.InstallHandle(new CompositeEdgePointAndAnchorHandle() { Cursor = Cursors.Cross });
            editor.InstallHandle(new CompositeNewEdgePointHandle(IsOrthogonal) { Cursor = Cursors.Cross });

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
            return externalizer.Save(
                Model,
                (key, obj) => key == "SourceMemberEnd" || key == "TargetMemberEnd"
            );
        }

        public override string GetText() {
            var ret = Model.Name;
            if (Model.Keywords != null) {
                ret = ret + Environment.NewLine + Model.Keywords;
            }
            return ret;
        }

        // === IEdgeCnotroller ==========
        public bool CanConnectSource(object connected) {
            return connected is UmlType;
        }

        public bool CanConnectTarget(object connected) {
            return connected is UmlType;
        }

        public void ConnectSource(object connected) {
            Model.SourceMemberEnd.Type = connected as UmlType;
        }

        public void ConnectTarget(object connected) {
            Model.TargetMemberEnd.Type = connected as UmlType;
        }

        public void DisconnectSource() {
            Model.SourceMemberEnd.Type = null;
        }

        public void DisconnectTarget() {
            Model.TargetMemberEnd.Type = null;
        }

        private void UpdateMemberEndNameLabel(LineEdge figure, UmlProperty end, bool isSource) {
            var locKind = isSource? "EdgeFirstDistance": "EdgeLastDistance";

            var nameLabels = figure.Children.Where(
                fig => {
                    var constraint = figure.GetLayoutConstraint(fig) as object[];
                    var kind = (string) constraint[0];
                    var dir = (LocateDirectionKind) constraint[3];
                    return kind == locKind && dir == LocateDirectionKind.Left;
                }
            );

            var name = end.Name;
            if (string.IsNullOrEmpty(name)) {
                foreach (var label in nameLabels.ToArray()) {
                    figure.Children.Remove(label);
                }
            } else {
                var text = name;
                if (!StringUtil.IsNullOrWhitespace(end.Stereotype)) {
                    text = UmlTextUtil.GetStereotypeText(end) + " " + text;
                }
                if (end.Visibility != UmlVisibilityKind.None) {
                    text = UmlTextUtil.GetVisibilityText(end.Visibility) + " " + text;
                }
                if (nameLabels.Any()) {
                    var label = nameLabels.First() as INode;
                    label.StyledText = CreateLabelStyledText(text);
                } else {
                    var label = new Mkamo.Figure.Figures.Label();
                    label.StyledText = CreateLabelStyledText(text);
                    figure.Children.Add(label);
                    var constraint = IsOrthogonal() ?
                        new object[] { locKind, 4, 0, LocateDirectionKind.Left } :
                        new object[] { locKind, 8, 8, LocateDirectionKind.Left };
                    figure.SetLayoutConstraint(label, constraint);
                }
            }
        }

        private void UpdateMemberEndMultiplicityLabel(LineEdge figure, UmlProperty end, bool isSource) {
            var locKind = isSource? "EdgeFirstDistance": "EdgeLastDistance";

            var multiLabels = figure.Children.Where(
                fig => {
                    var constraint = figure.GetLayoutConstraint(fig) as object[];
                    var kind = (string) constraint[0];
                    var dir = (LocateDirectionKind) constraint[3];
                    return kind == locKind && dir == LocateDirectionKind.Right;
                }
            );
            var text = UmlTextUtil.GetMultiplicityText(end);
            if (string.IsNullOrEmpty(text)) {
                foreach (var label in multiLabels.ToArray()) {
                    figure.Children.Remove(label);
                }
            } else {
                if (multiLabels.Any()) {
                    var label = multiLabels.First() as INode;
                    label.StyledText = CreateLabelStyledText(text);
                } else {
                    var label = new Mkamo.Figure.Figures.Label();
                    label.StyledText = CreateLabelStyledText(text);
                    figure.Children.Add(label);
                    var constraint = IsOrthogonal() ?
                        new object[] { locKind, 4, 0, LocateDirectionKind.Right } :
                        new object[] { locKind, 8, 8, LocateDirectionKind.Right };
                    figure.SetLayoutConstraint(label, constraint);
                }
            }
        }

        private StyledText.Core.StyledText CreateLabelStyledText(string text) {
            var run = new Run(text);
            var para = new Paragraph(run);
            var ret = new StyledText.Core.StyledText(para);
            run.Font = MemopadApplication.Instance.Settings.GetDefaultUmlFont();
            return ret;
        }

        private bool IsOrthogonal() {
            return
                Figure.Router != null &&
                (Figure.Router.GetType() == typeof(OrthogonalRouter) || Figure.Router.GetType() == typeof(OrthogonalMidpointRouter));
        }

        private void HandleMemberEndPropertyChanged(object sender, PropertyChangedEventArgs e) {
            Host.Refresh(new RefreshContext(EditorRefreshKind.ModelUpdated, e));
        }

    }
}
