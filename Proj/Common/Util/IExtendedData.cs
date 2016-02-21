/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Common.Util {
    public interface IExtendedData {
        // ========================================
        // property
        // ========================================
        IDictionary<string, object> AsDictionary { get; }
        object this[string name] { get; set; }
        object this[ExtendedDataDef extendedDataDef] { get; set; }

        // ========================================
        // method
        // ========================================
        void CopyDataTo(IExtendedData extendable);
        void ComplementDataTo(IExtendedData extendable);
    }

    public class ExtendedDataDef {
        // ========================================
        // field
        // ========================================
        private string _name;
        private object _defaultValue;

        // ========================================
        // constructor
        // ========================================
        public ExtendedDataDef(string name, object defaultValue) {
            _name = name;
            _defaultValue = defaultValue;
        }
        // ========================================
        // property
        // ========================================
        public string Name {
            get { return _name; }
        }
        public object DefaultValue {
            get { return _defaultValue; }
        }
    }

}
