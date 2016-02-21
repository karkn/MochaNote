/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace Mkamo.Memopad.Core {
    [Serializable, DataContract]
    public class BootstrapSettings {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // static method
        // ========================================
        public static BootstrapSettings LoadBootstrapSettings(string filename) {
            var ret = default(BootstrapSettings);

            try {
                if (File.Exists(filename)) {
                    var serializer = new DataContractSerializer(typeof(BootstrapSettings));
                    using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
                        ret = serializer.ReadObject(stream) as BootstrapSettings;
                    }
                }
            } catch (Exception e) {
                Logger.Warn("Bootstrap Settings load failed", e);
            }

            if (ret == null) {
                ret = new BootstrapSettings();
            }

            return ret;
        }

        public static void SaveBootstrapSettings(BootstrapSettings settings, string filename) {
            var serializer = new DataContractSerializer(typeof(BootstrapSettings));
            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            using (var writer = XmlDictionaryWriter.CreateTextWriter(stream)) {
                serializer.WriteObject(writer, settings);
            }
        }

        // ========================================
        // property
        // ========================================
        [DataMember]
        public string MemoRoot { get; set; }

        [DataMember]
        public string Attestation { get; set; }

        [DataMember]
        public string Config { get; set; }

        // --- import / export ---
        [DataMember]
        public string LastExportDirectory { get; set; }
        [DataMember]
        public string LastImportDirectory { get; set; }

        // --- proxy ---
        //[DataMember]
        //public string ModelAssemblyVersionReferencedByProxy { get; set; }
    }
}
