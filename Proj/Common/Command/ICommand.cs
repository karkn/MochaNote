/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Command {
    public interface ICommand {
        // ========================================
        // property
        // ========================================
        /// <summary>
        /// 現在実行可能かどうかを返す．
        /// </summary>
        bool CanExecute { get; }

        /// <summary>
        /// 取り消し可能かどうかを返す．
        /// falseならばUndoStackに格納されない．
        /// </summary>
        bool CanUndo { get; }

        MergeJudge MergeJudge { get; set; }

        // ========================================
        // method
        // ========================================
        void Execute();
        void Undo();
        void Redo();

        /// <summary>
        /// nextと結合したcommandを返す．
        /// </summary>
        ICommand Chain(ICommand next);

        /// <summary>
        /// nextとマージすべきかどうかを返す．
        /// </summary>
        bool ShouldMerge(ICommand next);

        /// <summary>
        /// nextと消化すべきかどうかを返す．
        /// </summary>
        //bool ShouldConsume(ICommand next);

    }
}
