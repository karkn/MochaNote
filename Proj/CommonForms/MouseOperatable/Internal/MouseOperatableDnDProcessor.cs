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

namespace Mkamo.Common.Forms.MouseOperatable.Internal {
    internal class MouseOperatableDnDProcessor {
        // ========================================
        // field
        // ========================================
        private bool _isPrepared;
        private bool _isStarted;
        private bool _isPaused;
        private bool _isLastDnDCanceled;
        private IMouseOperatable _target;
        private MouseEventArgs _dragStartMouseEventArgs;
        //private long _sessionId;

        // ========================================
        // constructor
        // ========================================
        public MouseOperatableDnDProcessor() {
            ClearDnDState();
            _isLastDnDCanceled = false;
            //_sessionId = long.MinValue;
        }

        // ========================================
        // property
        // ========================================
        public bool IsPrepared {
            get { return _isPrepared; }
        }

        public bool IsStarted {
            get { return _isStarted; }
        }

        public bool IsPaused {
            get { return _isPaused; }
        }

        public bool IsLastDndCanceled {
            get { return _isLastDnDCanceled; }
        }

        internal IMouseOperatable _Target {
            get { return _target; }
            set { _target = value; }
        }

        //public long SessionId {
        //    get { return _sessionId; }
        //}

        // ========================================
        // method
        // ========================================
        public void PrepareDnD(IMouseOperatable target, MouseEventArgs e) {
            Size dragSize = SystemInformation.DragSize;
            _target = target;
            _isPrepared = true;
            _dragStartMouseEventArgs = e;

            //if (_sessionId < long.MaxValue) {
            //    ++_sessionId;
            //} else {
            //    _sessionId = long.MinValue;
            //}
        }

        public bool ShouldStartDnD(Point pt) {
            return _isPrepared && _target != null;
        }

        public void StartDnD(MouseEventArgs e) {
            if (!_isStarted) {
                _target.HandleDragStart(_dragStartMouseEventArgs);
                _isStarted = true;
                _isPaused = false;
            }
            _target.HandleDragMove(e);
        }

        public void ProcessDnD(MouseEventArgs e) {
            if (_isStarted && !_isPaused && _target != null) {
                _target.HandleDragMove(e);
            }
        }

        public void PauseDnD() {
            if (!_isPaused && _isStarted && _target != null) {
                _target.HandleDragCancel();
                //_target.HandleDragPause(EventArgs.Empty);
                _isPaused = true;
            }
        }

        public void RestartDnD() {
            if (_isPaused && _isStarted && _target != null) {
                _target.HandleDragStart(_dragStartMouseEventArgs);
                //_target.HandleDragRestart(_dragStartMouseEventArgs);
                _isPaused = false;
            }
        }

        public void FinishDnD(MouseEventArgs e) {
            if (_isStarted && _target != null) {
                _target.HandleDragFinish(e);
            }
            ClearDnDState();
            _isLastDnDCanceled = false;
        }

        public void CancelDnD(EventArgs e) {
            if (_isStarted && _target != null) {
                _target.HandleDragCancel();
            }
            ClearDnDState();
            _isLastDnDCanceled = true;
        }

        public void ClearDnDState() {
            _isPrepared = false;
            _isStarted = false;
            _isPaused = false;
            _target = null;
            _dragStartMouseEventArgs = null;
        }

    }
}
