/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Controllers;
using System.Windows.Forms;
using Mkamo.Editor.Core;
using Mkamo.Model.Uml;
using Mkamo.Model.Core;
using System.Drawing;
using Mkamo.Editor.Forms;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Common.Command;
using Mkamo.Figure.Core;
using Mkamo.Editor.Commands;
using Mkamo.Editor.Focuses;

namespace Mkamo.Memopad.Internal.Controllers.UIProviders {
    internal abstract class AbstractUmlClassifierUIProvider: AbstractUIProvider {
        // ========================================
        // field
        // ========================================
        private AbstractController _owner;
        private UmlClassifier _classifier;
        private INode _node;

        private ToolStripMenuItem _addAttribute;
        private ToolStripMenuItem _addOperation;

        //private ToolStripMenuItem _toggleAttributesVisible;
        //private ToolStripMenuItem _toggleOperationsVisible;

        // ========================================
        // constructor
        // ========================================
        public AbstractUmlClassifierUIProvider(AbstractController owner, UmlClassifier classifier): base(true) {
            _owner = owner;
            _node = owner.Figure as INode;
            _classifier = classifier;
        }

        // ========================================
        // property
        // ========================================
        protected virtual UmlClassifier _Classifier {
            get { return _classifier; }
        }

        // ========================================
        // method
        // ========================================
        public override ContextMenuStrip GetContextMenu(MouseEventArgs e) {
            if (_addAttribute == null || _addOperation == null) {
                InitItems();
            }
            _ContextMenu.Items.Clear();
            _ContextMenu.Items.Add(_addAttribute);
            _ContextMenu.Items.Add(_addOperation);
            //_ContextMenu.Items.Add(_toggleAttributesVisible);
            //_ContextMenu.Items.Add(_toggleOperationsVisible);
            return _ContextMenu;
        }

        public override void ConfigureDetailForm(DetailSettingsForm detailForm) {
            /// background detail page
            var bgPage = new NodeBackgroundDetailPage(new[] { _owner.Host });
            bgPage.Background = _node.IsBackgroundEnabled? _node.Background: null;
            bgPage.IsModified = false;
            detailForm.RegisterPage("背景", bgPage);

            /// border detail page
            var borderPage = new NodeBorderLineDetailPage(
                new [] {
                    _owner.Host,
                    _owner.Host.Children.ElementAt(0),
                    _owner.Host.Children.ElementAt(1),
                }
            );
            borderPage.LineColor = _node.Foreground;
            borderPage.LineWidth = _node.BorderWidth;
            borderPage.LineDashStyle = _node.BorderDashStyle;
            borderPage.IsModified = false;
            detailForm.RegisterPage("枠線", borderPage);
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void InitItems() {
            _addAttribute = new ToolStripMenuItem();
            _addAttribute.Text = "属性を追加(&A)";
            _addAttribute.Click += (sender, ev) => {
                var attrs = _owner.Host.Children.First();
                if (!attrs.IsEnabled) {
                    attrs.Enable();
                }
                var cmd = _owner.Host.Root.FindEditorFor(_classifier.Attributes).RequestCreateNode(
                    new DelegatingModelFactory<UmlProperty>(
                        () => {
                            var ret = UmlFactory.CreateProperty();
                            ret.Name = "attribute";
                            return ret;
                        }
                    ),
                    Rectangle.Empty
                );
                _node.AdjustSize();

                /// 編集状態にしておく
                var createCmd = cmd as CreateNodeCommand;
                if (createCmd != null && createCmd.CreatedEditor != null) {
                    var created = createCmd.CreatedEditor;
                    created.RequestFocus(FocusKind.Begin, Point.Empty);
                    var focus = created.Focus as StyledTextFocus;
                    if (focus != null) {
                        focus.SelectAll();
                    }
                }
            };

            _addOperation = new ToolStripMenuItem();
            _addOperation.Text = "操作を追加(&M)";
            _addOperation.Click += (sender, ev) => {
                var opes = _owner.Host.Children.Last();
                if (!opes.IsEnabled) {
                    opes.Enable();
                }
                var cmd = _owner.Host.Root.FindEditorFor(_classifier.Operations).RequestCreateNode(
                    new DelegatingModelFactory<UmlOperation>(
                        () => {
                            var ret = UmlFactory.CreateOperation();
                            ret.Name = "operation";
                            return ret;
                        }
                    ),
                    Rectangle.Empty
                );
                _node.AdjustSize();

                /// 編集状態にしておく
                var createCmd = cmd as CreateNodeCommand;
                if (createCmd != null && createCmd.CreatedEditor != null) {
                    var created = createCmd.CreatedEditor;
                    created.RequestFocus(FocusKind.Begin, Point.Empty);
                    var focus = created.Focus as StyledTextFocus;
                    if (focus != null) {
                        focus.SelectAll();
                    }
                }
            };

            //_toggleAttributesVisible = new ToolStripMenuItem();
            //_toggleAttributesVisible.Text = "属性を表示(&V)";
            //_toggleAttributesVisible.Click += (sender, ev) => {
            //    var attrs = _owner.Host.Children.First();
            //    attrs.IsEnabled = !attrs.IsEnabled;
            //    //_owner.Host.Children.First().Figure.IsVisible =
            //    //    !_owner.Host.Children.First().Figure.IsVisible;
            //    _node.AdjustSize();
            //};

            //_toggleOperationsVisible = new ToolStripMenuItem();
            //_toggleOperationsVisible.Text = "操作を表示(&W)";
            //_toggleOperationsVisible.Click += (sender, ev) => {
            //    var opes = _owner.Host.Children.Last();
            //    opes.IsEnabled = !opes.IsEnabled;
            //    //_owner.Host.Children.Last().Figure.IsVisible =
            //    //    !_owner.Host.Children.Last().Figure.IsVisible;
            //    _node.AdjustSize();
            //};
        }

    }
}
