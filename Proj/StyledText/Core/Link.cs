/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Mkamo.StyledText.Core {
    /// <summary>
    /// Immutable．
    /// </summary>
    [Serializable]
    [DataContract]
    public class Link: ICloneable {
        // ========================================
        // static method
        // ========================================
        public static bool operator==(Link a, Link b) {
            if (object.ReferenceEquals(a, b)) {
                return true;
            }
            if ((object) a == null) {
                return (object) b == null;
            } else {
                return a.Equals(b);
            }
        }

        public static bool operator!=(Link a, Link b) {
            return !(a == b);
        }

        // ========================================
        // field
        // ========================================
        [DataMember]
        private string _uri;
        [DataMember]
        private string _relationship;

        // ========================================
        // constructor
        // ========================================
        public Link(string uri, string relationship) {
            _uri = uri;
            _relationship = relationship;
        }

        public Link(string uri): this(uri, null) {
        }

        // ========================================
        // property
        // ========================================
        public string Uri {
            get { return _uri; }
        }

        public string Relationship {
            get { return _relationship; }
        }

        // ========================================
        // method
        // ========================================
        public override bool Equals(object obj) {
            var link = obj as Link;
            if (link != null) {
                return (_uri == link._uri) && (_relationship == link._relationship);
            } else {
                return false;
            }
        }

        public override int GetHashCode() {
            var ret = _uri == null? 0: _uri.GetHashCode();
            ret ^= _relationship == null? 0: _relationship.GetHashCode();
            return ret;
        }

        public object Clone() {
            return new Link(_uri, _relationship);
        }
    }
}
