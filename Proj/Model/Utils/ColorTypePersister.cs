/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Container.Core;
using System.Drawing;

namespace Mkamo.Model.Utils {
    public class ColorTypePersister: ITypePersister {
        // ========================================
        // static field
        // ========================================
        private static readonly string KeyArgb = "argb";

        // ========================================
        // method
        // ========================================
        public void Save(object value, IDictionary<string, string> values) {
            values[KeyArgb] = ((Color) value).ToArgb().ToString();
        }

        public object Load(IDictionary<string, string> values) {
            return Color.FromArgb(int.Parse(values[KeyArgb]));
        }
    }
}
