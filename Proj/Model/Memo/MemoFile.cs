/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;
using Mkamo.Container.Core;
using System.Runtime.Serialization;

namespace Mkamo.Model.Memo {
    [Externalizable(Type = typeof(MemoFile), FactoryMethodType = typeof(MemoFactory), FactoryMethod = "CreateFile")]
    [DataContract, Serializable]
    public class MemoFile: MemoContent {
        // ========================================
        // field
        // ========================================
        [DataMember(Name = "Name")]
        private string _name;
        [DataMember(Name = "Path")]
        private string _path;
        [DataMember(Name = "IsEmbedded")]
        private bool _isEmbedded;
        [DataMember(Name = "EmbeddedId")]
        private string _embeddedId;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoFile() {
            _name = "";
            _path = "";
            _isEmbedded = false;
            _embeddedId = "";
        }

        // ========================================
        // property
        // ========================================
        public override bool IsMarkable {
            get { return true; }
        }

        [Persist, External]
        public virtual string Name {
            get { return _name; }
            set{
                if (_name== value) {
                    return;
                }
                var old = _name;
                _name = value;
                OnPropertySet(this, "Name", old, value);
            }
        }

        [Persist, External]
        public virtual string Path {
            get { return _path; }
            set{
                if (_path== value) {
                    return;
                }
                var old = _path;
                _path = value;
                OnPropertySet(this, "Path", old, value);
            }
        }

        [Persist, External]
        public virtual bool IsEmbedded {
            get { return _isEmbedded; }
            set{
                if (_isEmbedded== value) {
                    return;
                }
                var old = _isEmbedded;
                _isEmbedded = value;
                OnPropertySet(this, "IsEmbedded", old, value);
            }
        }

        [Persist, External]
        public virtual string EmbeddedId {
            get { return _embeddedId; }
            set{
                if (_embeddedId== value) {
                    return;
                }
                var old = _embeddedId;
                _embeddedId = value;
                OnPropertySet(this, "EmbeddedId", old, value);
            }
        }

        // ========================================
        // method
        // ========================================

    }
}
