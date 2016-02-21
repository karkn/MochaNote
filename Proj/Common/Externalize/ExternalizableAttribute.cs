/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Externalize {
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
        Inherited = true)]
    public class ExternalizableAttribute: Attribute {
        // ========================================
        // field
        // ========================================
        private Type _type;

        private Type _factoryMethodType;
        private string _factoryMethod;
        private string[] _factoryMethodParamKeys;

        private string[] _constructorParamKeys;

        private string _saved;
        private string _loaded;

        // ========================================
        // property
        // ========================================
        public Type Type {
            get { return _type; }
            set { _type = value; }
        }

        public Type FactoryMethodType {
            get { return _factoryMethodType; }
            set { _factoryMethodType = value; }
        }

        public string FactoryMethod {
            get { return _factoryMethod; }
            set { _factoryMethod = value; }
        }

        public string[] FactoryMethodParamKeys {
            get { return _factoryMethodParamKeys; }
            set { _factoryMethodParamKeys = value; }
        }

        public string[] ConstructorParamKeys {
            get { return _constructorParamKeys; }
            set { _constructorParamKeys = value; }
        }

        public string Saved {
            get { return _saved; }
            set { _saved = value; }
        }

        public string Loaded {
            get { return _loaded; }
            set { _loaded = value; }
        }
    }
}
