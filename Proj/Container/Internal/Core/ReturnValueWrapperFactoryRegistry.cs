/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Reflection;
using Mkamo.Common.Collection;
using Mkamo.Common.Event;
using Mkamo.Common.Reflection;
using Mkamo.Container.Internal.Wrappers;
using Mkamo.Container.Core;

namespace Mkamo.Container.Internal.Core {
    internal class ReturnValueWrapperFactoryRegistry {
        // ========================================
        // field
        // ========================================
        private InsertionOrderedDictionary<Predicate<Type>, IReturnValueWrapperFactory> _predToWrapperFactory;

        // ========================================
        // constructor
        // ========================================
        public ReturnValueWrapperFactoryRegistry() {
        }

        // ========================================
        // property
        // ========================================
        protected InsertionOrderedDictionary<Predicate<Type>, IReturnValueWrapperFactory> _PredToColWrapperTypeMap {
            get {
                if (_predToWrapperFactory == null) {
                    _predToWrapperFactory = new InsertionOrderedDictionary<Predicate<Type>, IReturnValueWrapperFactory>();
                    RegisterWellKnownTypes();
                }
                return _predToWrapperFactory;
            }
        }

        // ========================================
        // method
        // ========================================
        public object CreateWrapper(IEntityContainer container, object owner, object real, PropertyInfo prop) {
            var realType = real.GetType();
            var propType = prop.PropertyType;
            foreach (var pred in _PredToColWrapperTypeMap.InsertionOrderedKeys) {
                if (pred(realType)) {
                    return _PredToColWrapperTypeMap[pred].CreateWrapper(container, owner, real, realType);
                }
                if (pred(propType)) {
                    return _PredToColWrapperTypeMap[pred].CreateWrapper(container, owner, real, propType);
                }
            }
            return null;
        }


        // --- private ---
        private void RegisterWellKnownTypes() {
            {
                Predicate<Type> dictTypePred = (type) => {
                    return GenericTypeUtil.EqualsGenericIDictionary(type);
                };
                _predToWrapperFactory[dictTypePred] = new IDictionaryWrapperFactory();
            }

            {
                Predicate<Type> assocColTypePred = (type) => {
                    return GenericTypeUtil.EqualsGenericAssociationCollection(type);
                };
                _predToWrapperFactory[assocColTypePred] = new AssociationCollectionWrapperFactory();
            }

            {
                Predicate<Type> colTypePred = (type) => {
                    return GenericTypeUtil.EqualsGenericCollection(type);
                };
                _predToWrapperFactory[colTypePred] = new CollectionWrapperFactory();
            }

            {
                Predicate<Type> listTypePred = (type) => {
                    return GenericTypeUtil.EqualsGenericIList(type);
                };
                _predToWrapperFactory[listTypePred] = new IListWrapperFactory();
            }

            {
                Predicate<Type> colTypePred = (type) => {
                    return GenericTypeUtil.EqualsGenericICollection(type);
                };
                _predToWrapperFactory[colTypePred] = new IListWrapperFactory();
            }
        }
    }
}
