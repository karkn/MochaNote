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
using Mkamo.Common.Command;
using Mkamo.Common.Forms.Input;
using Mkamo.Common.Visitor;

namespace Mkamo.Editor.Internal.Core {
    internal class EditorSite: IEditorSite {
        // ========================================
        // field
        // ========================================
        private readonly EditorCanvas _owner;

        private bool _suppressUpdateHandleLayer;
        private UpdateHandleLayerVisitor _updateHandleLayerVisitor;

        // ========================================
        // constructor
        // ========================================
        public EditorSite(EditorCanvas owner) {
            _owner = owner;

            _suppressUpdateHandleLayer = false;
            _updateHandleLayerVisitor = new UpdateHandleLayerVisitor(this);
        }

        // ========================================
        // property
        // ========================================
        public bool SuppressUpdateHandleLayer {
            get { return _suppressUpdateHandleLayer; }
            set { _suppressUpdateHandleLayer = value; }
        }

        public EditorCanvas EditorCanvas {
            get { return _owner; }
        }

        public IControllerFactory ControllerFactory {
            get { return _owner.ControllerFactory; }
        }

        public ISelectionManager SelectionManager {
            get { return _owner._SelectionManager; }
        }

        public IFocusManager FocusManager {
            get { return _owner._FocusManager; }
        }

        public ICommandExecutor CommandExecutor {
            get { return _owner.CommandExecutor; }
        }

        //public IEditorDataAggregatorManager EditorDataAggregatorManager {
        //    get { return _owner.EditorDataAggregatorManager; }
        //}
        public IEditorCopyExtenderManager EditorCopyExtenderManager {
            get { return _owner.EditorCopyExtenderManager; }
        }

        public IGridService GridService {
            get { return _owner.GridService; }
        }

        public IFigure PrimaryLayer {
            get { return _owner._PrimaryLayer; }
        }

        public IFigure HandleLayer {
            get { return _owner._HandleLayer; }
        }

        public IFigure ShowOnPointHandleLayer {
            get { return _owner._ShowOnPointHandleLayer; }
        }

        public IFigure FeedbackLayer {
            get { return _owner._FeedbackLayer; }
        }

        public IFigure FocusLayer {
            get { return _owner._FocusLayer; }
        }

        public IDirtManager DirtManager {
            get { return _owner.RootEditor.Figure.DirtManager; }
        }

        public Caret Caret {
            get { return _owner.Caret; }
        }

        // ========================================
        // method
        // ========================================
        public void UpdateHandleLayer() {
            if (_suppressUpdateHandleLayer) {
                return;
            }

            using (DirtManager.BeginDirty()) {
                ShowOnPointHandleLayer.Children.Clear();
                HandleLayer.Children.Clear();

                _owner.RootEditor.Accept(_updateHandleLayerVisitor);
            }
        }

        public void UpdateFocusLayer() {
            using (DirtManager.BeginDirty()) {
                FocusLayer.Children.Clear();
                if (
                    FocusManager.FocusedEditor != null &&
                    FocusManager.FocusedEditor.Focus != null &&
                    FocusManager.FocusedEditor.Focus.Figure != null
                ) {
                    FocusLayer.Children.Add(FocusManager.FocusedEditor.Focus.Figure);
                }
            }
        }


        private class UpdateHandleLayerVisitor: IVisitor<IEditor> {
            private EditorSite _site;

            public UpdateHandleLayerVisitor(EditorSite site) {
                _site = site;
            }

            public IFigure HandleLayer {
                get { return _site.HandleLayer; }
            }

            public IFigure ShowOnPointHandleLayer {
                get { return _site.ShowOnPointHandleLayer; }
            }

            public bool Visit(IEditor editor) {
                for (int i = 0, len = editor.AuxiliaryHandles.Count; i < len; ++i) {
                    var handle = editor.AuxiliaryHandles[i];

                    var stickyKind = editor.GetStickyKind(handle);
                    if (
                        editor.Figure != null &&
                        editor.Figure.IsVisible &&
                        !(editor.IsFocused && handle.HideOnFocus)
                        //(stickyKind == HandleStickyKind.Always || stickyKind == HandleStickyKind.MouseEnter || !editor.IsFocused)
                    ) {
                        var handleFig = handle.Figure;
                        if (stickyKind == HandleStickyKind.Always) {
                            handleFig.IsVisible = true;
                            HandleLayer.Children.Add(handleFig);

                        } else if (stickyKind == HandleStickyKind.Selected) {
                            if (editor.IsSelected) {
                                handleFig.IsVisible = true;
                                HandleLayer.Children.Add(handleFig);
                            } else {
                                handleFig.IsVisible = false;
                            }

                        } else if (stickyKind == HandleStickyKind.MouseOver) {
                            if (editor.IsSelected) {
                                handleFig.IsVisible = true;
                                HandleLayer.Children.Add(handleFig);
                            } else {
                                handleFig.IsVisible = true;
                                ShowOnPointHandleLayer.Children.Add(handleFig);
                            }

                        } else if (stickyKind == HandleStickyKind.MouseEnter) {
                            if (editor.IsSelected || editor.IsMouseEntered) {
                                handleFig.IsVisible = true;
                                HandleLayer.Children.Add(handleFig);
                            } else {
                                handleFig.IsVisible = true;
                                ShowOnPointHandleLayer.Children.Add(handleFig);
                            }

                        } else if (stickyKind == HandleStickyKind.SelectedIncludingChildren) {
                            if (editor.IsSelected) {
                                handleFig.IsVisible = true;
                                HandleLayer.Children.Add(handleFig);
                            } else {
                                var foundSelected = false;
                                editor.Accept(
                                    child => {
                                        if (child.IsSelected) {
                                            foundSelected = true;
                                            return true;
                                        }
                                        return false;
                                    }
                                );
                                if (foundSelected) {
                                    handleFig.IsVisible = true;
                                    HandleLayer.Children.Add(handleFig);
                                } else {
                                    handleFig.IsVisible = false;
                                }
                            }
                        }
                    }
                }
                return false;
            }

            public void EndVisit(IEditor elem) {
            }
        }
    }
}
