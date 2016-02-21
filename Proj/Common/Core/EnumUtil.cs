/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Core {
    public static class EnumUtil {
        public static bool HasAllFlags(int value, int flags) {
            return (value & flags) == flags;
        }

        public static bool HasAnyFlags(int value, int flags) {
            return (value & flags) != 0;
        }

        public static bool HasNotFlag(int value, int flags) {
            return (value & flags) == 0;
        }

		public static T Append<T>(this System.Enum type, T value) {
            return (T) (object) (((int) (object) type | (int) (object) value));
        }

        public static T Remove<T>(this System.Enum type, T value) {
            return (T) (object) (((int) (object) type & ~(int) (object) value));
        }

        public static bool HasAll<T>(this System.Enum type, T value) {
            return ((int) (object) type & (int) (object) value) == (int) (object) value;
        }

        public static bool HasAny<T>(this System.Enum type, T value) {
            return ((int) (object) type & (int) (object) value) != 0;
        }

    }
}
