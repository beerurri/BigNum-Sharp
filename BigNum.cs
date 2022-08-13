using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigNum {
    public class BigNum {
        /// <summary>
        /// Represents Number object
        /// </summary>
        public class Number {
            /// <summary>
            /// Sign of Number<br />
            /// <c>true</c> if positive <c>(>=0)</c>,<br />
            /// otherwise <c>false</c>
            /// </summary>
            public bool Positive { get; set; }

            /// <summary>
            /// List of digits of a Number
            /// </summary>
            public List<byte> Num { get; set; }

            /// <summary>
            /// Point index from the end of a number, <c>0</c> if no point
            /// </summary>
            public int Point { get; set; }

            /// <summary>
            /// Creates (and returns) deep copy
            /// </summary>
            public Number Clone() {
                return new Number() {
                    Positive = Positive,
                    Num = new List<byte>(Num),
                    Point = Point
                };
            }
        }

        public static readonly Number Zero = new Number() { Positive = true, Point = 0, Num = new List<byte>() { 0 } };
        public static readonly Number One = new Number() { Positive = true, Point = 0, Num = new List<byte>() { 1 } };
        public static readonly Number Ten = new Number() { Positive = true, Point = 0, Num = new List<byte>() { 1, 0 } };
        public static readonly Number Two = new Number() { Positive = true, Point = 0, Num = new List<byte>() { 2 } };
        /// <summary>
        /// <c>0.5</c> or <c>1/2</c>
        /// </summary>
        public static readonly Number Half = new Number() { Positive = true, Point = 1, Num = new List<byte>() { 0, 5 } };

        // COMPARISON

        /// <summary>
        /// Used in comparing<br />
        /// <c>G</c> : Greater<br />
        /// <c>L</c> : Less<br />
        /// <c>E</c> : Equal
        /// </summary>
        public enum Comparison {
            G,
            L,
            E
        }

        /// <summary>
        /// Compares two Numbers<br />
        /// Returns:<br />
        /// <c>Comparison.E</c> : <c>num1</c> equals <c>num2</c><br /> 
        /// <c>Comparison.G</c> : <c>num1</c> greater than <c>num2</c><br />
        /// <c>Comparison.L</c> : <c>num1</c> less than <c>num2</c>
        /// </summary>
        public static Comparison Compare(Number __num1, Number __num2) {
            Number num1 = __num1.Clone();
            Number num2 = __num2.Clone();

            if (num1.Positive && !num2.Positive)
                return Comparison.G;

            if (!num1.Positive && num2.Positive)
                return Comparison.L;
            
            if (num1.Positive && num2.Positive) {
                if (num1.Num.Count - num1.Point > num2.Num.Count - num2.Point)
                    return Comparison.G;
                if (num1.Num.Count - num1.Point < num2.Num.Count - num2.Point)
                    return Comparison.L;

                Number[] nums = Shift(num1, num2);
                List<byte> _num1 = nums[0].Num;
                List<byte> _num2 = nums[1].Num;

                for (int i = 0; i < _num1.Count; i++) {
                    if (_num1[i] > _num2[i])
                        return Comparison.G;
                    if (_num1[i] < _num2[i])
                        return Comparison.L;
                }
            } else { // !num1.Positive && !num2.Positive
                if (num1.Num.Count - num1.Point > num2.Num.Count - num2.Point)
                    return Comparison.L;
                if (num1.Num.Count - num1.Point < num2.Num.Count - num2.Point)
                    return Comparison.G;

                Number[] nums = Shift(num1, num2);
                List<byte> _num1 = nums[0].Num;
                List<byte> _num2 = nums[1].Num;

                for (int i = 0; i < _num1.Count; i++) {
                    if (_num1[i] > _num2[i])
                        return Comparison.L;
                    if (_num1[i] < _num2[i])
                        return Comparison.G;
                }
            }

            return Comparison.E;
        }

        // SUMMATION
        public static async Task<string> SummAsync(string num1, string num2) {
            return NumberToString(Summ(StringToNumber(num1), StringToNumber(num2)));
        }

        public static string Summ(string num1, string num2) {
            return NumberToString(Summ(StringToNumber(num1), StringToNumber(num2)));
        }

        public static async Task<Number> SummAsync(Number num1, Number num2) {
            return Summ(num1, num2);
        }

        public static Number Summ(Number __num1, Number __num2) {
            Number num1 = __num1.Clone();
            Number num2 = __num2.Clone();

            if (num1.Positive && !num2.Positive)
                return Sub(num1, Abs(num2));

            if (!num1.Positive && num2.Positive)
                return Sub(num2, Abs(num1));

            bool posRes = !(!num1.Positive && !num2.Positive);

            Number[] nums = Shift(num1, num2);
            List<byte> _num1 = nums[0].Num;
            List<byte> _num2 = nums[1].Num;

            List<byte> _ret = Enumerable.Repeat((byte)0, _num1.Count).ToList();

            for (int i = _ret.Count - 1; i > 0; i--) {
                _ret[i] += (byte)(_num1[i] + _num2[i]);

                _ret[i - 1] += (byte)(_ret[i] / 10);
                _ret[i] = (byte)(_ret[i] % 10);
            }

            _ret[0] += (byte)(_num1[0] + _num2[0]);

            byte rem = (byte)(_ret[0] / 10);
            if (rem != 0) {
                _ret.Insert(0, rem);
                _ret[1] = (byte)(_ret[1] % 10);
            }

            Number ret = new();
            ret.Positive = posRes;
            ret.Num = _ret;
            ret.Point = Math.Max(nums[0].Point, nums[1].Point);

            return FixNumber(ret);
        }

        // SUBTRACTION
        public static async Task<string> SubAsync(string num1, string num2) {
            return NumberToString(Sub(StringToNumber(num1), StringToNumber(num2)));
        }

        public static string Sub(string num1, string num2) {
            return NumberToString(Sub(StringToNumber(num1), StringToNumber(num2)));
        }

        public static async Task<Number> SubAsync(Number num1, Number num2) {
            return Sub(num1, num2);
        }

        public static Number Sub(Number __num1, Number __num2) {
            Number num1 = __num1.Clone();
            Number num2 = __num2.Clone();

            if (num1.Positive && !num2.Positive)
                return Summ(num1, Abs(num2));

            if (!num1.Positive && num2.Positive)
                return ReverseSign(Summ(Abs(num1), num2));

            if (!num1.Positive && !num2.Positive)
                return Sub(Abs(num2), Abs(num1));

            // >>> if (num1.Positive && num2.Positive) >>>

            Comparison comp = Compare(num1, num2);
            if (comp == Comparison.E)
                return Zero;

            if (comp == Comparison.L)
                return ReverseSign(Sub(num2, num1));

            // >>> if (num1 > num2)

            Number[] nums = Shift(num1, num2);
            List<byte> _num1 = nums[0].Num;
            List<byte> _num2 = nums[1].Num;

            List<byte> _ret = Enumerable.Repeat((byte)0, _num1.Count).ToList();

            for (int i = _ret.Count - 1; i > 0; i--) {
                if (_num1[i] < _num2[i]) {
                    _num1[i] += 10;
                    for (int j = i - 1; j >= 0; j--) {
                        if (_num1[j] == 0)
                            _num1[j] = 9;
                        else {
                            _num1[j] -= 1;
                            break;
                        }
                    }
                }
                _ret[i] = (byte)(_num1[i] - _num2[i]);
            }
            _ret[0] = (byte)(_num1[0] - _num2[0]);

            return FixNumber(new Number() {
                Positive = true,
                Point = Math.Max(nums[0].Point, nums[1].Point),
                Num = _ret
            });
        }

        // MULTIPLICATION
        public static async Task<string> MultAsync(string num1, string num2) {
            return NumberToString(Mult(StringToNumber(num1), StringToNumber(num2)));
        }

        public static string Mult(string num1, string num2) {
            return NumberToString(Mult(StringToNumber(num1), StringToNumber(num2)));
        }

        public static async Task<Number> MultAsync(Number num1, Number num2) {
            return Mult(num1, num2);
        }

        public static Number Mult(Number _num1, Number _num2) {
            Number num1 = _num1.Clone();
            Number num2 = _num2.Clone();

            bool posRes = true;
            if ((!num1.Positive && num2.Positive) || (num1.Positive && !num2.Positive))
                posRes = false;

            num1 = Abs(num1);
            num2 = Abs(num2);

            Comparison comp1Zero = Compare(num1, Zero);
            Comparison comp2Zero = Compare(num2, Zero);

            if (comp1Zero == Comparison.E || comp2Zero == Comparison.E)
                return Zero;

            Comparison comp1One = Compare(num1, One);
            Comparison comp2One = Compare(num2, One);
            if (comp1One == Comparison.E)
                return new Number() {
                    Positive = posRes,
                    Num = num2.Num,
                    Point = num2.Point
                };
            if (comp2One == Comparison.E)
                return new Number() {
                    Positive = posRes,
                    Num = num1.Num,
                    Point = num1.Point
                };

            Number ret = new();
            ret.Point = num1.Point + num2.Point;
            ret.Positive = posRes;

            List<int> A = ByteListToIntList(num1.Num);
            List<int> B = ByteListToIntList(num2.Num);
            List<int> C = Enumerable.Repeat(0, (A.Count + B.Count + 1)).ToList();

            for (int i = A.Count - 1; i >= 0; i--) {
                for (int j = B.Count - 1; j >= 0; j--) {
                    C[C.Count - (A.Count - i + B.Count - j) + 1] += A[i] * B[j];
                }
                for (int k = C.Count - (A.Count - i); k >= C.Count - (A.Count - i + B.Count) + 1; k--) {
                    C[k - 1] += C[k] / 10;
                    C[k] %= 10;
                }
            }

            ret.Num = C.Select(x => (byte)x).ToList();

            return FixNumber(ret);
        }

        // DIVISION
        public static async Task<string> DivAsync(string num1, string num2, int precision = 100) {
            return NumberToString(Div(StringToNumber(num1), StringToNumber(num2), precision));
        }

        public static string Div(string num1, string num2, int precision = 100) {
            return NumberToString(Div(StringToNumber(num1), StringToNumber(num2), precision));
        }

        public static async Task<Number> DivAsync(Number num1, Number num2, int precision = 100) {
            return Div(num1, num2, precision);
        }

        public static Number Div(Number _num1, Number _num2, int precision = 100) {
            Number num1 = _num1;
            Number num2 = _num2;

            bool posRes = true;
            if ((!num1.Positive && num2.Positive) || (num1.Positive && !num2.Positive))
                posRes = false;

            num1 = Abs(num1);
            num2 = Abs(num2);

            Comparison comp1Zero = Compare(num1, Zero);
            Comparison comp2Zero = Compare(num2, Zero);

            if (comp1Zero == Comparison.E && comp2Zero != Comparison.E)
                return Zero;

            if (comp1Zero != Comparison.E && comp2Zero == Comparison.E)
                throw new DivideByZeroException($"Div(num1, num2): Division of num1 by zero");

            Comparison comp2One = Compare(num2, One);

            if (comp2One == Comparison.E)
                return new Number() {
                    Positive = posRes,
                    Num = num1.Num,
                    Point = num1.Point
                };

            Number ret = Zero;

            //uint counter = 0;
            for (int counter = 0; counter < precision * 2 - 1; counter++) {
                Number[] eu = EuDiv(num1, num2);

                ret = Summ(ret, TenDiv(eu[0], counter));
                num1 = Mult(eu[1], Ten);

                if (Compare(num1, Zero) == Comparison.E)
                    break;
            }

            return Truncate(ret, precision);
        }

        /// <summary>
        /// Euclidean division – or division with remainder<br />
        /// Returns <c>Number[2] = { integerQuotient, integerRemainder }</c>
        /// </summary>
        public static Number[] EuDiv(Number _num1, Number _num2) {
            Number num1 = Abs(_num1.Clone());
            Number num2 = Abs(_num2.Clone());

            Number[] ret = new Number[2] { Zero, Zero };

            Comparison comp = Compare(_num1, _num2);
            while (comp != Comparison.L) {
                _num1 = Sub(_num1, _num2);
                ret[0] = Summ(ret[0], One);
                comp = Compare(_num1, _num2);
            }
            ret[1] = _num1;

            return ret;
        }

        // POW
        public static async Task<string> PowAsync(string num, int power) {
            return NumberToString(Pow(StringToNumber(num), power));
        }

        public static string Pow(string num, int power) {
            return NumberToString(Pow(StringToNumber(num), power));
        }

        public static async Task<Number> PowAsync(Number num, int power) {
            return Pow(num, power);
        }

        public static Number Pow(Number _num, int power) {
            Number num = _num.Clone();
            if (power == 0)
                return One;
            if (power == 1)
                return num;

            if (power < 0)
                return Div(One, Pow(num, -power));

            Number ret = One;
            for (uint i = 0; i < power; i++) {
                ret = Mult(ret, num);
            }
            return ret;
        }

        // ROOT (TO DO)
        public static string Root(string _num, int power) {
            return NumberToString(Root(StringToNumber(_num), power));
        }

        public static Number Root(Number _num, int power, int precision = 100) {
            if (power == 0)
                throw new DivideByZeroException("Root with 0 power");

            Number num = _num.Clone();

            if (power < 0)
                return Div(One, Root(num, -power));

            if (Compare(One, num) == Comparison.E)
                return One;

            if (Compare(ReverseSign(One), num) == Comparison.E)
                return (power % 2 == 0 ? One : ReverseSign(One));

            Number root = Mult(TenDiv(One, 1), num);
            Number eps = TenDiv(One, precision);

            while (Compare(Sub(root, Div(num, root)), eps) == Comparison.G) {
                root = Mult(Half, Summ(root, Div(num, root)));
            }

            return root;
        }

        // FEATURES

        /// <summary>
        /// Like <c>Math.Abs()</c>
        /// </summary>
        public static Number Abs(Number _num) {
            Number num = _num.Clone();
            num.Positive = true;
            return num;
        }

        /// <summary>
        /// 'Multiplies' <c>_num</c> by <c>-1</c>
        /// </summary>
        public static Number ReverseSign(Number _num) {
            Number num = _num.Clone();
            num.Positive = !num.Positive;
            return num;
        }

        /// <summary>
        /// Divides <c>num</c> by <c>10^zeroCount</c> (moves Point)
        /// </summary>
        public static string TenDiv(string num, int zerosCount) {
            return NumberToString(TenDiv(StringToNumber(num), zerosCount));
        }

        /// <summary>
        /// Divides <c>num</c> by <c>10^zeroCount</c> (moves Point)
        /// </summary>
        public static Number TenDiv(Number _num, int zerosCount) {
            Number num = _num.Clone();
            Number ret = new();
            ret.Num = num.Num;
            ret.Positive = num.Positive;
            ret.Point = num.Point + zerosCount;

            if (ret.Num.Count <= ret.Point)
                ret.Num.InsertRange(0, Enumerable.Repeat((byte)0, ret.Point - ret.Num.Count + 1).ToList());

            return FixNumber(ret);
        }

        /// <summary>
        /// Rounds the <c>_num</c> towards zero.
        /// </summary>
        public static Number Truncate(Number _num, int precision) {
            Number num = _num.Clone();
            if (num.Point <= precision)
                return num;

            int toRem = num.Point - precision;

            num.Num.RemoveRange((num.Num.Count - toRem), toRem);
            num.Point = num.Point - toRem;

            return num;
        }

        /// <summary>
        /// Rounds down toward negative infinity
        /// </summary>
        public static Number Floor(Number _num, int precision) {
            Number num = _num.Clone();
            if (num.Point == 0 || num.Point <= precision)
                return num;

            if (num.Positive)
                return Truncate(num, precision);

            int toRem = num.Point - precision;

            byte l = num.Num[num.Num.Count - toRem + 1];

            num.Num.RemoveRange((num.Num.Count - toRem), toRem);
            num.Point = num.Point - toRem;

            if (l > 4) {
                num.Num[num.Num.Count - 1] += 1;
                for (int i = num.Num.Count - 1; i > 0; i--) {
                    if (num.Num[i] / 10 != 0) {
                        num.Num[i - 1] /= 10;
                        num.Num[i] %= 10;
                    } else
                        break;
                }
                if (num.Num[0] > 9) {
                    num.Num.Insert(0, (byte)(num.Num[0] / 10));
                    num.Num[1] %= 10;
                }
            }

            return num;
        }

        /// <summary>
        /// Rounds up toward positive infinity
        /// </summary>
        public static Number Ceiling(Number _num, int precision) {
            Number num = _num.Clone();
            if (num.Point == 0 || num.Point <= precision)
                return num;

            if (!num.Positive)
                return Truncate(num, precision);

            int toRem = num.Point - precision;

            byte l = num.Num[num.Num.Count - toRem + 1];

            num.Num.RemoveRange((num.Num.Count - toRem), toRem);
            num.Point = num.Point - toRem;

            if (l > 4) {
                num.Num[num.Num.Count - 1] += 1;
                for (int i = num.Num.Count - 1; i > 0; i--) {
                    if (num.Num[i] / 10 != 0) {
                        num.Num[i - 1] /= 10;
                        num.Num[i] %= 10;
                    } else
                        break;
                }
                if (num.Num[0] > 9) {
                    num.Num.Insert(0, (byte)(num.Num[0] / 10));
                    num.Num[1] %= 10;
                }
            }

            return num;
        }

        /// <summary>
        /// Converts Number to its string representation
        /// </summary>
        public static string NumberToString(Number num) {
            string ret = string.Empty;
            if (num.Point > 0) {
                for (int i = 0; i < num.Num.Count - num.Point; i++) {
                    ret += num.Num[i].ToString();
                }
                ret += ".";
                for (int i = num.Num.Count - num.Point; i < num.Num.Count; i++) {
                    ret += num.Num[i].ToString();
                }
            } else {
                for (var i = 0; i < num.Num.Count; i++) {
                    ret += num.Num[i].ToString();
                }
            }
            return FixString((num.Positive ? "" : "-") + ret);
        }

        /// <summary>
        /// Converts string to Number
        /// </summary>
        public static Number StringToNumber(string num) {
            num = FixString(num);
            Number ret = new Number();
            ret.Num = new();

            if (num.Contains('-')) {
                ret.Positive = false;
                num = num.Replace("-", "");
            } else
                ret.Positive = true;

            List<string> parts = new();
            if (num.Contains('.')) {
                parts = num.Split('.').ToList();
                ret.Point = parts[1].Length;
            } else
                parts.Add(num);

            foreach (string part in parts) {
                foreach (char ch in part.ToCharArray()) {
                    ret.Num.Add(byte.Parse(ch.ToString()));
                }
            }

            return ret;
        }

        /// <summary>
        /// The number e, aka Euler's number
        /// </summary>
        public static string eString(int precision = 200) {
            return NumberToString(eNumber(precision));
        }

        /// <summary>
        /// The number e, aka Euler's number
        /// </summary>
        public static Number eNumber(int precision = 200) {
            Number e = One;
            Number fact = One;
            Number iter = One;
            Task[] t = new Task[2];

            while (fact.Num.Count <= precision * 1.5) {
                Number _fact = fact.Clone();
                t[0] = Task.Run(() => {
                    e = Summ(e, Div(One, _fact, precision: precision));
                });
                t[1] = Task.Run(() => {
                    iter = Summ(iter, One);
                    fact = Mult(fact, iter);
                });
                Task.WaitAll(t);
            }
            return e;
        }

        // STAFF ONLY

        /// <summary>
        /// Fixes Number represented as string
        /// </summary>
        public static string FixString(string num) {
            bool pos = !num.Contains('-');
            if (!pos)
                num = num.Replace("-", "");

            num = num.Replace(',', '.');

            if (num.Contains('.')) {
                num = num.TrimStart('0');
                if (num.StartsWith('.'))
                    num = "0" + num;
                num = num.TrimEnd('0');
                if (num.EndsWith('.'))
                    return (pos ? "" : "-") + num.Replace(".", "");
                else
                    return (pos ? "" : "-") + num;
            } else {
                num = num.TrimStart('0');
                if (string.IsNullOrEmpty(num))
                    num = "0";

                return (pos ? "" : "-") + num;
            }
        }

        /// <summary>
        /// Fixes Number
        /// </summary>
        public static Number FixNumber(Number _num) {
            Number num = _num.Clone();
            for (int i = 0; i < num.Num.Count - num.Point; i++) {
                if (num.Num[i] != 0) {
                    num.Num.RemoveRange(0, i);
                    break;
                }
            }
            if (num.Num.Count == num.Point)
                num.Num.Insert(0, (byte)0);
            for (int i = num.Num.Count - 1; i > num.Num.Count - num.Point; i--) {
                if (num.Num[i] != 0) {
                    int toRem = num.Num.Count - 1 - i;
                    num.Num.RemoveRange(i + 1, toRem);
                    num.Point -= toRem;
                    break;
                }
            }
            return num;
        }

        /// <summary>
        /// Prepairs Number for Comparison, Summation and Subtraction
        /// </summary>
        private static Number[] Shift(Number _num1, Number _num2) {
            Number num1 = _num1.Clone();
            Number num2 = _num2.Clone();
            if (num1.Point == 0 && num2.Point == 0) {
                if (num1.Num.Count == num2.Num.Count)
                    return new Number[] { num1, num2 };
            }

            if (num1.Point == 0 && num2.Point != 0) {
                num1.Num.AddRange(Enumerable.Repeat((byte)0, num2.Point));
                num1.Point = num2.Point;
            }

            if (num1.Point != 0 && num2.Point == 0) {
                num2.Num.AddRange(Enumerable.Repeat((byte)0, num1.Point));
                num2.Point = num1.Point;
            }

            if (num1.Point != 0 && num2.Point != 0) {
                if (num1.Point > num2.Point) {
                    num2.Num.AddRange(Enumerable.Repeat((byte)0, (num1.Point - num2.Point)));
                    num2.Point = num1.Point;
                } else if (num1.Point < num2.Point) {
                    num1.Num.AddRange(Enumerable.Repeat((byte)0, (num2.Point - num1.Point)));
                    num1.Point = num2.Point;
                }
            }

            if (num1.Num.Count > num2.Num.Count)
                num2.Num.InsertRange(0, Enumerable.Repeat((byte)0, (num1.Num.Count - num2.Num.Count)));
            else if (num1.Num.Count < num2.Num.Count)
                num1.Num.InsertRange(0, Enumerable.Repeat((byte)0, (num2.Num.Count - num1.Num.Count)));

            return new Number[] { num1, num2 };
        }

        private static List<int> ByteListToIntList(List<byte> l) {
            return l.Select(x => (int)x).ToList();
        }
    }
}
