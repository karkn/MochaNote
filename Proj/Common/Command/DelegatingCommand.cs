/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Diagnostics;

namespace Mkamo.Common.Command {
    public class DelegatingCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Action _action;
        private Action _undoAction;
        private Action _redoAction;

        private Func<bool> _canExecuteFunc;

        // ========================================
        // constructor
        // ========================================
        /// <summary>
        /// actionにExecute()で呼ばれる処理，
        /// undoActionにUndo()で呼ばれる処理，
        /// redoActionにRedo()で呼ばれる処理，
        /// canExecuteFuncでCanExecuteで呼ばれる処理を渡す．
        /// 
        /// undoActionがnullならばundo不可なコマンドとなる．
        /// canExecuteFuncがnullなら常に実行可能なコマンドとなる．
        /// redoActionがnullならばredo時にactionが呼ばれる．
        /// </summary>
        public DelegatingCommand(Action action, Action undoAction, Action redoAction, Func<bool> canExecuteFunc) {
            Contract.Requires(action != null);

            _action = action;
            _undoAction = undoAction;
            _redoAction = redoAction;
            _canExecuteFunc = canExecuteFunc;
        }

        public DelegatingCommand(Action action, Action undoAction, Func<bool> canExecuteFunc)
            : this(action, undoAction, null, canExecuteFunc) { 
        }
        
        public DelegatingCommand(Action action, Action undoAction, Action redoAction)
            : this(action, undoAction, redoAction, null) { 
        }
        
        public DelegatingCommand(Action action, Action undoAction): this(action, undoAction, null, null) { 
        }

        public DelegatingCommand(Action action): this(action, null, null, null) { 
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _canExecuteFunc == null? true: _canExecuteFunc(); }
        }

        public override bool CanUndo {
            get { return _undoAction != null ;}
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            if (_action != null) {
                _action();
            }
        }

        public override void Undo() {
            if (_undoAction != null) {
                _undoAction();
            }
        }

        public override void Redo() {
            if (_redoAction != null) {
                _redoAction();
            } else {
                if (_action != null) {
                    _action();
                }
            }
        }
    }
}
