/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Core;
using Mkamo.Editor.Core;
using Mkamo.Model.Memo;
using Mkamo.Common.IO;
using System.IO;
using Mkamo.StyledText.Writer;
using Mkamo.Figure.Figures;

namespace Mkamo.Memopad.Internal.Core {
    internal class CsvExporter {
        public void Export(string outputFilePath, IEditor tableEditor) {
            PathUtil.EnsureDirectoryExists(Path.GetDirectoryName(outputFilePath));
            File.WriteAllText(outputFilePath, ExportCsv(tableEditor), Encoding.Default);
        }

        public string ExportCsv(IEditor tableEditor) {
            var fig = tableEditor.Figure as TableFigure;
            if (fig == null) {
                return "";
            }
            var data = fig.TableData;

            var settings = new PlainTextWriterSettings();
            var buf = new StringBuilder();
            var cellBuf = new StringBuilder();
            foreach (var row in data.Rows) {
                var firstCol = true;
                foreach (var cell in row.Cells) {
                    if (!firstCol) {
                        buf.Append(",");
                    }
                    firstCol = false;

                    var stext = cell.Value.StyledText;
                    var lines = stext.Lines;
                    if (!cell.IsMerged && lines.Length > 0) {
                        var firstLine = true;
                        foreach (var line in lines) {
                            if (!firstLine) {
                                cellBuf.Append('\n');
                            }
                            firstLine = false;
                            cellBuf.Append(line);
                        }
                        var text = cellBuf.ToString();
                        text = text.Replace("\"", "\"\"");
                        buf.Append("\"" + text + "\"");
                    }

                    cellBuf.Clear();
                }
                buf.AppendLine();
            }

            return buf.ToString();
        }
    }
}
