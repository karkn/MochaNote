using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Mkamo.StyledText.Writer {
    public static class XmlUtil {
        public static void Save(string filename, StyledText.Core.StyledText stext) {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
            using (var writer = XmlDictionaryWriter.CreateBinaryWriter(fs)) {
                var ser = new DataContractSerializer(typeof(StyledText.Core.StyledText), null, int.MaxValue, false, true, null);

                //ser.WriteObject(fs, stext);
                ser.WriteObject(writer, stext);

                writer.Close();
                fs.Close();
            }
        }

        public static StyledText.Core.StyledText Load(string filename) {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
            using (var reader = XmlDictionaryReader.CreateBinaryReader(fs, XmlDictionaryReaderQuotas.Max)) {
                var ser = new DataContractSerializer(typeof(StyledText.Core.StyledText));

                //var deserialized = (StyledText.Core.StyledText) ser.ReadObject(fs);
                var deserialized = (StyledText.Core.StyledText) ser.ReadObject(reader);

                reader.Close();
                fs.Close();

                return deserialized;
            }
        }
    }
}
