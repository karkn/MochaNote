/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;
using Mkamo.Container.Core;

namespace Mkamo.Memopad.Core {
    [Serializable]
    public class MemoIdCollection: Collection<string> {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // static method
        // ========================================
        internal static MemoIdCollection LoadIdsFromSdf(string tableName, SqlServerAccessor accessor) {
            var ret = default(MemoIdCollection);

            try {
                var xml = accessor.LoadTextDataValue(tableName);
                if (xml != null) {
                    var serializer = new DataContractSerializer(typeof(MemoIdCollection));
                    using (var xmlReader = new StringReader(xml))
                    using (var reader = XmlReader.Create(xmlReader)) {
                        ret = serializer.ReadObject(reader) as MemoIdCollection;
                    }
                }
            } catch (Exception e) {
                Logger.Warn("Recent memoinfos load failed", e);
            }

            if (ret == null) {
                ret = new MemoIdCollection();
            }

            return ret;
        }

        internal static MemoIdCollection LoadIdsFromFile(string fileName) {
            var ret = default(MemoIdCollection);

            try {
                if (File.Exists(fileName)) {
                    var serializer = new DataContractSerializer(typeof(MemoIdCollection));
                    using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    using (var reader = XmlReader.Create(stream)) {
                        ret = serializer.ReadObject(reader) as MemoIdCollection;
                    }
                }
            } catch (Exception e) {
                Logger.Warn("Recent memoinfos load failed", e);
            }

            if (ret == null) {
                ret = new MemoIdCollection();
            }

            return ret;
        }

        internal static void SaveIdsToSdf(MemoIdCollection ids, string tableName, SqlServerAccessor accessor) {
            var buf = new StringBuilder();
            using (var writer = XmlWriter.Create(buf)) {
                var serializer = new DataContractSerializer(typeof(MemoIdCollection));
                serializer.WriteObject(writer, ids);
            }

            var xml = buf.ToString();
            if (accessor.IsTextDataExists(tableName)) {
                accessor.UpdateTextData(tableName, xml);
            } else {
                accessor.InsertTextData(tableName, xml);
            }
        }

        internal static void SaveIdsToFile(MemoIdCollection ids, string fileName) {
            var serializer = new DataContractSerializer(typeof(MemoIdCollection));
            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            using (var writer = XmlWriter.Create(stream)) {
                serializer.WriteObject(writer, ids);
            }
        }



        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================

    }
}
