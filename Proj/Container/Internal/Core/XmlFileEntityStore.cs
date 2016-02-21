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
using Mkamo.Common.DataType;

namespace Mkamo.Container.Internal.Core {
    internal class XmlFileEntityStore: XmlEntityStore {
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

        // ========================================
        // field
        // ========================================
        private string _storeRoot;

        // ========================================
        // constructor
        // ========================================
        public XmlFileEntityStore(): this(DefaultStoreRootPath) {
        }

        public XmlFileEntityStore(string storeRoot): base() {
            if (!PathUtil.IsValidPath(storeRoot)) {
                throw new ArgumentException("storeRoot is not valid: " + storeRoot);
            }
            _storeRoot = storeRoot;
            PathUtil.EnsureDirectoryExists(_storeRoot);
        }


        // ========================================
        // property
        // ========================================
        public string StoreRoot {
            get { return _storeRoot; }
        }

        // ========================================
        // method
        // ========================================
        // === IEntityStore ==========
        public override void Begin() {
        }
    
        public override void Commit() {
        }

        public override void Load(object target) {
            var entity = EntityContainer.AsEntity(target);
            if (entity == null) {
                throw new ArgumentNullException("target");
            }

            var entityType = entity.Type;
            var id = entity.Id;
            var contentFilePath = GetEntityContentFilePath(entityType, id);

            var settings = new XmlReaderSettings();
            {
                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;
            }

            using (var reader = XmlReader.Create(contentFilePath, settings)) {
                var context = new StoreContext() {
                    Id = id,
                    Type = entityType,
                    Reader = reader,
                };

                reader.Read(); /// fetch doctype
                reader.Read(); /// fetch Type
                LoadType(context, target, entityType);
            }
        }

        public override void Insert(object target) {
            Save(target);
        }

        public override void Update(object target) {
            Save(target);
        }

        public override bool Remove(object target) {
            var entity = EntityContainer.AsEntity(target);
            if (entity == null) {
                throw new ArgumentNullException("target");
            }

            var entityType = entity.Type;
            var id = entity.Id;
            var entityDirPath = GetEntityDirectoryPath(entityType, id);

            if (Directory.Exists(entityDirPath)) {
                Directory.Delete(entityDirPath, true);
                return true;
            } else {
                return false;
            }
        }

        public override bool IsEntityExists(Type targetType, string id) {
            return File.Exists(GetEntityContentFilePath(targetType, id));
        }

        public override IEnumerable<string> GetIds(Type type) {
            var dirInfo = new DirectoryInfo(GetTypeDirectoryPath(type));
            if (!dirInfo.Exists) {
                return new string[0];
            }

            var subdirs = dirInfo.GetDirectories();
            return subdirs.Select(subdir => subdir.Name).ToArray();
        }

        public override IEnumerable<string> GetIdsLike(Type type, string likeParam) {
            throw new NotImplementedException();
        }

        public override string LoadExtendedTextData(Type type, string id, string key) {
            if (!IsEntityExists(type, id)) {
                return null;
            }

            var filepath = GetEntityExtendedDataFilePath(type, id, key);
            try {
                if (!File.Exists(filepath)) {
                    return null;
                }
                return File.ReadAllText(filepath, Encoding.UTF8);
            } catch (Exception e) {
                Logger.Warn("Load extended data failed", e);
                return null;
            }
        }

        public override void SaveExtendedTextData(Type type, string id, string key, string value) {
            if (!IsEntityExists(type, id)) {
                return;
            }

            var filepath = GetEntityExtendedDataFilePath(type, id, key);
            try {
                File.WriteAllText(filepath, value, Encoding.UTF8);
            } catch (Exception e) {
                Logger.Warn("Save extended data failed", e);
            }
        }

        public override byte[] LoadExtendedBinaryData(Type type, string id, string key) {
            if (!IsEntityExists(type, id)) {
                return null;
            }

            var filepath = GetEntityExtendedObjectDataFilePath(type, id, key);
            try {
                if (!File.Exists(filepath)) {
                    return null;
                }
                return File.ReadAllBytes(filepath);
            } catch (Exception e) {
                Logger.Warn("Load extended data object failed", e);
                return null;
            }
        }

        public override void SaveExtendedBinaryData(Type type, string id, string key, byte[] value) {
            if (!IsEntityExists(type, id)) {
                return;
            }

            var filepath = GetEntityExtendedObjectDataFilePath(type, id, key);
            try {
                File.WriteAllBytes(filepath, value);
            } catch (Exception e) {
                Logger.Warn("Save extended data object failed", e);
            }
        }

        //public override IEnumerable<string> GetPropertyValues(Type type, string propName) {
        //    throw new NotImplementedException();
        //}

        //public override IEnumerable<Tuple<string, string>> GetIdsAndPropertyValues(Type type, string propName) {
        //    throw new NotImplementedException();
        //}

        //public override IEnumerable<string> GetIdsWherePropertyValueIsEmpty(Type type, string propName) {
        //    throw new NotImplementedException();
        //}

        public override string LoadRawData(Type type, string id) {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// idとプロパティの値を格納したdictionaryを返す．
        /// </summary>
        //public override IDictionary<string, object> GetPropertyValues(Type type, string propName) {
        //    var ret = new Dictionary<string, object>();

        //    //var typeDirPath = GetTypeDirectoryPath(type);
        //    //var typeDir = new DirectoryInfo(typeDirPath);
        //    //if (!typeDir.Exists) {
        //    //    return ret;
        //    //}


        //    //var entityDirs = typeDir.GetDirectories();

        //    //var prop = type.GetProperty(propName);
        //    //Contract.Requires(prop != null);

        //    //var propType = prop.PropertyType;
        //    //Contract.Requires(
        //    //    propType.IsPrimitive ||
        //    //        typeof(string).IsAssignableFrom(propType) ||
        //    //        typeof(decimal).IsAssignableFrom(propType) ||
        //    //        typeof(DateTime).IsAssignableFrom(propType)
        //    //);

        //    //foreach (var entityDir in entityDirs) {
        //    //    var entityXmlFilePath = Path.Combine(entityDir.FullName, EntityContentFilename);
        //    //    var id = entityDir.Name;

        //    //    var root = XElement.Load(entityXmlFilePath);
        //    //    var propElem = root.Element(propName);
        //    //    var children = propElem.Elements();

        //    //    Contract.Requires(children.Any());
                
        //    //    var first = children.First();
        //    //    var name = first.Name.LocalName;
        //    //    switch (name) {
        //    //        case IntElementName: {
        //    //            ret.Add(id, (int) first);
        //    //            break;
        //    //        }
        //    //        case BoolElementName: {
        //    //            ret.Add(id, (bool) first);
        //    //            break;
        //    //        }
        //    //        case LongElementName: {
        //    //            ret.Add(id, (long) first);
        //    //            break;
        //    //        }
        //    //        case FloatElementName: {
        //    //            ret.Add(id, (float) first);
        //    //            break;
        //    //        }
        //    //        case DoubleElementName: {
        //    //            ret.Add(id, (double) first);
        //    //            break;
        //    //        }
        //    //        case ByteElementName: {
        //    //            ret.Add(id, (byte) (int) first);
        //    //            break;
        //    //        }
        //    //        case CharElementName: {
        //    //            ret.Add(id, (char) (int) first);
        //    //            break;
        //    //        }
        //    //        case DecimalElementName: {
        //    //            ret.Add(id, (decimal) first);
        //    //            break;
        //    //        }
        //    //        case DatetimeElementName: {
        //    //            ret.Add(id, (DateTime) first);
        //    //            break;
        //    //        }
        //    //        case StringElementName: {
        //    //            ret.Add(id, (string) first);
        //    //            break;
        //    //        }
        //    //    }
        //    //}

        //    return ret;
        //}


        // --- protected ---
        protected void Save(object target) {
            var entity = EntityContainer.AsEntity(target);
            if (entity == null) {
                throw new ArgumentNullException("target");
            }

            var entityType = entity.Type;
            var id = entity.Id;
            PathUtil.EnsureDirectoryExists(GetEntityDirectoryPath(entityType, id));
            var contentFilePath = GetEntityContentFilePath(entityType, id);

            var settings = new XmlWriterSettings();
            {
                settings.Indent = true;
            }

            _NValues = 0;
            using (var writer = XmlWriter.Create(contentFilePath, settings)) {
                var context = new StoreContext() {
                    Id = id,
                    Type = entityType,
                    Writer = writer,
                };

                writer.WriteStartDocument();
                {
                    SaveType(context, target, entityType);
                }
                writer.WriteEndDocument();
            }
        }

        protected override object LoadSerializableObject(string typeName, string id, string propName) {
            var filename = Path.Combine(
                GetEntityDirectoryPath(typeName, id),
                propName
            );
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
                var formatter = new BinaryFormatter();
                //formatter.Binder = new DelegatingSerializationBinder(
                //    (assemName, typeName) => {
                //        typeName = typeName.Replace("PublicKeyToken=null", "PublicKeyToken=89184b2d63ba68a6");
                //        var name = new AssemblyName(assemName);
                //        var assem = default(Assembly);
                //        try {
                //            assem = Assembly.Load(name.Name);
                //        } catch {
                //            assem = Assembly.Load(name);
                //        }
                //        return assem.GetType(typeName);
                //    }
                //);
                return formatter.Deserialize(stream);
            }
        }

        protected override void SaveSerializableBytes(string typeName, string id, string propName, object value) {
            var path = Path.Combine(GetEntityDirectoryPath(typeName, id), propName + ".ser");
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write)) {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, value);
            }
        }

        protected string GetTypeDirectoryPath(Type type) {
            return Path.Combine(StoreRoot, type.FullName);
        }
        protected string GetTypeDirectoryPath(string type) {
            return Path.Combine(StoreRoot, type);
        }
        protected string GetEntityDirectoryPath(Type type, string id) {
            return Path.Combine(GetTypeDirectoryPath(type), id);
        }
        protected string GetEntityDirectoryPath(string type, string id) {
            return Path.Combine(GetTypeDirectoryPath(type), id);
        }
        protected string GetEntityContentFilePath(Type type, string id) {
            return Path.Combine(GetEntityDirectoryPath(type, id), EntityContentFilename);
        }
        protected string GetEntityExtendedDataFilePath(Type type, string id, string key) {
            return Path.Combine(GetEntityDirectoryPath(type, id), key + EntityExtendedDataFileExtension);
        }
        protected string GetEntityExtendedObjectDataFilePath(Type type, string id, string key) {
            return Path.Combine(GetEntityDirectoryPath(type, id), key + EntityExtendedObjectDataFileExtension);
        }

    }
}
