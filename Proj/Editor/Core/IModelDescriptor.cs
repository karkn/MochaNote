/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Editor.Core {
    /// <summary>
    /// Modelの内容を説明する情報を持つクラス．
    /// 子Editor作成時やClipboardペースト時などに親が子を持てるかの確認などに使う．
    /// </summary>
    public interface IModelDescriptor {
        // ========================================
        // property
        // ========================================
        Type ModelType { get; }
        string Description { get; }

        // ========================================
        // method
        // ========================================
    }

    [Serializable]
    public class DefaultModelDescriptor: IModelDescriptor {
        // ========================================
        // field
        // ========================================
        private Type _modelType;
        private string _description;

        // ========================================
        // constructor
        // ========================================
        public DefaultModelDescriptor(Type modelType, string description) {
            _modelType = modelType;
            _description = description;
        }

        public DefaultModelDescriptor(Type modelType): this(modelType, null) {
        }

        public DefaultModelDescriptor(object model): this(model.GetType(), null) {
        }

        // ========================================
        // property
        // ========================================
        public virtual Type ModelType {
            get { return _modelType; }
        }

        public virtual string Description {
            get { return _description; }
        }

        // ========================================
        // method
        // ========================================
    }

}
