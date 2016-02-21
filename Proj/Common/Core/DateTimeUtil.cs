/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Core {
    public static class DateTimeUtil {
        // ========================================
        // static method
        // ========================================
        public static DateTime GetFirstDayOfWeek(DateTime date, DayOfWeek start) {
            var diff = start - date.DayOfWeek;
            if (diff > 0) {
                diff -= 7;
            }
            
            return date.AddDays(diff);
        }
    }
}
