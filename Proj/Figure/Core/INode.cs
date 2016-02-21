/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.DataType;
using System.Drawing.Drawing2D;
using Mkamo.StyledText.Core;

namespace Mkamo.Figure.Core {
    using StyledText = Mkamo.StyledText.Core.StyledText;
using Mkamo.Common.Command;

    /// <summary>
    /// 矩形で表現される図形．
    /// </summary>
    public interface INode: IConnectable {
        // ========================================
        // event
        // ========================================
        event EventHandler MaxSizeChanged;

        // ========================================
        // property
        // ========================================
        // --- appearance---
        Color Foreground { get; set; }
        IBrushDescription Background { get; set; }
        int BorderWidth { get; set; }
        DashStyle BorderDashStyle { get; set; }

        bool IsForegroundEnabled { get; set; }
        bool IsBackgroundEnabled { get; set; }

        // --- bounds ---
        Size MinSize { get; set; }
        Size MaxSize { get; set; }
        AutoSizeKinds AutoSizeKinds { get; set; }

        Insets Padding { get; set; }
        Rectangle ClientArea { get; }

        Line LeftLine { get; }
        Line RightLine { get; }
        Line TopLine { get; }
        Line BottomLine { get; }

        INodeOuterFrame OuterFrame { get; }

        // --- styled text ---
        StyledText StyledText { get; set; }
        Size StyledTextSize { get; }
        Rectangle StyledTextBounds { get; }
        Range Selection { get; set; }
        Color SelectionBorderColor { get; set; }
        IBrushDescription SelectionBrush { get; set; }

        bool ShowLineBreak { get; set; }
        bool ShowBlockBreak { get; set; }

        // --- plain text ---
        FontDescription Font { get; set; }
        Color FontColor { get; set; }
        string Text { get; set; }
        Size TextSize { get; }
        HorizontalAlignment TextHorizontalAlignment { get; set; }
        VerticalAlignment TextVerticalAlignment { get; set; }

        // ========================================
        // method
        // ========================================
        // --- size ---
        Size MeasureAutoSize(Size expected);
        void AdjustSize();

        // --- update ---
        UpdateStyledTextContext BeginUpdateStyledText(Action pre, Action post);
        UpdateStyledTextContext BeginUpdateStyledText(Action pre);
        UpdateStyledTextContext BeginUpdateStyledText();
        void EndUpdateStyledText(UpdateStyledTextContext context);

        void DirtyAllVisLines();
        void DirtyAllVisLinesAfter(Block block);
        void DirtySizeAndVisLine(Block block);
        void DirtyAllBounds();
        void DirtyAllBoundsAfter(Block block);
        void DirtyAllBoundsBefore(Block block);
        

        // --- styled text ---
        int GetCharIndexAt(Point location);
        Block GetBlockAt(Graphics g, Point location);
        LineSegment GetLineSegmentAt(Graphics g, Point location);
        Inline GetInlineAt(Point location);

        Point? GetConnectionPoint(object option);

        /// <summary>
        /// Text，StyledTextの矩形を含めたBounds．
        /// </summary>
        Rectangle GetStyledTextBoundsFor(Graphics g, Rectangle bounds);
        Rectangle GetStyledTextBoundsFor(Rectangle bounds);
        Rectangle GetBoundsWithText();
        Rectangle GetBlockBounds(Graphics g, int blockIndex);
        Rectangle GetBlockBounds(Graphics g, Block block);
        Rectangle GetLineSegmentBounds(Graphics g, LineSegment line);
        Rectangle GetCharRect(int charIndex);
        Rectangle[] GetStringRect(int charIndex, int length);
        Size GetBlockSize(Block block);
        Size GetLineSegmentSize(Graphics g, LineSegment line);
        Size GetInlineSize(Graphics g, Inline inline);

        Rectangle GetVisualLineBounds(int charIndex);
        Range GetVisualLineRange(int charIndex);
        bool IsVisualLineHead(int charIndex);
        bool IsVisualLineEnd(Graphics g, int charIndex);
        /// <summary>
        /// charIndexを含むVisualLineが最初の行かどうかを返す．
        /// </summary>
        bool IsFirstVisualLine(int charIndex);
        /// <summary>
        /// charIndexを含むVisualLineが最後の行かどうかを返す．
        /// </summary>
        bool IsLastVisualLine(int charIndex);

        /// <summary>
        /// blockの行頭文字の表示矩形を返す。
        /// </summary>
        Rectangle GetBulletRect(Graphics g, Block block);

        bool IsInSelection(Point location);
        bool IsInBullet(Point location);
        bool IsInBullet(Point loc, ListKind listKind);
        ICommand GetProcessCheckBoxBulletCommand(Point location);
    }

    public interface INodeOuterFrame {
        bool IntersectsWith(Line line);
        
        /// <summary>
        /// 外枠との交点を求める．
        /// 複数の交点がある場合，line.Startに最も近い点を返す．
        /// </summary>
        Point GetIntersectionPoint(Line line);

        /// <summary>
        /// ptに最も近い外枠上点を返す．
        /// </summary>
        Point GetNearestPoint(Point pt);
    }

    public class UpdateStyledTextContext: IDisposable {
        private INode _owner;
        private Action _action;
        internal UpdateStyledTextContext(INode owner, Action action) {
            _owner = owner;
            _action = action;
        }
        public void Dispose() {
            _owner.EndUpdateStyledText(this);
        }
        public Action Action {
            get { return _action; }
        }
    }

    [Flags, Serializable]
    public enum AutoSizeKinds {
        None = 0x00,
        GrowWidth = 0x01,
        ReduceWidth = 0x02,
        GrowHeight = 0x04,
        ReduceHeight = 0x08,
        FitWidth = GrowWidth | ReduceWidth,
        FitHeight = GrowHeight | ReduceHeight,
        GrowBoth = GrowWidth | GrowHeight,
        ReduceBoth = ReduceWidth | ReduceHeight,
        FitBoth = GrowBoth | ReduceBoth,
    }
}
