using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticulous.Threading;
using NUnit.Framework;

namespace Meticulous.Tests.Threading
{
    #region AtomicBooleanTests

    public abstract class AtomicBooleanTestsBase<T>
        where T : IAtomic<bool>
    {
        [TestCase(true, true, ExpectedResult = true)]
        [TestCase(true, false, ExpectedResult = false)]
        [TestCase(false, true, ExpectedResult = true)]
        [TestCase(false, false, ExpectedResult = false)]
        [TestCase(null, true, ExpectedResult = true)]
        [TestCase(null, false, ExpectedResult = false)]
        public bool ValueTest(bool? initialValue, bool newValue)
        {
            var b = Create(initialValue);
            b.Value = newValue;
            return b.Value;
        }

        [TestCase(true, true, ExpectedResult = true)]
        [TestCase(true, false, ExpectedResult = true)]
        [TestCase(false, true, ExpectedResult = false)]
        [TestCase(false, false, ExpectedResult = false)]
        [TestCase(null, true, ExpectedResult = false)]
        [TestCase(null, false, ExpectedResult = false)]
        public bool ExchangeTest(bool? initialValue, bool newValue)
        {
            var b = Create(initialValue);
            var result = b.Exchange(newValue);
            return result;
        }

        [TestCase(true, true, ExpectedResult = false)]
        [TestCase(true, false, ExpectedResult = true)]
        [TestCase(false, true, ExpectedResult = true)]
        [TestCase(false, false, ExpectedResult = false)]
        [TestCase(null, true, ExpectedResult = true)]
        [TestCase(null, false, ExpectedResult = false)]
        public bool TrySetTest(bool? initialValue, bool newValue)
        {
            var b = Create(initialValue);
            var result = b.TrySet(newValue);
            return result;
        }

        protected abstract T Create(bool? initialValue);

    }

    [TestFixture]
    public class AtomicBooleanValueTests : AtomicBooleanTestsBase<AtomicBooleanValue>
    {
        protected override AtomicBooleanValue Create(bool? initialValue)
        {
            if (initialValue.HasValue)
                return new AtomicBooleanValue(initialValue.Value);

            return new AtomicBooleanValue();
        }
    }

    [TestFixture]
    public class AtomicBooleanTests : AtomicBooleanTestsBase<AtomicBoolean>
    {
        protected override AtomicBoolean Create(bool? initialValue)
        {
            if (initialValue.HasValue)
                return new AtomicBoolean(initialValue.Value);

            return new AtomicBoolean();
        }
    }

    #endregion

    #region AtomicIntergerTests

    public abstract class AtomicIntegerTestsBase<T>
        where T : IAtomic<int>
    {
        [TestCase(10, 12, ExpectedResult = 12)]
        [TestCase(null, 2, ExpectedResult = 2)]
        public int ValueTest(int? initialValue, int newValue)
        {
            var b = Create(initialValue);
            b.Value = newValue;
            return b.Value;
        }

        [TestCase(23, 42, ExpectedResult = 23)]
        [TestCase(0, 42, ExpectedResult = 0)]
        [TestCase(null, 12, ExpectedResult = 0)]
        public int ExchangeTest(int? initialValue, int newValue)
        {
            var b = Create(initialValue);
            var result = b.Exchange(newValue);
            return result;
        }

        [TestCase(10, 42, ExpectedResult = true)]
        [TestCase(42, 42, ExpectedResult = false)]
        [TestCase(null, 0, ExpectedResult = false)]
        [TestCase(null, 42, ExpectedResult = true)]
        public bool TrySetTest(int? initialValue, int newValue)
        {
            var b = Create(initialValue);
            var result = b.TrySet(newValue);
            return result;
        }

        protected abstract T Create(int? initialValue);

    }

    [TestFixture]
    public class AtomicIntergerValueTests : AtomicIntegerTestsBase<AtomicIntegerValue>
    {
        protected override AtomicIntegerValue Create(int? initialValue)
        {
            if (initialValue.HasValue)
                return new AtomicIntegerValue(initialValue.Value);

            return new AtomicIntegerValue();
        }
    }

    [TestFixture]
    public class AtomicIntegerTests : AtomicIntegerTestsBase<AtomicInteger>
    {
        protected override AtomicInteger Create(int? initialValue)
        {
            if (initialValue.HasValue)
                return new AtomicInteger(initialValue.Value);

            return new AtomicInteger();
        }
    }

    #endregion
}
