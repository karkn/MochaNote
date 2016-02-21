/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Mkamo.Common.Core {
    public static class Extensions {
        /// <summary>
        /// if(reallyLongIntegerVariableName.In(1,6,9,11))
        /// {
        ///       // do something....
        /// }
        /// 
        /// if(reallyLongStringVariableName.In("string1","string2","string3"))
        /// {
        ///       // do something....
        /// }
        /// 
        /// if(reallyLongMethodParameterName.In(SomeEnum.Value1, SomeEnum.Value2, SomeEnum.Value3, SomeEnum.Value4)
        /// {
        ///   // do something....
        /// }
        /// </summary>
        public static bool In<T>(this T source, params T[] list) {
            if (null == source) {
                throw new ArgumentNullException("source");
            }
            return list.Contains(source);
        }


        public static bool Between<T>(this T actual, T lower, T upper) where T: IComparable<T> {
            return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) < 0;
        }

        public static bool Between<T>(this T actual, T lower, T upper, bool lowerInclusive, bool upperInclusive) where T: IComparable<T> {
            return
                (lowerInclusive ? actual.CompareTo(lower) >= 0 : actual.CompareTo(lower) > 0) &&
                (upperInclusive ? actual.CompareTo(upper) <= 0 : actual.CompareTo(upper) < 0);
        }

        /// <summary>Serializes an object of type T in to an xml string</summary>
        public static string XmlSerialize<T>(this T obj) where T: class, new() {
            if (obj == null) throw new ArgumentNullException("obj");

            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StringWriter()) {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }

        /// <summary>Deserializes an xml string in to an object of Type T</summary>
        public static T XmlDeserialize<T>(this string xml) where T: class, new() {
            if (xml == null) throw new ArgumentNullException("xml");

            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(xml)) {
                try {
                    return (T) serializer.Deserialize(reader);
                } catch {
                    return null; /// Could not be deserialized to this type.
                }
            }
        }
    }
}
