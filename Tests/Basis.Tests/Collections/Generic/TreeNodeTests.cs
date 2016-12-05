using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticulous.Collections.Generic;
using NUnit.Framework;

namespace Meticulous.Tests.Collections.Generic
{
    [TestFixture]
    public class TreeNodeTests
    {
        [TestCase]
        public void Add_ChildTest()
        {
            var node = TreeNode<int>.Create(12);
            var subNode = node.AddChild(23);

            Assert.AreSame(subNode.Parent, node);
        }

        [TestCase]
        public void EnumerateAllTest()
        {
            var node = TreeNode<int>.Create(1);
            var subNode = node.AddChild(2);
            var subSubNode = subNode.AddChild(3);
            var subSubSubNode = subSubNode.AddChild(4);

            var all = node.EnumerateAll().ToArray();

            Assert.AreEqual(all.Length, 3);
            Assert.AreEqual(all[0].Data, 2);
            Assert.AreEqual(all[1].Data, 3);
            Assert.AreEqual(all[2].Data, 4);
        }


    }
}
