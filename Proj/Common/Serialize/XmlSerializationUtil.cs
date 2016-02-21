using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;

namespace Mkamo.Common.Serialize {
    public static class XmlSerializationUtil {
        public static void Save<T>(string filename, T obj) {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
            using (var writer = XmlDictionaryWriter.CreateBinaryWriter(fs)) {
                var ser = new DataContractSerializer(typeof(T), null, int.MaxValue, false, true, null);

                ser.WriteObject(writer, obj);

                writer.Close();
                fs.Close();
            }
        }

        public static T Load<T>(string filename) {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
            using (var reader = XmlDictionaryReader.CreateBinaryReader(fs, XmlDictionaryReaderQuotas.Max)) {
                var ser = new DataContractSerializer(typeof(T));

                var deserialized = (T) ser.ReadObject(reader);

                reader.Close();
                fs.Close();

                return deserialized;
            }
        }

    }
}
