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
using Mkamo.Common.Diagnostics;

namespace Mkamo.Editor.Roles {
    using Editor = Mkamo.Editor.Internal.Editors.Editor;
using System.Drawing;

    public abstract class AbstractRole: IRole {
        // ========================================
        // field
        // ========================================
        private Editor _host;

        // ========================================
        // constructor
        // ========================================
        public AbstractRole() {
        }

        // ========================================
        // property
        // ========================================
        // ------------------------------
        // protected
        // ------------------------------
        protected IEditor _Host {
            get { return _host; }
        }

        // ========================================
        // method
        // ========================================
        public virtual void Installed(IEditor host) {
            _host = host as Editor;
            Contract.Requires(_host != null);
        }

        public virtual void Uninstalled(IEditor host) {
            _host = null;
        }

        public abstract bool CanUnderstand(IRequest request);
        public abstract ICommand CreateCommand(IRequest request);
        public abstract IFigure CreateFeedback(IRequest request);
        public abstract void UpdateFeedback(IRequest request, IFigure feedback);
        public abstract void DisposeFeedback(IRequest request, IFigure feedback);

        // ------------------------------
        // protected
        // ------------------------------
        protected Point GetGridAdjustedPoint(Point pt) {
            return _Host.Site.GridService.GetAdjustedPoint(pt);
        }

        protected int GetGridAdjustedX(int x) {
            return _Host.Site.GridService.GetAdjustedX(x);
        }

        protected int GetGridAdjustedY(int y) {
            return _Host.Site.GridService.GetAdjustedY(y);
        }


        protected Size GetGridAdjustedDiff(Point pt) {
            return _Host.Site.GridService.GetAdjustedDiff(pt);
        }
 
        protected int GetGridAdjustedDiffX(int x) {
            return _Host.Site.GridService.GetAdjustedDiffX(x);
        }
 
        protected int GetGridAdjustedDiffY(int y) {
            return _Host.Site.GridService.GetAdjustedDiffY(y);
        }

        protected Rectangle GetGridAdjustedRect(Rectangle rect, bool adjustSize) {
            return _Host.Site.GridService.GetAdjustedRect(rect, adjustSize);
        }

        /// <summary>
        /// moveDeltaだけ移動した結果がグリッドに合うようにしたmoveDeltaを返す．
        /// </summary>
        protected Size GetGridAdjustedMoveDelta(Size moveDelta) {
            return moveDelta + GetGridAdjustedDiff(_Host.Figure.Location + moveDelta);
        }

        /// <summary>
        /// sizeDeltaだけサイズを変更した結果がグリッドに合うようにしたsizeDeltaを返す．
        /// </summary>
        protected Size GetGridAdjustedSizeDelta(Size moveDelta, Size sizeDelta) {
            var aMoveDelta = GetGridAdjustedMoveDelta(moveDelta);
            return sizeDelta +
                GetGridAdjustedDiff(_Host.Figure.Location + aMoveDelta + _Host.Figure.Size + sizeDelta);
        }
    }
}
