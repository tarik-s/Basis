using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticulous.Threading;
using NUnit.Framework;

namespace Meticulous.Tests.Threading
{
    [TestFixture]
    public class AtomicTest
    {

    }



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
}
