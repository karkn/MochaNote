/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Mkamo.Common.Structure;
using Mkamo.Common.Util;
using Mkamo.Common.Event;
using Mkamo.Common.Forms.MouseOperatable;
using System.Windows.Forms;
using Mkamo.Figure.Figures;
using Mkamo.Common.Externalize;
using Mkamo.Common.Visitor;

namespace Mkamo.Figure.Core {
    public interface IFigure:
        IStructured<IFigure>, IVisitable<IFigure>, IMouseOperatable, IKeyOperatable, IExternalizable
    {
        // ========================================
        // event
        // ========================================
        event EventHandler<DetailedPropertyChangedEventArgs> ParentChanged;
        event EventHandler<DetailedPropertyChangedEventArgs> ChildrenChanged;
        event EventHandler<DetailedPropertyChangedEventArgs> DescendantChanged;

        event EventHandler<BoundsChangedEventArgs> BoundsChanged;
        event EventHandler<EventArgs> VisibleChanged;
        event EventHandler<EventArgs> LayoutDone;

        event EventHandler<MouseEventArgs> MouseClick;
        event EventHandler<MouseEventArgs> MouseDoubleClick;
        event EventHandler<MouseEventArgs> MouseTripleClick;
        event EventHandler<MouseEventArgs> MouseDown;
        event EventHandler<MouseEventArgs> MouseUp;
        event EventHandler<MouseEventArgs> MouseMove;
        event EventHandler<EventArgs> MouseEnter;
        event EventHandler<EventArgs> MouseLeave;
        event EventHandler<MouseHoverEventArgs> MouseHover;
        event EventHandler<MouseEventArgs> DragStart;
        event EventHandler<MouseEventArgs> DragMove;
        event EventHandler<MouseEventArgs> DragFinish;
        event EventHandler<EventArgs> DragCancel;

        event EventHandler<ShortcutKeyProcessEventArgs> ShortcutKeyProcess;
        event EventHandler<KeyEventArgs> KeyDown;
        event EventHandler<KeyEventArgs> KeyUp;
        event EventHandler<KeyPressEventArgs> KeyPress;
        event EventHandler<PreviewKeyDownEventArgs> PreviewKeyDown;


        // ========================================
        // property
        // ========================================
        // --- bounds ---
        Rectangle Bounds { get; set; }
        Point Location { get; set; }
        Size Size { get; set; }
        int Left { get; set; }
        int Top { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        int Right { get; }
        int Bottom { get; }
        Point Center { get; }

        /// <summary>
        /// 描画に使用する矩形．
        /// </summary>
        Rectangle PaintBounds { get; }

        /// <summary>
        /// Measure()によって測定された最適サイズを返す．
        /// </summary>
        Size PreferredSize { get; }

        // --- structure ---
        RootFigure Root { get; }

        // --- layout ---
        ILayout Layout { get; set; }
        IDictionary<IFigure, object> LayoutConstraints { get; }

        // --- misc ---
        bool IsVisible { get; set; }

        IDirtManager DirtManager { get; }

        /// <summary>
        /// Mementoには保存しない拡張データ．
        /// </summary>
        IDictionary<string, object> TransientData { get; }

        /// <summary>
        /// Mementoに保存する拡張データ．
        /// </summary>
        IDictionary<string, object> PersistentData { get; }

        // --- cursor ---
        Func<MouseEventArgs, Cursor> CursorProvider { get; set; }

        // ========================================
        // method
        // ========================================
        // --- bounds ---
        bool ContainsPoint(Point pt);
        bool IntersectsWith(Rectangle rect);
        void Move(Size delta);
        void Move(Size delta, IEnumerable<IFigure> movingFigures);

        // --- paint ---
        void Paint(Graphics g);

        /// <summary>
        /// 現在viewport内に表示されている矩形．
        /// 全く表示されていない場合はRectangle.Emptyを返す．
        /// </summary>
        Rectangle GetVisibleBounds();

        // --- text ---
        Size MeasureText(Graphics g, string text, Font font, int proposedWidth);
        Size MeasureText(Graphics g, string text, Font font, int clipWidth, out int drawableLen);

        // --- layout ---
        /// <summary>
        /// constraintに収まるようなPreferredSizeを計測する．
        /// </summary>
        void Measure(SizeConstraint constraint);

        /// <summary>
        /// 子Figureを配置する．
        /// </summary>
        void Arrange();

        /// <summary>
        /// childにlayoutに使用する制約を設定する．
        /// childはChildrenに含まれているIFigureでなければならない．
        /// </summary>
        void SetLayoutConstraint(IFigure child, object constraint);
        object GetLayoutConstraint(IFigure child);

        // --- cloning ---
        /// <summary>
        /// このfigureの複製を作る．子も含む．
        /// </summary>
        IFigure CloneFigure();

        /// <summary>
        /// このfigureの複製を作る．子は含まない．
        /// </summary>
        IFigure CloneFigureOnly();

        /// <summary>
        /// このfigureの複製を作る．
        /// </summary>
        IFigure CloneFigure(ExternalizableFilter filter);

        // --- invalidation ---
        /// <summary>
        /// 自分の矩形の領域の表示をInvalidateする．
        /// </summary>
        void InvalidatePaint();

        /// <summary>
        /// 自分の配下のfigureのレイアウトをInvalidateする．
        /// </summary>
        void InvalidateLayout();

        // --- find ---
        /// <summary>
        /// finderに適合するIFigureを返す．
        /// 親子構造の葉の方から逆順で探す．
        /// stopOnThisNotFinderTargetがtrueの場合，
        /// このIFigure自体がfinderに適合しなければ子IFigureの走査をしない．
        /// </summary>
        IFigure FindFigure(Predicate<IFigure> finder, bool stopOnThisNotFinderTarget);

        /// <summary>
        /// finderに適合するIFigureを返す．
        /// 親子構造の葉の方から逆順で探す．
        /// </summary>
        IFigure FindFigure(Predicate<IFigure> finder);

        IList<IFigure> FindFigures(Predicate<IFigure> finder, bool stopOnThisNotFinderTarget);
        IList<IFigure> FindFigures(Predicate<IFigure> finder);

        // --- z-order ---
        void BringToBefore(IFigure before);
        void BringToAfter(IFigure after);
        void BringToFront(int step);
        void BringToBack(int step);
        void BringToFrontMost();
        void BringToBackMost();

        // --- util ---
        void MakeTransparent(float ratio);
        /// <summary>
        /// マウスイベントを転送するようにmouseOperatableを登録する
        /// </summary>
        void ForwardMouseEvents(IMouseOperatable mouseOperatable);
        /// <summary>
        /// マウスイベント転送のmouseOperatableの登録を解除する
        /// </summary>
        void StopForwardMouseEvents(IMouseOperatable mouseOperabable);
    }
}
