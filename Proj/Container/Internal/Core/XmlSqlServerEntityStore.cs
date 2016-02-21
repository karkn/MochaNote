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

namespace Mkamo.Container.Internal.Core {
    internal class XmlSqlServerEntityStore: XmlEntityStore {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(global::System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private SqlServerAccessor _memoAccessor;
        private SqlServerAccessor _exDataAccessor;

        // ========================================
        // constructor
        // ========================================
        public XmlSqlServerEntityStore(SqlCeConnection memoConn, SqlCeConnection exDataConn): base() {
            if (memoConn == null || exDataConn == null) {
                throw new ArgumentException("memoConn or exDataConn");
            }

            _memoAccessor = new SqlServerAccessor(memoConn);
            _exDataAccessor = new SqlServerAccessor(exDataConn);
        }


        // ========================================
        // property
        // ========================================

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
            var contentStream = _memoAccessor.LoadEntityValue(entityType.FullName, id);

            var settings = new XmlReaderSettings();
            {
                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;
            }

            using (var reader = XmlReader.Create(new StringReader(contentStream), settings)) {
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
            Save(target, false);
        }

        public override void Update(object target) {
            Save(target, true);
        }

        public override bool Remove(object target) {
            var entity = EntityContainer.AsEntity(target);
            if (entity == null) {
                throw new ArgumentNullException("target");
            }

            var entityType = entity.Type.FullName;
            var id = entity.Id;

            _memoAccessor.RemoveEntity(entityType, id);
            _memoAccessor.RemoveSerializableProperty(entityType, id);

            _exDataAccessor.RemoveExtendedTextData(entityType, id);
            _exDataAccessor.RemoveExtendedBlobData(entityType, id);

            return true;
        }

        public override bool IsEntityExists(Type targetType, string id) {
            return _memoAccessor.IsEntityExists(targetType.FullName, id);
        }

        public override IEnumerable<string> GetIds(Type type) {
            return _memoAccessor.GetEntityIds(type.FullName);
        }

        public override IEnumerable<string> GetIdsLike(Type type, string likeParam) {
            return _memoAccessor.GetEntityIdsLike(type.FullName, likeParam);
        }

        public override string LoadExtendedTextData(Type type, string id, string key) {
            return _exDataAccessor.LoadExtendedTextDataValue(type.FullName, id, key);
        }

        public override void SaveExtendedTextData(Type type, string id, string key, string value) {
            if (_exDataAccessor.IsExtendedTextDataExists(type.FullName, id, key)) {
                _exDataAccessor.UpdateExtendedTextData(type.FullName, id, key, value);
            } else {
                _exDataAccessor.InsertExtendedTextData(type.FullName, id, key, value);
            }
        }

        public override byte[] LoadExtendedBinaryData(Type type, string id, string key) {
            return _exDataAccessor.LoadExtendedBlobValue(type.FullName, id, key);
        }

        public override void SaveExtendedBinaryData(Type type, string id, string key, byte[] value) {
            if (_exDataAccessor.IsExtendedBlobDataExists(type.FullName, id, key)) {
                _exDataAccessor.UpdateExtendedBlobData(type.FullName, id, key, value);
            } else {
                _exDataAccessor.InsertExtendedBlobData(type.FullName, id, key, value);
            }
        }

        /// <summary>
        /// idのxmlデータを返す。
        /// </summary>
        public override string LoadRawData(Type type, string id) {
            return _memoAccessor.LoadEntityValue(type.FullName, id);
        }

        // --- protected ---
        protected void Save(object target, bool update) {
            var entity = EntityContainer.AsEntity(target);
            if (entity == null) {
                throw new ArgumentNullException("target");
            }

            var entityType = entity.Type;
            var id = entity.Id;

            var settings = new XmlWriterSettings();
            {
                settings.Indent = false;
            }

            var swriter = new StringWriter();

            _NValues = 0;

            using (var writer = XmlWriter.Create(swriter, settings)) {
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

            if (update) {
                _memoAccessor.UpdateEntity(entityType.FullName, id, swriter.ToString());
            } else {
                _memoAccessor.InsertEntity(entityType.FullName, id, swriter.ToString());
            }
        }

        protected override object LoadSerializableObject(string typeName, string id, string propName) {
            var bytes = _memoAccessor.LoadSerializablePropertyValue(typeName, id, propName);
            using (var stream = new MemoryStream(bytes)) {
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
            using (var stream = new MemoryStream()) {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, value);
                if (_memoAccessor.IsSerializablePropertyExists(typeName, id, propName)) {
                    _memoAccessor.UpdateSerializableProperty(typeName, id, propName, stream.GetBuffer());
                } else {
                    _memoAccessor.InsertSerializableProperty(typeName, id, propName, stream.GetBuffer());
                }
            }
        }

    }
}
