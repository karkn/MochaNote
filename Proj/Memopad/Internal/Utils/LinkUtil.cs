/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.StyledText.Core;
using System.Windows.Forms;
using Mkamo.Memopad.Internal.Core;
using System.Diagnostics;

namespace Mkamo.Memopad.Internal.Utils {
    internal static class LinkUtil {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // static method
        // ========================================
        public static void GoLink(Link link) {
            if (link != null) {
                var facade = MemopadApplication.Instance;
                var uri = link.Uri;
                if (UriUtil.IsMemoUri(uri)) {
                    var info = UriUtil.GetMemoInfo(link.Uri);
                    if (info != null) {
                        if (!facade.LoadMemo(info)) {
                            MessageBox.Show("ノートは開けませんでした", "ノートロードエラー");
                        }
                        return;
                    }
                } else if (UriUtil.IsHttpUri(uri) || UriUtil.IsFileUri(uri)) {
                    try {
                        Process.Start(uri);
                    } catch (Exception ex) {
                        Logger.Warn("Can't open " + uri, ex);
                        MessageBox.Show(uri + "を開けませんでした", "リンクオープンエラー");
                    }
                    return;
                }
            }
        }
    }
}
