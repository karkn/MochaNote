/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Mkamo.Common.Collection;
using Mkamo.Common.IO;
using Mkamo.Common.Reflection;
using Mkamo.Container.Core;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using System.Xml.XPath;
using Mkamo.Common.Diagnostics;
using System.Xml.Linq;
using System.Text;
using Mkamo.Common.Serialize;
using System.Data.SqlServerCe;
using System.Data;
using Mkamo.Common.DataType;
using Mkamo.Common.Core;

namespace Mkamo.Container.Internal.Core {
    internal abstract class XmlEntityStore: IEntityStore {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(global::System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string DefaultStoreRootPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".store");
        private const string EntityContentFilename = "entity.xml";
        private const string EntityExtendedDataFileExtension = ".dat";
        private const string EntityExtendedObjectDataFileExtension = ".obj";

        protected const string EntityElementName = "Entity";
        protected const string SaveElementName = "EntitySave";
        protected const string SerializableElementName = "Serializable";
        protected const string EnumElementName = "Enum";
        protected const string TypeElementName = "Type";

        protected const string IntElementName = "int";
        protected const string BoolElementName = "bool";
        protected const string LongElementName = "long";
        protected const string FloatElementName = "float";
        protected const string DoubleElementName = "double";
        protected const string ByteElementName = "byte";
        protected const string CharElementName = "char";

        protected const string StringElementName = "string";
        protected const string DecimalElementName = "decimal";
        protected const string DatetimeElementName = "datetime";

        protected const string ElemElementName = "Elem";
        protected const string KeyElementName = "Key";
        protected const string ValueElementName = "Value";

        protected const string TypeAttributeName = "Type";
        protected const string AssemblyAttributeName = "Assembly";
        protected const string IdAttributeName = "Id";

        // ========================================
        // field
        // ========================================
        private IEntityContainer _container;

        private Dictionary<Type, ITypePersister> _typePersisters; /// lazy

        private int _valueQuota = 1024;
        private int _nValues = 0;

        // ========================================
        // constructor
        // ========================================
        protected XmlEntityStore() {
        }


        // ========================================
        // property
        // ========================================
        public IEntityContainer EntityContainer {
            get { return _container; }
            set { _container = value; }
        }

        public int ValueQuota {
            get { return _valueQuota; }
            set { _valueQuota = value; }
        }

        public IDictionary<Type, ITypePersister> TypePersisters {
            get {
                if (_typePersisters == null) {
                    _typePersisters = new Dictionary<Type, ITypePersister>();
                }
                return _typePersisters;
            }
        }

        protected int _NValues {
            get { return _nValues; }
            set { _nValues = value; }
        }

        // ========================================
        // method
        // ========================================
        // === IEntityStore ==========
        public string CreateId() {
            return Guid.NewGuid().ToString();
        }

        public abstract void Begin();

        public abstract void Commit();

        public abstract void Load(object target);

        public abstract void Insert(object target);

        public abstract void Update(object target);

        public abstract bool Remove(object target);

        public abstract bool IsEntityExists(Type targetType, string id);

        public abstract IEnumerable<string> GetIds(Type type);
        public abstract IEnumerable<string> GetIdsLike(Type type, string likeParam);

        public abstract string LoadExtendedTextData(Type type, string id, string key);

        public abstract void SaveExtendedTextData(Type type, string id, string key, string value);

        public abstract byte[] LoadExtendedBinaryData(Type type, string id, string key);

        public abstract void SaveExtendedBinaryData(Type type, string id, string key, byte[] value);

        //public abstract IEnumerable<string> GetPropertyValues(Type type, string propName);
        //public abstract IEnumerable<Tuple<string, string>> GetIdsAndPropertyValues(Type type, string propName);
        //public abstract IEnumerable<string> GetIdsWherePropertyValueIsEmpty(Type type, string propName);
        public abstract string LoadRawData(Type type, string id);


        /// <summary>
        /// idとプロパティの値を格納したdictionaryを返す．
        /// </summary>
        //public abstract IDictionary<string, object> GetPropertyValues(Type type, string propName);

        // --- protected ---
        /// pre: Type start element must be fetched
        /// post: Type end element must be fetched
        protected void LoadType(StoreContext context, object target, Type targetType) {
            var reader = context.Reader;
            if (!reader.IsEmptyElement) {
                while (reader.Read()) { /// fetch Property start element or Type end element
                    if (reader.IsStartElement()) {
                        if (reader.Name == SaveElementName) {
                            LoadByLoadMethod(context, target, targetType);
                        } else {
                            LoadProperty(context, target, targetType, reader.Name);
                        }
                    } else {
                        reader.ReadEndElement(); /// consume EndOfType
                        break;
                    }
                }
            }
        }

        /// pre: Persister start element must be fetched
        /// post: Persister end element must be fetched
        protected void LoadByLoadMethod(StoreContext context, object target, Type targetType) {
            var reader = context.Reader;
            var entityAttr = GetEntityAttribute(targetType);
            if (entityAttr == null || entityAttr.Load == null) {
                SkipToEndElement(context, SaveElementName);
                return;
            }

            var loadMethod = targetType.GetMethod(
                entityAttr.Load, new Type[] { typeof(IDictionary<string, string>) }
            );
            var values = new Dictionary<string, string>();
            LoadDictionaryProperty(context, values, new Type[] { typeof(string), typeof(string) });
            //loadMethod.Invoke(target, new object[] { values });
            InvokeAction(target, loadMethod, values);
        }

        /// pre: Property start element must be fetched
        /// post: Property end element must be fetched
        protected void LoadProperty(StoreContext context, object target, Type targetType, string propName) {
            var reader = context.Reader;
            //var prop = targetType.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public);
            var prop = TypeService.Instance.GetPublicInstanceProperty(targetType, propName);
            if (prop == null) {
                /// 無視して現在のプロパティのend elementまで読み進める
                /// reader.Skip()では進みすぎてしまう
                SkipToEndElement(context, reader.Name);
                return;
            }

            var isEntity = IsEntityDefined(targetType);
            var persistAttr = GetPersistAttribute(prop);

            if (!isEntity || (isEntity && persistAttr != null && persistAttr.Enabled)) {
                var propType = prop.PropertyType;
    
                if (propType.IsArray) {
                    if (HasPublicSetter(prop)) {
                        var elemType = propType.GetElementType();
                        LoadArrayProperty(context, target, prop, elemType);
                    } else {
                        SkipToEndElement(context, reader.Name);
                    }
                } else if (GenericTypeUtil.IsGenericIDictionary(propType)) {
                    if (HasPublicGetter(prop)) {
                        var elemTypes = GenericTypeUtil.GetGenericArgumentOfGenericIDictionary(propType);
                        var propValue = GetValue(target, prop);
                        LoadDictionaryProperty(context, propValue, elemTypes);
                    } else {
                        SkipToEndElement(context, reader.Name);
                    }
                } else if (GenericTypeUtil.IsGenericICollection(propType)) {
                    if (HasPublicGetter(prop)) {
                        var elemType = GenericTypeUtil.GetGenericArgumentOfGenericICollection(propType);
                        var propValue = GetValue(target, prop);
                        LoadCollectionProperty(context, target, targetType, prop, propValue, elemType);
                    } else {
                        SkipToEndElement(context, reader.Name);
                    }
                } else if (GenericTypeUtil.IsGenericIEnumerable(propType) && persistAttr.Add != null) {
                    if (HasPublicGetter(prop)) {
                        var elemType = GenericTypeUtil.GetGenericArgumentOfGenericIEnumerable(propType);
                        var propValue = GetValue(target, prop);
                        LoadEnumerableProperty(context, target, targetType, prop, propValue, elemType);
                    } else {
                        SkipToEndElement(context, reader.Name);
                    }
                } else {
                    if (HasPublicSetter(prop)) {
                        LoadSingletonProperty(context, target, prop, propType);
                    } else {
                        SkipToEndElement(context, reader.Name);
                    }
                }
            } else {
                SkipToEndElement(context, reader.Name);
            }
        }

        /// pre: Property start element must be fetched
        /// post: Property end element must be fetched
        protected void LoadArrayProperty(
            StoreContext context, object target, PropertyInfo prop, Type elemType
        ) {
            var reader = context.Reader;
            List<object> elemValues = null;
            while (reader.Read()) { /// fetch Elem start elment or Property end element
                if (reader.IsEmptyElement) {
                    /// do nothing
                } else if (reader.IsStartElement(ElemElementName)) {
                    if (elemValues == null) {
                        elemValues = new List<object>();
                    }
                    reader.ReadStartElement(ElemElementName); /// consume Elem and fetch value
                    var value = ReadValue(context, elemType);
                    elemValues.Add(value);

                } else {
                    break;
                }
            }
            var elemArray = Array.CreateInstance(elemType, elemValues.Count);
            Array.Copy(elemValues.ToArray(), elemArray, elemArray.Length);
            SetValue(target, prop, elemArray);
        }

        /// pre: Property start element must be fetched
        /// post: Property end element must be fetched
        protected void LoadEnumerableProperty(
            StoreContext context, object target, Type targetType, PropertyInfo prop, object propValue, Type elemType
        ) {
            var reader = context.Reader;
            if (!reader.IsEmptyElement) {
                var persistAttr = GetPersistAttribute(prop);
                var addMethodName = persistAttr.Add;
                var addMethod = addMethodName == null?
                    null:
                    //targetType.GetMethod(addMethodName, new Type[] { elemType });
                    TypeService.Instance.GetAddMethod(targetType, addMethodName, elemType);

                while (reader.Read()) { /// fetch Elem start elment or Property end element
                    if (reader.IsEmptyElement) {
                        /// do nothing
                    } else if (reader.IsStartElement(ElemElementName)) {
                        reader.ReadStartElement(ElemElementName); /// consume Elem and fetch Value
                        var value = ReadValue(context, elemType);

                        if (!persistAttr.AllowNull && value == null) {
                            Logger.Warn("Value is null");

                        } else {
                            if (addMethod != null) {
                                //addMethod.Invoke(target, new object[] { value });
                                InvokeAction(target, addMethod, value);
                            }
                        }
                    } else {
                        break;
                    }
                }
            }
        }

        /// pre: Property start element must be fetched
        /// post: Property end element must be fetched
        protected void LoadCollectionProperty(
            StoreContext context, object target, Type targetType, PropertyInfo prop, object propValue, Type elemType
        ) {
            var reader = context.Reader;
            if (!reader.IsEmptyElement) {
                var persistAttr = GetPersistAttribute(prop);
                var addMethodName = persistAttr.Add;
                var addMethod = addMethodName == null?
                    typeof(ICollection<>).MakeGenericType(elemType).GetMethod("Add"):
                    //targetType.GetMethod(addMethodName, new Type[] { elemType });
                    TypeService.Instance.GetAddMethod(targetType, addMethodName, elemType);

                while (reader.Read()) { /// fetch Elem start elment or Property end element
                    if (reader.IsEmptyElement) {
                        /// do nothing
                    } else if (reader.IsStartElement(ElemElementName)) {
                        reader.ReadStartElement(ElemElementName); /// consume Elem and fetch Value
                        var value = ReadValue(context, elemType);

                        if (!persistAttr.AllowNull && value == null) {
                            /// AssociationCollectionでエラーになるので追加しない
                            Logger.Warn("Value is null");
  
                        } else {
                            if (addMethodName == null) {
                                //addMethod.Invoke(propValue, new object[] { value });
                                InvokeAction(propValue, addMethod, value);
                            } else {
                                //addMethod.Invoke(target, new object[] { value });
                                InvokeAction(target, addMethod, value);
                            }
                        }
                    } else {
                        break;
                    }
                }
            }
        }

        /// pre: Property start element must be fetched
        ///      when custom type persister used, Type start element must be fetched
        /// post: Property end element must be fetched
        ///       when custom type persister used, Type end element must be fetched
        protected void LoadDictionaryProperty(
            StoreContext context, object propValue, Type[] elemTypes
        ) {
            var reader = context.Reader;
            if (!reader.IsEmptyElement) {
                while (reader.Read()) { /// fetch Elem start elment or Property end element
                    if (reader.IsEmptyElement) {
                        /// do nothing
                    } else if (reader.IsStartElement(ElemElementName)) {
                        var setItemMethod =
                            typeof(IDictionary<,>).MakeGenericType(elemTypes).GetMethod("set_Item");
                        reader.ReadStartElement(ElemElementName); /// consume Elem and fetch Value
                        reader.ReadStartElement(KeyElementName);
                        var key = ReadValue(context, elemTypes[0]);
                        reader.ReadEndElement();
                        reader.ReadStartElement(ValueElementName);
                        var value = ReadValue(context, elemTypes[1]);
                        reader.ReadEndElement();

                        setItemMethod.Invoke(propValue, new object[] { key, value });

                    } else {
                        break;
                    }
                }
            }
        }

        /// pre: Property start element must be fetched
        /// post: Property end element must be fetched
        protected void LoadSingletonProperty(StoreContext context, object target, PropertyInfo prop, Type propType) {
            var reader = context.Reader;
            if (!reader.IsEmptyElement) {
                reader.Read(); /// consume start element and fetch Value
                SetValue(target, prop, ReadValue(context, propType));
            }
        }

        /// pre: EnumType start element must be fetched
        /// post: next of EnumType end element must be fetched
        protected object ReadEnumProperty(StoreContext context, Type valueType) {
            var reader = context.Reader;
            object ret = null;
            //reader.ReadStartElement(valueType.Name);  /// consume EnumType start element and fetch EnumValue
            reader.ReadStartElement(EnumElementName);  /// consume EnumType start element and fetch EnumValue
            {
                ret = Enum.Parse(valueType, reader.ReadContentAsString());
            }
            reader.ReadEndElement(); /// consume end of EnumType
            return ret;
        }

        protected object ReadTypeConverterProperty(StoreContext context, Type valueType) {
            var reader = context.Reader;
            var converter = TypeDescriptor.GetConverter(valueType);
            var ret = default(object);
            //reader.ReadStartElement(valueType.Name);  /// consume TypeConverterProp start element and fetch Value
            reader.ReadStartElement(TypeElementName);  /// consume TypeConverterProp start element and fetch Value
            {
                ret = converter.ConvertFromString(reader.ReadContentAsString());
            }
            reader.ReadEndElement(); /// consume end of TypeConverterProperty
            return ret;
        }

        protected abstract object LoadSerializableObject(string typeName, string id, string propName);

        protected object ReadSerializableProperty(StoreContext context) {
            var reader = context.Reader;

            var propname = reader.GetAttribute(0);
            var ret = LoadSerializableObject(context.Type.FullName, context.Id, propname);

            reader.Read(); /// consume Serializable Empty Element and fetch Property end element
            return ret;
        }


        /// pre: Value (prop type start element or Proxy empty element or primitive value) must be fetched
        /// post: next of Value node must be fetched
        protected object ReadValue(StoreContext context, Type valueType) {
            var reader = context.Reader;
            object ret = null;
            
            // todo: 要素名で判別すべきかvalueTypeで判別すべきか
            //       valueTypeで判別するのなら型を要素名として保存する必要はない
            switch (reader.Name) {
                case IntElementName: {
                    ret = reader.ReadElementContentAsInt();
                    break;
                }
                case BoolElementName: {
                    ret = reader.ReadElementContentAsBoolean();
                    break;
                }
                case LongElementName: {
                    ret = reader.ReadElementContentAsLong();
                    break;
                }
                case FloatElementName: {
                    ret = reader.ReadElementContentAsFloat();
                    break;
                }
                case DoubleElementName: {
                    ret = reader.ReadElementContentAsDouble();
                    break;
                }
                case ByteElementName: {
                    ret = (byte) reader.ReadElementContentAsInt();
                    break;
                }
                case CharElementName: {
                    ret = (char) reader.ReadElementContentAsInt();
                    break;
                }
                case DecimalElementName: {
                    ret = reader.ReadElementContentAsDecimal();
                    break;
                }
                case DatetimeElementName: {
                    ret = reader.ReadElementContentAsDateTime();
                    break;
                }
                case StringElementName: {
                    ret = reader.ReadElementContentAsString();
                    break;
                }
                default: {
                    if (reader.IsStartElement(EnumElementName)) {
                        /// Enum型
                        ret = ReadEnumProperty(context, valueType);

                    } else if (reader.IsStartElement(EntityElementName)) {
                        /// Entity
                        var typeStr = reader.GetAttribute(0);
                        var assemStr = reader.GetAttribute(1);
                        var id = reader.GetAttribute(2);
                        var type = GetTypeObject(typeStr, assemStr);
                        try {
                            ret = _container.Find(type, id);
                        } catch (Exception e) {
                            ret = null;
                            Logger.Warn(
                                "Failed to load entity: assembly=" + assemStr + ", type=" + typeStr + ", id=" + id,
                                e
                            );
                        }

                        reader.Read(); /// consume IEntity Empty Element and fetch Property end element

                    } else if (reader.IsStartElement(SerializableElementName)) {
                        /// Serializable
                        ret = ReadSerializableProperty(context);
                    
                    } else {
                        if (reader.IsStartElement()) { /// start of type of prop
                            var typeStr = reader.GetAttribute(0);
                            var assemStr = reader.GetAttribute(1);
                            var type = GetTypeObject(typeStr, assemStr);

                            if (TypePersisters.ContainsKey(type)) {
                                /// TypePersister
                                var values = new Dictionary<string, string>();
                                LoadDictionaryProperty(
                                    context, values, new Type[] { typeof(string), typeof(string) }
                                );
                                ret = TypePersisters[type].Load(values);
                                reader.ReadEndElement();

                            } else {
                                var service = TypeService.Instance;
                                if (service.HasStringInterconvertibility(type)) {
                                    /// TypeConverter
                                    ret = ReadTypeConverterProperty(context, valueType);

                                } else {
                                    /// normal
                                    //ret = Activator.CreateInstance(type);
                                    var act = service.GetDefaultActivator(type);
                                    ret = act();

                                    if (!reader.IsEmptyElement) {
                                        LoadType(context, ret, valueType);
                                    } else {
                                        reader.Read(); /// consume Empty Element and fetch next
                                    }
                                }
                            }
                        } else {
                            throw new InvalidOperationException();
                        }
                    }
                    break;
                }
            };
            return ret;
        }

        protected void SaveType(StoreContext context, object target, Type targetType) {
            if (target == null) {
                return;
            }
            var writer = context.Writer;

            writer.WriteStartElement(TypeElementName);
            writer.WriteAttributeString(TypeAttributeName, targetType.FullName);
            writer.WriteAttributeString(AssemblyAttributeName, targetType.Assembly.GetName().FullName);
            {
                var props = TypeService.Instance.GetPublicInstanceProperties(targetType);
                foreach (var prop in props) {
                    SaveProperty(context, target, targetType, prop);
                }

                SaveBySaveMethod(context, target, targetType);
            }
            writer.WriteEndElement();
        }

        protected void SaveBySaveMethod(StoreContext context, object target, Type targetType) {
            if (target == null) {
                return;
            }
            var writer = context.Writer;

            var entityAttr = GetEntityAttribute(targetType);
            if (entityAttr != null && entityAttr.Save != null) {
                var saveMethod = targetType.GetMethod(
                    entityAttr.Save, new Type[] { typeof(IDictionary<string, string>) }
                );
                var values = new InsertionOrderedDictionary<string, string>();
                //saveMethod.Invoke(target, new object[] { values });
                InvokeAction(target, saveMethod, values);
                writer.WriteStartElement(SaveElementName);
                SaveDictionaryProperty(
                    context, SaveElementName, values, new Type[] { typeof(string), typeof(string) }
                );
                writer.WriteEndElement();
            }
        }

        protected void SaveProperty(StoreContext context, object target, Type targetType, PropertyInfo prop) {
            var writer = context.Writer;
            var isEntity = IsEntityDefined(targetType);
            var persistAttr = GetPersistAttribute(prop);
            var propName = prop.Name;

            if (!isEntity || (isEntity && persistAttr != null && persistAttr.Enabled)) {
                PropertyUtil.ProcessProperty(
                    target,
                    prop,
                    (kind, propValue) => writer.WriteStartElement(propName),
                    (kind, propValue) => writer.WriteEndElement(),
                    (kind, value, key) => {
                        switch (kind) {
                            case PropertyUtil.PropertyKind.Array: {
                                var valueType = GetRealType(value);
                                writer.WriteStartElement(ElemElementName);
                                WriteValue(context, propName, valueType, value);
                                writer.WriteEndElement();
                                break;
                            }
                            case PropertyUtil.PropertyKind.Dictionary: {
                                writer.WriteStartElement(ElemElementName);

                                writer.WriteStartElement(KeyElementName);
                                WriteValue(context, propName, GetRealType(key), key);
                                writer.WriteEndElement();

                                writer.WriteStartElement(ValueElementName);
                                WriteValue(context, propName, GetRealType(value), value);
                                writer.WriteEndElement();

                                writer.WriteEndElement();
                                break;
                            }
                            case PropertyUtil.PropertyKind.Enumerable: {
                                var valueType = GetRealType(value);
                                writer.WriteStartElement(ElemElementName);
                                WriteValue(context, propName, valueType, value);
                                writer.WriteEndElement();
                                break;
                            }
                            case PropertyUtil.PropertyKind.Single: {
                                var valueType = GetRealType(value);
                                WriteValue(context, propName, valueType, value);
                                break;
                            }
                        }
                    }
                );
            }
        }

        protected void SaveDictionaryProperty(StoreContext context, string propName, object propValue, Type[] elemTypes) {
            var writer = context.Writer;
            var dictType = typeof(IDictionary<,>).MakeGenericType(elemTypes);
            var getKeysMethod = dictType.GetMethod("get_Keys");
            var getItemMethod = dictType.GetMethod("get_Item");

            var keys = getKeysMethod.Invoke(propValue, null) as ICollection;
            foreach (var key in keys) {
                writer.WriteStartElement(ElemElementName);
                {
                    writer.WriteStartElement(KeyElementName);
                    WriteValue(context, propName, GetRealType(key), key);
                    writer.WriteEndElement();

                    var value = getItemMethod.Invoke(propValue, new object[] { key });
                    writer.WriteStartElement(ValueElementName);
                    WriteValue(context, propName, GetRealType(value), value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }
        
        protected void SaveEntityProperty(StoreContext context, IEntity entity) {
            var writer = context.Writer;
            writer.WriteStartElement(EntityElementName);
            writer.WriteAttributeString(TypeAttributeName, entity.Type.FullName);
            writer.WriteAttributeString(AssemblyAttributeName, entity.Type.Assembly.GetName().FullName);
            writer.WriteAttributeString(IdAttributeName, entity.Id);
            writer.WriteEndElement();
        }

        protected void SaveEnumProperty(StoreContext context, Type valueType, object value) {
            var writer = context.Writer;
            writer.WriteStartElement(EnumElementName);
            writer.WriteAttributeString(TypeAttributeName, valueType.FullName);
            writer.WriteAttributeString(AssemblyAttributeName, valueType.Assembly.GetName().FullName);
            {
                writer.WriteValue(value.ToString());
            }
            writer.WriteEndElement();
        }

        protected abstract void SaveSerializableBytes(string typeName, string id, string propName, object value);

        protected void SaveSerializableProperty(StoreContext context, string valueName, object value, Type valueType) {
            var writer = context.Writer;
            SaveSerializableBytes(context.Type.FullName, context.Id, valueName, value);

            writer.WriteStartElement(SerializableElementName);
            //writer.WriteAttributeString("filename", filename);
            writer.WriteAttributeString("key", valueName);
            writer.WriteEndElement();
        }


        protected void SaveTypeConverterProperty(StoreContext context, Type valueType, object value) {
            var writer = context.Writer;
            var converter = TypeDescriptor.GetConverter(valueType);
            writer.WriteStartElement(TypeElementName);
            writer.WriteAttributeString(TypeAttributeName, valueType.FullName);
            writer.WriteAttributeString(AssemblyAttributeName, valueType.Assembly.GetName().FullName);
            {
                writer.WriteValue(converter.ConvertToString(value));
            }
            writer.WriteEndElement();
        }

        protected void SaveTypeByTypePersister(StoreContext context, string valueName, Type valueType, object value) {
            var writer = context.Writer;
            writer.WriteStartElement(TypeElementName);
            writer.WriteAttributeString(TypeAttributeName, valueType.FullName);
            writer.WriteAttributeString(AssemblyAttributeName, valueType.Assembly.GetName().FullName);
            {
                var values = new InsertionOrderedDictionary<string, string>();
                TypePersisters[valueType].Save(value, values);
                SaveDictionaryProperty(context, valueName, values, new Type[] { typeof(string), typeof(string) });
            }
            writer.WriteEndElement();
        }

        protected void WriteValue(StoreContext context, string valueName, Type valueType, object value) {
            var writer = context.Writer;

            ++_nValues;
            if (_nValues > _valueQuota) {
                throw new InvalidOperationException("Value count is over quota");
            }
            
            if (valueType.IsPrimitive) {
                if (valueType == typeof(int)) {
                    writer.WriteStartElement(IntElementName);
                    writer.WriteValue((int) value);
                    writer.WriteEndElement();
                } else if (valueType == typeof(bool)) {
                    writer.WriteStartElement(BoolElementName);
                    writer.WriteValue((bool) value);
                    writer.WriteEndElement();
                } else if (valueType == typeof(long)) {
                    writer.WriteStartElement(LongElementName);
                    writer.WriteValue((long) value);
                    writer.WriteEndElement();
                } else if (valueType == typeof(float)) {
                    writer.WriteStartElement(FloatElementName);
                    writer.WriteValue((float) value);
                    writer.WriteEndElement();
                } else if (valueType == typeof(double)) {
                    writer.WriteStartElement(DoubleElementName);
                    writer.WriteValue((double) value);
                    writer.WriteEndElement();
                } else if (valueType == typeof(byte)) {
                    writer.WriteStartElement(ByteElementName);
                    writer.WriteValue((byte) value);
                    writer.WriteEndElement();
                } else if (valueType == typeof(char)) {
                    writer.WriteStartElement(CharElementName);
                    writer.WriteValue((char) value);
                    writer.WriteEndElement();
                }
            } else {
                if (valueType == typeof(decimal)) {
                    writer.WriteStartElement(DecimalElementName);
                    writer.WriteValue((decimal) value);
                    writer.WriteEndElement();
                } else if (valueType == typeof(DateTime)) {
                    writer.WriteStartElement(DatetimeElementName);
                    writer.WriteValue((DateTime) value);
                    writer.WriteEndElement();
                } else if (valueType == typeof(string)) {
                    if (value != null) {
                        writer.WriteStartElement(StringElementName);
                        writer.WriteValue((string) value);
                        writer.WriteEndElement();
                    }
                } else if (valueType.IsEnum) {
                    /// Enum
                    SaveEnumProperty(context, valueType, value);
                } else {
                    var entity = _container.AsEntity(value);
                    if (entity == null) {
                        if (TypePersisters.ContainsKey(valueType)) {
                            /// TypePersister
                            SaveTypeByTypePersister(context, valueName, valueType, value);
                        } else if (TypeService.Instance.HasStringInterconvertibility(valueType)) {
                            /// TypeConverter
                            SaveTypeConverterProperty(context, valueType, value);
                        } else if (IsSerializableDefined(valueType)) {
                            /// Serializable
                            SaveSerializableProperty(context, valueName, value, valueType);
                        } else {
                            /// normal
                            SaveType(context, value, valueType);
                            
                        }
                    } else {
                        /// Entity
                        if (entity.State == PersistentState.Removed ||
                            entity.State == PersistentState.Discarded
                        ) {
                            /// do nothing
                            /// nullと同じ扱いにする
                        } else {
                            SaveEntityProperty(context, entity);
                        }
                    }
                }
            }
        }

        protected void SkipToEndElement(StoreContext context, string name) {
            var reader = context.Reader;
            var depth = reader.Depth;
            if (reader.IsEmptyElement) {
                /// EmptyElementの場合はそのエレメントをfetchするだけ
                reader.Read();
            } else {
                while (!(reader.NodeType == XmlNodeType.EndElement && reader.Name == name && reader.Depth == depth)) {
                    reader.Read();
                }
            }
        }

        protected Type GetRealType(object obj) {
            var entity = _container.AsEntity(obj);
            return entity == null? obj.GetType(): entity.Type;
        }

        protected bool IsEntityDefined(Type type) {
            return TypeService.Instance.IsEntityDefined(type);
        }
        protected bool IsSerializableDefined(Type type) {
            return TypeService.Instance.IsSerializableDefined(type);
        }
        protected EntityAttribute GetEntityAttribute(Type type) {
            return TypeService.Instance.GetEntityAttribute(type);
        }
        protected PersistAttribute GetPersistAttribute(PropertyInfo prop) {
            return TypeService.Instance.GetPersistAttribute(prop);
        }
        protected bool HasPublicGetter(PropertyInfo prop) {
            return TypeService.Instance.HasPublicGetter(prop);
        }
        protected bool HasPublicSetter(PropertyInfo prop) {
            return TypeService.Instance.HasPublicSetter(prop);
        }
        protected object GetValue(object target, PropertyInfo prop) {
            return TypeService.Instance.GetValue(target, prop);
        }
        protected void SetValue(object target, PropertyInfo prop, object value) {
            TypeService.Instance.SetValue(target, prop, value);
        }
        public void InvokeAction(object target, MethodInfo method, object arg) {
            TypeService.Instance.InvokeAction(target, method, arg);
        }

        protected Type GetTypeObject(string typeStr, string assemStr) {
            return TypeService.Instance.GetTypeObject(typeStr, assemStr);
        }


        // ========================================
        // type
        // ========================================
        protected struct StoreContext {
            public string Id;
            public Type Type;
            public XmlWriter Writer;
            public XmlReader Reader;
        }


    }
}
