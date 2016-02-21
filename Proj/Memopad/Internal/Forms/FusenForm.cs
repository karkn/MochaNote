/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Internal.Controls;
using Mkamo.Memopad.Internal.Core;
using ComponentFactory.Krypton.Toolkit;
using Mkamo.Memopad.Internal.Controllers;
using Mkamo.Editor.Tools;
using Mkamo.Editor.Core;
using Mkamo.Editor.Focuses;
using Mkamo.Common.Command;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Memopad.Properties;
using Mkamo.Common.Win32.User32;
using Mkamo.Common.Win32.Core;
using System.Runtime.InteropServices;
using Mkamo.Model.Memo;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Mkamo.Common.Serialize;
using Mkamo.StyledText.Core;
using Mkamo.Figure.Core;
using Mkamo.Common.Core;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class FusenForm: MemopadFormBase {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private PageContent _content;

        private MemopadApplication _app;

        private ToolStripForm _toolStripForm;
        private bool _isShown;

        // ========================================
        // constructor
        // ========================================
        public FusenForm() {
            InitializeComponent();

            ShowInTaskbar = false;
            Icon = Resources.confidante;
            //TransparencyKey = Color.White;

            _app = MemopadApplication.Instance;
        }

        private void InitDropDowns() {
            _toolStripForm._addFigureToolStripDropDownButton.DropDown = _ToolSelectorDropDown;
            _toolStripForm._addFigureToolStripDropDownButton.DropDownOpening += (s, e) => {
                _ToolSelectorDropDown.Prepare();
            };

            _toolStripForm._setNodeStyleToolStripDropDownButton.DropDown = _NodeStyleSelectorDropDown;
            _toolStripForm._setNodeStyleToolStripDropDownButton.DropDownOpening += (s, e) => {
                _NodeStyleSelectorDropDown.Prepare();
            };

            _toolStripForm._setLineStyleToolStripDropDownButton.DropDown = _LineStyleSelectorDropDown;
            _toolStripForm._setLineStyleToolStripDropDownButton.DropDownOpening += (s, e) => {
                _LineStyleSelectorDropDown.Prepare();
            };

            _toolStripForm._addFreehandToolStripDropDownButton.DropDown = _FreehandSelectorDropDown;
            _toolStripForm._addFreehandToolStripDropDownButton.DropDownOpening += (s, e) => {
                _FreehandSelectorDropDown.Prepare();
            };

        }

        private void InitMemoMarkToolStripSplitButton() {
            _toolStripForm._memoMarkToolStripSplitButton.Image = Resources.star;
            _toolStripForm._memoMarkToolStripSplitButton.Tag = MemoMarkKind.Important;
        }

        // ========================================
        // destructor
        // ========================================
        private void CleanUp() {
            if (_content != null) {
                _content.Dispose();
            }
            if (_toolStripForm != null) {
                _toolStripForm.Close();
                _toolStripForm.Dispose();
            }
        }

        // ========================================
        // property
        // ========================================
        public PageContent PageContent {
            get { return _content; }
        }

        // ------------------------------
        // protected internal
        // ------------------------------
        protected internal override string _GlobalHighlight {
            get { return base._GlobalHighlight; }
            set {
                base._GlobalHighlight = value;
                var canvas = _content.EditorCanvas;
                var hls = Highlight.CreateHighlights(value);
                canvas.HighlightRegistry.GlobalHighlights = hls;
                canvas.DirtyAllVisualLines();
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected override EditorCanvas _EditorCanvas {
            get { return _content == null ? null : _content.EditorCanvas; }
        }

        protected override PageContent _PageContent {
            get { return _content; }
        }

        protected override ComboBox _ParagraphKindToolStripComboBox {
            get { return _toolStripForm._paragraphKindToolStripComboBox.ComboBox; }
        }

        protected override ComboBox _FontNameToolStripComboBox {
            get { return _toolStripForm._fontNameToolStripComboBox.ComboBox; }
        }

        protected override ComboBox _FontSizeToolStripComboBox {
            get { return _toolStripForm._fontSizeToolStripComboBox.ComboBox; }
        }

        
        protected MemoInfo _MemoInfo {
            get { return _content == null ? null : _content.MemoInfo; }
        }

        // ========================================
        // method
        // ========================================
        public void NewMemo(MemoInfo info, Memo memo) {
            _content = CreateMemoPageContent(info);
            _content.SetCompact(true);
            _content.IsModified = true;
            Controls.Add(_content);

            var canvas = _content.EditorCanvas;
            canvas.EditorContent = memo;

            var caret = canvas.Caret;
            caret.Position = MemopadConsts.DefaultCaretPosition;
            caret.Show();
            canvas.RootEditor.Content.RequestSelect(SelectKind.True, true);

            canvas.Select();

            Text = _content.Title;
            Show();

            InitToolStripForm(true);
        }

        public void OpenMemo(MemoInfo info, bool background, bool enabled) {
            if (_content != null && info == _content.MemoInfo) {
                return;
            }

            _content = CreateMemoPageContent(info);
            _content.SetCompact(true);

            var canvas = _content.EditorCanvas;
            MemoSerializeUtil.LoadEditor(canvas, info.MementoId);
            _content.Memo.AccessedDate = DateTime.Now;

            _content.Enabled = enabled;
            Controls.Add(_content);

            var caret = canvas.Caret;
            caret.Position = MemopadConsts.DefaultCaretPosition;
            caret.Show();
            canvas.RootEditor.Content.RequestSelect(SelectKind.True, true);

            canvas.Select();

            var app = MemopadApplication.Instance;
            var memo = app.Container.Find<Memo>(_MemoInfo.MemoId);
            var bytes = app.Container.LoadExtendedBinaryData(memo, "FusenBounds");
            var bounds = Rectangle.Empty;
            if (bytes != null) {
                StartPosition = FormStartPosition.Manual;
                bounds = (Rectangle) BinaryFormatterUtil.FromBytes(bytes);
                Bounds = bounds;
            }

            Text = _content.Title;
            Show();
            if (bytes != null) {
                /// なぜかShow()でHeightが+2されてしまうのでもう一度設定
                Bounds = bounds;
            }

            if (background) {
                User32PI.SetActiveWindow(MemopadApplication.Instance.MainForm.Handle);
            }
            InitToolStripForm(!background);
        }

        public void EnsureFocusCommited () {
            EnsureFocusCommited(_EditorCanvas);
        }

        public void Save() {
            SavePageContent(_content);

            var app = MemopadApplication.Instance;
            var memo = app.Container.Find<Memo>(_MemoInfo.MemoId);
            app.Container.SaveExtendedBinaryData(memo, "FusenBounds", BinaryFormatterUtil.ToBytes(Bounds));
        }

        public void HideToolStripForm() {
            if (_toolStripForm != null) {
                _toolStripForm.Hide();
            }
        }

        public void UpdateMainToolStrip() {
            if (_SuppressToolStripUpdate) {
                return;
            }

            _toolStripForm._topMostToolStripButton.Checked = TopMost;
            _toolStripForm._showInfoToolStripButton.Checked = !_content.IsCompact;
        }

        // ------------------------------
        // protected
        // ------------------------------
        // --- tool strip ---
        protected internal override void UpdateToolStrip() {
            if (_SuppressToolStripUpdate) {
                return;
            }
            if (_toolStripForm == null || _toolStripForm.IsDisposed) {
                return;
            }

            // todo:
            /// Enabledの設定
            var canCut = EditorCanvas.CanCut();
            var canCopy = EditorCanvas.CanCopy();
            var canPaste = EditorCanvas.CanPaste();
            var canUndo = EditorCanvas.CanUndo();
            var canRedo = EditorCanvas.CanRedo();

            var canModFontName = EditorCanvas.CanModifyFontName();
            var canModFontSize = EditorCanvas.CanModifyFontSize();
            var canModFontStyle = EditorCanvas.CanModifyFontStyle();
            var canModifyHAlign = EditorCanvas.CanModifyHorizontalAlignment();
            var canModifyVAlign = EditorCanvas.CanModifyVerticalAlignment();
            var canModifyListKind = EditorCanvas.CanModifyListKind();

            _toolStripForm._importantToolStripButton.Enabled = true;
            _toolStripForm._unimportantToolStripButton.Enabled = true;
            _toolStripForm._memoMarkToolStripSplitButton.Enabled = true;

            _toolStripForm._cutToolStripButton.Enabled = canCut;
            _toolStripForm._copyToolStripButton.Enabled = canCopy;
            _toolStripForm._pasteToolStripButton.Enabled = canPaste;
            _toolStripForm._undoToolStripButton.Enabled = canUndo;
            _toolStripForm._redoToolStripButton.Enabled = canRedo;
            _toolStripForm._searchInMemoToolStripButton.Enabled = true;

            _toolStripForm._fontNameToolStripComboBox.Enabled = canModFontName;
            _toolStripForm._fontSizeToolStripComboBox.Enabled = canModFontSize;
            _toolStripForm._fontBoldToolStripButton.Enabled = canModFontStyle;
            _toolStripForm._fontItalicToolStripButton.Enabled = canModFontStyle;
            _toolStripForm._fontUnderlineToolStripButton.Enabled = canModFontStyle;
            _toolStripForm._fontStrikeoutToolStripButton.Enabled = canModFontStyle;

            SetTextColorButtonToolStripItemEnabled(_toolStripForm._textColorButtonToolStripItem, EditorCanvas.CanModifyTextColor());

            _toolStripForm._leftHorizontalAlignmentToolStripButton.Enabled = canModifyHAlign;
            _toolStripForm._centerHorizontalAlignmentToolStripButton.Enabled = canModifyHAlign;
            _toolStripForm._rightHorizontalAlignmentToolStripButton.Enabled = canModifyHAlign;
            _toolStripForm._verticalAlignToolStripDropDownButton.Enabled = canModifyVAlign;

            _toolStripForm._orderedListToolStripButton.Enabled = canModifyListKind;
            _toolStripForm._unorderedListToolStripButton.Enabled = canModifyListKind;
            _toolStripForm._specialListToolStripButton.Enabled = canModifyListKind;
            _toolStripForm._selectSpecialListToolStripDropDownButton.Enabled = canModifyListKind;

            _toolStripForm._selectToolToolStripButton.Enabled = true;
            _toolStripForm._handToolToolStripButton.Enabled = true;
            _toolStripForm._adjustSpaceToolToolStripButton.Enabled = true;
            _toolStripForm._addFreehandToolStripDropDownButton.Enabled = true;
            _toolStripForm._addFigureToolStripDropDownButton.Enabled = true;
            _toolStripForm._addImageToolStripButton.Enabled = true;
            _toolStripForm._addFileToolStripDropDownButton.Enabled = true;
            _toolStripForm._addTableToolStripButton.Enabled = true;

            _toolStripForm._setNodeStyleToolStripDropDownButton.Enabled =
                !EditorCanvas.FocusManager.IsEditorFocused &&
                EditorCanvas.SelectionManager.SelectedEditors.Any(e => e.Model is MemoShape);
                //EditorCanvas.SelectionManager.SelectedEditors.Any(e => e.Model is MemoShape || e.Model is MemoTableCell);
            _toolStripForm._setLineStyleToolStripDropDownButton.Enabled =
                !EditorCanvas.FocusManager.IsEditorFocused &&
                EditorCanvas.SelectionManager.SelectedEditors.Any(e => e.Model is MemoEdge);
            SetShapeColorButtonToolStripItemEnabled(
                _toolStripForm._shapeColorButtonToolStripItem,
                !EditorCanvas.FocusManager.IsEditorFocused &&
                EditorCanvas.SelectionManager.SelectedEditors.Any(
                    e => e.Model is MemoShape || e.Model is MemoTableCell
                )
            );


            /// check
            var memo = _EditorCanvas.EditorContent as Memo;
            _toolStripForm._importantToolStripButton.Checked = memo.Importance == MemoImportanceKind.High;
            _toolStripForm._unimportantToolStripButton.Checked = memo.Importance == MemoImportanceKind.Low;

            _toolStripForm._selectToolToolStripButton.Checked = EditorCanvas.Tool is SelectTool;
            _toolStripForm._handToolToolStripButton.Checked = EditorCanvas.Tool is HandTool;
            _toolStripForm._adjustSpaceToolToolStripButton.Checked = EditorCanvas.Tool is AdjustSpaceTool;

            /// Text，Checkedの設定
            if (EditorCanvas.FocusManager.IsEditorFocused) {
                /// フォーカスあり
                var focus = EditorCanvas.FocusManager.Focus as StyledTextFocus;
                var font = focus.GetNextInputFont();

                SetFontComboBoxTextWithoutEventHandling(
                    canModFontName? font.Name: "",
                    canModFontSize? font.Size.ToString(): ""
                );
                _toolStripForm._fontBoldToolStripButton.Checked = canModFontStyle? font.IsBold: false;
                _toolStripForm._fontItalicToolStripButton.Checked = canModFontStyle? font.IsItalic: false;
                _toolStripForm._fontUnderlineToolStripButton.Checked = canModFontStyle ? font.IsUnderline : false;
                _toolStripForm._fontStrikeoutToolStripButton.Checked = canModFontStyle? font.IsStrikeout: false;

                var model = EditorCanvas.FocusManager.FocusedEditor.Model;
                if (model is MemoText || model is MemoShape || model is MemoTableCell) {
                    /// MemoTextとMemoShapeとMemoTableCellだけ有効にしておく
                    var para = focus.GetBlockAtCaretIndex() as Paragraph;
                    if (para != null) {
                        EnableParagraphKindComboBox();
                        var paraKind = focus.GetParagraphKind();
                        var paraKindText = paraKind == null ? "" : GetStringFromParagraphKind(paraKind.Value);
                        SetParagraphKindComboBoxTextWithoutEventHandling(paraKindText);

                        _toolStripForm._unorderedListToolStripButton.Enabled = true;
                        _toolStripForm._orderedListToolStripButton.Enabled = true;
                        _toolStripForm._specialListToolStripButton.Enabled = true;
                        _toolStripForm._selectSpecialListToolStripDropDownButton.Enabled = true;
                        _toolStripForm._indentToolStripButton.Enabled = para.ListLevel < 10;
                        _toolStripForm._outdentToolStripButton.Enabled = para.ListLevel > 0;

                        _toolStripForm._unorderedListToolStripButton.Checked = para.ListKind == ListKind.Unordered;
                        _toolStripForm._orderedListToolStripButton.Checked = para.ListKind == ListKind.Ordered;
                        _toolStripForm._specialListToolStripButton.Checked =
                             para.ListKind == ListKind.CheckBox ||
                             para.ListKind == ListKind.TriStateCheckBox ||
                             para.ListKind == ListKind.Star ||
                             para.ListKind == ListKind.LeftArrow ||
                             para.ListKind == ListKind.RightArrow;

                        _toolStripForm._addCommentToolStripButton.Enabled = model is MemoText;
                    } else {
                        DisableParagraphPropUI();
                        _toolStripForm._addCommentToolStripButton.Enabled = false;
                    }
                } else {
                    DisableParagraphPropUI();
                    _toolStripForm._addCommentToolStripButton.Enabled = false;
                }

            } else {
                /// フォーカスなし
                SetFontComboBoxTextWithoutEventHandling("", "");
                _toolStripForm._fontBoldToolStripButton.Checked = false;
                _toolStripForm._fontItalicToolStripButton.Checked = false;
                _toolStripForm._fontUnderlineToolStripButton.Checked = false;
                _toolStripForm._fontStrikeoutToolStripButton.Checked = false;

                _toolStripForm._unorderedListToolStripButton.Checked = false;
                _toolStripForm._orderedListToolStripButton.Checked = false;
                _toolStripForm._specialListToolStripButton.Checked = false;

                DisableParagraphPropUI();
                _toolStripForm._addCommentToolStripButton.Enabled = false;
            }
        }

        private void DisableParagraphPropUI() {
            DisableParagraphKindComboBox();

            _toolStripForm._indentToolStripButton.Enabled = false;
            _toolStripForm._outdentToolStripButton.Enabled = false;
        }

        protected override PageContent CreateMemoPageContent(MemoInfo info) {
            var ret = base.CreateMemoPageContent(info);
            ret.TitleChanged += HandleMemoTitleChanged;
            ret.EditorCanvas.ToolChanged += HandleEditorCanvasToolChanged;
            return ret;
        }

        protected override void DisposeMemoPageContent(PageContent pageContent) {
            pageContent.TitleChanged -= HandleMemoTitleChanged;
            pageContent.EditorCanvas.ToolChanged -= HandleEditorCanvasToolChanged;
            base.DisposeMemoPageContent(pageContent);
        }

        protected override void FocusEditorCanvas() {
            Activate();
            _EditorCanvas.Select();
        }

        protected override void OnShown(EventArgs e) {
            base.OnShown(e);
            _isShown = true;
        }

        //protected override void WndProc(ref Message m) {
        //    switch (m.Msg) {
        //        case (int) WindowMessage.MOVING:
        //            /// 画面端から出ないようにする
        //            var rect = (RECT) Marshal.PtrToStructure(m.LParam, typeof(RECT));

        //            var screen = Screen.GetWorkingArea(this);
        //            if (rect.left < screen.Left) {
        //                rect.left = screen.Left;
        //                rect.right = rect.left + Width;
        //            }
        //            if (rect.right > screen.Right) {
        //                rect.left = screen.Right - Width;
        //                rect.right = screen.Right;
        //            }
        //            if (rect.top < screen.Top) {
        //                rect.top = screen.Top;
        //                rect.bottom = rect.top + Height;
        //            }
        //            if (rect.bottom > screen.Bottom) {
        //                rect.top = screen.Bottom - Height;
        //                rect.bottom = screen.Bottom;
        //            }
        //            Marshal.StructureToPtr(rect, m.LParam, false);
        //            return;
        //    }
        //    base.WndProc(ref m);
        //}

        protected override CreateParams CreateParams {
            get {
                /// Alt+Tabで表示されないようにする
                const int WS_EX_TOOLWINDOW = 0x00000080;
                var cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | WS_EX_TOOLWINDOW;
                return cp;
            }
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if (_content != null) {
                _content.Invalidate();
            }

            RelocateToolStripForm();
        }
        
        protected override void OnMove(EventArgs e) {
            base.OnMove(e);
            
            RelocateToolStripForm();
        }

        protected override void OnActivated(EventArgs e) {
            base.OnActivated(e);

            if (_isShown && _toolStripForm != null) {
                RelocateToolStripForm();
                User32PI.ShowWindow(_toolStripForm.Handle, WindowShowStyle.ShowNoActivate);
            }
        }

        protected override void OnDeactivate(EventArgs e) {
            base.OnDeactivate(e);
            if (_toolStripForm != null) {
                if (Form.ActiveForm != this && Form.ActiveForm != _toolStripForm) {
                    _toolStripForm.Hide();
                }
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private void InitToolStripForm(bool show) {
            _toolStripForm = new ToolStripForm();
            _toolStripForm.TargetForm = this;
            InitDropDowns();
            InitMemoMarkToolStripSplitButton();

            InitToolStripHandlers();
            if (_toolStripForm._fontNameToolStripComboBox.Items.Count == 0) {
                _UILogic.InitFontNameToolStripComboBox(_toolStripForm._fontNameToolStripComboBox.ComboBox);
                _UILogic.InitFontSizeToolStripComboBox(_toolStripForm._fontSizeToolStripComboBox.ComboBox);
            }
            RelocateToolStripForm();
            UpdateToolStrip();
            if (show) {
                User32PI.ShowWindow(_toolStripForm.Handle, WindowShowStyle.ShowNoActivate);
            }
        }

        private void RelocateToolStripForm() {
            if (_toolStripForm != null) {
                //_toolStripForm.Width = Width;
                _toolStripForm.ClientSize = new Size(ClientSize.Width, _toolStripForm._mainToolStrip.Height + _toolStripForm._editToolStrip.Height);
                //_toolStripForm.ClientSize = new Size(ClientSize.Width, _toolStripForm.ClientSize.Height);
                if (Top < _toolStripForm.Height) {
                    _toolStripForm.Location = new Point(Left, Bottom);
                } else {
                    _toolStripForm.Location = new Point(Left, Top - _toolStripForm.Height);
                }
            }
        }

        private void SetFontComboBoxTextWithoutEventHandling(string fontName, string fontSize) {
            _SuppressFontChangeEventHandler = true;
            try {
                if (_toolStripForm._fontNameToolStripComboBox.Text != fontName) {
                    _toolStripForm._fontNameToolStripComboBox.Text = fontName;
                }
                if (_toolStripForm._fontSizeToolStripComboBox.Text != fontSize) {
                    _toolStripForm._fontSizeToolStripComboBox.Text = fontSize;
                }
            } finally {
                _SuppressFontChangeEventHandler = false;
            }
        }

        // --- handler ---
        private void HandleMemoTitleChanged(object sender, EventArgs e) {
            Text = _content.Title;
        }

        private void HandleEditorCanvasToolChanged(object sender, EventArgs e) {
            EnsureFocusCommited();

            var tool = _EditorCanvas.Tool;

            _toolStripForm._selectToolToolStripButton.Checked = tool is SelectTool;
            _toolStripForm._handToolToolStripButton.Checked = tool is HandTool;
            _toolStripForm._adjustSpaceToolToolStripButton.Checked = EditorCanvas.Tool is AdjustSpaceTool;

            if (tool is SelectTool) {
                _EditorCanvas.Cursor = Cursors.Default;
                if (
                    _EditorCanvas.SelectionManager.SelectedEditors.Count() == 1 &&
                    _EditorCanvas.SelectionManager.SelectedEditors.First().Model is Memo
                ) {
                    _EditorCanvas.Caret.Show();
                }
            } else if (tool is HandTool) {
                using (var stream = new MemoryStream(Resources.cursor_hand)) {
                    _EditorCanvas.Cursor = new Cursor(stream);
                }
                _EditorCanvas.Caret.Hide();
            } else if (tool is FreehandTool || tool is EraserTool || tool is DragSelectTool) {
                _EditorCanvas.Cursor = Cursors.Cross;
                _EditorCanvas.Caret.Hide();
            } else {
                _EditorCanvas.Cursor = Cursors.Default;
                _EditorCanvas.Caret.Hide();
            }
        }

 
    }
}
