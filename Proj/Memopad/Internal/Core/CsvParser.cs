/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Model.Memo;
using Mkamo.Model.Core;
using System.IO;
using Mkamo.Common.Csv;
using Mkamo.StyledText.Core;
using Mkamo.Common.String;
using Mkamo.Common.Core;

namespace Mkamo.Memopad.Internal.Core {
    internal class CsvParser {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // constructor
        // ========================================
        public MemoTable ParseCsv(string csv) {
            if (StringUtil.IsNullOrWhitespace(csv)) {
                return null;
            }

            /// Excelでコピーした場合，
            /// 最後に\0が付いているので取り除いておく
            if (csv[csv.Length - 1] == '\0') {
                csv = csv.Remove(csv.Length - 1);
            }

            var ret = MemoFactory.CreateTable();

            try {
                using (var reader = new CsvReader(new StringReader(csv), false)) {
                    reader.MissingFieldAction = MissingFieldAction.ReplaceByEmpty;

                    var colCount = reader.FieldCount;
                    for (int i = 0; i < colCount; ++i) {
                        ret.AddColumn();
                    }

                    while (reader.ReadNextRecord()) {
                        var row = ret.AddRow();
                        for (int i = 0; i < colCount; ++i) {
                            var stext = new StyledText.Core.StyledText();
                            stext.Font = MemopadApplication.Instance.Settings.GetDefaultMemoTextFont();
                            var referer = new StyledTextReferer(stext, null, null, null, null);
                            referer.InsertText(reader[i], false);

                            var cell = row.Cells.ElementAt(i);
                            cell.StyledText = stext;
                        }
                    }
                }

            } catch (Exception e) {
                Logger.Warn("Parse csv failed.", e);
                MemopadApplication.Instance.Container.Remove(ret);
                return null;
            }

            return ret;
        }
    }
}
