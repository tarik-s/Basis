using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous
{
    //IComparable, IFormattable, IConvertible, IComparable<int>, IEquatable<int>

    [Serializable]
    public struct Progress : IComparable
    {
        private const int c_maxPrecision = 9;
        private const int c_minPrecision = 0;
        private const int c_defaultPrecision = 2;
        private const int c_intDigitCount = 2;

        private static readonly Progress s_undefined = new Progress(false);
        private static readonly int[] s_factors = new int[c_maxPrecision + 1] { 1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000, 1000000000 };

        private readonly int _value;

        private Progress(bool _)
        {
            _value = -1;
        }

        #region Constructors

        public Progress(int value)
        {
            Check.ArgumentInRange(value, "value", 0, s_factors[c_intDigitCount]);

            _value = value * s_factors[c_maxPrecision - c_intDigitCount];
        }

        public Progress(double value)
        {
            Check.ArgumentInRange(value, "value", 0.0, 1.0);

            _value = (int)(value * s_factors[c_maxPrecision]);
        }

        public Progress(double value, int precision)
        {
            Check.ArgumentInRange(value, "value", 0.0, 1.0);
            Check.ArgumentInRange(precision, "precision", c_minPrecision, c_maxPrecision);

            _value = (int)(value * s_factors[precision]) * s_factors[c_maxPrecision - precision];
        }

        #endregion

        #region Static properties

        public static int MaxPrecision
        {
            get { return c_maxPrecision; }
        }

        public static int MinPrecision
        {
            get { return c_minPrecision; }
        }

        public static int DefaultPrecision
        {
            get { return c_defaultPrecision; }
        }

        public static Progress Undefined
        {
            get { return s_undefined; }
        }

        #endregion

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (obj is Progress)
            {
                var p = (Progress)obj;
                return _value == p._value;
            }
            return false;
        }

        public override string ToString()
        {
            return ToString(c_defaultPrecision);
        }

        public string ToString(int precision)
        {
            if (_value < 0)
                return "undefined";

            return String.Format("{0} %", _value);
        }

        public int ToInt()
        {
            CheckProgress();

            return _value / s_factors[c_maxPrecision - c_intDigitCount];
        }


        public static explicit operator int(Progress p)
        {
            return p.ToInt();
        }

        private void CheckProgress()
        {
            if (_value < 0)
                throw new InvalidOperationException("Undefined progress");
        }
    }

}
