/*  Tuples.cs
 *  Erik Forbes
 *  http://shadowcoding.blogspot.com */

using System;
using System.Collections.Generic;

namespace G25 {
	public static class Tuples {
		public static Tuple<T1, T2> Tuple<T1, T2>(T1 value1, T2 value2) {
			return new Tuple<T1, T2>(value1, value2);
		}
		public static Tuple<T1, T2, T3> Tuple<T1, T2, T3>(T1 value1, T2 value2, T3 value3) {
			return new Tuple<T1, T2, T3>(value1, value2, value3);
		}
		public static Tuple<T1, T2, T3, T4> Tuple<T1, T2, T3, T4>(T1 value1, T2 value2, T3 value3, T4 value4) {
			return new Tuple<T1, T2, T3, T4>(value1, value2, value3, value4);
		}
		public static Tuple<T1, T2, T3, T4, T5> Tuple<T1, T2, T3, T4, T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) {
			return new Tuple<T1, T2, T3, T4, T5>(value1, value2, value3, value4, value5);
		}

		public static Tuple<T1, T2> Default<T1, T2>() {
			return new Tuple<T1, T2>(default(T1), default(T2));
		}
		public static Tuple<T1, T2, T3> Default<T1, T2, T3>() {
			return new Tuple<T1, T2, T3>(default(T1), default(T2), default(T3));
		}
		public static Tuple<T1, T2, T3, T4> Default<T1, T2, T3, T4>() {
			return new Tuple<T1, T2, T3, T4>(default(T1), default(T2), default(T3), default(T4));
		}
		public static Tuple<T1, T2, T3, T4, T5> Default<T1, T2, T3, T4, T5>() {
			return new Tuple<T1, T2, T3, T4, T5>(default(T1), default(T2), default(T3), default(T4), default(T5));
		}

		public static IEnumerable<Tuple<T1, T2>> Zip<T1, T2>(IEnumerable<T1> first, IEnumerable<T2> second) {
			IEnumerator<T1> enum1 = first.GetEnumerator();
            IEnumerator<T2> enum2 = second.GetEnumerator();

			while (enum1.MoveNext() && enum2.MoveNext()) {
				yield return Tuple(enum1.Current, enum2.Current);
			}
		}
		public static IEnumerable<Tuple<T1, T2, T3>> Zip<T1, T2, T3>(IEnumerable<T1> first, IEnumerable<T2> second, IEnumerable<T3> third) {
            IEnumerator<T1> enum1 = first.GetEnumerator();
            IEnumerator<T2> enum2 = second.GetEnumerator();
            IEnumerator<T3> enum3 = third.GetEnumerator();

			while (enum1.MoveNext() && enum2.MoveNext() && enum3.MoveNext()) {
				yield return Tuple(enum1.Current, enum2.Current, enum3.Current);
			}
		}
		public static IEnumerable<Tuple<T1, T2, T3, T4>> Zip<T1, T2, T3, T4>(IEnumerable<T1> first, IEnumerable<T2> second, IEnumerable<T3> third, IEnumerable<T4> fourth) {
            IEnumerator<T1> enum1 = first.GetEnumerator();
            IEnumerator<T2> enum2 = second.GetEnumerator();
            IEnumerator<T3> enum3 = third.GetEnumerator();
            IEnumerator<T4> enum4 = fourth.GetEnumerator();

			while (enum1.MoveNext() && enum2.MoveNext() && enum3.MoveNext() && enum4.MoveNext()) {
				yield return Tuple(enum1.Current, enum2.Current, enum3.Current, enum4.Current);
			}
		}
		public static IEnumerable<Tuple<T1, T2, T3, T4, T5>> Zip<T1, T2, T3, T4, T5>(IEnumerable<T1> first, IEnumerable<T2> second, IEnumerable<T3> third, IEnumerable<T4> fourth, IEnumerable<T5> fifth) {
            IEnumerator<T1> enum1 = first.GetEnumerator();
            IEnumerator<T2> enum2 = second.GetEnumerator();
            IEnumerator<T3> enum3 = third.GetEnumerator();
            IEnumerator<T4> enum4 = fourth.GetEnumerator();
            IEnumerator<T5> enum5 = fifth.GetEnumerator();

			while (enum1.MoveNext() && enum2.MoveNext() && enum3.MoveNext() && enum4.MoveNext() && enum5.MoveNext()) {
				yield return Tuple(enum1.Current, enum2.Current, enum3.Current, enum4.Current, enum5.Current);
			}
		}
	}

	public struct Tuple<T1, T2> {
		private readonly T1 _value1; public T1 Value1 { get { return _value1; } }
		private readonly T2 _value2; public T2 Value2 { get { return _value2; } }
		public Tuple(T1 value1, T2 value2) { _value1 = value1; _value2 = value2; }

		public override bool Equals(object obj) {
			if (!(obj is Tuple<T1, T2>)) return false;
			if (obj == null) return false;

			Tuple<T1, T2> t = (Tuple<T1, T2>)obj;
			return (Value1.Equals(t.Value1) && Value2.Equals(t.Value2));
		}

		public override int GetHashCode() {
			return Value1.GetHashCode() ^ Value2.GetHashCode();
		}

		public KeyValuePair<T1, T2> AsKeyValuePair() {
			return new KeyValuePair<T1, T2>(Value1, Value2);
		}

        public override string ToString()
        {
            return "(" + Value1.ToString() + ", " + Value2.ToString() + ")";
        }

	}
	public struct Tuple<T1, T2, T3> {
		private readonly T1 _value1; public T1 Value1 { get { return _value1; } }
		private readonly T2 _value2; public T2 Value2 { get { return _value2; } }
		private readonly T3 _value3; public T3 Value3 { get { return _value3; } }
		public Tuple(T1 value1, T2 value2, T3 value3) { _value1 = value1; _value2 = value2; _value3 = value3; }

		public override bool Equals(object obj) {
			if (!(obj is Tuple<T1, T2, T3>)) return false;
			if (obj == null) return false;

			Tuple<T1, T2, T3> t = (Tuple<T1, T2, T3>)obj;
			return (Value1.Equals(t.Value1) && Value2.Equals(t.Value2) && Value3.Equals(t.Value3));
		}

		public override int GetHashCode() {
			return Value1.GetHashCode() ^ Value2.GetHashCode() ^ Value3.GetHashCode();
		}

        public override string ToString()
        {
            return "(" + Value1.ToString() + ", " + Value2.ToString() + ", " + Value3.ToString() + ")";
        }

    }
	public struct Tuple<T1, T2, T3, T4> {
		private readonly T1 _value1; public T1 Value1 { get { return _value1; } }
		private readonly T2 _value2; public T2 Value2 { get { return _value2; } }
		private readonly T3 _value3; public T3 Value3 { get { return _value3; } }
		private readonly T4 _value4; public T4 Value4 { get { return _value4; } }
		public Tuple(T1 value1, T2 value2, T3 value3, T4 value4) { _value1 = value1; _value2 = value2; _value3 = value3; _value4 = value4; }

		public override bool Equals(object obj) {
			if (!(obj is Tuple<T1, T2, T3, T4>)) return false;
			if (obj == null) return false;

			Tuple<T1, T2, T3, T4> t = (Tuple<T1, T2, T3, T4>)obj;
			return (Value1.Equals(t.Value1) && Value2.Equals(t.Value2) && Value3.Equals(t.Value3) && Value4.Equals(t.Value4));
		}

		public override int GetHashCode() {
			return Value1.GetHashCode() ^ Value2.GetHashCode() ^ Value3.GetHashCode() ^ Value4.GetHashCode();
		}
        public override string ToString()
        {
            return "(" + Value1.ToString() + ", " + Value2.ToString() + ", " + Value3.ToString() + ", " + Value4.ToString() + ")";
        }

    }
	public struct Tuple<T1, T2, T3, T4, T5> {
		private readonly T1 _value1; public T1 Value1 { get { return _value1; } }
		private readonly T2 _value2; public T2 Value2 { get { return _value2; } }
		private readonly T3 _value3; public T3 Value3 { get { return _value3; } }
		private readonly T4 _value4; public T4 Value4 { get { return _value4; } }
		private readonly T5 _value5; public T5 Value5 { get { return _value5; } }
		public Tuple(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5) { _value1 = value1; _value2 = value2; _value3 = value3; _value4 = value4; _value5 = value5; }

		public override bool Equals(object obj) {
			if (!(obj is Tuple<T1, T2, T3, T4, T5>)) return false;
			if (obj == null) return false;

			Tuple<T1, T2, T3, T4, T5> t = (Tuple<T1, T2, T3, T4, T5>)obj;
			return (Value1.Equals(t.Value1) && Value2.Equals(t.Value2) && Value3.Equals(t.Value3) && Value4.Equals(t.Value4) && Value5.Equals(t.Value5));
		}

		public override int GetHashCode() {
			return Value1.GetHashCode() ^ Value2.GetHashCode() ^ Value3.GetHashCode() ^ Value4.GetHashCode() ^ Value5.GetHashCode();
		}

        public override string ToString()
        {
            return "(" + Value1.ToString() + ", " + Value2.ToString() + ", " + Value3.ToString() + ", " + Value4.ToString() + ", " + Value5.ToString() + ")";
        }
    }

	public static class TupleExtensions {
		public static Tuple<IEnumerable<T1>, IEnumerable<T2>> Unzip<T1, T2>(IEnumerable<Tuple<T1, T2>> ienum) {
            List<T1> first = new List<T1>();
            List<T2> second = new List<T2>();

			foreach (Tuple<T1, T2> t in ienum) {
				first.Add(t.Value1);
				second.Add(t.Value2);
			}

            return Tuples.Tuple(first as IEnumerable<T1>, second as IEnumerable<T2>);
		}
		public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> Unzip<T1, T2, T3>(IEnumerable<Tuple<T1, T2, T3>> ienum) {
            List<T1> first = new List<T1>();
            List<T2> second = new List<T2>();
            List<T3> third = new List<T3>();

			foreach (Tuple<T1, T2, T3> t in ienum) {
				first.Add(t.Value1);
				second.Add(t.Value2);
				third.Add(t.Value3);
			}

            return Tuples.Tuple(first as IEnumerable<T1>, second as IEnumerable<T2>, third as IEnumerable<T3>);
		}
		public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> Unzip<T1, T2, T3, T4>(IEnumerable<Tuple<T1, T2, T3, T4>> ienum) {
            List<T1> first = new List<T1>();
            List<T2> second = new List<T2>();
            List<T3> third = new List<T3>();
            List<T4> fourth = new List<T4>();

            foreach (Tuple<T1, T2, T3, T4> t in ienum)
            {
				first.Add(t.Value1);
				second.Add(t.Value2);
				third.Add(t.Value3);
				fourth.Add(t.Value4);
			}

			return Tuples.Tuple(first as IEnumerable<T1>, second as IEnumerable<T2>, third as IEnumerable<T3>, fourth as IEnumerable<T4>);
		}
		public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> Unzip<T1, T2, T3, T4, T5>(IEnumerable<Tuple<T1, T2, T3, T4, T5>> ienum) {
            List<T1> first = new List<T1>();
            List<T2> second = new List<T2>();
            List<T3> third = new List<T3>();
            List<T4> fourth = new List<T4>();
            List<T5> fifth = new List<T5>();

            foreach (Tuple<T1, T2, T3, T4, T5> t in ienum)
            {
				first.Add(t.Value1);
				second.Add(t.Value2);
				third.Add(t.Value3);
				fourth.Add(t.Value4);
				fifth.Add(t.Value5);
			}

            return Tuples.Tuple(first as IEnumerable<T1>, second as IEnumerable<T2>, third as IEnumerable<T3>, fourth as IEnumerable<T4>, fifth as IEnumerable<T5>);
		}

		public static Tuple<T1, T2> AsTuple<T1, T2>(KeyValuePair<T1, T2> kvp) {
			return Tuples.Tuple(kvp.Key, kvp.Value);
		}

	}

}