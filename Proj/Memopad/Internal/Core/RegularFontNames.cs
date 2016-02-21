/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Text;
using System.Drawing;

namespace Mkamo.Memopad.Internal.Core {
    internal class RegularFontNames {
        // ========================================
        // static field
        // ========================================
        private static RegularFontNames _instance;

        // ========================================
        // static property
        // ========================================
        public static RegularFontNames Instance {
            get {
                return _instance ?? (_instance = new RegularFontNames());
            }
        }


        // ========================================
        // field
        // ========================================
        private string[] _fontNames;

        // ========================================
        // constructor
        // ========================================
        public RegularFontNames() {
        }

        // ========================================
        // property
        // ========================================
        public string[] FontNames {
            get {
                if (_fontNames == null) {
                    InitFontNames();
                }
                return _fontNames;
            }
        }

        // ========================================
        // method
        // ========================================
        private void InitFontNames() {
            var names = new List<string>();
            using (var fcol = new InstalledFontCollection()) {
                var ffs = fcol.Families;
                foreach (var ff in ffs) {
                    //if (ff.IsStyleAvailable(FontStyle.Regular)) {
                        names.Add(ff.Name);
                    //}
                    ff.Dispose();
                }
            }
            _fontNames = names.ToArray();
        }
    }
}
