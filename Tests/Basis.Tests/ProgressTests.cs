using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Meticulous.Tests
{
    [TestFixture]
    class ProgressTests
    {

        [TestCase(100, ExpectedResult = 100)]
        [TestCase(0, ExpectedResult = 0)]
        [TestCase(12, ExpectedResult = 12)]
        [TestCase(120, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(-1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public int ConstructWithInt(int value)
        {
            var p = new Progress(value);

            return p.ToInt();
        }


        [TestCase(0.0, ExpectedResult = 0)]
        [TestCase(1.0, ExpectedResult = 100)]
        [TestCase(0.3, ExpectedResult = 30)]
        [TestCase(12, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(-1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public int ConstructWithDouble(double value)
        {
            var p = new Progress(value);

            return p.ToInt();
        }

        [TestCase(0.0, 0, ExpectedResult = 0)]
        [TestCase(1.0, 0, ExpectedResult = 100)]
        [TestCase(0.99, 0, ExpectedResult = 0)]
        [TestCase(0.35, 1, ExpectedResult = 30)]
        [TestCase(0.35, 2, ExpectedResult = 35)]
        [TestCase(12, 0, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(-1, 0, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public int ConstructWithDoubleAndPrecision(double value, int precision)
        {
            var p = new Progress(value, precision);

            return p.ToInt();
        }

    }
}
