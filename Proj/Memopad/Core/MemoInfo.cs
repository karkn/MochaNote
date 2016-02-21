/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.String;
using System.ComponentModel;
using Mkamo.Common.Event;
using System.IO;
using Mkamo.Model.Memo;
using System.Runtime.Serialization;
using Mkamo.Container.Core;
using System.Xml;
using Mkamo.Common.Core;

namespace Mkamo.Memopad.Core {
    [Serializable, DataContract]
    public class MemoInfo: DetailedNotifyPropertyChangedBase {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // static method
        // ========================================
        public static bool operator==(MemoInfo a, MemoInfo b) {
            if (object.ReferenceEquals(a, b)) {
                return true;
            }

            if ((object) a == null) {
                return (object) b == null;
            } else {
                return a.Equals(b);
            }
        }

        public static bool operator!=(MemoInfo a, MemoInfo b) {
            return !(a == b);
        }
        

        internal static MemoInfoCollection LoadMemoInfos(SqlServerAccessor accessor) {
            var ret = default(MemoInfoCollection);

            try {
                var xml = accessor.LoadTextDataValue("MemoInfo");
                if (xml != null) {
                    var serializer = new DataContractSerializer(typeof(MemoInfoCollection));
                    using (var xmlReader = new StringReader(xml))
                    using (var reader = XmlReader.Create(xmlReader)) {
                        ret = serializer.ReadObject(reader) as MemoInfoCollection;
                    }
                }

            } catch (Exception e) {
                Logger.Warn("Memoinfos load failed", e);
                throw;
            }

            if (ret == null) {
                ret = new MemoInfoCollection();
            }

            return ret;
        }

        internal static MemoInfoCollection LoadRemovedMemoInfos(SqlServerAccessor accessor) {
            var ret = default(MemoInfoCollection);

            try {
                var xml = accessor.LoadTextDataValue("RemovedMemoInfo");
                if (xml != null) {
                    var serializer = new DataContractSerializer(typeof(MemoInfoCollection));
                    using (var xmlReader = new StringReader(xml))
                    using (var reader = XmlReader.Create(xmlReader)) {
                        ret = serializer.ReadObject(reader) as MemoInfoCollection;
                    }
                }
            } catch (Exception e) {
                Logger.Warn("RemovedMemoinfos load failed", e);
            }

            if (ret == null) {
                ret = new MemoInfoCollection();
            }

            return ret;
        }

        internal static void SaveMemoInfos(MemoInfoCollection memoInfos, SqlServerAccessor accessor) {
            var buf = new StringBuilder();
            using (var writer = XmlWriter.Create(buf)) {
                var serializer = new DataContractSerializer(typeof(MemoInfoCollection));
                serializer.WriteObject(writer, memoInfos);
            }

            var xml = buf.ToString();
            if (accessor.IsTextDataExists("MemoInfo")) {
                accessor.UpdateTextData("MemoInfo", xml);
            } else {
                accessor.InsertTextData("MemoInfo", xml);
            }
        }

        internal static void SaveRemovedMemoInfos(MemoInfoCollection removedMemoInfos, SqlServerAccessor accessor) {
            var buf = new StringBuilder();
            using (var writer = XmlWriter.Create(buf)) {
                var serializer = new DataContractSerializer(typeof(MemoInfoCollection));
                serializer.WriteObject(writer, removedMemoInfos);
            }

            var xml = buf.ToString();
            if (accessor.IsTextDataExists("RemovedMemoInfo")) {
                accessor.UpdateTextData("RemovedMemoInfo", xml);
            } else {
                accessor.InsertTextData("RemovedMemoInfo", xml);
            }
        }

        // ========================================
        // field
        // ========================================
        private string _title;
        private string _mementoId;
        private string _memoId;

        // ========================================
        // constructor
        // ========================================
        public MemoInfo() {
        }

        // ========================================
        // property
        // ========================================
        [DataMember]
        public string Title {
            get { return _title; }
            set {
                if (_title== value) {
                    return;
                }
                var old = _title;
                _title = value;
                OnPropertySet(this, "Title", old, value);
            }
        }

        [DataMember]
        public string MementoId {
            get { return _mementoId; }
            set {
                if (value == _mementoId) {
                    return;
                }
                var old = _mementoId;
                _mementoId = value;
                OnPropertySet(this, "MementoId", old, value);
            }
        }

        [DataMember]
        public string MemoId {
            get { return _memoId; }
            set {
                if (_memoId == value) {
                    return;
                }
                var old = _memoId;
                _memoId = value;
                OnPropertySet(this, "MemoId", old, value);
            }
        }

        // ========================================
        // method
        // ========================================
        public override bool Equals(object obj) {
            var info = obj as MemoInfo;
            if (info == null) {
                return false;
            }
            return
                info._memoId == _memoId &&
                info._mementoId == _mementoId;
        }

        public override int GetHashCode() {
            var ret = (_memoId == null? 0: _memoId.GetHashCode());
            ret = ret ^ (_mementoId == null? 0: _mementoId.GetHashCode());
            return ret;
        }

        public override string ToString() {
            return StringUtil.IsNullOrWhitespace(_title)? "(No Title)": _title;
        }

        public string Dump() {
            return  "title=" + _title + ",memoId=" + _memoId + ",mementoId=" + _mementoId;
        }
    }
}
