/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Core;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Internal.Core {
    internal class TypeService {
        // ========================================
        // static field
        // ========================================
        internal static readonly TypeService Instance = new TypeService();

        // ========================================
        // field
        // ========================================
        private ObjectActivatorCache<Flow> _activatorCache = new ObjectActivatorCache<Flow>();


        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public ObjectActivator<Flow> GetDefaultActivator(Type type) {
            return _activatorCache.GetDefaultActivator(type);
        }

    }
}
