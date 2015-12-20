using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Meticulous.Meta;

namespace Meticulous.Tests.Meta
{ 
    [TestFixture]
    class MetaModuleTests
    {
        [TestCase("hello")]
        public void CreationTest(string name)
        {
            var mb = new MetaModuleBuilder(name);

            var m = mb.Build();
        }

    }
}
