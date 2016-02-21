/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Mkamo.Common.DataType;
using System.Xml.XPath;

namespace Mkamo.Container.Core {
    public class SqlServerAccessor {
        // ========================================
        // field
        // ========================================
        private SqlCeConnection _connection;

        /// <summary>
        /// IsTableExistsがtrueであるtableNameをキャッシュ。
        /// テーブルは削除しないのでキャッシュしても問題ない。
        /// </summary>
        private HashSet<string> _existsTables;

        /// <summary>
        /// IsEntityExistsがtrueであるtypeNameとidをキャッシュ。
        /// </summary>
        private HashSet<string> _existsEntityTypeAndIds;

        // ========================================
        // constructor
        // ========================================
        public SqlServerAccessor(SqlCeConnection connection) {
            _connection = connection;
            _existsTables = new HashSet<string>();
            _existsEntityTypeAndIds = new HashSet<string>();
        }

        // ========================================
        // property
        // ========================================
        public SqlCeConnection Connection {
            get { return _connection; }
        }

        // ========================================
        // method
        // ========================================
        public string LoadText(string sql, params SqlCeParameter[] parameters) {
            using (var cmd = CreateCommand(sql, parameters)) {
                var result = cmd.ExecuteReader();
                try {
                    if (result.Read()) {
                        return result.GetString(0);
                    } else {
                        return null;
                    }
                } finally {
                    result.Close();
                }
            }
        }

        public byte[] LoadBytes(string sql, params SqlCeParameter[] parameters) {
            using (var cmd = CreateCommand(sql, parameters)) {

                var result = cmd.ExecuteReader();
                try {
                    if (result.Read()) {
                        var bin = result.GetSqlBinary(0);
                        return bin.Value;
                    } else {
                        return null;
                    }
                } finally {
                    result.Close();
                }
            }
        }

        public byte[] LoadMemento(string id) {
            if (!IsMementoTableExists()) {
                return null;
            }

            var sql = string.Format(
                "SELECT value FROM {0} WHERE id=@id",
                GetMementoTableName()
            );
            return LoadBytes(
                sql,
                new SqlCeParameter("id", id)
            );
        }

        public string LoadEntityValue(string type, string id) {
            if (!IsEntityTableExists(type)) {
                return null;
            }

            var sql = string.Format(
                "SELECT value FROM {0} WHERE id=@id",
                GetEntityTableName(type)
            );
            return LoadText(
                sql,
                new SqlCeParameter("id", id)
            );
        }

        public byte[] LoadSerializablePropertyValue(string type, string id, string name) {
            if (!IsSerializablePropertyTableExists(type)) {
                return null;
            }

            var sql = string.Format(
                "SELECT value FROM {0} WHERE id=@id AND name=@name",
                GetSerializablePropertyTableName(type)
            );
            return LoadBytes(
                sql,
                new SqlCeParameter("id", id),
                new SqlCeParameter("name", name)
            );
        }

        public string LoadExtendedTextDataValue(string type, string id, string name) {
            if (!IsExtendedTextDataTableExists(type)) {
                return null;
            }

            var sql =  string.Format(
                "SELECT value FROM {0} WHERE id=@id AND name=@name",
                GetExtendedTextDataTableName(type)
            );
            return LoadText(
                sql,
                new SqlCeParameter("id", id),
                new SqlCeParameter("name", name)
            );
        }

        public byte[] LoadExtendedBlobValue(string type, string id, string name) {
            if (!IsExtendedBlobDataTableExists(type)) {
                return null;
            }

            var sql = string.Format(
                "SELECT value FROM {0} WHERE id=@id AND name=@name",
                GetExtendedBlobDataTableName(type)
            );
            return LoadBytes(
                sql,
                new SqlCeParameter("id", id),
                new SqlCeParameter("name", name)
            );
        }

        public string LoadTextDataValue(string name) {
            if (!IsTextDataTableExists()) {
                return null;
            }

            var sql = string.Format(
                "SELECT value FROM {0} WHERE name=@name",
                GetTextDataTableName()
            );
            return LoadText(
                sql,
                new SqlCeParameter("name", name)
            );
        }

        // --- insert ---
        public void InsertMemento(string id, byte[] bytes) {
            EnsureMementoTableExists();
            using (var cmd = _connection.CreateCommand()) {
                cmd.CommandText = string.Format(
                    "INSERT INTO {0} VALUES ('{1}', @value)",
                    GetMementoTableName(),
                    id
                );
                cmd.Parameters.Add(new SqlCeParameter("value", bytes));
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertTextData(string name, string text) {
            EnsureTextDataTableExists();
            using (var cmd = _connection.CreateCommand()) {
                cmd.CommandText = string.Format(
                    "INSERT INTO {0} VALUES ('{1}', @value)",
                    GetTextDataTableName(),
                    name
                );
                cmd.Parameters.Add(new SqlCeParameter("value", text));
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertEntity(string typeName, string id, string xml) {
            EnsureEntityTableExists(typeName);
            using (var cmd = _connection.CreateCommand()) {
                cmd.CommandText = string.Format(
                    "INSERT INTO {0} VALUES ('{1}', @value)",
                    GetEntityTableName(typeName),
                    id
                    );
                cmd.Parameters.Add(new SqlCeParameter("value", xml));
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertSerializableProperty(string typeName, string id, string name, byte[] bytes) {
            EnsureSerializablePropertyTableExists(typeName);
            using (var cmd = _connection.CreateCommand()) {
                cmd.CommandText = string.Format(
                    "INSERT INTO {0} VALUES ('{1}', '{2}', @value)",
                    GetSerializablePropertyTableName(typeName),
                    id,
                    name
                );
                cmd.Parameters.Add(new SqlCeParameter("value", bytes));
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertExtendedTextData(string typeName, string id, string name, string text) {
            EnsureExtendedTextDataTableExists(typeName);
            using (var cmd = _connection.CreateCommand()) {
                cmd.CommandText = string.Format(
                    "INSERT INTO {0} VALUES ('{1}', '{2}', @value)",
                    GetExtendedTextDataTableName(typeName),
                    id,
                    name
                );
                cmd.Parameters.Add(new SqlCeParameter("value", text));
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertExtendedBlobData(string typeName, string id, string name, byte[] bytes) {
            EnsureExtendedBlobDataTableExists(typeName);
            using (var cmd = _connection.CreateCommand()) {
                cmd.CommandText = string.Format(
                    "INSERT INTO {0} VALUES ('{1}', '{2}', @value)",
                    GetExtendedBlobDataTableName(typeName),
                    id,
                    name
                );
                cmd.Parameters.Add(new SqlCeParameter("value", bytes));
                cmd.ExecuteNonQuery();
            }
        }

        // --- update ---
        public void UpdateTextData(string name, string text) {
            using (var cmd = _connection.CreateCommand()) {
                cmd.CommandText = string.Format(
                    "UPDATE {0} SET value=@value WHERE name='{1}'",
                    GetTextDataTableName(),
                    name
                );
                cmd.Parameters.Add(new SqlCeParameter("value", text));
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateMemento(string id, byte[] bytes) {
            using (var cmd = _connection.CreateCommand()) {
                cmd.CommandText = string.Format(
                    "UPDATE {0} SET value=@value WHERE id='{1}'",
                    GetMementoTableName(),
                    id
                );
                cmd.Parameters.Add(new SqlCeParameter("value", bytes));
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateEntity(string typeName, string id, string xml) {
            using (var cmd = _connection.CreateCommand()) {
                cmd.CommandText = string.Format(
                    "UPDATE {0} SET value=@value WHERE id='{1}'",
                    GetEntityTableName(typeName),
                    id
                );
                cmd.Parameters.Add(new SqlCeParameter("value", xml));
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateSerializableProperty(string typeName, string id, string name, byte[] bytes) {
            using (var cmd = _connection.CreateCommand()) {
                cmd.CommandText = string.Format(
                    "UPDATE {0} SET name='{2}', value=@value WHERE id='{1}' AND name='{2}'",
                    GetSerializablePropertyTableName(typeName),
                    id,
                    name
                );
                cmd.Parameters.Add(new SqlCeParameter("value", bytes));
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateExtendedTextData(string typeName, string id, string name, string text) {
            using (var cmd = _connection.CreateCommand()) {
                cmd.CommandText = string.Format(
                    "UPDATE {0} SET name='{2}', value=@value WHERE id='{1}' AND name='{2}'",
                    GetExtendedTextDataTableName(typeName),
                    id,
                    name
                );
                cmd.Parameters.Add(new SqlCeParameter("value", text));
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateExtendedBlobData(string typeName, string id, string key, byte[] bytes) {
            using (var cmd = _connection.CreateCommand()) {
                cmd.CommandText = string.Format(
                    "UPDATE {0} SET name='{2}', value=@value WHERE id='{1}' AND name='{2}'",
                    GetExtendedBlobDataTableName(typeName),
                    id,
                    key
                );
                cmd.Parameters.Add(new SqlCeParameter("value", bytes));
                cmd.ExecuteNonQuery();
            }
        }


        // --- remove ---
        public void RemoveEntityTable(string entityType) {
            var tableName = GetEntityTableName(entityType);

            if (IsTableExists(tableName)) {
                using (var cmd = _connection.CreateCommand()) {
                    cmd.CommandText = string.Format(
                        "DROP TABLE {0}",
                        tableName
                    );
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RemoveTextData(string name) {
            if (IsExistsRowByName(GetTextDataTableName(), string.Format("name='{0}'", name))) {
                using (var cmd = _connection.CreateCommand()) {
                    cmd.CommandText = string.Format(
                        "DELETE FROM {0} WHERE name='{1}'",
                        GetTextDataTableName(),
                        name
                    );
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RemoveMemento(string id) {
            if (IsExistsRowById(GetMementoTableName(), string.Format("id='{0}'", id))) {
                using (var cmd = _connection.CreateCommand()) {
                    cmd.CommandText = string.Format(
                        "DELETE FROM {0} WHERE id='{1}'",
                        GetMementoTableName(),
                        id
                    );
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RemoveEntity(string entityType, string id) {
            if (IsExistsRowById(GetEntityTableName(entityType), string.Format("id='{0}'", id))) {
                using (var cmd = _connection.CreateCommand()) {
                    cmd.CommandText = string.Format(
                        "DELETE FROM {0} WHERE id='{1}'",
                        GetEntityTableName(entityType),
                        id
                    );
                    cmd.ExecuteNonQuery();

                    /// キャッシュから削除
                    _existsEntityTypeAndIds.Remove(GetEntityTypeAndIdsKey(entityType, id));
                }
            }
        }

        public void RemoveSerializableProperty(string entityType, string id) {
            if (IsExistsRowById(GetSerializablePropertyTableName(entityType), string.Format("id='{0}'", id))) {
                using (var cmd = _connection.CreateCommand()) {
                    cmd.CommandText = string.Format(
                        "DELETE FROM {0} WHERE id='{1}'",
                        GetSerializablePropertyTableName(entityType),
                        id
                    );
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RemoveExtendedTextData(string entityType, string id) {
            if (IsExistsRowById(GetExtendedTextDataTableName(entityType), string.Format("id='{0}'", id))) {
                using (var cmd = _connection.CreateCommand()) {
                    cmd.CommandText = string.Format(
                        "DELETE FROM {0} WHERE id='{1}'",
                        GetExtendedTextDataTableName(entityType),
                        id
                    );
                    cmd.ExecuteNonQuery();
                }
            }
        }
        
        public void RemoveExtendedBlobData(string entityType, string id) {
            if (IsExistsRowById(GetExtendedBlobDataTableName(entityType), string.Format("id='{0}'", id))) {
                using (var cmd = _connection.CreateCommand()) {
                    cmd.CommandText = string.Format(
                        "DELETE FROM {0} WHERE id='{1}'",
                        GetExtendedBlobDataTableName(entityType),
                        id
                    );
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // --- ids ---
        public IEnumerable<string> GetEntityIds(string type) {
            if (!IsEntityTableExists(type)) {
                return Enumerable.Empty<string>();
            }

            var ret = new List<string>();

            var sql = string.Format(
                "SELECT id FROM {0}",
                GetEntityTableName(type)
            );

            using (var cmd = CreateCommand(sql)) {

                var result = cmd.ExecuteReader();
                try {
                    while (result.Read()) {
                        ret.Add(result.GetString(0));
                    }
                } finally {
                    result.Close();
                }

                return ret;
            }
        }

        public IEnumerable<string> GetEntityIdsLike(string type, string likeParam) {
            if (!IsEntityTableExists(type)) {
                return Enumerable.Empty<string>();
            }

            var ret = new List<string>();

            var sql = string.Format(
                "SELECT id FROM {0} WHERE value LIKE @likeParam",
                GetEntityTableName(type)
            );
            var sqlparam = new SqlCeParameter("likeParam", "%" + likeParam + "%");

            using (var cmd = CreateCommand(sql, sqlparam)) {

                var result = cmd.ExecuteReader();
                try {
                    while (result.Read()) {
                        ret.Add(result.GetString(0));
                    }
                } finally {
                    result.Close();
                }

                return ret;
            }
        }

        public IEnumerable<string> GetMementoIds() {
            if (!IsMementoTableExists()) {
                return new string[0];
            }

            var ret = new List<string>();
            
            var sql = string.Format(
                "SELECT id FROM {0}",
                GetMementoTableName()
            );

            using (var cmd = CreateCommand(sql)) {

                var result = cmd.ExecuteReader();
                try {
                    while (result.Read()) {
                        ret.Add(result.GetString(0));
                    }
                } finally {
                    result.Close();
                }

                return ret;
            }
        }

        // --- row existence ---
        private bool IsExistsRowById(string table, string cond, params SqlCeParameter[] parameters) {
            if (!IsTableExists(table)) {
                return false;
            }

            var sql = string.Format(
                "SELECT id FROM {0} WHERE " + cond,
                table
            );

            using (var cmd = CreateCommand(sql, parameters)) {

                var result = cmd.ExecuteReader();
                try {
                    return result.Read();
                } finally {
                    result.Close();
                }
            }
        }

        private bool IsExistsRowByName(string table, string cond, params SqlCeParameter[] parameters) {
            if (!IsTableExists(table)) {
                return false;
            }

            var sql = string.Format(
                "SELECT name FROM {0} WHERE " + cond,
                table
            );

            using (var cmd = CreateCommand(sql, parameters)) {

                var result = cmd.ExecuteReader();
                try {
                    return result.Read();
                } finally {
                    result.Close();
                }
            }
        }

        public bool IsTextDataExists(string name) {
            return IsExistsRowByName(
                GetTextDataTableName(),
                "name=@name",
                new SqlCeParameter("name", name)
            );
        }

        public bool IsMementoExists(string id) {
            return IsExistsRowById(
                GetMementoTableName(),
                "id=@id",
                new SqlCeParameter("id", id)
            );
        }

        public bool IsEntityExists(string targetType, string id) {
            if (_existsEntityTypeAndIds.Contains(GetEntityTypeAndIdsKey(targetType, id))) {
                return true;
            }

            var ret = IsExistsRowById(
                GetEntityTableName(targetType),
                "id=@id",
                new SqlCeParameter("id", id)
            );
            if (ret) {
                _existsEntityTypeAndIds.Add(GetEntityTypeAndIdsKey(targetType, id));
            }
            return ret;
        }

        public bool IsSerializablePropertyExists(string type, string id, string name) {
            return IsExistsRowById(
                GetSerializablePropertyTableName(type),
                "id=@id AND name=@name",
                new SqlCeParameter("id", id),
                new SqlCeParameter("name", name)
            );
        }

        public bool IsExtendedTextDataExists(string type, string id, string name) {
            return IsExistsRowById(
                GetExtendedTextDataTableName(type),
                "id=@id AND name=@name",
                new SqlCeParameter("id", id),
                new SqlCeParameter("name", name)
            );
        }

        public bool IsExtendedBlobDataExists(string type, string id, string name) {
            return IsExistsRowById(
                GetExtendedBlobDataTableName(type),
                "id=@id AND name=@name",
                new SqlCeParameter("id", id),
                new SqlCeParameter("name", name)
            );
        }

        // --- table existence ---
        public bool IsMementoTableExists() {
            return IsTableExists(GetMementoTableName());
        }

        public bool IsTextDataTableExists() {
            return IsTableExists(GetTextDataTableName());
        }

        public bool IsEntityTableExists(string entityType) {
            return IsTableExists(GetEntityTableName(entityType));
        }

        public bool IsSerializablePropertyTableExists(string entityType) {
            return IsTableExists(GetSerializablePropertyTableName(entityType));
        }

        public bool IsExtendedTextDataTableExists(string entityType) {
            return IsTableExists(GetExtendedTextDataTableName(entityType));
        }

        public bool IsExtendedBlobDataTableExists(string entityType) {
            return IsTableExists(GetExtendedBlobDataTableName(entityType));
        }

        public bool IsTableExists(string tableName) {
            if (_existsTables.Contains(tableName)) {
                return true;
            }

            var sql = string.Format(
                "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='{0}'",
                tableName
            );

            using (var cmd = CreateCommand(sql)) {
                var result = cmd.ExecuteReader();
                try {
                    var ret = result.Read();
                    if (ret) {
                        _existsTables.Add(tableName);
                    }
                    return ret;
                } finally {
                    result.Close();
                }
            }
        }

        // --- ensure table existence ---
        public void EnsureMementoTableExists() {
            if (!IsMementoTableExists()) {
                using (var create = _connection.CreateCommand()) {
                    create.CommandText = "CREATE TABLE " + GetMementoTableName() + " (id NVARCHAR(40) PRIMARY KEY, value IMAGE)";
                    create.ExecuteNonQuery();
                }
            }
        }

        public void EnsureTextDataTableExists() {
            if (!IsTextDataTableExists()) {
                using (var create = _connection.CreateCommand()) {
                    create.CommandText = "CREATE TABLE " + GetTextDataTableName() + " (name NVARCHAR(100) PRIMARY KEY, value NTEXT)";
                    create.ExecuteNonQuery();
                }
            }
        }

        public void EnsureEntityTableExists(string entityType) {
            if (!IsEntityTableExists(entityType)) {
                using (var create = _connection.CreateCommand()) {
                    create.CommandText = "CREATE TABLE " + GetEntityTableName(entityType) + " (id NVARCHAR(40) PRIMARY KEY, value NTEXT)";
                    create.ExecuteNonQuery();
                }
            }
        }

        public void EnsureSerializablePropertyTableExists(string entityType) {
            if (!IsSerializablePropertyTableExists(entityType)) {
                using (var create = _connection.CreateCommand()) {
                    create.CommandText = string.Format(
                        "CREATE TABLE {0} (id NVARCHAR(40) , name NVARCHAR(100), value IMAGE, CONSTRAINT pk PRIMARY KEY (id, name))",
                        GetSerializablePropertyTableName(entityType)
                    );
                    create.ExecuteNonQuery();
                }
            }
        }

        public void EnsureExtendedTextDataTableExists(string entityType) {
            if (!IsExtendedTextDataTableExists(entityType)) {
                using (var create = _connection.CreateCommand()) {
                    create.CommandText = string.Format(
                        "CREATE TABLE {0} (id NVARCHAR(40) , name NVARCHAR(100), value NTEXT, CONSTRAINT pk PRIMARY KEY (id, name))",
                        GetExtendedTextDataTableName(entityType)
                    );
                    create.ExecuteNonQuery();
                }
            }
        }

        public void EnsureExtendedBlobDataTableExists(string entityType) {
            if (!IsExtendedBlobDataTableExists(entityType)) {
                using (var create = _connection.CreateCommand()) {
                    create.CommandText = string.Format(
                        "CREATE TABLE {0} (id NVARCHAR(40) , name NVARCHAR(100), value IMAGE, CONSTRAINT pk PRIMARY KEY (id, name))",
                        GetExtendedBlobDataTableName(entityType)
                    );
                    create.ExecuteNonQuery();
                }
            }
        }

        // --- table name ---
        public string GetMementoTableName() {
            return "Memento";
        }

        public string GetTextDataTableName() {
            return "TextData";
        }

        public string GetEntityTableName(string type) {
            return type.Replace('.', '_');
        }

        public string GetSerializablePropertyTableName(string type) {
            return GetEntityTableName(type) + "_SerializableProperty";
        }

        public string GetExtendedTextDataTableName(string type) {
            return GetEntityTableName(type) + "_TextData";
        }

        public string GetExtendedBlobDataTableName(string type) {
            return GetEntityTableName(type) + "_BlobData";
        }

        // --- misc ---
        private string GetEntityTypeAndIdsKey(string targetType, string id) {
            return targetType + "#" + id;
        }

        private SqlCeCommand CreateCommand(string sql, params SqlCeParameter[] parameters) {
            var ret = _connection.CreateCommand();
            ret.CommandText = sql;

            if (parameters != null && parameters.Length > 0) {
                ret.Parameters.AddRange(parameters);
            }

            return ret;
        }

    }
}
