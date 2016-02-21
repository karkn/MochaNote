/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Common.Collection;
using Mkamo.Common.DataType;
using System.Reflection;
using Mkamo.Common.Reflection;
using Mkamo.Common.Forms.Core;
using Mkamo.Common.Core;

namespace Mkamo.Common.Forms.KeyMap {
    public class KeyBinder<T> {
        // ========================================
        // field
        // ========================================
        private Dictionary<ShortcutKey, string> _keyToId = new Dictionary<ShortcutKey, string>();

        private InsertionOrderedDictionary<string, KeyActionInfo<T>> _idToInfos;

        private Lazy<Dictionary<string, Action<Keys, IKeyMap<T>>>> _idToBindAction =
            new Lazy<Dictionary<string, Action<Keys, IKeyMap<T>>>>(() => new Dictionary<string, Action<Keys, IKeyMap<T>>>());

        // ========================================
        // constructor
        // ========================================
        public KeyBinder(IEnumerable<Type> types) {
            _idToInfos = CreateIdToInfos(types);
        }


        // ========================================
        // property
        // ========================================
        public IEnumerable<ShortcutKey> ShortcutKeys {
            get { return _keyToId.Keys; }
        }

        public IEnumerable<KeyActionInfo<T>> ActionInfos {
            get {
                foreach (var id in _idToInfos.InsertionOrderedKeys) {
                    yield return _idToInfos[id];
                }
            }
        }

        // ========================================
        // method
        // ========================================
        public void RegisterBindAction(string actionId, Action<Keys, IKeyMap<T>> bindAction) {
            _idToBindAction.Value[actionId] = bindAction;
        }

        public IEnumerable<ShortcutKey> GetShortcutKeys(string actionId) {
            foreach (var pair in _keyToId) {
                if (string.Equals(pair.Value, actionId, StringComparison.Ordinal)) {
                    yield return pair.Key;
                }
            }
        }

        public string GetActionId(ShortcutKey key) {
            var ret = default(string);
            if (_keyToId.TryGetValue(key, out ret)) {
                return ret;
            } else {
                return null;
            }
        }

        public ShortcutKey[] GetConflictedKeys(Keys prefix, Keys key) {
            var ret = new List<ShortcutKey>();

            var bothKey = new ShortcutKey(prefix, key);
            if (_keyToId.ContainsKey(bothKey)) {
                ret.Add(bothKey);
            }

            if (prefix == Keys.None) {
                var prefixKey = new ShortcutKey(key, Keys.KeyCode);
                foreach (var k in _keyToId.Keys) {
                    if (k.Prefix == key) {
                        ret.Add(k);
                    }
                }

            } else {
                var keyKey = new ShortcutKey(Keys.None, prefix);
                if (_keyToId.ContainsKey(keyKey)) {
                    ret.Add(keyKey);
                }
            }

            return ret.ToArray();
        }

        public void ClearBinds() {
            _keyToId.Clear();
        }

        public void SetBind(Keys key, string action) {
            SetBind(Keys.None, key, action);
        }

        public void SetBind(Keys prefix, Keys key, string action) {
            var conflicteds = GetConflictedKeys(prefix, key);
            if (conflicteds.Length > 0) {
                foreach (var con in conflicteds) {
                    UnsetBind(con.Prefix, con.Key);
                }
            }

            _keyToId.Add(new ShortcutKey(prefix, key), action);
        }

        public bool UnsetBind(Keys prefix, Keys key) {
            return _keyToId.Remove(new ShortcutKey(prefix, key));
        }

        public void Bind(IKeyMap<T> keyMap) {
            if (_idToInfos == null) {
                return;
            }

            foreach (var pair in _keyToId) {
                var prefix = pair.Key.Prefix;
                var shortcut = pair.Key.Key;

                if (shortcut == Keys.None) {
                    continue;
                }

                var targetKeyMap = keyMap;
                if (prefix != Keys.None) {
                    if (keyMap.IsPrefixDefined(prefix)) {
                        targetKeyMap = keyMap.GetPrefixKeyMap(prefix);
                    } else {
                        targetKeyMap = keyMap.SetPrefix(prefix);
                    }
                }

                var actionId = pair.Value;

                var handled = false;
                if (_idToBindAction.IsValueCreated) {
                    if (_idToBindAction.Value.ContainsKey(actionId)) {
                        var bindAction = _idToBindAction.Value[actionId];
                        bindAction(shortcut, targetKeyMap);
                        handled = true;
                    }
                }

                if (!handled && _idToInfos.ContainsKey(actionId)) {
                    var info = _idToInfos[actionId];
                    targetKeyMap.SetAction(shortcut, info.Action);
                }
            }
        }


        public void Save(string fileName) {

        }

        public void Load(string fileName) {

        }

        // ------------------------------
        // private
        // ------------------------------
        private InsertionOrderedDictionary<string, KeyActionInfo<T>> CreateIdToInfos(IEnumerable<Type> types) {
            var ret = new InsertionOrderedDictionary<string, KeyActionInfo<T>>();

            foreach (var type in types) {
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                foreach (var meth in methods) {
                    var paras = meth.GetParameters();
                    if (paras != null && paras.Length == 1 && paras[0].ParameterType == typeof(T)) {
                        var attr = Attribute.GetCustomAttribute(meth, typeof(KeyActionAttribute)) as KeyActionAttribute;
                        if (attr != null) {
                            var id = StringUtil.IsNullOrWhitespace(attr.Id) ? meth.Name : attr.Id;
                            var info = new KeyActionInfo<T>(id, attr.Description, MethodInfoUtil.ToStaticAction<T>(meth));
                            ret[id] = info;
                        }
                    }
                }
            }

            return ret;
        }

    }
}
