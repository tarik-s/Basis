using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticulous.Meta;
using NUnit.Framework;

namespace Meticulous.Tests.Meta
{
    [TestFixture]
    class MetaFieldTest
    {
        [TestCase("_name", EncapsulationLevel.Private)]
        public void CreationTest(string name, EncapsulationLevel encapsulationLevel)
        {
            var fb = new MetaFieldBuilder(name);
            fb.EncapsulationLevel = encapsulationLevel;
            fb.SetType(MetaType.Boolean);

            var field = fb.Build();
            Assert.AreEqual(field.Name, name);
            Assert.AreEqual(field.EncapsulationLevel, encapsulationLevel);
            Assert.AreEqual(field.Type, MetaType.Boolean);
            Assert.AreEqual(field.Module, null);
        }
    }
}
