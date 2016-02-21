/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Mkamo.Common.Core {
    public static class BinaryFormatterUtil {
        public static byte[] ToBytes(this object obj) {
            using (var stream = new MemoryStream()) {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Flush();
                return stream.GetBuffer();
            }
        }

        public static object FromBytes(this byte[] bytes) {
            using (var stream = new MemoryStream(bytes)) {
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(stream);
            }
        }

    }
}
