/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Model.Memo;
using Mkamo.Editor.Core;
using System.IO;
using Mkamo.Common.IO;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Memopad.Internal.Utils;

namespace Mkamo.Memopad.Internal.Core {
    internal class HtmlExporter {
        // ========================================
        // field
        // ========================================
        private StringBuilder _html;
        private DirectoryInfo _outputDir;
        private int _imageIndex;
        private int _fileIndex;

        private bool _isForClipboard;

        // ========================================
        // method
        // ========================================
        public void Export(string outputPath, EditorCanvas canvas) {
            PathUtil.EnsureDirectoryExists(outputPath);

            _isForClipboard = false;

            _outputDir = new DirectoryInfo(outputPath);
            _html = new StringBuilder();
            _imageIndex = 0;
            _fileIndex = 0;

            var memoEditor = canvas.RootEditor.Children.First();
            var contentEditors = memoEditor.Children.OrderBy(
                editor => editor.Figure.Bounds, new RectanglePositionalComparer()
            ).ToArray();
            var memo = canvas.EditorContent as Memo;

            _html.AppendLine("<html>");
            _html.AppendLine("<head>");
            _html.AppendLine(string.Format("<title>{0}</title>", memo.Title));
            ExportCSS();
            _html.AppendLine("</head>");
            _html.AppendLine("<body>");
            _html.AppendLine(string.Format("<h2 class=\"title\">{0}</h2>", memo.Title));
            _html.AppendLine("<p class=\"info\">");
            _html.AppendLine(string.Format("タグ: {0}<br/>", memo.TagsText));
            _html.AppendLine(string.Format("情報元: {0}<br/>", memo.Source));
            _html.AppendLine(string.Format("作成日時: {0}<br/>", memo.CreatedDate));
            _html.AppendLine(string.Format("更新日時: {0}", memo.ModifiedDate));
            _html.AppendLine("</p>");
            ExportContents(_html, contentEditors);
            _html.AppendLine("<hr />");
            ExportWholeImage(canvas);
            _html.AppendLine("</body>");
            _html.AppendLine("</html>");
        
            var filePath = Path.Combine(_outputDir.FullName, "note.html");
            File.WriteAllText(filePath, _html.ToString());
        }

        public string ExportHtml(IEnumerable<IEditor> editors) {
            var ret = new StringBuilder();
            ret.AppendLine("<html>");
            ret.AppendLine("<body>");

            _isForClipboard = true;

            var sorted = editors.OrderBy(
                editor => editor.Figure.Bounds, new RectanglePositionalComparer()
            ).ToArray();

            ExportContents(ret, sorted);
            
            ret.AppendLine("</body>");
            ret.AppendLine("</html>");
            return ret.ToString();
        }

        private void ExportCSS() {
            var css = @"
<style type=""text/css"">
<!--
    table {
        border-collapse: collapse;
        border: solid thin silver;
    }
    img.border {
        border: solid thin silver;
    }
-->
</style>
";
            _html.AppendLine(css);
        }

        private void ExportWholeImage(EditorCanvas canvas) {
            var pngFilename = "note.png";
            var pngFilepath = Path.Combine(_outputDir.FullName, pngFilename);
            canvas.SaveAsPng(pngFilepath);

            //var emfFilename = "note.emf";
            //var emfFilepath = Path.Combine(_outputDir.FullName, emfFilename);
            //canvas.SaveAsEmf(emfFilepath);

            _html.AppendLine("<h3>全体イメージ</h3>");
            _html.AppendLine("<p>");
            _html.AppendLine(string.Format("<img class=\"border\" src=\"{0}\" />", pngFilename));
            _html.AppendLine("</p>");
        }

        private void ExportContents(StringBuilder buf, IEnumerable<IEditor> contents) {
            foreach (var content in contents) {
                var model = content.Model;

                if (model is MemoText) {
                    ExportMemoText(buf, content);
                } else if (model is MemoTable) {
                    ExportMemoTable(buf, content);
                } else if (model is MemoImage) {
                    ExportMemoImage(buf, content);
                } else if (model is MemoFile) {
                    ExportMemoFile(buf, content);
                }
            }
        }

        private void ExportMemoText(StringBuilder buf, IEditor editor) {
            var text = editor.Model as MemoText;
            if (text == null) {
                return;
            }

            buf.AppendLine(text.StyledText.ToHtmlText());
        }

        private void ExportMemoTable(StringBuilder buf, IEditor editor) {
            var table = editor.Model as MemoTable;
            if (table == null) {
                return;
            }

            buf.AppendLine("<table border=\"1\">");
            foreach (var row in table.Rows) {
                buf.AppendLine("<tr>");
                foreach (var cell in row.Cells) {
                    buf.AppendLine("<td>");
                    if (cell.StyledText.IsEmpty) {
                        buf.AppendLine("&nbsp;");
                    } else {
                        buf.AppendLine(cell.StyledText.ToHtmlText());
                    }
                    buf.AppendLine("</td>");
                }
                buf.AppendLine("</tr>");
            }
            buf.AppendLine("</table>");
        }

        private void ExportMemoImage(StringBuilder buf, IEditor editor) {
            var image = editor.Model as MemoImage;
            if (image == null) {
                return;
            }

            if (_isForClipboard) {
                var fileImageDesc = image.Image as FileImageDescription;
                if (fileImageDesc != null) {
                    buf.AppendLine(string.Format("<p><img src=\"{0}\" /></p>", fileImageDesc.GetFullPath()));
                }

            } else {
                var img = image.Image.CreateImage();
                var filename = "Image_" + _imageIndex.ToString();
                img.Save(Path.Combine(_outputDir.FullName, filename));
                ++_imageIndex;
                buf.AppendLine(string.Format("<p><img src=\"{0}\" /></p>", filename));
            }
        }

        private void ExportMemoFile(StringBuilder buf, IEditor editor) {
            if (_isForClipboard) {
                return;
            }

            var file = editor.Model as MemoFile;
            if (file == null) {
                return;
            }

            var sfilepath = MemoFileEditorHelper.GetFullPath(file);
            if (!File.Exists(sfilepath)) {
                return;
            }

            var filedir = "File_" + _fileIndex.ToString();
            var filename = Path.GetFileName(sfilepath);
            var rfilepath = Path.Combine(filedir, filename);
            var tfilepath = Path.Combine(_outputDir.FullName, rfilepath);
            PathUtil.EnsureDirectoryExists(Path.GetDirectoryName(tfilepath));
            File.Copy(sfilepath, tfilepath, true);
            ++_fileIndex;

            buf.AppendLine(string.Format("<p><a href=\"{0}\">{1}</a></p>", rfilepath, filename));
        }
    }
}
