/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Command {
    public class CommandExecutor: ICommandExecutor {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(global::System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private Stack<ICommand> _undoStack = new Stack<ICommand>();
        private Stack<ICommand> _redoStack = new Stack<ICommand>();

        private int _chainDepth = 0;
        private ICommand _chainedCommand = null;

        // ========================================
        // constructor
        // ========================================
        public CommandExecutor() {
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<CommandEventArgs> CommandExecuted;
        public event EventHandler<CommandEventArgs> CommandUndone;
        public event EventHandler<CommandEventArgs> CommandRedone;
        public event EventHandler<CommandEventArgs> CommandChainEnded;

        // ========================================
        // property
        // ========================================
        public bool CanUndo {
            get { return _undoStack.Count > 0 && _undoStack.Peek().CanUndo; }
        }

        public bool CanRedo {
            get { return _redoStack.Count > 0 && _redoStack.Peek().CanExecute; }
        }

        public bool InChain {
            get { return _chainDepth > 0; }
        }

        // ========================================
        // method
        // ========================================
        public void Execute(ICommand command) {
            if (command == null || !command.CanExecute) {
                return;
            }

            try {
                command.Execute();
                if (Logger.IsDebugEnabled) {
                    Logger.Debug("command executed " + command.ToString());
                }

                if (InChain) {
                    if (command.CanUndo) {
                        _chainedCommand = _chainedCommand == null ? command : _chainedCommand.Chain(command);
                    }
                } else {
                    PushToUndo(command);
                }

                OnCommandExecuted(command);
            } catch (Exception e) {
                Logger.Warn("command execution failure: " + command.ToString(), e);
            }
        }

        public ICommand Undo() {
            if (!CanUndo) {
                return null;
            }
            if (InChain) {
                Logger.Warn("Undo invoked in chain");
                throw new InvalidOperationException("in chain");
            }

            var undo = _undoStack.Pop();
            try {
                undo.Undo();
                _redoStack.Push(undo);
                OnCommandUndone(undo);
                return undo;
            } catch (Exception e) {
                Logger.Warn("command undo failure: " + undo.ToString(), e);
                Clear();
                return null;
            }
        }

        public ICommand Redo() {
            if (!CanRedo) {
                return null;
            }
            if (InChain) {
                Logger.Warn("Redo invoked in chain");
                throw new InvalidOperationException("in chain");
            }

            var redo = _redoStack.Pop();
            try {
                redo.Redo();
                _undoStack.Push(redo);
                OnCommandRedone(redo);
                return redo;
            } catch (Exception e) {
                Logger.Warn("command redo failure: " + redo.ToString(), e);
                Clear();
                return null;
            }
        }

        public void Clear() {
            _undoStack.Clear();
            _redoStack.Clear();
        }

        public CommandChainContext BeginChain() {
            ++_chainDepth;
            return new CommandChainContext(this);
        }

        public void EndChain() {
            if (_chainDepth > 0) {
                --_chainDepth;

                if (_chainDepth == 0) {
                    if (_chainedCommand != null) {
                        PushToUndo(_chainedCommand);
                        OnCommandChainEnded(_chainedCommand);
                        if (Logger.IsDebugEnabled) {
                            Logger.Debug("command chain ended: " + _chainedCommand.ToString());
                        }
                        _chainedCommand = null;
                    }
                }
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnCommandExecuted(ICommand command) {
            var handler = CommandExecuted;
            if (handler != null) {
                handler(this, new CommandEventArgs(command));
            }
        }

        protected virtual void OnCommandUndone(ICommand command) {
            var handler = CommandUndone;
            if (handler != null) {
                handler(this, new CommandEventArgs(command));
            }
        }

        protected virtual void OnCommandRedone(ICommand command) {
            var handler = CommandRedone;
            if (handler != null) {
                handler(this, new CommandEventArgs(command));
            }
        }

        protected virtual void OnCommandChainEnded(ICommand command) {
            var handler = CommandChainEnded;
            if (handler != null) {
                handler(this, new CommandEventArgs(command));
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private void PushToUndo(ICommand command) {
            if (command.CanUndo) {
                _redoStack.Clear();
                if (_undoStack.Count > 0) {
                    var prev = _undoStack.Peek();
                    var compo = prev as CompositeCommand;
                    if (compo != null) {
                        if (compo.Children.Count > 0) {
                            prev = compo.Children.Last();
                            // todo: CompositeCommandが3階層以上あったときこれでは
                            // 一番最後のleafになるcommandをとれないのを直す
                        }
                    }
                    if (prev.ShouldMerge(command)) {
                        var top = _undoStack.Pop();
                        _undoStack.Push(top.Chain(command));
                        if (Logger.IsDebugEnabled) {
                            Logger.Debug("command merged " + command.ToString());
                        }
                    } else {
                        _undoStack.Push(command);
                        if (Logger.IsDebugEnabled) {
                            Logger.Debug("command pushed to undo " + command.ToString());
                        }
                    }

                } else {
                    _undoStack.Push(command);
                    if (Logger.IsDebugEnabled) {
                        Logger.Debug("command pushed to undo " + command.ToString());
                    }
                }
            }
        }

    }
}
