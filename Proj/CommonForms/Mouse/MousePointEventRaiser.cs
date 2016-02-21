/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Mkamo.Common.Forms.Mouse {
    public class MousePointEventProducer<TState>: IDisposable {
        // ========================================
        // field
        // ========================================
        private Timer _startTimer = new Timer();
        private Timer _endTimer = new Timer();

        private Size _hoverSize = SystemInformation.MouseHoverSize;
        private int _hoverTime = 1000;//SystemInformation.MouseHoverTime;
        private int _pointDelay = 7000;

        private Point _prevLocation = Point.Empty;
        private bool _inPointing = false;

        private TState _state = default(TState);
        private TState _defaultState = default(TState);

        private Func<Point, TState> _stateProvider = null;

        // ========================================
        // constructor
        // ========================================
        public MousePointEventProducer() {
            _startTimer.Enabled = false;
            _startTimer.Tick += HandleStartTimerTick;
            _startTimer.Interval = _hoverTime;

            _endTimer.Enabled = false;
            _endTimer.Tick += HandleEndTimerTick;
            _endTimer.Interval = _pointDelay;
        }

        // ========================================
        // destructor
        // ========================================
        public void Dispose() {
            _startTimer.Dispose();
            _endTimer.Dispose();
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler MousePointStarted;
        public event EventHandler MousePointEnded;

        // ========================================
        // property
        // ========================================
        public TState State {
            get { return _state; }
        }

        public TState DefaultState {
            get { return _defaultState; }
            set { _defaultState = value; }
        }

        public Func<Point, TState> StateProvider {
            get { return _stateProvider; }
            set { _stateProvider = value; }
        }

        // ========================================
        // method
        // ========================================
        public void HandleMouseMove(object sender, MouseEventArgs e) {
            if (IsMoved(e.Location, _prevLocation)) {
                _prevLocation = e.Location;
                if (_stateProvider != null) {
                    var newState = _stateProvider(e.Location);
                    if (EqualityComparer<TState>.Default.Equals(_state, newState)) {
                        return;
                    }
                    if (_inPointing) {
                        FinishPoint();
                    }
                    _state = newState;
                    if (EqualityComparer<TState>.Default.Equals(_defaultState, newState)) {
                        CancelPrepare();
                        return;
                    }
                }
                Prepare();
            }
        }

        public void HandleMouseLeave(object sender, EventArgs e) {
            Deactivate();
        }

        public void HandleMouseDown(object sender, MouseEventArgs e) {
            Deactivate();
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnMousePointStarted() {
            var handler = MousePointStarted;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }
        
        protected virtual void OnMousePointEnded() {
            var handler = MousePointEnded;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private void Deactivate() {
            if (_inPointing) {
                FinishPoint();
            }
            _startTimer.Enabled = false;
            _endTimer.Enabled = false;
        }

        private bool IsMoved(Point pt, Point prevPt) {
            if (!_hoverSize.IsEmpty && !prevPt.IsEmpty) {
                var x = _hoverSize.Width / 2;
                var y = _hoverSize.Height / 2;
                var containsX = pt.X >= prevPt.X - x && pt.X <= prevPt.X + x;
                var containsY = pt.Y >= prevPt.Y - y && pt.Y <= prevPt.Y + y;
                if (containsX && containsY) {
                    return false;
                }
            }
            return true;
        }

        private void Prepare() {
            _startTimer.Enabled = true;
        }

        private void CancelPrepare() {
            _startTimer.Enabled = false;
        }

        private void BeginPoint() {
            _inPointing = true;
            _startTimer.Enabled = false;
            OnMousePointStarted();
            _endTimer.Enabled = true;
        }

        private void FinishPoint() {
            _inPointing = false;
            _endTimer.Enabled = false;
            OnMousePointEnded();
        }

        private void HandleStartTimerTick(object sender, EventArgs e) {
            BeginPoint();
        }

        private void HandleEndTimerTick(object sender, EventArgs e) {
            FinishPoint();
        }
    }
}
