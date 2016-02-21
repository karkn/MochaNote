/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Common.IO;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Model.Memo;
using Mkamo.StyledText.Writer;
using System.IO;

namespace Mkamo.Memopad.Internal.Core {
    internal class PlainTextExporter {
        private PlainTextWriterSettings _settings = new PlainTextWriterSettings();

        public void Export(string filePath, EditorCanvas canvas) {
            PathUtil.EnsureDirectoryExists(Path.GetDirectoryName(filePath));

            var text = new StringBuilder();

            var memoEditor = canvas.RootEditor.Children.First();
            var contentEditors = memoEditor.Children.OrderBy(
                editor => editor.Figure.Bounds, new RectanglePositionalComparer()
            ).ToArray();
            var memo = canvas.EditorContent as Memo;


            text.AppendLine(memo.Title);
            text.AppendLine(string.Format("タグ: {0}", memo.TagsText));
            text.AppendLine(string.Format("情報元: {0}", memo.Source));
            text.AppendLine(string.Format("作成日時: {0}", memo.CreatedDate));
            text.AppendLine(string.Format("更新日時: {0}", memo.ModifiedDate));
 
            text.AppendLine();
            ExportContents(text, contentEditors);

            File.WriteAllText(filePath, text.ToString());
        }

        public string ExportText(IEnumerable<IEditor> editors) {
            var ret = new StringBuilder();
            var sorted = editors.OrderBy(
                editor => editor.Figure.Bounds, new RectanglePositionalComparer()
            ).ToArray();

            ExportContents(ret, sorted);
            
            return ret.ToString();
        }

        // ------------------------------
        // private
        // ------------------------------
        private void ExportContents(StringBuilder buf, IEnumerable<IEditor> contents) {
            foreach (var content in contents) {
                var model = content.Model;

                if (model is MemoText) {
                    ExportMemoText(buf, content);
                } else if (model is MemoTable) {
                    ExportMemoTable(buf, content);
                //} else if (model is MemoImage) {
                //    ExportMemoImage(content);
                //} else if (model is MemoFile) {
                //    ExportMemoFile(content);
                }
            }
        }

        private void ExportMemoText(StringBuilder buf, IEditor editor) {
            var text = editor.Model as MemoText;
            if (text == null) {
                return;
            }

            buf.AppendLine(text.StyledText.ToPlainText(_settings));
            buf.AppendLine();
        }

        private void ExportMemoTable(StringBuilder buf, IEditor editor) {
            var table = editor.Model as MemoTable;
            if (table == null) {
                return;
            }

            var csvExporter = new CsvExporter();
            var csv = csvExporter.ExportCsv(editor);
            buf.Append(csv);
            buf.AppendLine();
        }
    }
}
