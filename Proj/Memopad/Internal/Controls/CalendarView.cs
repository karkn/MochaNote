/*
 * Copyright (c) 2007-2011, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Memopad.Internal.Controls {
    internal partial class CalendarView: UserControl {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        public CalendarView() {
            InitializeComponent();
        }

        // ========================================
        // property
        // ========================================
        public override Font Font {
            get { return base.Font; }
            set {
                base.Font = value;

                _monthCalendar.StateCommon.Header.Content.ShortText.Font = value;
                _monthCalendar.StateCommon.Day.Content.ShortText.Font = value;
                _monthCalendar.StateCommon.DayOfWeek.Content.ShortText.Font = value;
            }
        }

        // ========================================
        // method
        // ========================================
    }
}
