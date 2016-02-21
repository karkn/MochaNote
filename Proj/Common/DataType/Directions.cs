/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Core;

namespace Mkamo.Common.DataType {
    [Flags, Serializable]
    public enum Directions {
        None = 0,

        Left = 0x01,
        Right = 0x02,
        Up = 0x04,
        Down = 0x08,

        UpLeft = Up | Left,
        UpRight = Up | Right,
        DownLeft = Down | Left,
        DownRight = Down | Right,

        HorizontalNeutral = Left | Right,
        VerticalNeutral = Up | Down,
        Neutral = HorizontalNeutral | VerticalNeutral,
    }

    public static class DirectionUtil {
        public static bool ContainsLeft(Directions dir) {
            return EnumUtil.HasAllFlags((int) dir, (int) Directions.Left);
        }
        public static bool ContainsRight(Directions dir) {
            return EnumUtil.HasAllFlags((int) dir, (int) Directions.Right);
        }
        public static bool ContainsUp(Directions dir) {
            return EnumUtil.HasAllFlags((int) dir, (int) Directions.Up);
        }
        public static bool ContainsDown(Directions dir) {
            return EnumUtil.HasAllFlags((int) dir, (int) Directions.Down);
        }
    }
}
