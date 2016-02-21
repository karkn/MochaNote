/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.Serialization;

namespace Mkamo.Common.Externalize.Internal {
    [Serializable]
    internal class Memento: IMemento {
        // ========================================
        // static field
        // ========================================
        private static readonly string[] EmptyParamKeys = new string[0];

        // ========================================
        // field
        // ========================================
        private string _typeName;
        private string _assemblyName;

        private string[] _constructorParamKeys = EmptyParamKeys;

        private string _factoryTypeName;
        private string _factoryAssemblyName;
        private string _factoryMethodName;
        private string[] _factoryMethodParamKeys = EmptyParamKeys;

        private Dictionary<string, object> _values = new Dictionary<string, object>();

        [NonSerialized]
        private ExternalizeContext _context;

        // ========================================
        // constructor
        // ========================================
        public Memento(Type type) {
            _typeName = type.FullName;
            _assemblyName = type.Assembly.GetName().FullName;
        }

        // ========================================
        // property
        // ========================================
        public string TypeName {
            get { return _typeName; }
        }

        public string AssemblyName {
            get { return _assemblyName; }
        }

        public string[] ConstructorParamKeys {
            get { return _constructorParamKeys; }
            set { _constructorParamKeys = value; }
        }

        public MethodInfo FactoryMethod {
            set {
                if (value == null || !value.IsStatic || !value.IsPublic) {
                    throw new ArgumentException("value");
                }
                _factoryMethodName = value.Name;

                var type = value.ReflectedType;
                _factoryTypeName = type.FullName;
                _factoryAssemblyName = type.Assembly.GetName().FullName;
            }
        }

        public string FactoryMethodName {
            get { return _factoryMethodName; }
        }

        public string FactoryTypeName {
            get { return _factoryTypeName; }
        }

        public string FactoryAssemblyName {
            get { return _factoryAssemblyName; }
        }

        public string[] FactoryMethodParamKeys {
            get { return _factoryMethodParamKeys; }
            set { _factoryMethodParamKeys = value; }
        }


        public Dictionary<string, object> Values {
            get { return _values; }
        }

        public ExternalizeContext Context {
            get { return _context; }
            set {
                if (value == _context) {
                    return;
                }
                _context = value;
            }
        }

        // ========================================
        // method
        // ========================================
        public void SetConstructorParamKeys(string[] paramKeys) {
            ConstructorParamKeys = paramKeys;
        }

        public void SetFactoryMethod(MethodInfo factoryMethod, string[] paramKeys) {
            FactoryMethod = factoryMethod;
            FactoryMethodParamKeys = paramKeys;
        }

        public bool Contains(string key) {
            return _values.ContainsKey(key);
        }
        
        public int ReadInt(string key) {
            object ret;
            if (_values.TryGetValue(key, out ret)) {
                return (int) ret;
            } else {
                return 0;
            }
        }

        public long ReadLong(string key) {
            object ret;
            if (_values.TryGetValue(key, out ret)) {
                return (long) ret;
            } else {
                return 0L;
            }
        }

        public byte ReadByte(string key) {
            object ret;
            if (_values.TryGetValue(key, out ret)) {
                return (byte) ret;
            } else {
                return default(byte);
            }
        }

        public short ReadShort(string key) {
            object ret;
            if (_values.TryGetValue(key, out ret)) {
                return (short) ret;
            } else {
                return default(short);
            }
        }

        public decimal ReadDecimal(string key) {
            object ret;
            if (_values.TryGetValue(key, out ret)) {
                return (decimal) ret;
            } else {
                return default(decimal);
            }
        }

        public char ReadChar(string key) {
            object ret;
            if (_values.TryGetValue(key, out ret)) {
                return (char) ret;
            } else {
                return default(char);
            }
        }

        public bool ReadBool(string key) {
            object ret;
            if (_values.TryGetValue(key, out ret)) {
                return (bool) ret;
            } else {
                return false;
            }
        }

        public float ReadFloat(string key) {
            object ret;
            if (_values.TryGetValue(key, out ret)) {
                return (float) ret;
            } else {
                return default(float);
            }
        }

        public double ReadDouble(string key) {
            object ret;
            if (_values.TryGetValue(key, out ret)) {
                return (double) ret;
            } else {
                return default(double);
            }
        }

        public DateTime ReadDateTime(string key) {
            object ret;
            if (_values.TryGetValue(key, out ret)) {
                return (DateTime) ret;
            } else {
                return default(DateTime);
            }
        }

        public string ReadString(string key) {
            object ret;
            if (_values.TryGetValue(key, out ret)) {
                return (string) ret;
            } else {
                return default(string);
            }
        }

        public object ReadSerializable(string key) {
            object ret;
            if (_values.TryGetValue(key, out ret)) {
                return ret;
            } else {
                return null;
            }
        }

        public object ReadExternalizable(string key) {
            object value;
            if (_values.TryGetValue(key, out value)) {
                return _context.GetExternalizable(key, value as IMemento);
            } else {
                return null;
            }
        }

        public IEnumerable<object> ReadExternalizables(string key) {
            object value;
            if (_values.TryGetValue(key, out value)) {
                var mems = value as List<IMemento>;
                var ret = new List<object>();
                foreach (var mem in mems) {
                    var elem = _context.GetExternalizable(key, mem);
                    if (elem != null) {
                        ret.Add(elem);
                    }
                }
                return ret;
            } else {
                return null;
            }
        }
        

        public void WriteInt(string key, int value) {
            _values[key] = value;
        }

        public void WriteLong(string key, long value) {
            _values[key] = value;
        }

        public void WriteByte(string key, byte value) {
            _values[key] = value;
        }

        public void WriteShort(string key, short value) {
            _values[key] = value;
        }

        public void WriteDecimal(string key, decimal value) {
            _values[key] = value;
        }

        public void WriteChar(string key, char value) {
            _values[key] = value;
        }

        public void WriteBool(string key, bool value) {
            _values[key] = value;
        }

        public void WriteFloat(string key, float value) {
            _values[key] = value;
        }

        public void WriteDouble(string key, double value) {
            _values[key] = value;
        }

        public void WriteDateTime(string key, DateTime value) {
            _values[key] = value;
        }

        public void WriteString(string key, string value) {
            _values[key] = value;
        }

        public void WriteSerializable(string key, object value) {
            _values[key] = value;
        }

        public void WriteExternalizable(string key, object value) {
            _values[key] = _context.GetMemento(key, value);
        }

        public void WriteExternalizables(string key, IEnumerable<object> values) {
            var mems = new List<IMemento>();
            foreach (var value in values) {
                var elem = _context.GetMemento(key, value);
                if (elem != null) {
                    mems.Add(elem);
                }
            }
            _values[key] = mems;
        }
    }
}
