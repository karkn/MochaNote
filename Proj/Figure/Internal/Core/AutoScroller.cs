/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Mkamo.Figure.Core;

namespace Mkamo.Figure.Internal.Core {
    internal class AutoScroller {

        private Timer _autoScrollTimer;

        private int _enableAutoScrollWidth;
        private int _scrollInterval;
        private ScrollDirection _scrollDirection;

        Canvas _canvas;

        // ========================================
        // constructor
        // ========================================
        public AutoScroller(Canvas canvas) {
            _autoScrollTimer = new Timer();
            _autoScrollTimer.Enabled = false;
            _autoScrollTimer.Interval = 200;
            _autoScrollTimer.Tick += HandleAutoScrollTimerTick;

            _enableAutoScrollWidth = 12;
            _scrollInterval = 32;
            _scrollDirection = ScrollDirection.None;

            _canvas = canvas;
        }

        // ========================================
        // property
        // ========================================
        public int EnableAutoScrollWidth {
            get { return _enableAutoScrollWidth; }
            set { _enableAutoScrollWidth = value; }
        }

        public int ScrollInterval {
            get { return _scrollInterval; }
            set { _scrollInterval = value; }
        }

        public bool IsInScrolling {
            get { return _autoScrollTimer.Enabled && _scrollDirection != ScrollDirection.None; }
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<AutoScrollEventArgs> AutoScrolling;
        public event EventHandler<AutoScrollEventArgs> AutoScrolled;

        // ========================================
        // method
        // ========================================
        public ScrollDirection GetPreferredScrollDirection(Point pt) {
            Size clientRectSize = _canvas.ClientRectangle.Size;

            // 上
            if (pt.X >= 0 && pt.X < clientRectSize.Width && pt.Y >= 0 && pt.Y < _enableAutoScrollWidth) {
                return ScrollDirection.Up;
            }

            // 下
            if (pt.X >= 0 && pt.X < clientRectSize.Width &&
                pt.Y >= clientRectSize.Height - _enableAutoScrollWidth && pt.Y < clientRectSize.Height
            ) {
                return ScrollDirection.Down;
            }

            // 左
            if (pt.X >= 0 && pt.X < _enableAutoScrollWidth && pt.Y >= 0 && pt.Y < clientRectSize.Height) {
                return ScrollDirection.Left;
            }

            // 右
            if (pt.X >= clientRectSize.Width - _enableAutoScrollWidth && pt.X < clientRectSize.Width &&
                pt.Y >= 0 && pt.Y < clientRectSize.Height) {
                return ScrollDirection.Right;
            }

            // 中止
            return ScrollDirection.None;
        }

        public void StartAutoScroll(ScrollDirection direction) {
            if (direction == ScrollDirection.None) {
                StopAutoScroll();
            } else {
                _scrollDirection = direction;
                _autoScrollTimer.Start();
            }
        }

        public void StopAutoScroll() {
            _scrollDirection = ScrollDirection.None;
            _autoScrollTimer.Stop();
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnAutoScrolling() {
            var handler = AutoScrolling;
            if (handler != null) {
                handler(this, new AutoScrollEventArgs(_scrollDirection, _scrollInterval));
            }
        }

        protected virtual void OnAutoScrolled() {
            var handler = AutoScrolled;
            if (handler != null) {
                handler(this, new AutoScrollEventArgs(_scrollDirection, _scrollInterval));
            }
        }

        protected void HandleAutoScrollTimerTick(object sender, EventArgs e) {
            switch (_scrollDirection) {
                case ScrollDirection.Up: {
                    OnAutoScrolling();
                    _canvas.AutoScrollPosition = new Point(
                        - _canvas.AutoScrollPosition.X,
                        - _canvas.AutoScrollPosition.Y - _scrollInterval
                    );
                    OnAutoScrolled();
                    break;
                }
                case ScrollDirection.Down: {
                    OnAutoScrolling();
                    _canvas.AutoScrollPosition = new Point(
                        - _canvas.AutoScrollPosition.X,
                        - _canvas.AutoScrollPosition.Y + _scrollInterval
                    );
                    OnAutoScrolled();
                    break;
                }
                case ScrollDirection.Left: {
                    OnAutoScrolling();
                    _canvas.AutoScrollPosition = new Point(
                        -_canvas.AutoScrollPosition.X - _scrollInterval,
                        -_canvas.AutoScrollPosition.Y
                    );
                    OnAutoScrolled();
                    break;
                }
                case ScrollDirection.Right: {
                    OnAutoScrolling();
                    _canvas.AutoScrollPosition = new Point(
                        - _canvas.AutoScrollPosition.X + _scrollInterval,
                        - _canvas.AutoScrollPosition.Y
                    );
                    OnAutoScrolled();
                    break;
                }
                case ScrollDirection.None: {
                    StopAutoScroll();
                    break;
                }
            }
        }
    }

}
