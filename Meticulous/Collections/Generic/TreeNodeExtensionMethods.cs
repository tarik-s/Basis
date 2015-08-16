using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Collections.Generic
{
    /// <summary>
    /// 
    /// </summary>
    public static class TreeNodeExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static IEnumerable<IReadOnlyTreeNode<T, TNode>> EnumerateAll<T, TNode>(this IReadOnlyTreeNode<T, TNode> @this)
            where TNode : IReadOnlyTreeNode<T, TNode>
        {
            Check.This(@this);

            return EnumerateAllImpl(@this);
        }


        private static IEnumerable<IReadOnlyTreeNode<T, TNode>> EnumerateAllImpl<T, TNode>(IReadOnlyTreeNode<T, TNode> node)
            where TNode : IReadOnlyTreeNode<T, TNode>
        {
            foreach (var child in node)
            {
                yield return child;

                foreach (var subChild in EnumerateAllImpl(child))
                {
                    yield return subChild;
                }
            }
        }
    }
}
