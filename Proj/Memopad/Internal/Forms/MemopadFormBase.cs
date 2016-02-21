/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Editor.Focuses;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Controls;
using Mkamo.Memopad.Internal.Core;
using System.Drawing;
using Mkamo.Memopad.Internal.Controllers;
using ComponentFactory.Krypton.Toolkit;
using Mkamo.Editor.Tools;
using Mkamo.Common.Command;
using Mkamo.Memopad.Core;
using Mkamo.Common.Win32.Core;
using System.IO;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Figure.Core;
using Mkamo.Editor.Requests;
using Mkamo.Model.Memo;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.StyledText.Core;
using Mkamo.Common.DataType;
using Mkamo.Editor.Commands;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Forms.Clipboard;

namespace Mkamo.Memopad.Internal.Forms {
    /// <summary>
    /// 主にEditorCanvasとのやりとりを実装。
    /// </summary>
    internal partial class MemopadFormBase: Form {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;
        private MemopadUILogic _uiLogic;

        private bool _isCompact;

        private string _globalHighlight;

        private ToolSelectorDropDown _toolSelectorDropDown;
        private NodeStyleSelectorDropDown _nodeStyleSelectorDropDown;
        private LineStyleSelectorDropDown _lineStyleSelectorDropDown;
        private FreehandSelectorDropDown _freehandSelectorDropDown;

        /// <summary>
        /// コードによるコンボボックスの値の変更によって
        /// イベントハンドラが呼ばれた場合は何もしないようにするため
        /// </summary>
        private bool _suppressFontChangeEventHandler;

        /// <summary>
        /// ノート内検索中の検索中にFontNameコンボボックスなどが更新されて
        /// ちらつくのを防ぐため
        /// </summary>
        private bool _suppressToolStripUpdate;

        private bool _suppressParagraphKindChangeEventHandler;

        private ListKind _currentSpecialListKind = ListKind.CheckBox;

        // ========================================
        // constructor
        // ========================================
        protected MemopadFormBase() {
            _facade = MemopadApplication.Instance;
            _uiLogic = new MemopadUILogic();

            _toolSelectorDropDown = new ToolSelectorDropDown(this);
            _nodeStyleSelectorDropDown = new NodeStyleSelectorDropDown(this);
            _lineStyleSelectorDropDown = new LineStyleSelectorDropDown(this);
            _freehandSelectorDropDown = new FreehandSelectorDropDown(this);
        }

        private void InitDropDowns() {
        }

        // ========================================
        // property
        // ========================================
        public EditorCanvas EditorCanvas {
            get { return _EditorCanvas; }
        }

        public IToolRegistry ToolRegistry {
            get {
                _toolSelectorDropDown.Prepare();
                return _toolSelectorDropDown;
            }
        }

        // ------------------------------
        // protected internal
        // ------------------------------
        protected internal virtual string _GlobalHighlight {
            get { return _globalHighlight; }
            set { _globalHighlight = value; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        // --- abstract ---
        // abstractにすると継承したFormがデザイナで開けなくなる
        protected virtual EditorCanvas _EditorCanvas {
            get { return null; }
        }

        protected virtual PageContent _PageContent {
            get { return null; }
        }

        protected virtual bool _EnableBackgroundImage {
            get { return false; }
        }

        protected virtual ComboBox _ParagraphKindToolStripComboBox {
            get { return null; }
        }

        protected virtual ComboBox _FontNameToolStripComboBox {
            get { return null; }
        }

        protected virtual ComboBox _FontSizeToolStripComboBox {
            get { return null; }
        }

        //protected virtual _textColorButtonToolStripItem

        protected ToolSelectorDropDown _ToolSelectorDropDown {
            get { return _toolSelectorDropDown; }
        }
        protected NodeStyleSelectorDropDown _NodeStyleSelectorDropDown {
            get { return _nodeStyleSelectorDropDown; }
        }
        protected LineStyleSelectorDropDown _LineStyleSelectorDropDown {
            get { return _lineStyleSelectorDropDown; }
        }
        protected FreehandSelectorDropDown _FreehandSelectorDropDown {
            get { return _freehandSelectorDropDown; }
        }

        protected MemopadUILogic _UILogic {
            get { return _uiLogic; }
        }

        protected bool _SuppressToolStripUpdate {
            get { return _suppressToolStripUpdate; }
            set { _suppressToolStripUpdate = value; }
        }

        protected bool _SuppressParagraphKindChangeEventHandler {
            get { return _suppressParagraphKindChangeEventHandler; }
            set { _suppressParagraphKindChangeEventHandler = value; }
        }

        public bool _SuppressFontChangeEventHandler {
            get { return _suppressFontChangeEventHandler; }
            set { _suppressFontChangeEventHandler = value; }
        }

        protected bool _IsCompact {
            get { return _isCompact; }
            set { _isCompact = value; }
        }

        protected ListKind _CurrentSpecialListKind {
            get { return _currentSpecialListKind; }
            set { _currentSpecialListKind = value; }
        }

        // ========================================
        // method
        // ========================================
        // abstractにすると継承したFormがデザイナで開けなくなる
        protected internal virtual void UpdateToolStrip() {
        }

        protected internal void SetParagraphListKind(ListKind listKind) {
            if (_EditorCanvas == null) {
                return;
            }

            if (_EditorCanvas.FocusManager.IsEditorFocused) {
                var focus = _EditorCanvas.FocusManager.Focus as StyledTextFocus;
                focus.ToggleList(listKind);
                UpdateToolStrip();
                return;
            }

            var selecteds = _EditorCanvas.SelectionManager.SelectedEditors;
            if (selecteds.Any()) {
                var bundle = new EditorBundle(_EditorCanvas.SelectionManager.SelectedEditors);
                var executor = _EditorCanvas.CommandExecutor;


                /// listKindでないParagraphがひとつでもあればon == true。そうでなければon == false。
                var on = false;
                var node = default(MemoStyledText);
                foreach (var selected in selecteds) {
                    var memoSText = selected.Model as MemoStyledText;

                    if (memoSText != null) {
                        var stext = memoSText.StyledText;
                        foreach (var block in stext.Blocks) {
                            var para = block as Paragraph;
                            if (para != null && para.ListKind != listKind) {
                                on = true;
                                break;
                            }
                        }
                        if (node == null) {
                            node = memoSText;
                        }
                    }

                    if (on) {
                        break;
                    }
                }

                if (node != null) {
                    var req = new SetStyledTextListKindRequest() {
                        ListKind = listKind,
                        On = on,
                    };
                    bundle.PerformCompositeRequest(req, executor);
                }
            }
        }

        protected internal void ToggleSpecialList() {
            SetParagraphListKind(_currentSpecialListKind);
        }

        // ------------------------------
        // protected
        // ------------------------------
        // abstractにすると継承したFormがデザイナで開けなくなる
        protected virtual void FocusEditorCanvas() {
        }

        /// <summary>
        /// タグ設定DropDownのようにコントロールを持つポップアップを表示したときに
        /// 一瞬タイトルバーが非アクティブ表示になるのを防ぐ
        /// </summary>
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case (int) WindowMessage.NCACTIVATE: {
                    /// タイトルバーを常にアクティブ色で描画する
                    m.WParam = (IntPtr) 1;
                    break;
                }
                case (int) WindowMessage.ACTIVATEAPP: {
                    /// アプリケーション自体のフォーカスが変わればNCACTIVE
                    m.Msg = (int) WindowMessage.NCACTIVATE;
                    break;
                }
            }

            base.WndProc(ref m);
        }

        protected virtual PageContent CreateMemoPageContent(MemoInfo info) {
            //if (_fontNameToolStripComboBox.Items.Count == 0) {
            //    _uiLogic.InitFontNameToolStripComboBox(_fontNameToolStripComboBox.ComboBox);
            //    _uiLogic.InitFontSizeToolStripComboBox(_fontSizeToolStripComboBox.ComboBox);
            //}

            var pageContent = new PageContent(info);
            pageContent.Theme = _facade.Theme;
            pageContent.Dock = DockStyle.Fill;
            pageContent.ContentModified += HandlePageContentModified;
            //pageContent.TitleChanged += HandleMemoTitleChanged;
            pageContent.InMemoSearcher.Searching += HandlePageContentSearching;
            pageContent.InMemoSearcher.Searched += HandlePageContentSearched;

            var canvas = pageContent.EditorCanvas;
            InitEditorCanvas(canvas);

            pageContent.SetCompact(_isCompact);

            return pageContent;
        }

        protected virtual void DisposeMemoPageContent(PageContent pageContent) {
            pageContent.ContentModified -= HandlePageContentModified;
            //pageContent.TitleChanged -= HandleMemoTitleChanged;
            pageContent.EditorCanvas.SelectionManager.SelectionChanged -= HandleEditorCanvasSelectionChanged;
            pageContent.EditorCanvas.FocusManager.FocusChanged -= HandleEditorCanvasFocusChanged;
            pageContent.EditorCanvas.CommandExecutor.CommandChainEnded -= HandleEditorCanvasCommandExecuted;
            pageContent.EditorCanvas.CommandExecutor.CommandExecuted -= HandleEditorCanvasCommandExecuted;
            pageContent.EditorCanvas.CommandExecutor.CommandUndone -= HandleEditorCanvasCommandUndoneOrRedone;
            pageContent.EditorCanvas.CommandExecutor.CommandRedone -= HandleEditorCanvasCommandUndoneOrRedone;
            pageContent.EditorCanvas.Caret.CaretMoved -= HandleMemoEditorCanvasCaretMoved;
            pageContent.EditorCanvas.Caret.VisibleChanged -= HandleMemoEditorCanvasCaretVisibleChanged;
            pageContent.InMemoSearcher.Searching -= HandlePageContentSearching;
            pageContent.InMemoSearcher.Searched -= HandlePageContentSearched;
        }

        protected void EnsureFocusCommited(EditorCanvas canvas) {
            if (canvas == null) {
                return;
            }
            if (canvas.FocusManager.IsEditorFocused) {
                canvas.FocusManager.FocusedEditor.RequestFocusCommit(true);
            }
        }

        protected void SavePageContent(PageContent pageContent) {
            var info = pageContent.MemoInfo;
            var canvas = pageContent.EditorCanvas;

            if (pageContent.IsModified) {
                MemoSerializeUtil.SaveEditor(info.MementoId, canvas);
                var memo = canvas.EditorContent as Memo;

                _facade.Container.SaveExtendedTextData(memo, "FullText", canvas.GetFullText());
                _facade.Container.SaveExtendedTextData(memo, "SummaryText", MemoTextUtil.GetSummaryText(canvas));
                MemoMarkUtil.SaveMarkIdsCache(memo, MemoMarkUtil.GetMemoMarkIds(memo));
                MemoOutlineUtil.SaveOutline(pageContent.EditorCanvas);

                pageContent.IsModified = false;
            }
        }

        protected void SetTextColorButtonToolStripItemEnabled(ToolStripItem textColorButtonToolStripItem, bool enabled) {
            if (enabled && !textColorButtonToolStripItem.Enabled) {
                textColorButtonToolStripItem.Enabled = true;
            } else if (!enabled && textColorButtonToolStripItem.Enabled) {
                textColorButtonToolStripItem.Enabled = false;
            }
        }

        protected void SetShapeColorButtonToolStripItemEnabled(ToolStripItem shapeColorButtonToolStripItem, bool enabled) {
            if (enabled && !shapeColorButtonToolStripItem.Enabled) {
                shapeColorButtonToolStripItem.Enabled = true;
            } else if (!enabled && shapeColorButtonToolStripItem.Enabled) {
                shapeColorButtonToolStripItem.Enabled = false;
            }
        }

        protected void AddImage(IEditor target, string imageFilePath) {
            var img = default(Image);
            if (File.Exists(imageFilePath)) {
                try {
                    img = Image.FromFile(imageFilePath);
                } catch (Exception e) {
                    Logger.Warn("Illegal image file", e);
                    MessageBox.Show(this, "不正な形式の画像ファイルです。", "画像ファイルロードエラー");
                    return;
                }

                MemoEditorHelper.AddImage(target, new Point(8, 8), img, true, true);
            }
        }

        protected void SetFontBase(Func<FontDescription, FontDescription> fontProvider, FontModificationKinds kind) {
            if (_EditorCanvas == null) {
                return;
            }
            if (_EditorCanvas.FocusManager.IsEditorFocused) {
                /// Focusがある場合はその内容を変更する
                var focus = _EditorCanvas.FocusManager.Focus as StyledTextFocus;
                if (focus != null) {
                    focus.SetFont((flow) => fontProvider(flow.Font));
                }
                // todo: PlainTextのときの処理

            } else {
                /// Focusがない場合はRequestを送る
                var selecteds = _EditorCanvas.SelectionManager.SelectedEditors;
                if (selecteds.Any()) {
                    var bundle = new EditorBundle(_EditorCanvas.SelectionManager.SelectedEditors);
                    var executor = _EditorCanvas.CommandExecutor;

                    var nodeFig = selecteds.First().Figure as INode;
                    if (nodeFig != null) {
                        var req = new SetPlainTextFontRequest() {
                            Font = fontProvider(nodeFig.Font),
                        };
                        bundle.PerformCompositeRequest(req, executor);
                    }

                    var node = selecteds.First().Model as MemoStyledText;
                    if (node != null) {
                        var stext = node.StyledText;
                        var stfont = stext.Inlines.First().Font;
                        var req = new SetStyledTextFontRequest() {
                            FontProvider = (flow) => fontProvider(stfont),
                        };
                        bundle.PerformCompositeRequest(req, executor);
                    }
                }
            }
        }

        protected void SetTextColorBase(Color color) {
            if (_EditorCanvas == null) {
                return;
            }
            if (_EditorCanvas.FocusManager.IsEditorFocused) {
                /// Focusがある場合はその内容を変更する
                var focus = _EditorCanvas.FocusManager.Focus as StyledTextFocus;
                if (focus != null) {
                    focus.SetColor(color);
                }
                // todo: PlainTextのときの処理

            } else {
                /// Focusがない場合はRequestを送る
                var selecteds = _EditorCanvas.SelectionManager.SelectedEditors;
                if (selecteds.Any()) {
                    var bundle = new EditorBundle(_EditorCanvas.SelectionManager.SelectedEditors);
                    var executor = _EditorCanvas.CommandExecutor;

                    //var nodeFig = selecteds.First().Figure as INode;
                    //if (nodeFig != null) {
                    //    var req = new SetPlainTextFontRequest() {
                    //        Font = fontProvider(nodeFig.Font),
                    //    };
                    //    bundle.PerformCompositeRequest(req, executor);
                    //}

                    var node = selecteds.First().Model as MemoStyledText;
                    if (node != null) {
                        var stext = node.StyledText;
                        var req = new SetStyledTextColorRequest(color);
                        bundle.PerformCompositeRequest(req, executor);
                    }
                }
            }
        }

        // --- paragraph ---
        protected void EnableParagraphKindComboBox() {
            if (_ParagraphKindToolStripComboBox.Enabled) {
                return;
            }
            _ParagraphKindToolStripComboBox.BeginUpdate();
            _ParagraphKindToolStripComboBox.Items.Clear();
            _ParagraphKindToolStripComboBox.Items.AddRange(
                new [] {
                    "標準",
                    "見出し1",
                    "見出し2",
                    "見出し3",
                    "見出し4",
                    "見出し5",
                    "見出し6",
                }
            );
            _ParagraphKindToolStripComboBox.EndUpdate();
            _ParagraphKindToolStripComboBox.Enabled = true;
        }

        protected void DisableParagraphKindComboBox() {
            if (!_ParagraphKindToolStripComboBox.Enabled) {
                return;
            }

            _ParagraphKindToolStripComboBox.BeginUpdate();
            _ParagraphKindToolStripComboBox.Items.Clear();
            _ParagraphKindToolStripComboBox.Items.AddRange(
                new [] {
                    "",
                }
            );
            SetParagraphKindComboBoxTextWithoutEventHandling("");
            _ParagraphKindToolStripComboBox.EndUpdate();
            _ParagraphKindToolStripComboBox.Enabled = false;
        }

        protected void SetParagraphKindComboBoxTextWithoutEventHandling(string paragraphKind) {
            if (_ParagraphKindToolStripComboBox.Text == paragraphKind) {
                return;
            }
            _suppressParagraphKindChangeEventHandler = true;
            _ParagraphKindToolStripComboBox.Text = paragraphKind;
            _suppressParagraphKindChangeEventHandler = false;
        }

        protected void SetFocusParagraphKind(ParagraphKind paragraphKind) {
            if (_EditorCanvas == null) {
                return;
            }

            if (_EditorCanvas.FocusManager.IsEditorFocused) {
                var focus = _EditorCanvas.FocusManager.Focus as StyledTextFocus;
                focus.SetParagraphKind(paragraphKind);
                UpdateToolStrip();
            }
        }

        protected ParagraphKind GetParagraphKindFromString(string kind) {
            var ret = ParagraphKind.Normal;
            switch (kind) {
                case "標準":
                    ret = ParagraphKind.Normal;
                    break;
                case "見出し1":
                    ret = ParagraphKind.Heading1;
                    break;
                case "見出し2":
                    ret = ParagraphKind.Heading2;
                    break;
                case "見出し3":
                    ret = ParagraphKind.Heading3;
                    break;
                case "見出し4":
                    ret = ParagraphKind.Heading4;
                    break;
                case "見出し5":
                    ret = ParagraphKind.Heading5;
                    break;
                case "見出し6":
                    ret = ParagraphKind.Heading6;
                    break;
            }
            return ret;
        }

        protected string GetStringFromParagraphKind(ParagraphKind paragraphKind) {
            switch (paragraphKind) {
                case ParagraphKind.Normal:
                    return "標準";
                case ParagraphKind.Heading1:
                    return "見出し1";
                case ParagraphKind.Heading2:
                    return "見出し2";
                case ParagraphKind.Heading3:
                    return "見出し3";
                case ParagraphKind.Heading4:
                    return "見出し4";
                case ParagraphKind.Heading5:
                    return "見出し5";
                case ParagraphKind.Heading6:
                    return "見出し6";
                default:
                    return "";
            }
        }

        protected void SetFontStyleBase(Func<FontDescription, FontStyle> fontStyleProvider) {
            SetFontBase((font) => new FontDescription(font, fontStyleProvider(font)), FontModificationKinds.Style);
        }

        protected void SetStyledTextAlignmentBase(
            AlignmentModificationKinds kinds,
            Mkamo.Common.DataType.HorizontalAlignment horizontaliAlign,
            VerticalAlignment verticalAlign
        ) {
            if (_EditorCanvas.FocusManager.IsEditorFocused) {
                /// Focusがある場合はその内容を変更する
                var focus = _EditorCanvas.FocusManager.Focus as StyledTextFocus;
                if (focus != null) {
                    if ((kinds & AlignmentModificationKinds.Horizontal) == AlignmentModificationKinds.Horizontal) {
                        focus.SetHorizontalAlignment(horizontaliAlign);
                    }
                    if ((kinds & AlignmentModificationKinds.Vertical) == AlignmentModificationKinds.Vertical) {
                        focus.SetVerticalAlignment(verticalAlign);
                    }
                }

                return;
            }

            var selecteds = _EditorCanvas.SelectionManager.SelectedEditors;
            if (selecteds.Any()) {
                var bundle = new EditorBundle(_EditorCanvas.SelectionManager.SelectedEditors);
                var executor = _EditorCanvas.CommandExecutor;

                //var nodeFig = selecteds.First().Figure as INode;
                //if (nodeFig != null) {
                //    var req = new SetPlainTextFontRequest() {
                //        Font = fontProvider(nodeFig.Font),
                //    };
                //    bundle.PerformCompositeRequest(req, executor);
                //}

                var node = selecteds.First().Model as MemoStyledText;
                if (node != null) {
                    var req = new SetStyledTextAlignmentRequest() {
                        Kinds = kinds,
                        HorizontalAlignment = horizontaliAlign,
                        VerticalAlignment = verticalAlign,
                    };
                    bundle.PerformCompositeRequest(req, executor);
                }
            }
        }

        protected void SetShapeColorBase(Color color) {
            if (_EditorCanvas == null) {
                return;
            }

            var background = default(IBrushDescription);
            if (color.A == 0 && color.R == 0 && color.G == 0 && color.B == 0) {
                background = null;
            } else {
                background = new SolidBrushDescription(color);
            }

            var editors = _EditorCanvas.SelectionManager.SelectedEditors;
            var cmd = default(ICommand);
            foreach (var editor in editors) {
                if (editor.Model is MemoShape || editor.Model is MemoTableCell) {
                    if (cmd == null) {
                        cmd = new SetNodeBackgroundCommand(editor, background);
                    } else {
                        cmd = cmd.Chain(new SetNodeBackgroundCommand(editor, background));
                    }
                }
            }
            if (cmd != null) {
                _EditorCanvas.CommandExecutor.Execute(cmd);
            }
        }


        // ------------------------------
        // private
        // ------------------------------
        private void InitEditorCanvas(EditorCanvas canvas) {
            canvas.TransientData[MemopadConsts.FormEditorTransientDataKey] = this;
            canvas.ImeMode = _facade._Settings.EditorCanvasImeOn ? ImeMode.On : ImeMode.Off;

            canvas.NoSelectionKeyMap = _facade.KeySchema.NoSelectionKeyMap;
            canvas.UseClearType = _facade._Settings.UseClearType;
            canvas.HighlightRegistry.GlobalHighlights = Highlight.CreateHighlights(_globalHighlight);
            canvas.Font = MemopadApplication.Instance.Settings.GetDefaultMemoTextFont().CreateFont();
            canvas.BackColor = Color.White;
            //canvas.BackColor = Color.Ivory;
            canvas.MiniToolBarBorderColor = _facade.KryptonPalette.GetBorderColor1(PaletteBorderStyle.ContextMenuOuter, PaletteState.Normal);
            canvas.MiniToolBarRenderer = _facade._MiniToolBarRenderer;
            //canvas.ImageCopyRightString = null;

            if (_EnableBackgroundImage && _facade.WindowSettings.MemoEditorBackgroundImageEnabled) {
                try {
                    canvas.CanvasBackgroundImage = _facade.WindowSettings.CreateMemoEditorBackgroundImage();
                } catch (Exception e) {
                    Logger.Warn("Failed to set background: " + e);
                }
            }

            canvas.ControllerFactory = new MemoControllerFactory();
            canvas.Tool = new SelectTool();
            canvas.EditorCopyExtenderManager.RegisterExtension(CopyTextAndHtml);
            canvas.AbbrevWordProvider.AdditionalWordsProvider = _facade._AdditionalAbbrevWordProvider;

            canvas.SelectionManager.SelectionChanged += HandleEditorCanvasSelectionChanged;
            canvas.FocusManager.FocusChanged += HandleEditorCanvasFocusChanged;

            canvas.CommandExecutor.CommandExecuted += HandleEditorCanvasCommandExecuted;
            canvas.CommandExecutor.CommandChainEnded += HandleEditorCanvasCommandExecuted;
            canvas.CommandExecutor.CommandUndone += HandleEditorCanvasCommandUndoneOrRedone;
            canvas.CommandExecutor.CommandRedone += HandleEditorCanvasCommandUndoneOrRedone;

            canvas.Caret.CaretMoved += HandleMemoEditorCanvasCaretMoved;
            canvas.Caret.VisibleChanged += HandleMemoEditorCanvasCaretVisibleChanged;
        }

        private void CopyTextAndHtml(IEnumerable<IEditor> editors, IDataObject dataObj) {
            var textExporter = new PlainTextExporter();
            dataObj.SetData(DataFormats.UnicodeText, textExporter.ExportText(editors));

            var htmlExporter = new HtmlExporter();
            var html = htmlExporter.ExportHtml(editors);
            dataObj.SetData(DataFormats.Html, ClipboardUtil.GetCFHtmlMemoryStream(html));
        }

        // --- editor canvas handler ---
        private void HandleEditorCanvasFocusChanged(object sender, Mkamo.Editor.Core.FocusChangedEventArgs e) {
            if (e.OldFocusedEditor != null) {
                var stFocus = e.OldFocusedEditor.Focus as StyledTextFocus;
                if (stFocus != null) {
                    stFocus.SelectionChanged -= HandleEditorCanvasFocusSelectionChanged;
                }
            }
            if (e.NewFocusedEditor != null) {
                var stFocus = e.NewFocusedEditor.Focus as StyledTextFocus;
                if (stFocus != null) {
                    stFocus.SelectionChanged += HandleEditorCanvasFocusSelectionChanged;
                }
            }

            UpdateToolStrip();
        }

        private void HandleEditorCanvasSelectionChanged(object sender, EventArgs e) {
            UpdateToolStrip();
        }

        private void HandleEditorCanvasFocusSelectionChanged(object sender, EventArgs e) {
            UpdateToolStrip();
        }

        private void HandleEditorCanvasCommandExecuted(object sender, CommandEventArgs e) {
            UpdateToolStrip();
        }

        private void HandleEditorCanvasCommandUndoneOrRedone(object sender, CommandEventArgs e) {
            if (!_EditorCanvas.SelectionManager.SelectedEditors.Any()) {
                _EditorCanvas.RootEditor.Content.RequestSelect(SelectKind.True, true);
            }
            UpdateToolStrip();
        }

        private void HandleMemoEditorCanvasCaretVisibleChanged(object sender, EventArgs e) {
            if (_EditorCanvas != null && _EditorCanvas.Caret.IsVisible) {
                UpdateToolStrip();
            }
        }

        private void HandleMemoEditorCanvasCaretMoved(object sender, EventArgs e) {
            if (_EditorCanvas != null && _EditorCanvas.Caret.IsVisible) {
                UpdateToolStrip();
            }
        }

        // --- page content handler ---
        private void HandlePageContentModified(object sender, EventArgs e) {
            var content = (PageContent) sender;
            _facade.AddRecentlyModifiedMemoInfo(content.MemoInfo);

            /// 日付を更新するために必要だが編集のたびにちらつくのでやらない
            /// _memoListBox.Invalidate();
            if (_facade.IsMainFormLoaded) {
                var info = _PageContent.MemoInfo;
                _facade.MainForm.InvalidateMemoListBox(new [] { info });
            }
        }
 
        private void HandlePageContentSearching(object sender, EventArgs e) {
            _suppressToolStripUpdate = true;
        }

        private void HandlePageContentSearched(object sender, EventArgs e) {
            _suppressToolStripUpdate = false;
            UpdateToolStrip();
        }

    }
}
