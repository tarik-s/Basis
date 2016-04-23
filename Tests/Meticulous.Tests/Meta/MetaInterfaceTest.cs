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

            var b1 = new MetaInterfaceBuilder("ITest2");
            b1.AddBaseInterface(intf);

            var b2 = new MetaInterfaceBuilder("IBaseTest");
            b1.AddBaseInterface(b2);

            var mb1 = b1.AddMethod("TestMethod2");
            var rtb1 = mb1.ReturnType;

            rtb1.SetType(MetaType.Float);

            var intf1 = b1.Build();

            Assert.AreEqual(intf1.BaseInterfaces.Length, 2);
            Assert.AreEqual(intf1.BaseInterfaces[0], intf);

        }

    }
}
