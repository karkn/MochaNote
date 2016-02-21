/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Common.DataType;

namespace Mkamo.Common.Forms.KeyMap {
    public class KeyMap<TTarget>: IKeyMap<TTarget> {

        // ========================================
        // field
        // ========================================
        private readonly Dictionary<Keys, ActionRegistration> _keyToActions = new Dictionary<Keys, ActionRegistration>();
        private Dictionary<Keys, PrefixRegistration> _prefixedKeyMaps; /// lazy

        private PrefixRegistration _currentPrefix = null;

        // ========================================
        // constructor
        // ========================================
        public KeyMap() {
        }

        // ========================================
        // property
        // ========================================
        private Dictionary<Keys, PrefixRegistration> _PrefixedKeyMaps {
            get {
                return _prefixedKeyMaps??
                    (_prefixedKeyMaps = new Dictionary<Keys, PrefixRegistration>());
            }
        }


        // ========================================
        // method
        // ========================================
        public bool IsDefined(Keys key) {
            return
                _currentPrefix == null?
                _keyToActions.ContainsKey(key):
                true;
        }

        public Func<TTarget, bool> GetAction(Keys key) {
            if (_currentPrefix == null) {
                ActionRegistration ret;
                if (_keyToActions.TryGetValue(key, out ret)) {
                    return (target) => {
                        ret.Action(target);
                        return ret.IsConsumeKey;
                    };
                } else {
                    return null;
                }
            } else {
                var currentKeyMap  = _currentPrefix.KeyMap;
                if (currentKeyMap.IsDefined(key)) {
                    Func<TTarget, bool> action = currentKeyMap.GetAction(key);
                    return (target) => {
                        var ret = action(target);
                        var retJudge = _currentPrefix.ReturnJudge;
                        var retAct = _currentPrefix.ReturnAction;
                        var isConsumeKey = _currentPrefix.IsConsumeKeyOnReturn;
                        if (retJudge == null || retJudge(key, target)) {
                            _currentPrefix = null;
                            if (retAct != null) {
                                retAct(key, target);
                            }
                            return ret || isConsumeKey;
                        } else {
                            return ret;
                        }
                    };
                } else {
                    return (target) => {
                        var retJudge = _currentPrefix.ReturnJudge;
                        var retAct = _currentPrefix.ReturnAction;
                        var isConsumeKey = _currentPrefix.IsConsumeKeyOnReturn;
                        if (retJudge == null || retJudge(key, target)) {
                            _currentPrefix = null;
                            if (retAct != null) {
                                retAct(key, target);
                            }
                        }
                        return isConsumeKey;
                    };
                }
            }
        }

        public void SetAction(Keys key, Action<TTarget> action) {
            if (_prefixedKeyMaps != null && _prefixedKeyMaps.ContainsKey(key)) {
                UnsetPrefix(key);
            }
            _keyToActions[key] = new ActionRegistration(action, true);
        }

        public void SetAction(Keys key, Action<TTarget> action, bool isConsumeKey) {
            if (_prefixedKeyMaps != null && _prefixedKeyMaps.ContainsKey(key)) {
                UnsetPrefix(key);
            }
            _keyToActions[key] = new ActionRegistration(action, isConsumeKey);
        }

        public void UnsetAction(Keys key) {
            if (_prefixedKeyMaps != null && _prefixedKeyMaps.ContainsKey(key)) {
                UnsetPrefix(key);
            } else {
                _keyToActions.Remove(key);
            }
        }

        public void Clear() {
            _keyToActions.Clear();
            if (_prefixedKeyMaps != null) {
                _prefixedKeyMaps.Clear();
            }
            _currentPrefix = null;
        }

        public bool IsPrefixDefined(Keys key) {
            return _prefixedKeyMaps == null ? false : _prefixedKeyMaps.ContainsKey(key);
        }

        public KeyMap<TTarget> SetPrefix(Keys key) {
            return SetPrefix(key, null, null, null, true);
        }

        public KeyMap<TTarget> SetPrefix(
            Keys key,
            Action<Keys, TTarget> transitAction,
            Action<Keys, TTarget> returnAction,
            Func<Keys, TTarget, bool> returnJudge,
            bool isConsumeKeyOnReturn
        ) {
            var keyMap = new KeyMap<TTarget>();

            SetAction(
                key,
                (target) => {
                    if (transitAction != null) {
                        transitAction(key, target);
                    }
                    _currentPrefix = _PrefixedKeyMaps[key];
                }
            );

            _PrefixedKeyMaps[key] = new PrefixRegistration(
                keyMap,
                returnAction,
                returnJudge,
                isConsumeKeyOnReturn
            );

            return keyMap;
        }

        public void UnsetPrefix(Keys key) {
            _PrefixedKeyMaps.Remove(key);
            UnsetAction(key);
        }

        public KeyMap<TTarget> GetPrefixKeyMap(Keys key) {
            PrefixRegistration ret;
            if (_PrefixedKeyMaps.TryGetValue(key, out ret)) {
                return ret.KeyMap;
            } else {
                return null;
            }
        }

        public void ClearPrefixInput() {
            _currentPrefix = null;
        }

        // ========================================
        // class
        // ========================================
        private class ActionRegistration {
            public Action<TTarget> Action;
            public bool IsConsumeKey;

            public ActionRegistration(
                Action<TTarget> action,
                bool isConsumeKey
            ) {
                Action = action;
                IsConsumeKey = isConsumeKey;
            }
        }
        
        private class PrefixRegistration {
            public KeyMap<TTarget> KeyMap;
            public Action<Keys, TTarget> ReturnAction;
            public Func<Keys, TTarget, bool> ReturnJudge;
            public bool IsConsumeKeyOnReturn;

            public PrefixRegistration(
                KeyMap<TTarget> keyMap,
                Action<Keys, TTarget> returnAction,
                Func<Keys, TTarget, bool> returnJudge,
                bool isConsumeKey
            ) {
                KeyMap = keyMap;
                ReturnAction = returnAction;
                ReturnJudge = returnJudge;
                IsConsumeKeyOnReturn = isConsumeKey;
            }
        }
    }
}
