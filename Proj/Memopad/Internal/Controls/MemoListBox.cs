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
using Mkamo.Container.Core;
using Mkamo.Model.Memo;
using System.Drawing.Drawing2D;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Common.Forms.Mouse;
using Mkamo.Common.Win32;
using System.Collections;
using Mkamo.Common.Collection;
using Mkamo.Common.DataType;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing.Text;
using System.Drawing.Imaging;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Common.Win32.User32;
using Mkamo.Common.String;
using Mkamo.Common.Win32.Core;
using Mkamo.Common.Core;
using Mkamo.Memopad.Properties;
using Mkamo.Common.Win32.Gdi32;
using Mkamo.Control.Core;

namespace Mkamo.Memopad.Internal.Controls {
    internal partial class MemoListBox: ListBox{
        // ========================================
        // static field
        // ========================================
        private const int IndentWidth = 16;
        private const int SummaryTextLineCount = 2;
        private const int ItemPadding = 5;

        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;
        private TextFormatFlags _textFormatFlags;
        private TextFormatFlags _summaryTextFormatFlags;
        private MemoListBoxComparer _comparer;

        private Image _noteImage;
        private Image _noteImportantImage;
        private Image _noteUnimportantImage;
        private Image _previewImage;

        private Point _currentMouseLocation;
        private int _currentMousePointItemIndex;

        private bool _disableEraseBackground;

        // --- item display ---
        private MemoListBoxDisplayItem[] _displayItems;

        // --- font ---
        private Font _captionFont;
        private Font _descriptionFont;

        // --- dnd ---
        private bool _canDragStart;
        private bool _isSelectionDeferred;
        private Message _deferredMessage;
        private bool _isDragPrepared;
        private bool _isDragStarted;
        private Point _dragStartPoint;
        private Rectangle _dragStartRect;

        // --- preview ---
        private Popup _previewPopup;
        private int _prevPreviewIndex;
        private bool _enablePreview;
        private bool _enablePreviewFollowMouseMove;

        // --- filter ---
        private Func<IEnumerable<MemoInfo>, IEnumerable<MemoInfo>> _filter;

        // ========================================
        // constructor
        // ========================================
        public MemoListBox() {
            _facade = MemopadApplication.Instance;

            _captionFont = Font;
            _descriptionFont = Font;

            InitializeComponent();

            _disableEraseBackground = false;

            _comparer = new MemoListBoxComparer();
            _canDragStart = false;

            _prevPreviewIndex = -1;

            _noteImage = Resources.sticky_note;
            _noteImportantImage = Resources.sticky_note_important;
            _noteUnimportantImage = Resources.sticky_note_unimportant;
            _previewImage = Resources.magnifier;

            ImeMode = System.Windows.Forms.ImeMode.Disable;
            AllowDrop = true;
            DrawMode = DrawMode.OwnerDrawFixed;
            SelectionMode = SelectionMode.MultiExtended;
            ResizeRedraw = true;

            _displayItems = new[] {
                MemoListBoxDisplayItem.Title,
                MemoListBoxDisplayItem.CreatedDate,
                MemoListBoxDisplayItem.Tag,
            };

            _textFormatFlags =
                TextFormatFlags.SingleLine | TextFormatFlags.Left | TextFormatFlags.EndEllipsis |
                TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;

            _summaryTextFormatFlags =
                TextFormatFlags.Top | TextFormatFlags.Left | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding |
                TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis;

            UpdateItemHeight();
        }


        private void CleanUp() {
            DisposePreviewPopup();
        }



        // ========================================
        // property
        // ========================================
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Font CaptionFont {
            get { return _captionFont; }
            set {
                if (value == null || value == _captionFont) {
                    return;
                }
                _captionFont = value;
                Font = _captionFont;
                UpdateItemHeight();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Font DescriptionFont {
            get { return _descriptionFont; }
            set {
                if (value == null || value == _descriptionFont) {
                    return;
                }
                _descriptionFont = value;
                UpdateItemHeight();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MemoListBoxDisplayItem SortKey {
            get { return _comparer.SortKey; }
            set {
                if (value == _comparer.SortKey) {
                    return;
                }
                Sorted = false;
                _comparer.SortKey = value;
                Sorted = true;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SortsAscendingOrder {
            get { return _comparer.IsAscendingOrder; }
            set {
                if (value == _comparer.IsAscendingOrder) {
                    return;
                }
                Sorted = false;
                _comparer.IsAscendingOrder = value;
                Sorted = true;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool SortsImportanceOrder {
            get { return _comparer.IsOrderByImportance; }
            set {
                if (value == _comparer.IsOrderByImportance) {
                    return;
                }
                Sorted = false;
                _comparer.IsOrderByImportance = value;
                Sorted = true;
            }
        }


        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MemoListBoxDisplayItem[] DisplayItems {
            get { return _displayItems; }
            set {
                if (value == null || !value.Contains(MemoListBoxDisplayItem.Title)) {
                    return;
                }
                _displayItems = value;
                UpdateItemHeight();
                Invalidate();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanDragStart {
            get { return _canDragStart; }
            set { _canDragStart = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<IEnumerable<MemoInfo>, IEnumerable<MemoInfo>> Filter {
            get { return _filter; }
            set{
                if (value == _filter) {
                    return;
                }
                _filter = value;
            }
        }
        

        public IEnumerable<MemoInfo> MemoInfos {
            get {
                //var ret = new List<MemoInfo>();
                //foreach (MemoInfo info in Items) {
                //    ret.Add(info);
                //}
                //return ret;
                foreach (MemoInfo info in Items) {
                    yield return info;
                }
            }
        }

        // ========================================
        // method
        // ========================================
        public void UpdateList(IEnumerable<MemoInfo> memoInfos, bool keepSelection, bool keepTopIndex) {
            BeginUpdate();

            try {
                var oldSelecteds = default(List<MemoInfo>);
                if (keepSelection) {
                    oldSelecteds = new List<MemoInfo>();
                    foreach (MemoInfo selected in SelectedItems) {
                        oldSelecteds.Add(selected);
                    }
                }
                var oldTopIndex = TopIndex;

                var addings = _filter == null ? memoInfos : _filter(memoInfos);

                Sorted = false;
                Items.Clear();
                foreach (var info in addings) {
                    Items.Add(info);
                }
                Sorted = true;

                if (keepSelection && oldSelecteds != null) {
                    foreach (var oldSelected in oldSelecteds) {
                        if (Items.Contains(oldSelected)) {
                            SelectedItems.Add(oldSelected);
                        }
                    }
                }
                if (keepTopIndex) {
                    TopIndex = oldTopIndex;
                }

            } finally {
                EndUpdate();
            }

        }

        //public void UpdateList(IEnumerable<MemoInfo> memoInfos) {
        //    UpdateList(memoInfos, false, true);
        //}

        public void InvalidateList(IEnumerable<MemoInfo> memoInfos) {
            Update();
            _disableEraseBackground = true;
            foreach (var info in memoInfos) {
                var index = Items.IndexOf(info);
                if (index > -1) {
                    var rect = GetItemRectangle(index);
                    Invalidate(rect);
                }
            }
            Update();
            _disableEraseBackground = false;
        }

        public void InvalidateWithoutEraceBackground() {
            Update();
            _disableEraseBackground = true;
            try {
                Invalidate(false);
                Update();
            } finally {
                _disableEraseBackground = false;
            }
        }

        public void EnsureVisible(int index) {
            ListBoxUtil.EnsureVisible(this, index);
        }

        public void ClosePreviewPopup() {
            _prevPreviewIndex = -1;
            DisposePreviewPopup();
            _enablePreview = false;
            _enablePreviewFollowMouseMove = false;
        }

        // --- preview ---
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.Space || keyData == (Keys.Space | Keys.Shift)) {
                var index = SelectedIndex;
                if (index == _prevPreviewIndex) {
                    if (keyData == Keys.Space && index < Items.Count - 1) {
                        ListBoxUtil.SelectNextItem(this);
                    } else if (keyData == (Keys.Space | Keys.Shift) && index > 0) {
                        ListBoxUtil.SelectPreviousItem(this);
                    } else {
                        ClosePreviewPopup();
                    }

                } else if (index > -1 && index < Items.Count) {
                    Cursor = Cursors.WaitCursor;
                    try {
                        ShowPreviewPopup(index, false);
                    } finally {
                        Cursor = Cursors.Default;
                    }
                }
                return true;
            }
            if (keyData == Keys.Escape || keyData == (Keys.G | Keys.Control)) {
                ClosePreviewPopup();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnSelectedIndexChanged(EventArgs e) {
            base.OnSelectedIndexChanged(e);
            SyncPreviewAndSelectedIndex();
        }

        private void SyncPreviewAndSelectedIndex() {
            if (!_enablePreview) {
                return;
            }

            var index = SelectedIndex;
            if (index != _prevPreviewIndex && index > -1 && index < Items.Count) {
                Cursor = Cursors.WaitCursor;
                try {
                    ShowPreviewPopup(index, false);
                } finally {
                    Cursor = Cursors.Default;
                }
            }
        }

        // --- draw ---
        /// <summary>
        /// info，memoに対してそれを表示するための情報を返す．
        /// タイトル行かどうか，文字，表示領域を格納したtuple返す．
        /// </summary>
        private IEnumerable<Tuple<MemoListBoxDisplayItem, string, Rectangle>> GetDisplayItemLines(
            Rectangle bounds, MemoInfo info, Memo memo
        ) {
            var ret = new List<Tuple<MemoListBoxDisplayItem, string, Rectangle>>();

            var imgRect = new Rectangle(
                new Point(bounds.Left + ItemPadding, bounds.Top + ItemPadding),
                _noteImage.Size
            );
            var textLeft = imgRect.Right + 3;

            var cTop = bounds.Top + ItemPadding;
            foreach (var vi in _displayItems) {
                var text = string.Empty;
                var rect = Rectangle.Empty;
                switch (vi) {
                    case MemoListBoxDisplayItem.Title: {
                        text = info.Title;
                        rect = new Rectangle(
                            textLeft,
                            cTop,
                            bounds.Width - (textLeft + ItemPadding),
                            (int) (_captionFont.GetHeight() + 0.5)
                        );
                        break;
                    }
                    case MemoListBoxDisplayItem.CreatedDate: {
                        text = "作成日時: " + memo.CreatedDate.ToString("yyyy/MM/dd HH:mm");
                        rect = new Rectangle(
                            textLeft,
                            cTop,
                            bounds.Width - (textLeft + ItemPadding),
                            (int) (_descriptionFont.GetHeight() + 0.5)
                        );
                        break;
                    }
                    case MemoListBoxDisplayItem.ModifiedDate: {
                        text = "更新日時: " + memo.ModifiedDate.ToString("yyyy/MM/dd HH:mm");
                        rect = new Rectangle(
                            textLeft,
                            cTop,
                            bounds.Width - (textLeft + ItemPadding),
                            (int) (_descriptionFont.GetHeight() + 0.5)
                        );
                        break;
                    }
                    case MemoListBoxDisplayItem.AccessedDate: {
                        text = "アクセス日時: " + memo.AccessedDate.ToString("yyyy/MM/dd HH:mm");
                        rect = new Rectangle(
                            textLeft,
                            cTop,
                            bounds.Width - (textLeft + ItemPadding),
                            (int) (_descriptionFont.GetHeight() + 0.5)
                        );
                        break;
                    }
                    case MemoListBoxDisplayItem.Tag: {
                        text = memo.TagsText;
                        rect = new Rectangle(
                            textLeft,
                            cTop,
                            bounds.Width - (textLeft + ItemPadding),
                            (int) (_descriptionFont.GetHeight() + 0.5)
                        );
                        break;
                    }
                    case MemoListBoxDisplayItem.SummaryText: {
                        text = StringUtil.GetHead(GetSummaryText(info), SummaryTextLineCount);
                        rect = new Rectangle(
                            ItemPadding,
                            cTop + 3,
                            bounds.Width - ItemPadding - ItemPadding,
                            (int) (_descriptionFont.GetHeight() * SummaryTextLineCount + 0.5)
                        );
                        break;
                    }
                }

                ret.Add(Tuple.Create(vi, text, rect));
                if (vi == MemoListBoxDisplayItem.Title) {
                    cTop += rect.Height + 3;
                } else {
                    cTop += rect.Height;
                }
            }

            return ret;
        }

        protected override void OnDrawItem(DrawItemEventArgs e) {
            base.OnDrawItem(e);

            /// ListBoxが空のときにListBoxが選択されるとe.Indexが-1
            /// デザイン時にe.Index == 0がくるのでItems.Countも調べておく
            if (e.Index < 0 || e.Index >= Items.Count || Items.Count < 1) {
                return;
            }

            /// 折りたたんだのを開くとき0のことがある
            if (e.Bounds.Width == 0 || e.Bounds.Height == 0) {
                return;
            }

            var index = e.Index;
            var item = Items[index];
            var info = item as MemoInfo;
            if (info == null) {
                return;
            }

            var container = _facade.Container;
            var memo = container.Find<Memo>(info.MemoId);
            if (memo == null) {
                return;
            }

            var bounds = new Rectangle(Point.Empty, e.Bounds.Size);
            if (bounds.IsEmpty) {
                return;
            }

            var memoIconRect = new Rectangle(
                new Point(ItemPadding, ItemPadding),
                _noteImage.Size
            );

            var previewIconRect = new Rectangle(
                new Point(bounds.Width - ItemPadding - _previewImage.Width, ItemPadding),
                _previewImage.Size
            );

            var marks = memo.Marks;
            var markCount = marks.Count < 3 ? marks.Count : 3;
            var markImages = new List<Image>();
            {
                for (int i = 0; i < markCount; ++i) {
                    var def = MemoMarkUtil.GetMarkDefinition(marks[i]);
                    markImages.Add(def.Image);
                }
            }
            var cLeft = bounds.Width - (ItemPadding * markCount + markImages.Sum(image => image.Width));
            var markIconsAndRects = new List<Tuple<Image, Rectangle>>();
            {
                for (int i = 0; i < markCount; ++i) {
                    var image = markImages[i];
                    var rect = new Rectangle(
                        new Point(cLeft, ItemPadding),
                        image.Size
                    );
                    markIconsAndRects.Add(Tuple.Create(image, rect));
                    cLeft += image.Width + ItemPadding;
                }
            }


            var currentContext = BufferedGraphicsManager.Current;

            var lines = GetDisplayItemLines(bounds, info, memo);

            if ((e.State & DrawItemState.Selected) != DrawItemState.None) {
                var borderColor = Focused ? Color.FromArgb(50, 150, 255) : Color.FromArgb(220, 220, 220);
                var borderInnerColor = Focused? Color.FromArgb(235, 245, 255): Color.FromArgb(250, 250, 250);
                var backColor = Focused ? Color.FromArgb(200, 230, 255) : Color.FromArgb(230, 230, 230);
                var backColor2 = Focused ? Color.FromArgb(220, 240, 255) : Color.FromArgb(245, 245, 245);

                using (var buf = currentContext.Allocate(e.Graphics, bounds))
                using (var borderPen = new Pen(borderColor))
                using (var borderInnerPen = new Pen(borderInnerColor))
                using (var backBrush = new LinearGradientBrush(bounds, backColor2, backColor, 90)) {
                    var g = buf.Graphics;

                    var activeFolder = _facade.ActiveFolder;
                    var isFocused = activeFolder == null || activeFolder.ContainingMemos.Contains(memo);

                    var rect = new Rectangle(Point.Empty, bounds.Size - new Size(1, 2)); /// 下の区切り線の幅1も含めて高さは2引く
                    g.FillRectangle(backBrush, bounds);
                    g.DrawRectangle(borderPen, rect);

                    rect.Inflate(-1, -1);
                    g.DrawRectangle(borderInnerPen, rect);

                    switch (memo.Importance) {
                        case MemoImportanceKind.Normal:
                            g.DrawImage(_noteImage, memoIconRect);
                            break;
                        case MemoImportanceKind.High:
                            g.DrawImage(_noteImportantImage, memoIconRect);
                            break;
                        case MemoImportanceKind.Low:
                            g.DrawImage(_noteUnimportantImage, memoIconRect);
                            break;
                    }

                    foreach (var line in lines) {
                        switch (line.Item1) {
                            case MemoListBoxDisplayItem.Title:
                                var fontColor = isFocused ? SystemColors.WindowText : SystemColors.ControlDark;
                                //var fontStyle = e.Index == _currentMousePointItemIndex ? FontStyle.Underline : FontStyle.Regular;
                                var fontStyle = FontStyle.Regular;
                                if (e.Index == _currentMousePointItemIndex) {
                                    var globalTitleRect = RectUtil.Translate(line.Item3, (Size) e.Bounds.Location);
                                    var globalIconRect = RectUtil.Translate(previewIconRect, (Size) e.Bounds.Location);
                                    if (globalTitleRect.Contains(_currentMouseLocation) && !globalIconRect.Contains(_currentMouseLocation)) {
                                        fontStyle = FontStyle.Underline;
                                    }
                                }
                                using (var font = new Font(e.Font, fontStyle)) {
                                    TextRenderer.DrawText(g, line.Item2, font, line.Item3, fontColor, Color.Transparent, _textFormatFlags);
                                }
                                break;
                            case MemoListBoxDisplayItem.CreatedDate:
                            case MemoListBoxDisplayItem.ModifiedDate:
                            case MemoListBoxDisplayItem.AccessedDate:
                            case MemoListBoxDisplayItem.Tag:
                                TextRenderer.DrawText(
                                    g, line.Item2, _descriptionFont, line.Item3, SystemColors.ControlDark, Color.Transparent, _textFormatFlags
                                );
                                break;
                            case MemoListBoxDisplayItem.SummaryText:
                                TextRenderer.DrawText(
                                    g, line.Item2, _descriptionFont, line.Item3, SystemColors.ControlDark, Color.Transparent, _summaryTextFormatFlags
                                );
                                break;
                        }
                    }

                    foreach (var markIconAndRect in markIconsAndRects) {
                        g.DrawImage(markIconAndRect.Item1, markIconAndRect.Item2);
                    }

                    if (e.Index == _currentMousePointItemIndex) {
                        g.DrawImage(_previewImage, previewIconRect);
                    }

                    g.DrawLine(Pens.Silver, bounds.Left + ItemPadding, bounds.Bottom - 1, bounds.Right - (ItemPadding * 2) + 1, bounds.Bottom - 1);

                    Gdi32Util.CopyGraphics(e.Graphics, e.Bounds, buf.Graphics, Point.Empty);
                }

            } else {
                //var backColor = e.Index % 2 == 0 ? e.BackColor : Color.FromArgb(245, 245, 245);
                var backColor = e.BackColor;

                using (var buf = currentContext.Allocate(e.Graphics, bounds))
                using (var backBrush = new SolidBrush(backColor)) {
                    var g = buf.Graphics;

                    var activeFolder = _facade.ActiveFolder;
                    var isFocused = activeFolder == null || activeFolder.ContainingMemos.Contains(memo);

                    var r = bounds;
                    r.Inflate(1, 1);
                    g.FillRectangle(backBrush, r);

                    switch (memo.Importance) {
                        case MemoImportanceKind.Normal:
                            g.DrawImage(_noteImage, memoIconRect);
                            break;
                        case MemoImportanceKind.High:
                            g.DrawImage(_noteImportantImage, memoIconRect);
                            break;
                        case MemoImportanceKind.Low:
                            g.DrawImage(_noteUnimportantImage, memoIconRect);
                            break;
                    }

                    foreach (var line in lines) {
                        switch (line.Item1) {
                            case MemoListBoxDisplayItem.Title:
                                var fontColor = isFocused ? e.ForeColor : SystemColors.ControlDark;
                                //var fontStyle = e.Index == _currentMousePointItemIndex ? FontStyle.Underline : FontStyle.Regular;
                                var fontStyle = FontStyle.Regular;
                                if (e.Index == _currentMousePointItemIndex) {
                                    var globalTitleRect = RectUtil.Translate(line.Item3, (Size) e.Bounds.Location);
                                    var globalIconRect = RectUtil.Translate(previewIconRect, (Size) e.Bounds.Location);
                                    if (globalTitleRect.Contains(_currentMouseLocation) && !globalIconRect.Contains(_currentMouseLocation)) {
                                        fontStyle = FontStyle.Underline;
                                    }
                                }
                                using (var font = new Font(e.Font, fontStyle)) {
                                    TextRenderer.DrawText(g, line.Item2, font, line.Item3, fontColor, Color.Transparent, _textFormatFlags);
                                }
                                break;
                            case MemoListBoxDisplayItem.CreatedDate:
                            case MemoListBoxDisplayItem.ModifiedDate:
                            case MemoListBoxDisplayItem.AccessedDate:
                            case MemoListBoxDisplayItem.Tag:
                                TextRenderer.DrawText(
                                    g, line.Item2, _descriptionFont, line.Item3, SystemColors.ControlDark, Color.Transparent, _textFormatFlags
                                );
                                break;
                            case MemoListBoxDisplayItem.SummaryText:
                                TextRenderer.DrawText(
                                    g, line.Item2, _descriptionFont, line.Item3, SystemColors.ControlDark, Color.Transparent, _summaryTextFormatFlags
                                );
                                break;
                        }
                    }

                    foreach (var markIconAndRect in markIconsAndRects) {
                        g.DrawImage(markIconAndRect.Item1, markIconAndRect.Item2);
                    }
                    if (e.Index == _currentMousePointItemIndex) {
                        g.DrawImage(_previewImage, previewIconRect);
                    }

                    g.DrawLine(Pens.Silver, bounds.Left + ItemPadding, bounds.Bottom - 1, bounds.Right - (ItemPadding * 2) + 1, bounds.Bottom - 1);

                    Gdi32Util.CopyGraphics(e.Graphics, e.Bounds, buf.Graphics, Point.Empty);
                }
            }
        }

        protected override void OnEnter(EventArgs e) {
            base.OnEnter(e);
            InvalidateWithoutEraceBackground();

            // メモリストで何も選択されていない状態で一番上以外の項目のタイトルをクリックすると
            // 一番上の項目のノートが表示されてしまう。
            // MouseUpで選択更新のためMouseClickされたタイミングではここで選択されたものが有効だから。
            //if (Items.Count > 0 && SelectedIndices.Count == 0) {
            //    SelectedIndex = TopIndex;
            //}
        }

        protected override void OnLeave(EventArgs e) {
            base.OnLeave(e);
            InvalidateWithoutEraceBackground();
        }

        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);
            ClosePreviewPopup();
        }

        //protected override void OnLostFocus(EventArgs e) {
        //    base.OnLostFocus(e);

        //    if (SelectedIndices.Count > 1) {
        //        /// こうしないと複数選択されていても最後に選択されたものしか再描画されない
        //        Update();
        //        _disableEraseBackground = true;
        //        try {
        //            foreach (int i in SelectedIndices) {
        //                var r = GetItemRectangle(i);
        //                Invalidate(r);
        //            }
        //            Update();
        //        } catch (IndexOutOfRangeException) {
        //            /// なぜかSelectedIndices.Countの範囲内にアクセスしても
        //            /// IndexOutOfRangeExceptionが起こることがあるのでつぶしておく
        //        } finally {
        //            _disableEraseBackground = false;
        //        }
        //    }

        //    ClosePreviewPopup();
        //}

        //protected override void OnGotFocus(EventArgs e) {
        //    base.OnGotFocus(e);

        //    if (SelectedIndices.Count > 1) {
        //        /// こうしないと複数選択されていても最後に選択されたものしか再描画されない

        //        Update();
        //        _disableEraseBackground = true;
        //        try {
        //            foreach (int i in SelectedIndices) {
        //                var r = GetItemRectangle(i);
        //                Invalidate(r);
        //            }
        //            Update();
        //        } finally {
        //            _disableEraseBackground = false;
        //        }
        //    }

        //    //if (SelectedIndices.Count == 0 && Items.Count > 0) {
        //    //    /// 何も選択されていなければ最初のものを選択
        //    //    SelectedIndex = 0;
        //    //}
        //}

        // --- sort ---
        protected override void Sort() {
            var arr = IListUtil.Cast<MemoInfo>(Items).ToArray();
            Array.Sort(arr, _comparer);
            for (int i = 0, len = arr.Length; i < len; ++i) {
                Items[i] = arr[i];
            }
        }

        // --- dnd ---
        private void ClearDragState() {
            _isDragPrepared = false;
            _dragStartPoint = Point.Empty;
            _dragStartRect = Rectangle.Empty;
        }

        private void SetUpDragState(Point pt) {
            _isDragPrepared = true;
            _isDragStarted = false;
            _dragStartPoint = pt;
            _dragStartRect = new Rectangle(
                _dragStartPoint.X - SystemInformation.DragSize.Width / 2,
                _dragStartPoint.Y - SystemInformation.DragSize.Height / 2,
                SystemInformation.DragSize.Width,
                SystemInformation.DragSize.Height
            );
        }

        protected override void WndProc(ref Message m) {
            if (m.Msg == (int) WindowMessage.LBUTTONDOWN && _canDragStart) {
                var wParam = (Int32) m.WParam;
                var isLButton = ((wParam) & 0x0001) == 0x0001;
                ///var isRButton = ((wParam) & 0x0002) == 0x0002;
                ///var isMButton = ((wParam) & 0x0010) == 0x0010;
                var isShiftPressed = ((wParam) & 0x0004) == 0x0004;
                var isControlPressed = ((wParam) & 0x0008) == 0x0008;

                if (isLButton && !isShiftPressed && !isControlPressed) {

                    var lParam = (Int32) m.LParam;
                    var pt = new Point(
                        (int) (lParam & 0xffff),
                        (int) (lParam >> 16)
                    );

                    var index = IndexFromPoint(pt);
                    if (index > -1) {
                        if (SelectedIndices.Contains(index)) {
                            /// MouseUpまで後回しにする
                            /// こうしないと複数選択のDnDができない
                            _isSelectionDeferred = true;
                            _deferredMessage = m;
                            SetUpDragState(pt);
                        } else {
                            /// 選択処理後DnDの準備
                            base.WndProc(ref m);
                            SetUpDragState(pt);
                        }
                        return; /// return

                    } else {
                        ClearDragState();
                    }
                } else {
                    ClearDragState();
                }
            } else if (m.Msg == (int) WindowMessage.ERASEBKGND && _disableEraseBackground) {
                /// Invalidate()でちらつきをなくす
                /// (preview iconなどのため)
                return;
            }

            base.WndProc(ref m);
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            if (!Focused) {
                Focus();
            }

            ClosePreviewPopup();

            if (e.Button == MouseButtons.Right) {
                var index = IndexFromPoint(e.X, e.Y);
                if (index > -1) {
                    /// 選択itemの更新
                    if (!SelectedIndices.Contains(index)) {
                        SelectedIndices.Clear();
                        SelectedIndex = index;
                    }
                }
                ClearDragState();
            }

        }

        protected override void OnMouseUp(MouseEventArgs e) {
            if (!_isDragStarted) {
                if (_isSelectionDeferred) {
                    var m = _deferredMessage;
                    base.WndProc(ref m);

                    if (e.Clicks == 1) {
                        OnMouseClick(e);
                    } else if (e.Clicks == 2) {
                        OnMouseDoubleClick(e);
                    }
                }
                base.OnMouseUp(e);

                _isDragStarted = false;
            }

            ClearDragState();

            _isSelectionDeferred = false;
            //if (_isSelectionDeferred) {

            //    /// クリックされたitem以外をSelectedIndicesから削除
            //    var oldSelecteds = new List<int>();
            //    for (int i = 0, len = SelectedIndices.Count; i < len; ++i) {
            //        oldSelecteds.Add(SelectedIndices[i]);
            //    }
            //    oldSelecteds.Reverse();

            //    var selectedIndex = IndexFromPoint(e.Location);
            //    foreach (var index in oldSelecteds) {
            //        if (index != selectedIndex) {
            //            SelectedIndices.Remove(index);
            //        }
            //    }

            //    SelectedIndex = selectedIndex;

            //    /// 標準のWndProcをやってないのでFocus()しておかないと選択領域が再描画されない
            //    InvalidateItem(selectedIndex);
                
            //    _isSelectionDeferred = false;
            //}
        }


        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            /// 下線+previewアイコン表示のため
            {
                _currentMouseLocation = e.Location;
    
                var cursor = Cursors.Default;

                if (Items.Count == 0) {
                    _currentMousePointItemIndex = -1;
                } else {
                    var prevItemIndex = _currentMousePointItemIndex;
                    _currentMousePointItemIndex = IndexFromPoint(e.Location);
                    //if (prevItemIndex != _currentMousePointItemIndex) {
                        Update();
                        _disableEraseBackground = true;
                        try {
                            if (prevItemIndex != -1) {
                                InvalidateItem(prevItemIndex);
                            }
                            if (_currentMousePointItemIndex != -1) {
                                InvalidateItem(_currentMousePointItemIndex);

                                var titleRect = GetTitleRect(_currentMousePointItemIndex);
                                if (titleRect.Contains(e.Location)) {
                                    var prevIconRect = GetPreviewIconRect(_currentMousePointItemIndex);
                                    if (!prevIconRect.Contains(e.Location)) {
                                        cursor = Cursors.Hand;
                                    }
                                }
                            }

                            Update();
                        } finally {
                            _disableEraseBackground = false;
                        }
                    //}
                }

                if (cursor != Cursor) {
                    Cursor = cursor;
                }
            }

            /// preview対象切り替え
            if (_enablePreview && _enablePreviewFollowMouseMove) {
                if (_currentMousePointItemIndex != _prevPreviewIndex) {
                    Cursor = Cursors.WaitCursor;
                    try {
                        ShowPreviewPopup(_currentMousePointItemIndex, true);
                    } finally {
                        Cursor = Cursors.Default;
                    }
                }
            }


            if (_isDragPrepared) {
                if (!_dragStartRect.Contains(e.X, e.Y)) {
                    _isDragStarted = true;
                    var indecies = SelectedIndices;
                    if (indecies.Count < 1) {
                        return;
                    }

                    var memoInfos = new MemoInfo[SelectedIndices.Count];
                    for (int i = 0, len = SelectedIndices.Count; i < len; ++i) {
                        memoInfos[i] = (MemoInfo) SelectedItems[i];
                    }

                    var effects = DoDragDrop(
                        memoInfos,
                        DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.Link
                    );

                    _isSelectionDeferred = false;
                    ClearDragState();
                }
            }
        }

        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
            base.OnQueryContinueDrag(e);

            /// マウスの右ボタンが押されていればドラッグをキャンセル
            if (DragDropUtil.IsRightButtonPressed(e)) {
                e.Action = DragAction.Cancel;
            }
        }

        protected override void OnMouseClick(MouseEventArgs e) {
            base.OnMouseClick(e);

            if (e.Button == MouseButtons.Left) {
                /// preview表示ON
                var index = IndexFromPoint(e.Location);
                if (index >= 0 && index < Items.Count) {
                    var previewIconRect = GetPreviewIconRect(index);
                    if (previewIconRect.Contains(e.Location)) {
                        Cursor = Cursors.WaitCursor;
                        try {
                            ShowPreviewPopup(index, true);
                        } finally {
                            Cursor = Cursors.Default;
                        }
                    } else {
                        var titleRect = GetTitleRect(index);
                        if (titleRect.Contains(e.Location)) {
                            OnMouseDoubleClick(e);
                        }
                    }
                }
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e) {
            ClosePreviewPopup();

            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);

            ClosePreviewPopup();
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);

            var prev = _currentMousePointItemIndex;
            _currentMousePointItemIndex = -1;

            Update();
            _disableEraseBackground = true;
            try {
                InvalidateItem(prev);
                Update();
            } finally {
                _disableEraseBackground = false;
            }

            if (_previewPopup != null && !_previewPopup.IsDisposed && _previewPopup.Visible) {
                var index = IndexFromPoint(PointToClient(MousePosition));
                if (index != SelectedIndex) {
                    ClosePreviewPopup();
                }
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private void UpdateItemHeight() {
            var height = ItemPadding + 1;

            if (_displayItems.Contains(MemoListBoxDisplayItem.Title)) {
                height += (int) (_captionFont.GetHeight() + 0.5) + 3;
            }
            if (_displayItems.Contains(MemoListBoxDisplayItem.CreatedDate)) {
                height += (int) (_descriptionFont.GetHeight() + 0.5);
            }
            if (_displayItems.Contains(MemoListBoxDisplayItem.ModifiedDate)) {
                height += (int) (_descriptionFont.GetHeight() + 0.5);
            }
            if (_displayItems.Contains(MemoListBoxDisplayItem.AccessedDate)) {
                height += (int) (_descriptionFont.GetHeight() + 0.5);
            }
            if (_displayItems.Contains(MemoListBoxDisplayItem.Tag)) {
                height += (int) (_descriptionFont.GetHeight() + 0.5);
            }
            if (_displayItems.Contains(MemoListBoxDisplayItem.SummaryText)) {
                height += 3 + (int) (Math.Ceiling(_descriptionFont.GetHeight()) * SummaryTextLineCount + 0.5);
            }

            ItemHeight = Math.Max(height + ItemPadding, _noteImage.Height);
        }


        private void ShowPreviewPopup(int index, bool followMouseMove) {
            if (index > -1 && index < Items.Count) {
                _enablePreview = true;
                _enablePreviewFollowMouseMove = followMouseMove;
                _prevPreviewIndex = index;

                var rect = GetItemRectangle(index);
                var info = Items[index] as MemoInfo;

                if (info != null) {

                    var bmp = default(Bitmap);
                    if (_facade.IsLoadedMemo(info)) {
                        /// Loadされていればそのcanvasからbitmapを作る
                        var content = _facade.FindPageContent(info);
                        bmp = MemoOutlineUtil.CreateOutline(content.EditorCanvas);

                    } else {
                        bmp = MemoOutlineUtil.LoadOrSaveAndLoadOutline(info);
                    }

                    DisposePreviewPopup();

                    var screen = Screen.FromControl(this);
                    var outline = new OutlinePreviewControl(screen, info.Title, bmp, 1f);
                    var rightTop = PointToScreen(new Point(rect.Right, rect.Top));
                    outline.MaxWidth = screen.WorkingArea.Width - rightTop.X - 2;
                    _previewPopup = new Popup(outline);
                    /// AutoClose = falseにしておかないとPopupにフォーカスを取られてしまう
                    _previewPopup.AutoClose = false;
                    _previewPopup.Tag = bmp;
                    _previewPopup.Show(this, new Point(rect.Right + 1, rect.Top));
                }
            }
        }

        private void DisposePreviewPopup() {
            if (_previewPopup != null) {
                var bmp = _previewPopup.Tag as Bitmap;
                if (bmp != null) {
                    bmp.Dispose();
                }
            }

            if (_previewPopup != null && !_previewPopup.IsDisposed) {
                _previewPopup.Close();
                _previewPopup.Dispose();
            }
        }

        private string GetSummaryText(MemoInfo info) {
            var ret = string.Empty;

            if (_facade.IsLoadedMemo(info)) {
                /// Loadされていればそのcanvasからbitmapを作る
                var content = _facade.FindPageContent(info);
                ret = MemoTextUtil.GetSummaryText(content.EditorCanvas);

            } else {
                ret = MemoTextUtil.LoadOrSaveAndLoadSummaryText(info);
            }

            return ret;
        }

        private Rectangle GetPreviewIconRect(int index) {
            if (index < 0 || index >= Items.Count) {
                return Rectangle.Empty;
            }

            var bounds = GetItemRectangle(index);
            return new Rectangle(
                new Point(bounds.Right - ItemPadding - _previewImage.Width, bounds.Top + ItemPadding),
                _previewImage.Size
            );
        }

        private Rectangle GetTitleRect(int index) {
            if (index < 0 || index >= Items.Count) {
                return Rectangle.Empty;
            }

            var bounds = GetItemRectangle(index);
            var textLeft = bounds.Left + ItemPadding + _noteImage.Width + 3;
            return new Rectangle(
                textLeft,
                bounds.Top + ItemPadding,     
                bounds.Width - (textLeft + ItemPadding),
                (int) (_captionFont.GetHeight() + 0.5)
            );
        }

        private void InvalidateItem(int index) {
            //var rect = GetPreviewIconRect(index);
            //if (!rect.IsEmpty) {
            //    Invalidate(rect, false);
            //}

            if (index < 0 || index >= Items.Count) {
                return;
            }

            Update();
            _disableEraseBackground = true;
            Invalidate(GetItemRectangle(index));
            Update();
            _disableEraseBackground = false;
        }


        // ========================================
        // class
        // ========================================
        private class MemoListBoxComparer: IComparer<MemoInfo> {
            // ========================================
            // field
            // ========================================
            private MemopadApplication _facade;

            private MemoListBoxDisplayItem _sortKey;
            private bool _isAscendingOrder;
            private bool _isOrderByImportance;

            // ========================================
            // constructor
            // ========================================
            public MemoListBoxComparer() {
                _facade = MemopadApplication.Instance;
                _sortKey = MemoListBoxDisplayItem.Title;
                _isAscendingOrder = true;
            }

            // ========================================
            // property
            // ========================================
            public MemoListBoxDisplayItem SortKey {
                get { return _sortKey; }
                set { _sortKey = value; }
            }

            public bool IsAscendingOrder {
                get { return _isAscendingOrder; }
                set { _isAscendingOrder = value; }
            }

            public bool IsOrderByImportance {
                get { return _isOrderByImportance; }
                set { _isOrderByImportance = value; }
            }

            // ========================================
            // method
            // ========================================
            public int Compare(MemoInfo x, MemoInfo y) {
                var ret = 0;
                if (_facade.ActiveFolder != null) {
                    var activeFolder = _facade.ActiveFolder;
                    var xMemo = _facade.Container.Find<Memo>(x.MemoId);
                    var yMemo = _facade.Container.Find<Memo>(y.MemoId);
                    var xContained = activeFolder.ContainingMemos.Contains(xMemo);
                    var yContained = activeFolder.ContainingMemos.Contains(yMemo);
                    if (xContained && !yContained) {
                        return -1;
                    } else if (!xContained && yContained) {
                        return 1;
                    }
                }

                if (_isOrderByImportance) {
                    var xMemo = _facade.Container.Find<Memo>(x.MemoId);
                    var yMemo = _facade.Container.Find<Memo>(y.MemoId);
                    var xImp = (int) xMemo.Importance;
                    var yImp = (int) yMemo.Importance;
                    if (xImp != yImp) {
                        return yImp - xImp;
                    }
                }

                switch (_sortKey) {
                    case MemoListBoxDisplayItem.Title: {
                        ret = CompareByTitle(x, y);
                        break;
                    }
                    case MemoListBoxDisplayItem.CreatedDate: {
                        ret = CompareByCreatedDate(x, y);
                        break;
                    }
                    case MemoListBoxDisplayItem.ModifiedDate: {
                        ret = CompareByModifiedDate(x, y);
                        break;
                    }
                    case MemoListBoxDisplayItem.AccessedDate: {
                        ret = CompareByAccessedDate(x, y);
                        break;
                    }
                    default: {
                        ret = CompareByTitle(x, y);
                        break;
                    }
                }
                return _isAscendingOrder? ret: -ret;
            }

            private int CompareByTitle(MemoInfo x, MemoInfo y) {
                if (string.IsNullOrEmpty(x.Title)) {
                    if (string.IsNullOrEmpty(y.Title)) {
                        return CompareByModifiedDate(x, y);
                    } else {
                        return -1;
                    }
                } else {
                    var ret = x.Title.CompareTo(y.Title);
                    if (ret != 0) {
                        return ret;
                    } else {
                        return CompareByModifiedDate(x, y);
                    }
                }
            }

            private int CompareByCreatedDate(MemoInfo x, MemoInfo y) {
                var container = _facade.Container;
                var xMemo = container.Find<Memo>(x.MemoId);
                var yMemo = container.Find<Memo>(y.MemoId);
                if (xMemo == null) {
                    if (yMemo == null) {
                        return 0;
                    } else {
                        return -1;
                    }
                } else {
                    if (yMemo == null) {
                        return 1;
                    } else {
                        return xMemo.CreatedDate.CompareTo(yMemo.CreatedDate);
                    }
                }
            }

            private int CompareByModifiedDate(MemoInfo x, MemoInfo y) {
                var container = _facade.Container;
                var xMemo = container.Find<Memo>(x.MemoId);
                var yMemo = container.Find<Memo>(y.MemoId);
                if (xMemo == null) {
                    if (yMemo == null) {
                        return 0;
                    } else {
                        return -1;
                    }
                } else {
                    if (yMemo == null) {
                        return 1;
                    } else {
                        return xMemo.ModifiedDate.CompareTo(yMemo.ModifiedDate);
                    }
                }
            }

            private int CompareByAccessedDate(MemoInfo x, MemoInfo y) {
                var container = _facade.Container;
                var xMemo = container.Find<Memo>(x.MemoId);
                var yMemo = container.Find<Memo>(y.MemoId);
                if (xMemo == null) {
                    if (yMemo == null) {
                        return 0;
                    } else {
                        return -1;
                    }
                } else {
                    if (yMemo == null) {
                        return 1;
                    } else {
                        return xMemo.AccessedDate.CompareTo(yMemo.AccessedDate);
                    }
                }
            }
        }

    }
}
