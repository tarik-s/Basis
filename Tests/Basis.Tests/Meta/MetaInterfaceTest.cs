using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticulous.Meta;
using NUnit.Framework;

namespace Meticulous.Tests.Meta
{
    [TestFixture]
    public class MetaInterfaceTest
    {
        [TestCase("ITest")]
        public void TestCreation(string name)
        {
            var b = new MetaInterfaceBuilder(name);

            var mb = b.AddMethod("TestMethod");

            var rtb = mb.ReturnType;
            rtb.SetType(MetaType.String);

            var intf = b.Build();

            Assert.AreEqual(intf.Name, name);
            Assert.AreEqual(intf.Module, null);

            Assert.AreEqual(intf.Methods[0].ReturnType, MetaType.String);
            Assert.AreEqual(intf.Methods[0].Name, "TestMethod");
        }

    }
}
