/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Mkamo.Common.Externalize {
    public interface IMemento {
        // ========================================
        // property
        // ========================================
        string[] ConstructorParamKeys { get; set; }

        MethodInfo FactoryMethod { set; }
        string[] FactoryMethodParamKeys { get; set; }

        // ========================================
        // method
        // ========================================
        void SetConstructorParamKeys(string[] paramKeys);
        void SetFactoryMethod(MethodInfo factoryMethod, string[] paramKeys);

        bool Contains(string key);

        int ReadInt(string key);
        long ReadLong(string key);
        byte ReadByte(string key);
        short ReadShort(string key);
        decimal ReadDecimal(string key);
        char ReadChar(string key);
        bool ReadBool(string key);
        float ReadFloat(string key);
        double ReadDouble(string key);
        DateTime ReadDateTime(string key);
        string ReadString(string key);
        object ReadSerializable(string key);
        object ReadExternalizable(string key);
        IEnumerable<object> ReadExternalizables(string key);

        void WriteInt(string key, int value);
        void WriteLong(string key, long value);
        void WriteByte(string key, byte value);
        void WriteShort(string key, short value);
        void WriteDecimal(string key, decimal value);
        void WriteChar(string key, char value);
        void WriteBool(string key, bool value);
        void WriteFloat(string key, float value);
        void WriteDouble(string key, double value);
        void WriteDateTime(string key, DateTime value);
        void WriteString(string key, string value);
        void WriteSerializable(string key, object value);
        void WriteExternalizable(string key, object value);
        void WriteExternalizables(string key, IEnumerable<object> values);
    }
}
