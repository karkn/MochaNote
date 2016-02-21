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
using System.Diagnostics;
using Mkamo.Model.Memo;
using System.IO;
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Internal.Helpers;

namespace Mkamo.Memopad.Internal.Handles {
    internal class MemoFileEditorHandle: MoveEditorHandle {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(global::System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        protected override void OnFigureMouseDoubleClick(MouseEventArgs e) {
            base.OnFigureMouseDoubleClick(e);
            
            var memoFile = Host.Model as MemoFile;
            if (memoFile != null) {
                var path = MemoFileEditorHelper.GetFullPath(memoFile);
                try {
                    Process.Start(path);
                } catch (Exception ex) {
                    Logger.Warn("Cannot open: path=" + path, ex);
                    MessageBox.Show(
                        memoFile.Path + "を開けませんでした",
                        "ファイルオープンエラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

    }
}
