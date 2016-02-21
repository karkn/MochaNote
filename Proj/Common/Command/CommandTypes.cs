/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Command {
    public delegate bool MergeJudge(ICommand next);
    //public delegate bool ConsumptionJudge(ICommand next);

    public class CommandChainContext: IDisposable {
        private ICommandExecutor _owner;
        internal CommandChainContext(ICommandExecutor owner) {
            _owner = owner;
        }
        public void Dispose() {
            _owner.EndChain();
        }
    }
}
