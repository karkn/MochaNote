/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Command {
    public abstract class AbstractCommand: ICommand {
        // ========================================
        // field
        // ========================================
        private CompositeCommand _parent;

        private MergeJudge _mergeJudge;

        // ========================================
        // constructor
        // ========================================
        protected AbstractCommand() {
        }

        // ========================================
        // property
        // ========================================
        public abstract bool CanExecute { get; }
        public abstract bool CanUndo { get; }

        public MergeJudge MergeJudge {
            get { return _mergeJudge; }
            set { _mergeJudge = value; }
        }

        protected internal virtual CompositeCommand _Parent {
            get { return _parent; }
            set {
                if (value == _parent) {
                    return;
                }
                _parent = value;
            }
        }

        // ========================================
        // method
        // ========================================
        public abstract void Execute();
        public abstract void Undo();

        public virtual void Redo() {
            Execute();
        }

        public virtual ICommand Chain(ICommand next) {
            var ret = new CompositeCommand();
            ret.Children.Add(this);
            ret.Children.Add(next);
            return ret;
        }

        public virtual bool ShouldMerge(ICommand next) {
            return _mergeJudge != null && _mergeJudge(next);
        }

    }
}
