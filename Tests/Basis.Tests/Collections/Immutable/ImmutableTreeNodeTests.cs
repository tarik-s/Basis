using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticulous.Collections.Generic;
using Meticulous.Collections.Immutable;
using NUnit.Framework;

namespace Meticulous.Tests.Collections.Immutable
{
    [TestFixture]
    class ImmutableTreeNodeTests
    {
        [Test]
        public void ToImmutableTest()
        {
            var node = TreeNode.Create(1);
            node.AddChild(11).AddChild(21);
            node.AddChild(12).AddChild(22);


            var imNode = node.ToImmutableTreeNode();
            
            Assert.AreEqual(imNode.Count, node.Count);

        }

    }
}
