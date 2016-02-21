/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Core {
    public class EventArgs<T>: EventArgs {
        private T _data;

        public EventArgs(T data) {
            _data = data;
        }

        public T Data {
            get { return _data; }
        }
    }
}
