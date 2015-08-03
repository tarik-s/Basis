using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Meticulous.Tests
{
    [TestFixture]
    public class ExceptionHandlerTests
    {
        [TestCase(ExpectedResult = false)]
        public bool NeverHandlingTest()
        {
            return Throw(ExceptionHandler.NeverHandling);
        }

        [TestCase(ExpectedResult = true)]
        public bool AlwaysHandlingTest()
        {
            return Throw(ExceptionHandler.AlwaysHandling);
        }


        private bool Throw(ExceptionHandler handler)
        {
            try
            {
                throw new Exception("ExceptionHandlerTests");
            }
            catch (Exception e)
            {
                return handler.HandleException(e);
            }
        }
    }
}
