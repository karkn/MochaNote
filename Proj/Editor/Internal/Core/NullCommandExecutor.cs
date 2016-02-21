/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Common.Command;

namespace Mkamo.Editor.Internal.Core {
    internal class NullCommandExecutor: ICommandExecutor {
        public event EventHandler<CommandEventArgs> CommandExecuted {
            add {}
            remove {}
        }
        public event EventHandler<CommandEventArgs> CommandUndone {
            add {}
            remove {}
        }
        public event EventHandler<CommandEventArgs> CommandRedone {
            add {}
            remove {}
        }

        public event EventHandler<CommandEventArgs> CommandChainEnded {
            add {}
            remove {}
        }

        public bool CanUndo {
            get { return false; }
        }

        public bool CanRedo {
            get { return false; }
        }

        public void Execute(ICommand command) {
        }

        public ICommand Undo() {
            return null;
        }

        public ICommand Redo() {
            return null;
        }

        public void Clear() {
        }

        public ICommand Merge(int count) {
            return null;
        }


        public CommandChainContext BeginChain() {
            return null;
        }

        public void EndChain() {
        }


    }
}
