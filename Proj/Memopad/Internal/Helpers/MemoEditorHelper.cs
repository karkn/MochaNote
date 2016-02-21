/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Core;
using Mkamo.Editor.Commands;
using Mkamo.Common.Diagnostics;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Core;
using System.Drawing;
using Mkamo.Model.Memo;
using System.Windows.Forms;
using Mkamo.Memopad.Internal.Utils;
using System.Drawing.Imaging;
using Mkamo.Common.Win32.User32;
using Mkamo.Model.Core;
using Mkamo.Figure.Core;
using Mkamo.Editor.Focuses;
using System.IO;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.IO;
using Mkamo.Common.DataType;
using Mkamo.StyledText.Util;
using Mkamo.StyledText.Core;
using Mkamo.Common.Core;
using Mkamo.Figure.Figures;
using Mkamo.Common.Command;
using Mkamo.Common.Win32.Gdi32;
using Mkamo.Memopad.Internal.Commands;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.String;

namespace Mkamo.Memopad.Internal.Helpers {
    internal static class MemoEditorHelper {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static IEnumerable<IEditor> EmptyEditors = new IEditor[0];

        private static string[] ImageExtensions = new[] { ".bmp", ".gif", ".jpg", ".jpeg", ".png", "tif" };
        private static string[] CsvExtensions = new[] { ".csv" };
        private static string[] TextExtensions = new[] { ".txt" };

        // ========================================
        // static method
        // ========================================
        public static void Paste(IEditor editor, bool inBlock) {
            if (editor.CanUnderstand(new PasteRequest())) {
                var caret = editor.Site.Caret;
                var loc = CaretUtil.GetExpectedMemoTextPosition(caret.Position);
                var cmd = editor.RequestPaste(loc, inBlock? EditorConsts.InBlockPasteDescription: null) as PasteCommand;
                cmd.MergeJudge = next => next is FocusCommand;
                if (cmd != null && cmd.PastedEditors != null) {
                    editor.Site.SelectionManager.DeselectAll();
                    foreach (var pasted in cmd.PastedEditors) {
                        pasted.RequestSelect(SelectKind.True, false);
                    }
                }
            }
        }

        public static IEditor PasteBlocksAndInlinesBase(IEditor editor, Point location, bool inBlock) {
            return AddTextBase(editor, location, (focus) => focus.PasteInlinesOrText(inBlock), false);
        }

        public static IEditor PasteText(IEditor editor, Point location, string text, bool inBlock) {
            return AddTextBase(editor, location, focus => focus.InsertText(text, inBlock), false);
        }

        public static IEditor AddBlocksAndInlines(IEditor editor, Point location, IEnumerable<Flow> blocksAndInlines, bool useCommandExecutor) {
            return AddTextBase(
                editor,
                location,
                (focus) => {
                    focus.InsertBlocksAndInlines(blocksAndInlines);
                    if (focus.Referer.Target.GetInlineAt(focus.Referer.CaretIndex - 1) is BlockBreak) {
                        focus.RemoveBackward();
                    }
                },
                useCommandExecutor
            );
        }

        public static IEditor AddString(IEditor editor, Point location, string s) {
            return AddTextBase(editor, location, (focus) => focus.Insert(s), true);
        }

        public static IEditor AddText(IEditor editor, Point location, string text, bool inBlock) {
            return AddTextBase(editor, location, focus => focus.InsertText(text, inBlock), true);
        }

        public static IEditor AddTextAsLink(IEditor editor, Point location, string text, string url, string relationship) {
            return AddTextBase(
                editor,
                location,
                focus => {
                    var charIndex = focus.Referer.CaretIndex;
                    var len = text.Length;
                    focus.InsertText(text, false);
                    focus.Selection.Range = new Range(charIndex, len);
                    focus.SetLink(url, relationship);
                    focus.Selection.Range = Range.Empty;
                },
                true
            );
        }

        public static IEnumerable<IEditor> AddFile(IEditor target, Point location, string filePath, bool embed, bool useCommandExecutor, bool arrange) {
            var isFile = File.Exists(filePath);
            var isDirectory = Directory.Exists(filePath);
            if (isFile || isDirectory) {
                var fileName = Path.GetFileName(filePath);
                var path = filePath;

                var embeddedId = "";

                if (!isFile && !isDirectory) {
                    MessageBox.Show("ファイルが見つかりませんでした。", "ファイルエラー");
                    return EmptyEditors;
                }

                /// 埋め込む場合はEmbeddedFileRootにコピー
                if (embed && isFile) {
                    embeddedId = Guid.NewGuid().ToString();
                    path = Path.Combine(embeddedId, fileName);
                    var fullPath = Path.Combine(MemopadConsts.EmbeddedFileRoot, path);
                    try {
                        PathUtil.EnsureDirectoryExists(Path.GetDirectoryName(fullPath));
                        File.Copy(filePath, fullPath);
                    } catch {
                        Logger.Warn("File copy failed. source=" + filePath + ",target=" + fullPath);
                        if (Directory.Exists(fullPath)) {
                            Directory.Delete(Path.GetDirectoryName(fullPath), true);
                        }
                        MessageBox.Show("ファイルのコピーに失敗しました。", "ファイルコピーエラー");
                    }
                }

                var bounds = new Rectangle(location, new Size(1, 1));
                var modelFactory = new DelegatingModelFactory<MemoFile>(
                    () => {
                        var ret = MemoFactory.CreateFile();
                        ret.Name = fileName;
                        ret.Path = path;
                        ret.IsEmbedded = embed && isFile;
                        ret.EmbeddedId = embeddedId;
                        return ret;
                    }
                );

                if (useCommandExecutor && arrange) {
                    target.Site.CommandExecutor.BeginChain();
                }

                var existingEditorBounds = default(Rectangle[]);
                if (arrange) {
                    existingEditorBounds = target.Children.Select(e => e.Figure.Bounds).ToArray();
                }

                var req = new CreateNodeRequest();
                req.ModelFactory = modelFactory;
                req.Bounds = bounds;
                req.AdjustSizeToGrid = true;
                var cmd = target.GetCommand(req) as CreateNodeCommand;

                var sizeCmd = new DelegatingCommand(
                    () => {
                        var node = cmd.CreatedEditor.Figure as INode;
                        if (node != null) {
                            node.AdjustSize();
                        }
                    },
                    () => {
                        cmd.CreatedEditor.Figure.Bounds = bounds;
                    }
                );

                var chain = cmd.Chain(sizeCmd);

                if (useCommandExecutor) {
                    target.Site.CommandExecutor.Execute(chain);
                } else {
                    chain.Execute();
                }

                if (arrange) {
                    var newLoc = RectUtil.GetPreferredLocation(
                        cmd.CreatedEditor.Figure.Bounds,
                        existingEditorBounds,
                        target.Root.Figure.Right,
                        MemopadConsts.DefaultCaretPosition.X,
                        MemopadConsts.DefaultCaretPosition.Y
                    );
                    var move = new ChangeBoundsCommand(
                        cmd.CreatedEditor,
                        (Size) newLoc - (Size) cmd.CreatedEditor.Figure.Location,
                        Size.Empty,
                        Directions.None,
                        new [] { cmd.CreatedEditor }
                    );
                    if (useCommandExecutor) {
                        target.Site.CommandExecutor.Execute(move);
                    } else {
                        move.Execute();
                    }
                    if (useCommandExecutor) {
                        target.Site.CommandExecutor.EndChain();
                    }
                }

                return new[] { cmd.CreatedEditor };
            }

            return EmptyEditors;
        }

        public static IEnumerable<IEditor> AddFileDrops(
            IEditor editor, Point location, IEnumerable<string> fileDrops, bool embed, bool useCommandExecutor, bool arrange
        ) {
            using (editor.Figure.DirtManager.BeginDirty()) {
                var ret = new List<IEditor>();
                var locDelta = Size.Empty;

                foreach (var fileDrop in fileDrops) {
                    var loc = arrange ? location : location + locDelta;

                    var fileExt = Path.GetExtension(fileDrop);
                    var isImageExt = ImageExtensions.Any(ext => string.Equals(fileExt, ext, StringComparison.OrdinalIgnoreCase));
                    var isCsvExt = CsvExtensions.Any(ext => string.Equals(fileExt, ext, StringComparison.OrdinalIgnoreCase));
                    var isTextExt = TextExtensions.Any(ext => string.Equals(fileExt, ext, StringComparison.OrdinalIgnoreCase));

                    var createds = default(IEnumerable<IEditor>);
                    if (isImageExt) {
                        try {
                            using (var image = Image.FromFile(fileDrop)) {
                                if (image is Bitmap) {
                                    createds = AddImage(editor, loc, image, useCommandExecutor, arrange);
                                } else if (image is Metafile) {
                                    createds = AddMetafile(editor, loc, (Metafile) image, useCommandExecutor, arrange);
                                }
                            }
                        } catch (Exception) {
                            /// 実は画像ファイルではなかった場合
                            createds = AddFile(editor, loc, fileDrop, embed, useCommandExecutor, arrange);
                        }

                    } else if (isCsvExt) {
                        try {
                            //editor.Site.SuppressUpdateHandleLayer = true;

                            var csv = StringUtil.ReadAllText(fileDrop);
                            createds = AddCsv(editor, loc, csv);
                            //var parser = new CsvParser();
                            //var table = parser.ParseCsv(text);
                            //if (table != null) {
                            //    var created = AddTable(editor, loc, table, true);
                            //    TableFigureHelper.AdjustRowAndColumnSizes(created.Figure as TableFigure);
                            //    createds = new[] { created };
                            //}

                            //editor.Site.SuppressUpdateHandleLayer = false;
                            //editor.Site.UpdateHandleLayer();

                        } catch (Exception e) {
                            Logger.Warn("Can't load csv file: " + fileDrop, e);
                        }

                    } else if (isTextExt) {
                        try {
                            var text = StringUtil.ReadAllText(fileDrop);
                            createds = new [] { AddText(editor, loc, text, false) };
                        } catch (Exception e) {
                            Logger.Warn("Can't load text file: " + fileDrop, e);
                        }
                    } else {
                        createds = AddFile(editor, loc, fileDrop, embed, useCommandExecutor, arrange);
                    }

                    if (createds != null) {
                        ret.AddRange(createds);
                        locDelta += new Size(8, 8);
                    }
                }
    
                return ret;
            }
        }

        public static IEnumerable<IEditor> AddCsv(IEditor editor, Point location, string csv) {
            try {
                editor.Site.SuppressUpdateHandleLayer = true;

                var parser = new CsvParser();
                var table = parser.ParseCsv(csv);
                if (table != null) {
                    var created = AddTable(editor, location, table, true);
                    TableFigureHelper.AdjustRowAndColumnSizes(created.Figure as TableFigure);
                    return new[] { created };
                } else {
                    return EmptyEditors;
                }

            } finally {
                editor.Site.SuppressUpdateHandleLayer = false;
                editor.Site.UpdateHandleLayer();
            }
        }

        public static IEnumerable<IEditor> AddImage(IEditor editor, Point location, Image img, bool useCommandExecutor, bool arrange) {
            var cmd = GetAddImageCommand(editor, location, img);
            if (cmd == null) {
                return EmptyEditors;
            }

            if (useCommandExecutor && arrange) {
                editor.Site.CommandExecutor.BeginChain();
            }

            var existingEditorBounds = default(Rectangle[]);
            if (arrange) {
                existingEditorBounds = editor.Children.Select(e => e.Figure.Bounds).ToArray();
            }

            if (useCommandExecutor) {
                editor.Site.CommandExecutor.Execute(cmd);
            } else {
                cmd.Execute();
            }

            if (arrange) {
                var newLoc = RectUtil.GetPreferredLocation(
                    cmd.CreatedEditor.Figure.Bounds,
                    existingEditorBounds,
                    editor.Root.Figure.Right,
                    MemopadConsts.DefaultCaretPosition.X,
                    MemopadConsts.DefaultCaretPosition.Y
                );
                var move = new ChangeBoundsCommand(
                    cmd.CreatedEditor,
                    (Size) newLoc - (Size) cmd.CreatedEditor.Figure.Location,
                    Size.Empty,
                    Directions.None,
                    new [] { cmd.CreatedEditor }
                );
                if (useCommandExecutor) {
                    editor.Site.CommandExecutor.Execute(move);
                } else {
                    move.Execute();
                }
                if (useCommandExecutor) {
                    editor.Site.CommandExecutor.EndChain();
                }
            }
            
            return new[] { cmd.CreatedEditor };
        }

        private static CreateNodeCommand GetAddImageCommand(IEditor editor, Point location, Image img) {
            using (editor.Figure.DirtManager.BeginDirty()) {
                if (img == null) {
                    return null;
                }

                var codec = ImageUtil.GetCodec(img);
                if (codec == null) {
                    codec = ImageUtil.GetCodec(ImageFormat.Png);
                }

                using (var encoderParams = ImageUtil.CreateEncoderParameters(100L)) {

                    var imgFilePath = GetNewImageFilePath();
                    img.Save(imgFilePath, codec, encoderParams);

                    var desc = new FileImageDescription(Path.GetFileName(imgFilePath));

                    var req = new CreateNodeRequest();
                    req.ModelFactory = new DelegatingModelFactory<MemoImage>(
                        () => {
                            var ret = MemoFactory.CreateImage();
                            ret.Image = desc;
                            return ret;
                        }
                    );
                    req.Bounds = new Rectangle(location, new Size(img.Width, img.Height));
                    req.AdjustSizeToGrid = false;

                    return editor.GetCommand(req) as CreateNodeCommand;
                }
            }
        }

        public static IEnumerable<IEditor> AddMetafile(IEditor editor, Point location, Metafile meta, bool useCommandExecutor, bool arrange) {
            using (editor.Figure.DirtManager.BeginDirty())
            using (var mem = new MemoryStream()) {


                if (useCommandExecutor && arrange) {
                    editor.Site.CommandExecutor.BeginChain();
                }

                var existingEditorBounds = default(Rectangle[]);
                if (arrange) {
                    existingEditorBounds = editor.Children.Select(e => e.Figure.Bounds).ToArray();
                }

                using (var bmp = new Bitmap(1, 1))
                using (var bmpg = Graphics.FromImage(bmp)) {
                    var hdc = bmpg.GetHdc();
                    using (var mf = new Metafile(mem, hdc)) {
                        bmpg.ReleaseHdc(hdc);
                        using (var g = Graphics.FromImage(mf)) {
                            g.DrawImage(meta, Point.Empty);
                        }
                    }
                }

                var imgFilePath = GetNewImageFilePath();
                File.WriteAllBytes(imgFilePath, mem.GetBuffer());
                var desc = new FileImageDescription(Path.GetFileName(imgFilePath));

                var req = new CreateNodeRequest();
                req.ModelFactory = new DelegatingModelFactory<MemoImage>(
                    () => {
                        var ret = MemoFactory.CreateImage();
                        ret.Image = desc;
                        return ret;
                    }
                );
                req.Bounds = new Rectangle(location, meta.Size);
                req.AdjustSizeToGrid = false;

                var cmd = editor.GetCommand(req) as CreateNodeCommand;
                if (useCommandExecutor) {
                    editor.Site.CommandExecutor.Execute(cmd);
                } else {
                    cmd.Execute();
                }

                meta.Dispose();


                if (arrange) {
                    var newLoc = RectUtil.GetPreferredLocation(
                        cmd.CreatedEditor.Figure.Bounds,
                        existingEditorBounds,
                        editor.Root.Figure.Right,
                        MemopadConsts.DefaultCaretPosition.X,
                        MemopadConsts.DefaultCaretPosition.Y
                    );
                    var move = new ChangeBoundsCommand(
                        cmd.CreatedEditor,
                        (Size) newLoc - (Size) cmd.CreatedEditor.Figure.Location,
                        Size.Empty,
                        Directions.None,
                        new [] { cmd.CreatedEditor }
                    );
                    if (useCommandExecutor) {
                        editor.Site.CommandExecutor.Execute(move);
                    } else {
                        move.Execute();
                    }
                    if (useCommandExecutor) {
                        editor.Site.CommandExecutor.EndChain();
                    }
                }
            
                return new[] { cmd.CreatedEditor };
            }
        }

        public static IEditor AddTable(IEditor editor, Point location, MemoTable table, bool useCommandExecutor) {
            if (table == null || table.RowCount == 0 || table.ColumnCount == 0) {
                return null;
            }

            using (editor.Figure.DirtManager.BeginDirty()) {

                var req = new CreateNodeRequest();
                req.ModelFactory = new DelegatingModelFactory<MemoTable>(
                    () => {
                        return table;
                    }
                );
                req.Bounds = new Rectangle(location, new Size(table.ColumnCount * 50, table.RowCount * 20));
                req.AdjustSizeToGrid = false;

                var cmd = editor.GetCommand(req) as CreateNodeCommand;
                if (useCommandExecutor) {
                    editor.Site.CommandExecutor.Execute(cmd);
                } else {
                    cmd.Execute();
                }
                return cmd.CreatedEditor;
            }
        }

        public static IEnumerable<IEditor> PasteBlocksAndInlines(IEditor editor, Point location, string description) {
            if (ClipboardUtil.ContainsBlocksAndInlines()) {
                return new[] {
                    PasteBlocksAndInlinesBase(editor, location, description == EditorConsts.InBlockPasteDescription)
                };
            } else {
                return EmptyEditors;
            }
        }

        public static IEnumerable<IEditor> PasteText(IEditor editor, Point location, string description) {
            if (Clipboard.ContainsText()) {
                return new[] { PasteText(editor, location, Clipboard.GetText(), description == EditorConsts.InBlockPasteDescription) };
            } else {
                return EmptyEditors;
            }
        }

        public static IEnumerable<IEditor> PasteFileDrops(IEditor editor, Point location, string description) {
            var drop = Clipboard.GetFileDropList();
            if (drop != null) {
                return AddFileDrops(editor, location, drop.Cast<string>(), true, false, false);
            } else {
                return EmptyEditors;
            }
        }

        public static IEnumerable<IEditor> PasteCsv(IEditor editor, Point location, string description) {
            if (Mkamo.Common.Forms.Clipboard.ClipboardUtil.ContainsCsv()) {
                var csv = Mkamo.Common.Forms.Clipboard.ClipboardUtil.GetCsvText();
                if (csv == null) {
                    return EmptyEditors;
                } else {
                    return AddCsv(editor, location, csv);
                }

            } else {
                return EmptyEditors;
            }
        }

        public static IEnumerable<IEditor> PasteImage(IEditor editor, Point location, string description) {
            using (var img = Clipboard.GetImage()) {
                return AddImage(editor, location, img, false, false);
            }
        }

        public static IEnumerable<IEditor> PasteMetafile(IEditor editor, Point location, string description) {
            var meta = default(Metafile);
            var hWnd = MemopadApplication.Instance.MainForm.Handle;
            if (User32PI.OpenClipboard(hWnd)) {
                try {
                    if (User32PI.IsClipboardFormatAvailable(ClipboardFormat.CF_ENHMETAFILE) != 0) {
                        var hMeta = User32PI.GetClipboardData(ClipboardFormat.CF_ENHMETAFILE);
                        meta = new Metafile(hMeta, false);
                    }
                } finally {
                    User32PI.CloseClipboard();
                }
            }

            if (meta == null) {
                return EmptyEditors;
            }
            return AddMetafile(editor, location, meta, false, false);
        }

        public static IEnumerable<IEditor> PasteHtml(IEditor editor, Point location, string description) {
            if (Clipboard.ContainsText(TextDataFormat.Html)) {
                var html = Common.Forms.Clipboard.ClipboardUtil.GetCFHtmlFromClipboard();
                return AddHtml(editor, location, html, false);
            } else {
                return EmptyEditors;
            }
        }

        public static IEnumerable<IEditor> AddHtml(IEditor editor, Point location, string html, bool useCommandExecutor) {
            if (StringUtil.IsNullOrWhitespace(html)) {
                return EmptyEditors;
            }

            //editor.Site.Caret.Hide();
            using (editor.Figure.DirtManager.BeginDirty()) {
                if (useCommandExecutor) {
                    editor.Site.CommandExecutor.BeginChain();
                }
                try {
    
                    /// PasteCommandでreturnしたEditor追加処理のUndo/Redoを処理するのでCommandExecutorを通さないでコマンドを実行する
                    var objs = new HtmlParser().ParseCFHtml(html);
                    var ret = new List<IEditor>();
                    var cy = location.Y;
                    foreach (var obj in objs) {
                        if (obj is StyledText.Core.StyledText) {
                            var stext = obj as StyledText.Core.StyledText;
                            var created = AddBlocksAndInlines(editor, new Point(location.X, cy), stext.Blocks.As<Block, Flow>(), useCommandExecutor);
                            var focus = created.GetCommand(new FocusRequest() { Value = FocusKind.Commit }) as FocusCommand;
                            focus.Execute();
                            if (focus.ResultKind == FocusCommitResultKind.Canceled) {
                                var rollback = created.GetCommand(new FocusRequest() { Value = FocusKind.Rollback });
                                rollback.Execute();
                            }
                            ret.Add(created);
                            cy += created.Figure.Height + 4;
                            
                        } else if (obj is Image) {
                            var img = obj as Image;
                            var createds = AddImage(editor, new Point(location.X + 8, cy), img, useCommandExecutor, false);
                            ret.AddRange(createds);
                            img.Dispose();
                            cy += createds.Sum(created => created.Figure.Height) + 20;
    
                        } else if (obj is MemoTable) {
                            editor.Site.SuppressUpdateHandleLayer = true;
                            var table = obj as MemoTable;
                            var created = AddTable(editor, new Point(location.X + 8, cy), table, useCommandExecutor);
                            var tableFig = created.Figure as TableFigure;
                            TableFigureHelper.AdjustRowAndColumnSizes(tableFig);
                            ret.Add(created);
                            editor.Site.SuppressUpdateHandleLayer = false;
                            editor.Site.UpdateHandleLayer();
                            cy += tableFig.Height + 20;
                        }
                    }
                    return ret.ToArray();
                } finally {
                    if (useCommandExecutor) {
                        editor.Site.CommandExecutor.EndChain();
                    }
                }
            }
        }

        public static void AddCommentForMemoText(StyledTextFocus focus) {
            if (!(focus.Host.Model is MemoText)) {
                return;
            }

            var editor = focus.Host;
            var memoEditor = editor.Parent;

            editor.Site.Caret.Hide();
            var charRect = focus.Figure.GetCharRect(focus.Referer.CaretIndex);
            var loc = new Point(focus.Figure.Right + 80, charRect.Top);

            editor.RequestFocusCommit(true);

            /// テキスト作成
            var created = MemoEditorHelper.AddText(memoEditor, loc, "(コメント)", false);
            var createdText = (MemoText) created.Model;
            var createdFocus = (StyledTextFocus) created.Focus;
            createdText.IsSticky = true;

            /// コメント線作成
            var cmd = new CreateCommentCommand(
                memoEditor,
                new DelegatingModelFactory<MemoAnchorReference>(
                    () => {
                        var ret = MemoFactory.CreateAnchorReference();
                        return ret;
                    }
                ),
                new [] { created.Figure.Location + new Size(1, 1), charRect.Location + new Size(1, 1), },
                created,
                focus.Host
            );
            editor.Site.CommandExecutor.Execute(cmd);
            editor.Site.Caret.Show();

            created.RequestSelect(SelectKind.True, true);
            createdFocus.SelectAll();
        }

        // ------------------------------
        // private
        // ------------------------------
        //private static ImageCodecInfo GetCodec(ImageFormat format) {
        //    var codecs = ImageCodecInfo.GetImageDecoders();
        //    foreach (var codec in codecs) {
        //        if (codec.FormatID == format.Guid) {
        //            return codec;
        //        }
        //    }
        //    return null;
        //}

        //private static ImageCodecInfo GetCodec(Image image) {
        //    var codecs = ImageCodecInfo.GetImageDecoders();
        //    foreach (var codec in codecs) {
        //        if (codec.FormatID == image.RawFormat.Guid) {
        //            return codec;
        //        }
        //    }
        //    return null;
        //}

        private static IEditor AddTextBase(IEditor editor, Point location, Action<StyledTextFocus> insert, bool useCommandExecutor) {
            using (editor.Figure.DirtManager.BeginDirty()) {
                if (editor.Site.FocusManager.FocusedEditor != null) {
                    editor.Site.FocusManager.FocusedEditor.RequestFocusCommit(true);
                }

                var req = new CreateNodeRequest();
                {
                    req.ModelFactory = new DelegatingModelFactory<MemoText>(
                        () => {
                            var ret = MemoFactory.CreateText();
                            var defaultFont = MemopadApplication.Instance.Settings.GetDefaultMemoTextFont();
                            if (defaultFont != null) {
                                ret.StyledText.Font = defaultFont;
                            }
                            return ret;
                        }
                    );
                    //req.Bounds = new Rectangle(location + MemopadConsts.MemoTextFirstCharDelta, new Size(40, 8));
                    req.Bounds = new Rectangle(location, new Size(40, 8));
                    req.AdjustSizeToGrid = true;
                }

                var cmd = editor.GetCommand(req) as CreateNodeCommand;
                if (useCommandExecutor) {
                    editor.Site.CommandExecutor.Execute(cmd);
                } else {
                    cmd.Execute();
                }

                /// focus commit commandがこのcreate node commandとマージされるようにしておく
                //cmd.MergeJudge = next => {
                //    var focusCmd = next as FocusCommand;
                //    if (focusCmd == null) {
                //        return false;
                //    }
                //    return focusCmd.Value == FocusKind.Commit;
                //    //next is FocusCommand;
                //};

                if (cmd != null && cmd.CreatedEditor != null) {
                    var canvasSize = editor.Site.EditorCanvas.ClientSize;
                    var maxWidth = 0;
                    var defaultMaxWidth = MemopadApplication.Instance.WindowSettings.MemoTextDefaultMaxWidth;
                    if (defaultMaxWidth <= 50) {
                        maxWidth = Math.Max((int) (canvasSize.Width * 0.9) - location.X, (int) (canvasSize.Width * 0.5));
                    } else {
                        maxWidth = defaultMaxWidth;
                    }
                    var maxSize = new Size(maxWidth, int.MaxValue);
                    ((INode) cmd.CreatedEditor.Figure).MaxSize = maxSize;

                    cmd.CreatedEditor.RequestFocus(FocusKind.Begin, location);
                    var focus = editor.Site.FocusManager.Focus as StyledTextFocus;
                    insert(focus);
                    
                    // 1文字目と2文字目が入れ替わってしまう
                    //cmd.CreatedEditor.RequestFocusCommit(true);
                    //cmd.CreatedEditor.RequestFocus(FocusKind.Begin, location);
                }

                return cmd.CreatedEditor;
            }
        }

        private static string GetNewImageFilePath() {
            return Path.Combine(MemopadConsts.EmbeddedImageRoot, Guid.NewGuid().ToString());
        }
    }
}
