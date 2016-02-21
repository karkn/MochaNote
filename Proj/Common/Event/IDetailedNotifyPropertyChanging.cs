/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Mkamo.Common.Event;
using System.ComponentModel;

namespace Mkamo.Common.Event {
    public interface IDetailedNotifyPropertyChanging: INotifyPropertyChanging {
        // ========================================
        // property
        // ========================================
        IQualifiedEventHandlers<DetailedPropertyChangingEventArgs> NamedPropertyChanging { get; }

        // ========================================
        // event
        // ========================================
        event EventHandler<DetailedPropertyChangingEventArgs> DetailedPropertyChanging;
    }
}
