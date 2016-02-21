/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Common.Util {
    [Serializable]
    public class ExtendedData: IExtendedData {
        // ========================================
        // field
        // ========================================
        private Dictionary<string, object> _extendedData; // lazy load

        // ========================================
        // constructor
        // ========================================
        public ExtendedData() {

        }

        // ========================================
        // property

        // ========================================
        public virtual IDictionary<string, object> AsDictionary {
            get {
                if (_extendedData == null) {
                    _extendedData = new Dictionary<string, object>();
                }
                return _extendedData;
            }
        }

        public virtual object this[string name] {
            get { return AsDictionary.ContainsKey(name)? AsDictionary[name]: null; }
            set { AsDictionary[name] = value; }
        }

        public virtual object this[ExtendedDataDef attributeDef] {
            get {
                return AsDictionary.ContainsKey(attributeDef.Name)?
                    AsDictionary[attributeDef.Name]: attributeDef.DefaultValue;
            }
            set { AsDictionary[attributeDef.Name] = value; }
        }

        // ========================================
        // method
        // ========================================
        public virtual void CopyDataTo(IExtendedData extendedData) {
            if (extendedData == null) {
                return;
            }
            foreach (string key in AsDictionary.Keys) {
                extendedData.AsDictionary[key] = AsDictionary[key];
            }
        }

        public virtual void ComplementDataTo(IExtendedData extendedData) {
            if (extendedData == null) {
                return;
            }
            foreach (string key in AsDictionary.Keys) {
                if (!extendedData.AsDictionary.ContainsKey(key)) {
                    extendedData.AsDictionary[key] = AsDictionary[key];
                }
            }
        }
    }
}
