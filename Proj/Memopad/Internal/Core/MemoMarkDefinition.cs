/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Model.Memo;
using System.Drawing;

namespace Mkamo.Memopad.Internal.Core {
    internal class MemoMarkDefinition {
        // ========================================
        // field
        // ========================================
        private MemoMarkKind _kind;
        private string _name;
        private Image _image;
        private MemoMarkLocation _location;

        // ========================================
        // constructor
        // ========================================
        public MemoMarkDefinition(MemoMarkKind kind, string name, Image image, MemoMarkLocation location) {
            _kind = kind;
            _name = name;
            _image = image;
            _location = location;
        }


        // ========================================
        // property
        // ========================================
        public MemoMarkKind Kind {
            get { return _kind; }
            set { _kind = value; }
        }

        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        public Image Image {
            get { return _image; }
            set { _image = value; }
        }

        public MemoMarkLocation Location {
            get { return _location; }
            set { _location = value; }
        }

        // ========================================
        // method
        // ========================================

    }
}
