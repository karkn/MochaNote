/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Figures;
using Mkamo.Figure.Core;
using Mkamo.Common.DataType;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using Mkamo.StyledText.Core;
using Mkamo.Figure.Layouts;
using Mkamo.Figure.Layouts.Locators;
using Mkamo.Model.Memo;
using Mkamo.Model.Core;
using Mkamo.Common.Externalize;
using Mkamo.Editor.Handles;
using System.Windows.Forms;
using Mkamo.Editor.Roles;
using Mkamo.Editor.Focuses;
using Mkamo.Editor.Controllers;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Editor.Utils;
using Mkamo.Editor.Forms;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Common.Core;
using Mkamo.Memopad.Internal.Handles;
using Mkamo.Memopad.Internal.Focuses;
using Mkamo.Memopad.Internal.Controllers.UIProviders;

namespace Mkamo.Memopad.Internal.Controllers {
    using StyledText = Mkamo.StyledText.Core.StyledText;
    using Mkamo.Common.Forms.Util;

    internal class MemoShapeController:
        AbstractMemoContentController<MemoShape, INode>, IConnectableController {

        // ========================================
        // field
        // ========================================
        private Lazy<IUIProvider> _uiProvider;

        private bool _firstFocus = true;

        // ========================================
        // constructor
        // ========================================
        internal MemoShapeController() {
            _uiProvider = new Lazy<IUIProvider>(() => new MemoShapeUIProvider(this));
        }

        // ========================================
        // property
        // ========================================
        public override IUIProvider UIProvider {
            get { return _uiProvider.Value; }
        }

        // ========================================
        // method
        // ========================================
        public override void ConfigureEditor(IEditor editor) {
            var facade = MemopadApplication.Instance;

            var editorHandle = new MemoShapeEditorHandle();
            editorHandle.KeyMap = facade.KeySchema.MemoContentEditorKeyMap;
            editor.InstallEditorHandle(editorHandle);

            editor.InstallHandle(new ResizeHandle(Directions.Left) { Cursor = Cursors.SizeWE });
            editor.InstallHandle(new ResizeHandle(Directions.Up) { Cursor = Cursors.SizeNS });
            editor.InstallHandle(new ResizeHandle(Directions.Right) { Cursor = Cursors.SizeWE });
            editor.InstallHandle(new ResizeHandle(Directions.Down) { Cursor = Cursors.SizeNS });
            editor.InstallHandle(new ResizeHandle(Directions.UpLeft) { Cursor = Cursors.SizeNWSE });
            editor.InstallHandle(new ResizeHandle(Directions.UpRight) { Cursor = Cursors.SizeNESW });
            editor.InstallHandle(new ResizeHandle(Directions.DownLeft) { Cursor = Cursors.SizeNESW });
            editor.InstallHandle(new ResizeHandle(Directions.DownRight) { Cursor = Cursors.SizeNWSE });

            editor.InstallRole(new SelectRole());
            editor.InstallRole(new FocusRole(InitFocus, CommitFocus));
            editor.InstallRole(new ResizeRole());
            editor.InstallRole(new RemoveRole());
            editor.InstallRole(new CopyRole());
            editor.InstallRole(new ReorderRole());
            editor.InstallRole(new SetStyledTextFontRole(() => Model.StyledText, FontModificationKinds.All));
            editor.InstallRole(new SetStyledTextColorRole(() => Model.StyledText));
            editor.InstallRole(new SetStyledTextAlignmentRole(() => Model.StyledText, AlignmentModificationKinds.All));

            var editorFocus = new MemoStyledTextFocus(Host, facade.Settings.KeyScheme == KeySchemeKind.Emacs, false);
            editorFocus.LinkClicked += (sender, e) => {
                LinkUtil.GoLink(e.Link);
            };

            editor.InstallFocus(editorFocus);
        }

        protected override INode CreateFigure(MemoShape model) {
            var ret = default(INode);
            switch (model.Kind) {
                case MemoShapeKind.RoundRect: {
                    ret = new RoundedRect();
                    break;
                }
                case MemoShapeKind.Triangle: {
                    ret = new Triangle();
                    break;
                }
                case MemoShapeKind.Ellipse: {
                    ret = new Ellipse();
                    break;
                }
                case MemoShapeKind.Diamond: {
                    ret = new Diamond();
                    break;
                }
                case MemoShapeKind.Parallelogram: {
                    ret = new Parallelogram();
                    break;
                }
                case MemoShapeKind.Cylinder: {
                    ret = new Cylinder();
                    break;
                }
                case MemoShapeKind.Paper: {
                    ret = new Paper();
                    break;
                }
                
                case MemoShapeKind.LeftArrow: {
                    ret = new ArrowFigure(Directions.Left);
                    break;
                }
                case MemoShapeKind.RightArrow: {
                    ret = new ArrowFigure(Directions.Right);
                    break;
                }
                case MemoShapeKind.UpArrow: {
                    ret = new ArrowFigure(Directions.Up);
                    break;
                }
                case MemoShapeKind.DownArrow: {
                    ret = new ArrowFigure(Directions.Down);
                    break;
                }

                case MemoShapeKind.LeftRightArrow: {
                    ret = new TwoHeadArrowFigure(false);
                    break;
                }
                case MemoShapeKind.UpDownArrow: {
                    ret = new TwoHeadArrowFigure(true);
                    break;
                }

                case MemoShapeKind.Pentagon: {
                    ret = new Pentagon();
                    break;
                }
                case MemoShapeKind.Chevron: {
                    ret = new Chevron();
                    break;
                }

                case MemoShapeKind.Equal: {
                    ret = new EqualFigure(false);
                    break;
                }
                case MemoShapeKind.NotEqual: {
                    ret = new EqualFigure(true);
                    break;
                }
                case MemoShapeKind.Plus: {
                    ret = new PlusFigure();
                    break;
                }
                case MemoShapeKind.Minus: {
                    ret = new MinusFigure();
                    break;
                }
                case MemoShapeKind.Times: {
                    ret = new TimesFigure();
                    break;
                }
                case MemoShapeKind.Devide: {
                    ret = new DevideFigure();
                    break;
                }

                case MemoShapeKind.Rect:
                default: {
                    ret = new Rect();
                    break;
                }
            }

            ret.BorderWidth = 1;
            ret.Background = new GradientBrushDescription(
                Color.FromArgb(230, 240, 255),
                Color.FromArgb(200, 220, 240),
                90f
            );

            ret.Foreground = Color.FromArgb(75, 125, 190);
            ret.MinSize = new Size(16, 16);
            //ret.AutoSizeKinds = AutoSizeKinds.GrowBoth;

            return ret;
        }

        protected override void RefreshEditor(RefreshContext context, INode figure, MemoShape model) {
            //if (node.StyledText.Text == "hoge\n" && figure is StyledTextFigure) {
            //    // 動的figure変更のテスト
            //    var fig = new RoundedRect();
            //    fig.AutoSizeKinds = AutoSizeKinds.GrowBoth;
            //    fig.Text = "hoge";
            //    _Host.Figure = fig;
            //} else {
                //var fig = figure as StyledTextFigure;
                figure.StyledText = model.StyledText.CloneDeeply() as StyledText;
                figure.AdjustSize();
            //}
            UpdateMemoMarkHandles(model);
        }

        public override IMemento GetModelMemento() {
            var externalizer = new Externalizer();
            return externalizer.Save(Model, (key, obj) => false);
        }

        public override string GetText() {
            var ret = Model.StyledText.Text;
            if (Model.Keywords != null) {
                ret = ret + Environment.NewLine + Model.Keywords;
            }
            return ret;
        }

        // === INodeController ==========
        public void ConnectOutgoing(object connected) {
            Model.Outgoings.Add(connected as MemoEdge);
        }

        public void ConnectIncoming(object connected) {
            Model.Incomings.Add(connected as MemoEdge);
        }

        public void DisconnectOutgoing(object disconnected) {
            Model.Outgoings.Remove(disconnected as MemoEdge);
        }

        public void DisconnectIncoming(object disconnected) {
            Model.Incomings.Remove(disconnected as MemoEdge);
        }


        // ------------------------------
        // private
        // ------------------------------
        private object InitFocus(IFocus focus, object model) {
            if (_firstFocus) {
                var app = MemopadApplication.Instance;
                focus.KeyMap = app.KeySchema.MemoContentFocusKeyMap;
                //app.KeySchema.MemoContentFocusKeyBinder.Bind(focus.KeyMap);
                _firstFocus = false;
            }

            var bgColor = Color.Ivory;
            if (Figure.Background != null && Figure.Background.IsDark) {
                bgColor = Color.DimGray;
            }
            focus.Figure.Background = new SolidBrushDescription(bgColor);

            return Model.StyledText == null? null: Model.StyledText.CloneDeeply();
        }

        private FocusUndoer CommitFocus(IFocus focus, object model, object value, bool isRedo, out bool isCancelled) {
            var memoStyledText = (MemoStyledText) model;
            var oldValue = memoStyledText.StyledText;
            var oldBounds = Figure.Bounds;

            if (focus.IsModified || isRedo) {
                isCancelled = false;
                memoStyledText.StyledText = (StyledText) value;
                return (f, m) => {
                    memoStyledText.StyledText = oldValue;
                    Figure.Bounds = oldBounds;
                };
            } else {
                isCancelled = false;
                return null;
            }
        }

    }
}
