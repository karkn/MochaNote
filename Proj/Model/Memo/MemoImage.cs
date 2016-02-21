/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Event;
using Mkamo.Container.Core;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;
using Mkamo.Common.Forms.Descriptions;
using System.Runtime.Serialization;

namespace Mkamo.Model.Memo {
    [Externalizable(Type = typeof(MemoImage), FactoryMethodType = typeof(MemoFactory), FactoryMethod = "CreateImage")]
    [DataContract, Serializable]
    public class MemoImage: MemoContent {
        // ========================================
        // field
        // ========================================
        [DataMember(Name = "Image")]
        private IImageDescription _image;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoImage() {
        }

        // ========================================
        // property
        // ========================================
        public override bool IsMarkable {
            get { return true; }
        }

        [Persist, External]
        public virtual IImageDescription Image {
            get { return _image; }
            set{
                if (_image == value) {
                    return;
                }
                var old = _image;
                _image = value;
                OnPropertySet(this, "Image", old, value);
            }
        }

        // ========================================
        // method
        // ========================================

    }
}
