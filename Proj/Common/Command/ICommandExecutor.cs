/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Command {
    public interface ICommandExecutor {
        // ========================================
        // event
        // ========================================
        event EventHandler<CommandEventArgs> CommandExecuted;
        event EventHandler<CommandEventArgs> CommandUndone;
        event EventHandler<CommandEventArgs> CommandRedone;
        event EventHandler<CommandEventArgs> CommandChainEnded;

        // ========================================
        // property
        // ========================================
        bool CanUndo { get; }
        bool CanRedo { get; }

        // ========================================
        // method
        // ========================================
        void Execute(ICommand command);
        ICommand Undo();
        ICommand Redo();
        void Clear();

        /// <summary>
        /// BeginChain()からEndChain()されるまでの間に
        /// Execute()で渡されたcommandをChain()して
        /// 一度のUndo()ですべてアンドゥできるようにする。
        /// </summary>
        CommandChainContext BeginChain();
        void EndChain();
    }
}
