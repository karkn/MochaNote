/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using Mkamo.Container.Core;

namespace Mkamo.Container.Internal.Core {
    internal class PersistenceMaintainer {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private EntityInterceptor _entityInterceptor;
        private EntityContainer _container;
        private PersistentState _state;
        private PersistentState _prevState;

        private object _owner;

        // ========================================
        // constructor
        // ========================================
        internal PersistenceMaintainer(
            EntityInterceptor entityInterceptor, EntityContainer container, PersistentState state
        ) {
            _entityInterceptor = entityInterceptor;
            _container = container;
            _state = state;
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<PersistentStateChangedEventArgs> PersistentStateChanged;


        // ========================================
        // property
        // ========================================
        public PersistentState State {
            get { return _state; }
            private set {
                if (_state == value) {
                    return;
                }
                _prevState = _state;
                _state = value;
                _container.MaintainStrongReference(_owner, value);
                OnPersistentStateChanged(value);
            }
        }

        public object Owner {
            get { return _owner; }
            set {
                if (_owner == value) {
                    return;
                }
                _owner = value;
            }
        }

        // ========================================
        // method
        // ========================================
        public void EnsureLoaded(object target) {
            if (State == PersistentState.Hollow) {
                var service = TypeService.Instance;

                _entityInterceptor.NeedRealInvocation = true;
                var onLoading = service.GetOnLoading(_entityInterceptor.Type);
                if (onLoading != null) {
                    onLoading.Invoke(_owner, null);
                }

                _container.Store.Load(target);

                if (Logger.IsDebugEnabled) {
                    Logger.Debug(_entityInterceptor.Type + " Loaded");
                }

                var onLoaded = service.GetOnLoaded(_entityInterceptor.Type);
                if (onLoaded != null) {
                    onLoaded.Invoke(_owner, null);
                }
                _entityInterceptor.NeedRealInvocation = false;

                State = PersistentState.Latest;
            }
        }

        public void Persist() {
            switch (State) {
                case PersistentState.Hollow:
                case PersistentState.Latest:
                case PersistentState.Updated:
                case PersistentState.New: {
                    /// do nothing
                    break;
                }

                case PersistentState.Removed: {
                    State = _prevState;
                    break;
                }

                case PersistentState.Discarded: {
                    if (_prevState == PersistentState.New) {
                        /// New状態のentityがRollback()かRemove()された
                        State = PersistentState.New;
                    } else if (_prevState == PersistentState.Removed) {
                        /// 今Discardedで前Removedならすでに削除されてしまっているEntityなので
                        /// ID作り直してNewにする
                        _entityInterceptor.Id = _container.Store.CreateId();
                        State = PersistentState.New;
                    } else {
                        throw new InvalidOperationException("prev state");
                    }
                    break;
                }

                case PersistentState.Transient: {
                    _entityInterceptor.Id = _container.Store.CreateId();
                    State = PersistentState.New;
                    break;
                }
            }
            
        }

        public void Reflect(object target) {
            switch (State) {
                case PersistentState.Hollow: {
                    /// do nothing
                    break;
                }
                case PersistentState.Latest: {
                    /// do nothing
                    break;
                }
                case PersistentState.Removed: {
                    _container.Store.Remove(target);
                    State = PersistentState.Discarded;
                    break;
                }
                case PersistentState.Updated: {
                    _container.Store.Update(target);
                    State = PersistentState.Latest;
                    break;
                }
                case PersistentState.New: {
                    _container.Store.Insert(target);
                    State = PersistentState.Latest;
                    break;
                }
                case PersistentState.Discarded: {
                    /// do nothing
                    break;
                }
                case PersistentState.Transient: {
                    /// do nothing
                    break;
                }
            }
            
        }

        public void Rollback(object target) {
            switch (State) {
                case PersistentState.Hollow: {
                    /// do nothing
                    break;
                }
                case PersistentState.Latest: {
                    /// do nothing
                    break;
                }
                case PersistentState.Removed: {
                    State = PersistentState.Hollow;
                    break;
                }
                case PersistentState.Updated: {
                    State = PersistentState.Hollow;
                    break;
                }
                case PersistentState.New: {
                    State = PersistentState.Discarded;
                    break;
                }
                case PersistentState.Discarded: {
                    /// do nothing
                    break;
                }
                case PersistentState.Transient: {
                    /// do nothing
                    break;
                }
            }
        }

        public void Dirty() {
            switch (State) {
                case PersistentState.Hollow: {
                    throw new InvalidOperationException("Dirty method must not be invoked for hollow entity");
                }
                case PersistentState.Latest: {
                    State = PersistentState.Updated;
                    break;
                }
                case PersistentState.Removed: {
                    /// do nothing
                    break;
                }
                case PersistentState.Updated: {
                    /// do nothing
                    break;
                }
                case PersistentState.New: {
                    /// do nothing
                    break;
                }
                case PersistentState.Discarded: {
                    /// do nothing
                    break;
                }
                case PersistentState.Transient: {
                    /// do nothing
                    break;
                }
            }
        }

        public void Remove() {
            switch (State) {
                case PersistentState.Hollow: {
                    State = PersistentState.Removed;
                    break;
                }
                case PersistentState.Latest: {
                    State = PersistentState.Removed;
                    break;
                }
                case PersistentState.Removed: {
                    /// do nothing
                    break;
                }
                case PersistentState.Updated: {
                    State = PersistentState.Removed;
                    break;
                }
                case PersistentState.New: {
                    State = PersistentState.Discarded;
                    break;
                }
                case PersistentState.Discarded: {
                    /// do nothing
                    break;
                }
                case PersistentState.Transient: {
                    /// do nothing
                    break;
                }
            }
        }


        // ------------------------------
        // private
        // ------------------------------
        private void OnPersistentStateChanged(PersistentState state) {
            var handler = PersistentStateChanged;
            if (handler != null) {
                handler(Owner, new PersistentStateChangedEventArgs(state));
            }
        }

    }
}
