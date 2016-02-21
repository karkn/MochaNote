/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Common.Event {
    [Serializable]
    public class QualifiedEventHandlers<TEventArgs>: IQualifiedEventHandlers<TEventArgs> where TEventArgs: EventArgs {
        // ========================================
        // static field
        // ========================================
        /// <summary>
        /// どのキーにも関連付けられていないイベントハンドラを関連付けるキー．
        /// </summary>
        private static readonly object NoQualifiedKey = new object();

        // ========================================
        // field
        // ========================================
        private Dictionary<object, EventHandler<TEventArgs>> _keyToEventHandler; /// lazy load

        // ========================================
        // constructor
        // ========================================
        public QualifiedEventHandlers() {

        }

        // ========================================
        // property
        // ========================================
        // === IQualifiedEventHandlers ==========
        public virtual bool IsEmpty {
            get { return _keyToEventHandler == null? true: _keyToEventHandler.Count == 0; }
        }

        // --- private ---
        private Dictionary<object, EventHandler<TEventArgs>> _KeyToEventHandler {
            get {
                if (_keyToEventHandler == null) {
                    _keyToEventHandler = new Dictionary<object,EventHandler<TEventArgs>>();
                }
                return _keyToEventHandler;
            }
        }
        
        private EventHandler<TEventArgs> this[object key] {
            get {
                EventHandler<TEventArgs> ret;
                if (_KeyToEventHandler.TryGetValue(key, out ret)) {
                    return ret;
                } else {
                    _KeyToEventHandler.Add(key, null);
                    return null;
                }
                
                //if (!_KeyToEventHandler.ContainsKey(key)) {
                //    _KeyToEventHandler.Add(key, null);
                //}
                //return _KeyToEventHandler[key];
            }
            set { _KeyToEventHandler[key] = value; }
        }


        // ========================================
        // method
        // ========================================
        // === IQualifiedEventHandlers ==========
        public virtual void AddHandler(EventHandler<TEventArgs> handler) {
            this[NoQualifiedKey] += handler;
        }

        public virtual void AddHandler(object key, EventHandler<TEventArgs> handler) {
            this[key] += handler;
        }

        public virtual void RemoveHandler(EventHandler<TEventArgs> handler) {
            this[NoQualifiedKey] -= handler;
        }

        public virtual void RemoveHandler(object key, EventHandler<TEventArgs> handler) {
            this[key] -= handler;
        }

        // === QualifiedEventHandlers ==========
        public virtual void Notify(object sender, object key, TEventArgs e) {
            if (IsEmpty) {
                return;
            }

            EventHandler<TEventArgs> tmp = this[key];
            if (tmp != null) {
                tmp(sender, e);
            }
            tmp = this[NoQualifiedKey];
            if (tmp != null) {
                tmp(sender, e);
            }
        }
    }
}
