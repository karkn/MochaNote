/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.DataType;

namespace Mkamo.Common.Disposable {
    using ResourceCreatorPolicyTuple = Tuple<Func<IDisposable>, Action<IDisposable>, ResourceDisposingPolicy>;

    public class ResourceCache: IDisposable {
        // ========================================
        // field
        // ========================================
        private Dictionary<object, ResourceCreatorPolicyTuple> _keyToResourceCreatorPolicyTuple;
        private Dictionary<object, IDisposable> _keyToResource;
        private Dictionary<object, IDisposable> _keyToImmediateResource;

        private ResourceUsingContext _context;
        private int _usingDepth;

        private bool _enabled;

        // ========================================
        // constructor
        // ========================================
        public ResourceCache() {
            _keyToResourceCreatorPolicyTuple = new Dictionary<object, ResourceCreatorPolicyTuple>();
            _keyToResource = new Dictionary<object, IDisposable>();
            _keyToImmediateResource = new Dictionary<object, IDisposable>();

            _context = new ResourceUsingContext(this);
            _usingDepth = 0;

            _enabled = true;
        }

        // ========================================
        // property
        // ========================================
        public IDisposable this[object key] {
            get { return GetResource(key); }
        }

        public bool Enabled {
            get { return _enabled; }
            set {
                if (value == _enabled) {
                    return;
                }
                _enabled = value;
                if (!_enabled) {
                    DisposeResources();
                }
            }
        }

        // ========================================
        // method
        // ========================================
        // === IDisposable ==========
        public void Dispose() {
            DisposeResources();
            _keyToResourceCreatorPolicyTuple.Clear();

            _keyToResource = null;
            _keyToImmediateResource = null;
            _keyToResourceCreatorPolicyTuple = null;

            _context = null;

            GC.SuppressFinalize(this);
        }

        // === ResourceCache ==========
        public void RegisterResourceCreator(
            object key,
            Func<IDisposable> resourceCreator,
            Action<IDisposable> resourceUpdator,
            ResourceDisposingPolicy policy
        ) {
            _keyToResourceCreatorPolicyTuple[key] =
                new ResourceCreatorPolicyTuple(resourceCreator, resourceUpdator, policy);
        }

        public void RegisterResourceCreator(
            object key,
            Func<IDisposable> resourceCreator,
            ResourceDisposingPolicy policy
        ) {
            RegisterResourceCreator(key, resourceCreator, null, policy);
        }
        
        public void UnregisterResourceCreator(object key) {
            if (_keyToResourceCreatorPolicyTuple.ContainsKey(key)) {
                _keyToResourceCreatorPolicyTuple.Remove(key);
            }
        }

        public ResourceUsingContext UseResource() {
            ++_usingDepth;
            return _context;
        }

        public void EndUseResoruce() {
            if (_usingDepth > 0) {
                --_usingDepth;
                if (_usingDepth == 0) {
                    DisposeImmediateResources();
                }
            }
        }

        public IDisposable GetResource(object key) {
            if (_usingDepth <= 0) {
                throw new InvalidOperationException("UseResource() must be invoked before");
            }

            if (_keyToImmediateResource.ContainsKey(key)) {
                return _keyToImmediateResource[key];
            }
            if (_keyToResource.ContainsKey(key)) {
                return _keyToResource[key];
            }

            if (_keyToResourceCreatorPolicyTuple.ContainsKey(key)) {
                var tuple = _keyToResourceCreatorPolicyTuple[key];
                var creator = tuple.Item1;
                if (creator == null) {
                    return null;
                }
                var resource = creator();
                if (resource != null) {
                    var policy = tuple.Item3;
                    if (_enabled && (policy == ResourceDisposingPolicy.Explicit)) {
                        _keyToResource[key] = resource;
                    } else {
                        _keyToImmediateResource[key] = resource;
                    }
                }
                return resource;
            }

            throw new ArgumentException("key is not registered");
        }

        public void UpdateResource(object key) {
            if (_keyToResource.ContainsKey(key) && _keyToResourceCreatorPolicyTuple.ContainsKey(key)) {
                var updator = _keyToResourceCreatorPolicyTuple[key].Item2;
                if (updator != null) {
                    updator(_keyToResource[key]);
                }
            }
        }

        public void DisposeResource(object key) {
            if (_keyToResource.ContainsKey(key)) {
                _keyToResource[key].Dispose();
                _keyToResource.Remove(key);
            }
            if (_keyToImmediateResource.ContainsKey(key)) {
                _keyToImmediateResource[key].Dispose();
                _keyToImmediateResource.Remove(key);
            }
        }

        public void DisposeResources() {
            foreach (var disposable in _keyToResource.Values) {
                disposable.Dispose();
            }
            _keyToResource.Clear();

            foreach (var disposable in _keyToImmediateResource.Values) {
                disposable.Dispose();
            }
            _keyToImmediateResource.Clear();
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected void DisposeImmediateResources() {
            foreach (var disposable in _keyToImmediateResource.Values) {
                disposable.Dispose();
            }
            _keyToImmediateResource.Clear();
        }
    }

    // ========================================
    // type
    // ========================================
    [Serializable]
    public enum ResourceDisposingPolicy {
        Immediate, /// EndUseResoruce()時に解放される
        Explicit,  /// 明示的にDisposeResource()しないと解放されない
    }

    // ========================================
    // class
    // ========================================
    public class ResourceUsingContext: IDisposable {
        private ResourceCache _owner;
        public ResourceUsingContext(ResourceCache owner) {
            _owner = owner;
        }
        public void Dispose() {
            _owner.EndUseResoruce();

            /// ひとつのインスタンスをずっと使いまわすのでいらない
            /// GC.SuppressFinalize(this);
        }
    }

}
