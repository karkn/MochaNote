/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.DataType {
    public static class Tuple {
        // ========================================
        // static method
        // ========================================
        public static Tuple<T1> Create<T1>(T1 item1) {
            return new Tuple<T1>(item1);
        }

        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2) {
            return new Tuple<T1, T2>(item1, item2);
        }

        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3) {
            return new Tuple<T1, T2, T3>(item1, item2, item3);
        }

        public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) {
            return new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        }
    }

    [Serializable]
    public struct Tuple<T1>: IEquatable<Tuple<T1>> {
        // ========================================
        // static field
        // ========================================
        public static readonly Tuple<T1> Empty = new Tuple<T1>();

        // ========================================
        // static method
        // ========================================
        public static bool operator ==(Tuple<T1> a, Tuple<T1> b) {
            return a.Equals(b);
        }

        public static bool operator !=(Tuple<T1> a, Tuple<T1> b) {
            return !a.Equals(b);
        }

        // ========================================
        // field
        // ========================================
        public T1 Item1;

        // ========================================
        // constructor
        // ========================================
        public Tuple(T1 item1) {
            Item1 = item1;
        }

        // ========================================
        // method
        // ========================================
        public override int GetHashCode() {
            return Item1 == null? base.GetHashCode(): Item1.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj is Tuple<T1>) {
                return Equals((Tuple<T1>) obj);
            } else {
                return false;
            }
        }

        public bool Equals(Tuple<T1> other) {
            return Item1 == null? other.Item1 == null: Item1.Equals(other.Item1);
        }
    }

    [Serializable]
    public struct Tuple<T1, T2>: IEquatable<Tuple<T1, T2>> {
        // ========================================
        // static field
        // ========================================
        public static readonly Tuple<T1, T2> Empty = new Tuple<T1, T2>();

        // ========================================
        // static method
        // ========================================
        public static bool operator ==(Tuple<T1, T2> a, Tuple<T1, T2> b) {
            return a.Equals(b);
        }

        public static bool operator !=(Tuple<T1, T2> a, Tuple<T1, T2> b) {
            return !a.Equals(b);
        }

        // ========================================
        // field
        // ========================================
        public T1 Item1;
        public T2 Item2;

        // ========================================
        // constructor
        // ========================================
        public Tuple(T1 item1, T2 item2) {
            Item1 = item1;
            Item2 = item2;
        }

        // ========================================
        // method
        // ========================================
        public override int GetHashCode() {
            var ret = Item1 == null? base.GetHashCode(): Item1.GetHashCode();
            ret = Item2 == null? ret: ret ^ Item2.GetHashCode();
            return ret;
        }

        public override bool Equals(object obj) {
            if (obj is Tuple<T1, T2>) {
                return Equals((Tuple<T1, T2>) obj);
            } else {
                return false;
            }
        }

        public bool Equals(Tuple<T1, T2> other) {
            var ret = Item1 == null? other.Item1 == null: Item1.Equals(other.Item1);
            if (!ret) {
                return false;
            }
            ret = Item2 == null? other.Item2 == null: Item2.Equals(other.Item2);
            return ret;
        }
    }

    [Serializable]
    public struct Tuple<T1, T2, T3>: IEquatable<Tuple<T1, T2, T3>> {
        // ========================================
        // static field
        // ========================================
        public static readonly Tuple<T1, T2, T3> Empty = new Tuple<T1, T2, T3>();

        // ========================================
        // static method
        // ========================================
        public static bool operator ==(Tuple<T1, T2, T3> a, Tuple<T1, T2, T3> b) {
            return a.Equals(b);
        }

        public static bool operator !=(Tuple<T1, T2, T3> a, Tuple<T1, T2, T3> b) {
            return !a.Equals(b);
        }

        // ========================================
        // field
        // ========================================
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;

        // ========================================
        // constructor
        // ========================================
        public Tuple(T1 item1, T2 item2, T3 item3) {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }
        
        // ========================================
        // method
        // ========================================
        public override int GetHashCode() {
            var ret = Item1 == null? base.GetHashCode(): Item1.GetHashCode();
            ret = Item2 == null? ret: ret ^ Item2.GetHashCode();
            ret = Item3 == null? ret: ret ^ Item3.GetHashCode();
            return ret;
        }

        public override bool Equals(object obj) {
            if (obj is Tuple<T1, T2, T3>) {
                return Equals((Tuple<T1, T2, T3>) obj);
            } else {
                return false;
            }
        }

        public bool Equals(Tuple<T1, T2, T3> other) {
            var ret = Item1 == null? other.Item1 == null: Item1.Equals(other.Item1);
            if (!ret) {
                return false;
            }
            ret = Item2 == null? other.Item2 == null: Item2.Equals(other.Item2);
            if (!ret) {
                return false;
            }
            ret = Item3 == null? other.Item3 == null: Item3.Equals(other.Item3);
            return ret;
        }
    }

    [Serializable]
    public struct Tuple<T1, T2, T3, T4>: IEquatable<Tuple<T1, T2, T3, T4>> {
        // ========================================
        // static field
        // ========================================
        public static readonly Tuple<T1, T2, T3, T4> Empty = new Tuple<T1, T2, T3, T4>();

        // ========================================
        // static method
        // ========================================
        public static bool operator ==(Tuple<T1, T2, T3, T4> a, Tuple<T1, T2, T3, T4> b) {
            return a.Equals(b);
        }

        public static bool operator !=(Tuple<T1, T2, T3, T4> a, Tuple<T1, T2, T3, T4> b) {
            return !a.Equals(b);
        }

        // ========================================
        // field
        // ========================================
        public T1 Item1;
        public T2 Item2;
        public T3 Item3;
        public T4 Item4;

        // ========================================
        // constructor
        // ========================================
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4) {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }

        // ========================================
        // method
        // ========================================
        public override int GetHashCode() {
            var ret = Item1 == null? base.GetHashCode(): Item1.GetHashCode();
            ret = Item2 == null? ret: ret ^ Item2.GetHashCode();
            ret = Item3 == null? ret: ret ^ Item3.GetHashCode();
            ret = Item4 == null? ret: ret ^ Item4.GetHashCode();
            return ret;
        }

        public override bool Equals(object obj) {
            if (obj is Tuple<T1, T2, T3, T4>) {
                return Equals((Tuple<T1, T2, T3, T4>) obj);
            } else {
                return false;
            }
        }

        public bool Equals(Tuple<T1, T2, T3, T4> other) {
            var ret = Item1 == null? other.Item1 == null: Item1.Equals(other.Item1);
            if (!ret) {
                return false;
            }
            ret = Item2 == null? other.Item2 == null: Item2.Equals(other.Item2);
            if (!ret) {
                return false;
            }
            ret = Item3 == null? other.Item3 == null: Item3.Equals(other.Item3);
            if (!ret) {
                return false;
            }
            ret = Item4 == null? other.Item4 == null: Item4.Equals(other.Item4);
            return ret;
        }
    }
}
