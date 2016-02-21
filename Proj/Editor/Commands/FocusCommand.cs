/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Core;
using Mkamo.Common.Command;
using System.Drawing;

namespace Mkamo.Editor.Commands {
    using Editor = Mkamo.Editor.Internal.Editors.Editor;

    public class FocusCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private FocusKind _value;
        private Point? _location;

        private FocusInitializer _initializer;
        private FocusCommiter _commiter;

        private FocusCommitResultKind _resultKind;
        private FocusUndoer _undoer;
        private object _commitedValue;

        // ========================================
        // constructor
        // ========================================
        /// <summary>
        /// begin focus
        /// </summary>
        public FocusCommand(IEditor target, Point? location, FocusInitializer initializer) {
            _target = target;
            _value = FocusKind.Begin;
            _location = location;
            _initializer = initializer;
        }

        /// <summary>
        /// commit focus
        /// </summary>
        public FocusCommand(IEditor target, FocusCommiter commiter) {
            _target = target;
            _value = FocusKind.Commit;
            _commiter = commiter;
        }

        /// <summary>
        /// rollback focus
        /// </summary>
        public FocusCommand(IEditor target) {
            _target = target;
            _value = FocusKind.Rollback;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null; }
        }

        public override bool CanUndo {
            get { return _value == FocusKind.Commit && _resultKind == FocusCommitResultKind.Commited; }
        }

        public IEditor Target {
            get { return _target; }
        }

        public FocusKind Value {
            get { return _value; }
        }

        public Point? Location {
            get { return _location; }
        }

        public FocusCommitResultKind ResultKind {
            get { return _resultKind; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            /// 明示的にUnfocusされていないEditorがある場合はRollback()しておく
            // RequestCommit(true)の方がいい
            var oldFocused = _target.Site.FocusManager.FocusedEditor as Editor;
            if (oldFocused != null && oldFocused != _target) {
                Rollback(oldFocused);
            }

            var editor = _target as Editor;
            if (_target != null) {
                switch (_value) {
                    case FocusKind.Begin: {
                        _target.Focus.Value = _initializer(_target.Focus, _target.Model);
                        Begin(_target, _location);
                        break;
                    }
                    case FocusKind.Commit: {
                        _commitedValue = _target.Focus.Value;
                        var isCancelled = false;
                        _undoer = _commiter(_target.Focus, _target.Model, _target.Focus.Value, false, out isCancelled);
                        if (isCancelled) {
                            _resultKind = FocusCommitResultKind.Canceled;
                        } else {
                            /// nullが返ったらundo不可(必要なし)
                            if (_undoer == null) {
                                _resultKind = FocusCommitResultKind.Noop;
                                Rollback(_target);
                            } else {
                                _resultKind = FocusCommitResultKind.Commited;
                                Commit(_target);
                            }
                        }

                        break;
                    }
                    case FocusKind.Rollback: {
                        Rollback(_target);
                        break;
                    }
                }
            }
        }

        public override void Redo() {
            var target = _target;

            /// CompositeCommand内で前がCreateNodeCommandならその結果をターゲットにする
            /// MemoTextの動作のために必要
            if (_Parent != null) {
                var index = _Parent.Children.IndexOf(this);
                if (index > 0) {
                    var prev = _Parent.Children[index - 1] as CreateNodeCommand;
                    if (prev != null) {
                        target = prev.CreatedEditor;
                    }
                }
            }
            var isCanceled = false;
            _commiter(target.Focus, target.Model, _commitedValue, true, out isCanceled);
        }

        public override void Undo() {
            if (_undoer != null) {
                _undoer(_target.Focus, _target.Model);
            }
        }


        // ------------------------------
        // private
        // ------------------------------
        private void Begin(IEditor editor, Point? location) {
            if (editor.Focus != null && !editor.IsFocused) {
                /// 先にeditor.IsFocused = true;でFocusLayerにFocusを追加しておかないと
                /// Begin()内でArgumentNullExceptionが起こる
                editor.IsFocused = true;
                editor.Focus.Begin(location);
            }
        }

        private bool Commit(IEditor editor) {
            if (editor.Focus != null && editor.IsFocused) {
                var ret = editor.Focus.Commit();
                if (ret) {
                    editor.IsFocused = false;
                }
                return ret;
            }
            return false;
        }

        private void Rollback(IEditor editor) {
            if (editor.Focus != null && editor.IsFocused) {
                editor.Focus.Rollback();
                editor.IsFocused = false;
            }
        }

    }
}
