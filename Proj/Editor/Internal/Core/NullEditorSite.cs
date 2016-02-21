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

namespace Mkamo.Editor.Internal.Core {
    internal class NullEditorSite: IEditorSite {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // property
        // ========================================
        public bool SuppressUpdateHandleLayer {
            get {
                Logger.Debug("SuppressUpdateHandleLayer get.");
                return false;
            }
            set {
                Logger.Debug("SuppressUpdateHandleLayer set.");
            }
        }

        public EditorCanvas EditorCanvas {
            get {
                Logger.Debug("NullEditorCanvas used.");
                return null;
            }
        }

        public IControllerFactory ControllerFactory {
            get {
                Logger.Debug("NullControllerFactory used.");
                return EditorConsts.NullControllerFactory;
            }
        }

        public ICommandExecutor CommandExecutor {
            get {
                Logger.Debug("NullCommandExecutor used.");
                return EditorConsts.NullCommandExecutor;
            }
        }

        public ISelectionManager SelectionManager {
            get {
                Logger.Debug("NullSelectionManager used.");
                return EditorConsts.NullSelectionManager;
            }
        }

        public IFocusManager FocusManager {
            get {
                Logger.Debug("NullFocusManager used.");
                return EditorConsts.NullFocusManager;
            }
        }

        //public IEditorDataAggregatorManager EditorDataAggregatorManager {
        //    get {
        //        Logger.Debug("NullEditorDataAggregatorManager used.");
        //        return null;
        //    }
        //}
        public IEditorCopyExtenderManager EditorCopyExtenderManager {
            get {
                Logger.Debug("NullEditorCopyExtenderManager used.");
                return null;
            }
        }

        public IGridService GridService {
            get {
                Logger.Debug("NullGridService used.");
                return null;
            }
        }


        public IFigure PrimaryLayer {
            get {
                Logger.Debug("NullPrimaryLayer used.");
                return FigureConsts.NullFigure;
            }
        }

        public IFigure HandleLayer {
            get {
                Logger.Debug("NullHandleLayer used.");
                return FigureConsts.NullFigure;
            }
        }

        public IFigure ShowOnPointHandleLayer {
            get {
                Logger.Debug("NullShowOnPointHandleLayer used.");
                return FigureConsts.NullFigure;
            }
        }

        public IFigure FeedbackLayer {
            get {
                Logger.Debug("NullFeedbackLayer used.");
                return FigureConsts.NullFigure;
            }
        }

        public IFigure FocusLayer {
            get {
                Logger.Debug("NullFocusLayer used.");
                return FigureConsts.NullFigure;
            }
        }

        public IDirtManager DirtManager {
            get {
                Logger.Debug("NullDirtManager used.");
                return FigureConsts.NullDirtManager;
            }
        }

        public Caret Caret {
            get {
                Logger.Debug("NullEditorSite.Caret used.");
                return null;
            }
        }

        // ========================================
        // method
        // ========================================
        public void UpdateHandleLayer() {
            Logger.Debug("UpdateHandleLayer invoked.");
        }

        public void UpdateFocusLayer() {
            Logger.Debug("UpdateFocusLayer invoked.");
        }

    }
}
