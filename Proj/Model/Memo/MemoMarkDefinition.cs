/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Container.Core;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;

namespace Mkamo.Model.Memo {
    //[Externalizable(Type = typeof(MemoMarkDefinition), FactoryMethodType = typeof(MemoFactory), FactoryMethod = "CreateMarkDefinition")]
    //public class MemoMarkDefinition: MemoElement {
    //    // ========================================
    //    // field
    //    // ========================================
    //    private string _name;
    //    private IImageDescription _image;
    //    private MemoMarkLocation _location;

    //    // ========================================
    //    // constructor
    //    // ========================================
    //    protected internal MemoMarkDefinition() {
    //        _name = "";
    //    }

    //    // ========================================
    //    // property
    //    // ========================================
    //    [Persist, External]
    //    public virtual string Name {
    //        get { return _name; }
    //        set{
    //            if (_name== value) {
    //                return;
    //            }
    //            var old = _name;
    //            _name = value;
    //            OnPropertySet(this, "Name", old, value);
    //        }
    //    }

    //    [Persist, External]
    //    public virtual IImageDescription Image {
    //        get { return _image; }
    //        set{
    //            if (_image == value) {
    //                return;
    //            }
    //            var old = _image;
    //            _image = value;
    //            OnPropertySet(this, "Image", old, value);
    //        }
    //    }

    //    [Persist, External]
    //    public virtual MemoMarkLocation Location {
    //        get { return _location; }
    //        set {
    //            if (_location == value) {
    //                return;
    //            }
    //            var old = _location;
    //            _location = value;
    //            OnPropertySet(this, "Location", old, value);
    //        }
    //    }

    //    // ========================================
    //    // method
    //    // ========================================

    //}
}
