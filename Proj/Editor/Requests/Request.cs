/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Editor.Requests {
    public class Request<T>: AbstractRequest {
        // ========================================
        // field
        // ========================================
        private string _id;
        private T _data;

        // ========================================
        // constructor
        // ========================================
        public Request(string id, T data) {
            _id = id;
            _data = data;
        }

        // ========================================
        // property
        // ========================================
        public override string Id {
            get { return _id; }
        }

        public T Data {
            get { return _data; }
        }
    }
}
