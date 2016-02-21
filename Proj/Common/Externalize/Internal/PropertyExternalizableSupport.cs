/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Mkamo.Common.Reflection;
using System.Security.Permissions;
using System.Collections;

namespace Mkamo.Common.Externalize.Internal {
    internal struct PropertyExternalizableSupport {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private object _target;
        private Type _type;
        private PropertyInfo[] _properties;

        // ========================================
        // constructor
        // ========================================
        public PropertyExternalizableSupport(object target) {
            _target = target;
            _type = _target.GetType();
            _properties = _type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public void SaveTo(IMemento memento, ExternalizeContext context) {
            foreach (var prop in _properties) {
                var externalAttr = TypeService.Instance.GetExternalAttribute(prop);
                if (externalAttr == null) {
                    continue;
                }

                var value = prop.GetValue(_target, null);
                if (value == null) {
                    continue;
                }

                var valueType = value.GetType();
                var propType = prop.PropertyType;
                var propName = prop.Name;


                if (GenericTypeUtil.IsGenericIDictionary(propType)) {
                    var elemTypes = GenericTypeUtil.GetGenericArgumentOfGenericIDictionary(valueType);
                    var isKeyExternalizable = IsExternalizable(elemTypes[0]);
                    var isValueExternalizable = IsExternalizable(elemTypes[1]);

                    var dictType = typeof(IDictionary<,>).MakeGenericType(elemTypes);
                    var getKeysMethod = dictType.GetMethod("get_Keys");
                    var getItemMethod = dictType.GetMethod("get_Item");

                    if (isKeyExternalizable) {
                        if (isValueExternalizable) {
                            var dict = new Dictionary<IMemento, IMemento>();
                            var keys = getKeysMethod.Invoke(value, null) as ICollection;
                            foreach (var key in keys) {
                                var entryValue = getItemMethod.Invoke(value, new[] { key });
                                var keyMem = context.GetMemento(propName, key);
                                var valMem = context.GetMemento(propName, entryValue);
                                if (keyMem != null && valMem != null) {
                                    dict.Add(keyMem, valMem);
                                }
                            }
                            memento.WriteSerializable(propName, dict);
                        } else {
                            var dict = new Dictionary<IMemento, object>();
                            var keys = getKeysMethod.Invoke(value, null) as ICollection;
                            foreach (var key in keys) {
                                var entryValue = getItemMethod.Invoke(value, new[] { key });
                                var keyMem = context.GetMemento(propName, key);
                                if (keyMem != null) {
                                    dict.Add(keyMem, entryValue);
                                }
                            }
                            memento.WriteSerializable(propName, dict);
                        }
                    } else {
                        if (isValueExternalizable) {
                            var dict = new Dictionary<object, IMemento>();
                            var keys = getKeysMethod.Invoke(value, null) as ICollection;
                            foreach (var key in keys) {
                                var entryValue = getItemMethod.Invoke(value, new[] { key });
                                var valMem = context.GetMemento(propName, entryValue);
                                if (valMem != null) {
                                    dict.Add(key, valMem);
                                }
                            }
                            memento.WriteSerializable(propName, dict);
                        } else {
                            var dict = new Dictionary<object, object>();
                            var keys = getKeysMethod.Invoke(value, null) as ICollection;
                            foreach (var key in keys) {
                                var entryValue = getItemMethod.Invoke(value, new[] { key });
                                dict.Add(key, entryValue);
                            }
                            memento.WriteSerializable(propName, dict);
                        }
                    }

                } else if (GenericTypeUtil.IsGenericICollection(propType)) {
                    var elemType = GenericTypeUtil.GetGenericArgumentOfGenericICollection(valueType);
                    var isElemExternalizable = IsExternalizable(elemType);

                    if (isElemExternalizable) {
                        var list = new List<IMemento>();
                        foreach (var v in value as IEnumerable) {
                            var elem = context.GetMemento(propName, v);
                            if (elem != null) {
                                list.Add(elem);
                            }
                        }
                        memento.WriteSerializable(propName, list);
                    } else {
                        var list = new List<object>();
                        foreach (var v in value as IEnumerable) {
                            list.Add(v);
                        }
                        memento.WriteSerializable(propName, list);
                    }
                
                } else if (GenericTypeUtil.IsGenericIEnumerable(propType) && externalAttr.Add != null) {
                    var elemType = GenericTypeUtil.GetGenericArgumentOfGenericIEnumerable(valueType);
                    var isElemExternalizable = IsExternalizable(elemType);

                    if (isElemExternalizable) {
                        var list = new List<IMemento>();
                        foreach (var v in value as IEnumerable) {
                            var elem = context.GetMemento(propName, v);
                            if (elem != null) {
                                list.Add(elem);
                            }
                        }
                        memento.WriteSerializable(propName, list);
                    } else {
                        var list = new List<object>();
                        foreach (var v in value as IEnumerable) {
                            list.Add(v);
                        }
                        memento.WriteSerializable(propName, list);
                    }

                } else {
                    if (valueType.IsPrimitive) {
                        memento.WriteSerializable(propName, value);
                    } else {
                        if (IsExternalizable(valueType)) {
                            memento.WriteExternalizable(propName, value);
                        } else {
                            if (externalAttr.Clone == null) {
                                memento.WriteSerializable(propName, value);
                            } else {
                                try {
                                    var flags =
                                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                                    var clone = _type.GetMethod(externalAttr.Clone, flags);
                                    var cloned = clone.Invoke(_target, null);
                                    memento.WriteSerializable(propName, cloned);
                                } catch (Exception e) {
                                    Logger.Error(
                                        string.Format(
                                            "Failed to invoke {0}.{1} for clone",
                                            _type.Name, externalAttr.Clone
                                        ),
                                        e
                                    );
                                    throw;
                                }
                            }
                        }
                    }
                }
            }

            var externalizableAttr = TypeService.Instance.GetExternalizableAttribute(_type);

            if (externalizableAttr.FactoryMethodType != null && externalizableAttr.FactoryMethod != null) {
                var factoryMethod = externalizableAttr.FactoryMethodType.GetMethod(
                    externalizableAttr.FactoryMethod,
                    BindingFlags.Public | BindingFlags.Static
                );
                if (factoryMethod != null) {
                    memento.FactoryMethod = factoryMethod;

                    var paras = externalizableAttr.FactoryMethodParamKeys;
                    if (paras != null) {
                        memento.FactoryMethodParamKeys = paras;
                    }
                }
            }

            if (externalizableAttr.ConstructorParamKeys != null) {
                memento.ConstructorParamKeys = externalizableAttr.ConstructorParamKeys;
            }

            if (externalizableAttr.Saved != null) {
                var saved = _type.GetMethod(externalizableAttr.Saved);
                saved.Invoke(_target, new object[] { memento, context });
            }
        }

        public void LoadFrom(IMemento memento, ExternalizeContext context) {
            var type = _target.GetType();
            foreach (var prop in _properties) {
                var externalAttr = TypeService.Instance.GetExternalAttribute(prop);
                if (externalAttr == null) {
                    continue;
                }

                var propName = prop.Name;
                var value = memento.ReadSerializable(propName);
                if (value == null) {
                    continue;
                }

                var valueType = value.GetType();
                var propType = prop.PropertyType;

                if (GenericTypeUtil.IsGenericIDictionary(propType)) {
                    var elemTypes = GenericTypeUtil.GetGenericArgumentOfGenericIDictionary(valueType);
                    var dictKeyType = elemTypes[0];
                    var dictValueType = elemTypes[1];

                    var setItemMethod = propType.GetMethod("set_Item");

                    if (typeof(IMemento).IsAssignableFrom(dictKeyType)) {
                        if (typeof(IMemento).IsAssignableFrom(dictValueType)) {
                            var dict = memento.ReadSerializable(propName) as IDictionary<IMemento, IMemento>;
                            foreach (var pair in dict) {
                                var keyEx = context.GetExternalizable(propName, pair.Key);
                                var valEx = context.GetExternalizable(propName, pair.Value);
                                if (keyEx != null && valEx != null) {
                                    setItemMethod.Invoke(value, new[] { keyEx, valEx });
                                }
                            }
                        } else {
                            var dict = memento.ReadSerializable(propName) as IDictionary<IMemento, object>;
                            foreach (var pair in dict) {
                                var keyEx = context.GetExternalizable(propName, pair.Key);
                                if (keyEx != null) {
                                    setItemMethod.Invoke(value, new[] { keyEx, pair.Value });
                                }
                            }
                        }
                    } else {
                        if (typeof(IMemento).IsAssignableFrom(dictValueType)) {
                            var dict = memento.ReadSerializable(propName) as IDictionary<object, IMemento>;
                            foreach (var pair in dict) {
                                var valEx = context.GetExternalizable(propName, pair.Value);
                                if (valEx != null) {
                                    setItemMethod.Invoke(value, new[] { pair.Key, valEx });
                                }
                            }
                        } else {
                            var dict = memento.ReadSerializable(propName) as IDictionary<object, IMemento>;
                            foreach (var pair in dict) {
                                setItemMethod.Invoke(value, new[] { pair.Key, pair.Value, });
                            }
                        }
                    }

                } else if (GenericTypeUtil.IsGenericICollection(propType)) {
                    var elemType = GenericTypeUtil.GetGenericArgumentOfGenericICollection(valueType);

                    if (typeof(IMemento).IsAssignableFrom(elemType)) {
                        var add = externalAttr.Add;
                        var addMethod = add != null? type.GetMethod(add): propType.GetMethod("Add");
                        var list = prop.GetValue(_target, null);
                        var externalizables = memento.ReadExternalizables(propName);
                        foreach (var ex in externalizables) {
                            addMethod.Invoke(list, new[] { ex });
                        }

                    } else {
                        var add = externalAttr.Add;
                        var addMethod = add != null? type.GetMethod(add): propType.GetMethod("Add");
                        var list = prop.GetValue(_target, null);
                        var deserialized = memento.ReadSerializable(propName) as IEnumerable;
                        foreach (var ex in deserialized) {
                            addMethod.Invoke(list, new[] { ex });
                        }
                    }

                } else if (GenericTypeUtil.IsGenericIEnumerable(propType) && externalAttr.Add != null) {
                    var elemType = GenericTypeUtil.GetGenericArgumentOfGenericIEnumerable(valueType);

                    if (typeof(IMemento).IsAssignableFrom(elemType)) {
                        var add = externalAttr.Add;
                        var addMethod = type.GetMethod(add);
                        var externalizables = memento.ReadExternalizables(propName);
                        foreach (var ex in externalizables) {
                            addMethod.Invoke(_target, new[] { ex });
                        }

                    } else {
                        var add = externalAttr.Add;
                        var addMethod = type.GetMethod(add);
                        var deserialized = memento.ReadSerializable(propName) as IEnumerable;
                        foreach (var ex in deserialized) {
                            addMethod.Invoke(_target, new[] { ex });
                        }
                    }

                } else {
                    if (valueType.IsPrimitive) {
                        prop.SetValue(_target, memento.ReadSerializable(propName), null);
                    } else {
                        if (typeof(IMemento).IsAssignableFrom(valueType)) {
                            prop.SetValue(_target, memento.ReadExternalizable(propName), null);
                        } else {
                            prop.SetValue(_target, memento.ReadSerializable(propName), null);
                        }
                    }
                }
            }

            var externalizableAttr = TypeService.Instance.GetExternalizableAttribute(_type);
            if (externalizableAttr.Loaded!= null) {
                var loaded= _type.GetMethod(externalizableAttr.Loaded);
                loaded.Invoke(_target, new object[] { memento, context });
            }
        }

        private bool IsExternalizable(Type type) {
            return
                typeof(IExternalizable).IsAssignableFrom(type) ||
                TypeService.Instance.IsExternalizableDefined(type);
        }
    }

}
