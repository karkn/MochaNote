/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Externalize.Internal;

namespace Mkamo.Common.Externalize {
    public class Externalizer {
        // ========================================
        // field
        // ========================================
        private Dictionary<string, object> _extendedData; /// lazy

        // ========================================
        // constructor
        // ========================================
        public Externalizer() {
        }

        // ========================================
        // property
        // ========================================
        public Dictionary<string, object> ExtendedData {
            get { return _extendedData?? (_extendedData = new Dictionary<string, object>()); }
        }

        // ========================================
        // method
        // ========================================
        // --- save ---
        public IMemento Save(object externalizable, ExternalizableFilter externalizableFilter) {
            var context = new ExternalizeContext(this, externalizableFilter, null);
            return context.CreateMemento(externalizable);
        }

        public IMemento Save(object externalizable) {
            return Save(externalizable, null);
        }


        // --- load ---
        public object Load(IMemento memento, MementoFilter mementoFilter) {
            var context = new ExternalizeContext(this, null, mementoFilter);
            return context.CreateExternalizable((Memento) memento);
        }
        public object Load(IMemento memento) {
            return Load(memento, null);
        }

        // --- load to ---
        //public void LoadTo(IMemento memento, object target, MementoFilter mementoFilter) {
        //    var context = new ExternalizeContext(this, null, mementoFilter);
        //    context.LoadExternalizable((Memento) memento, target);
        //}

        //public void LoadTo(IMemento memento, object target) {
        //    LoadTo(memento, target, null);
        //}

    }
}
