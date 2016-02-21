/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Handles;
using System.Windows.Forms;
using Mkamo.Figure.Core;
using Mkamo.StyledText.Core;
using System.Drawing;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Core;
using Mkamo.Common.Forms.Input;
using Mkamo.Memopad.Internal.Utils;
using System.Diagnostics;

namespace Mkamo.Memopad.Internal.Handles {
    internal class MemoShapeEditorHandle: MoveEditorHandle {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;

        // ========================================
        // constructor
        // ========================================
        public MemoShapeEditorHandle() {
            _facade = MemopadApplication.Instance;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        protected override Cursor GetMouseCursor(MouseEventArgs e) {
            var run = GetRun(e.Location);
            if (run != null && run.HasLink) {
                return Cursors.Hand;
            } else {
                return Cursors.SizeAll;
            }
        }

        protected override void OnFigureMouseClick(MouseEventArgs e) {
            var handled = false;

            if (
                e.Button == MouseButtons.Left &&
                !KeyUtil.IsAltPressed() &&
                !KeyUtil.IsControlPressed() &&
                !KeyUtil.IsShiftPressed()
            ) {
                var run = GetRun(e.Location);
                if (run != null && run.HasLink) {
                    LinkUtil.GoLink(run.Link);
                    handled = true;
                }
            }

            if (!handled) {
                base.OnFigureMouseClick(e);
            }
        }

        private Run GetRun(Point loc) {
            var fig = _Figure as INode;
            return fig.GetInlineAt(loc) as Run;
        }
    }
}
