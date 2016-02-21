/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Mkamo.Common.Reflection {
    public class AttributeCache {
        // ========================================
        // field
        // ========================================
        private Dictionary<MemberInfo, Attribute[]> _memberToAttributes;

        // ========================================
        // constructor
        // ========================================
        public AttributeCache() {
            _memberToAttributes = new Dictionary<MemberInfo, Attribute[]>();
        }

        // ========================================
        // member
        // ========================================
        public bool IsDefined(MemberInfo member, Type attrType) {
            var attrs = EnsureAttributesCached(member);
            return Array.Exists(
                attrs,
                elem => elem.GetType() == attrType
            );
        }

        public bool IsDefined<TAttribute>(MemberInfo member) {
            return IsDefined(member, typeof(TAttribute));
        }

        public Attribute GetCustomAttribute(MemberInfo member, Type attrType) {
            var attrs = EnsureAttributesCached(member);
            return Array.Find(
                attrs,
                elem => elem.GetType() == attrType
            );
        }

        public TAttribute GetCustomAttribute<TAttribute>(MemberInfo member) where TAttribute: Attribute {
            return GetCustomAttribute(member, typeof(TAttribute)) as TAttribute;
        }
        
        public Attribute[] GetCustomAttributes(MemberInfo member, Type attrType) {
            var attrs = EnsureAttributesCached(member);
            return Array.FindAll(
                attrs,
                elem => elem.GetType() == attrType
            );
        }

        public TAttribute[] GetCustomAttributes<TAttribute>(MemberInfo member) where TAttribute: Attribute {
            return GetCustomAttributes(member, typeof(TAttribute)) as TAttribute[];
        }

        // ------------------------------
        // private
        // ------------------------------
        private Attribute[] EnsureAttributesCached(MemberInfo member) {
            Attribute[] ret;
            if (!_memberToAttributes.TryGetValue(member, out ret)) {
                ret = Attribute.GetCustomAttributes(member);
                _memberToAttributes.Add(member, ret);
            }
            return ret;
        }

    }
}
