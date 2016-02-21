/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Core {
    public class DisposableBundle: IDisposable {
        private IEnumerable<IDisposable> _disposables;

        public DisposableBundle(IEnumerable<IDisposable> disposables) {
            _disposables = disposables;
        }

        public void Dispose() {
            if (_disposables != null) {
                foreach (var d in _disposables) {
                    if (d != null) {
                        d.Dispose();
                    }
                }
            }
        }
    }
}
