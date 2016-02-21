/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Association {
    public static class AssociationUtil {
        [Serializable]
        public enum EnsureResult {
            None, /// 何も行われず，関連は変わらなかった
            Set,  /// 関連が設定された
            Unset, /// 関連が解除された
        }

        public static EnsureResult EnsureAssociation<T>(
            T oldValue,
            T newValue,
            Action<T> fieldSetter,
            Action<T> inverseAssociator,
            Action<T> inverseUnassociator
        ) 
            where T: class
        {
            if (oldValue == newValue) {
                return EnsureResult.None;
            }

            if (oldValue != null) {
                fieldSetter(null);
                inverseUnassociator(oldValue);
            }

            fieldSetter(newValue);

            if (newValue != null) {
                inverseAssociator(newValue);
                return EnsureResult.Set;
            } else {
                return EnsureResult.Unset;
            }
        }

    }
}
