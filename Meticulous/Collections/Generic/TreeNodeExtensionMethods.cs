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
        /// Enumerates all.
        /// </summary>
        /// <typeparam name="T">The node data</typeparam>
        /// <typeparam name="TNode">The type of the node.</typeparam>
        /// <param name="this">The @this.</param>
        /// <returns></returns>
        public static IEnumerable<IReadOnlyTreeNode<T, TNode>> EnumerateAll<T, TNode>(this IReadOnlyTreeNode<T, TNode> @this)
            where TNode : IReadOnlyTreeNode<T, TNode>
        {
            Check.This(@this);

            return EnumerateAllImpl(@this);
        }

        /// <summary>
        /// For each.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TNode">The type of the node.</typeparam>
        /// <param name="this">The @this.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T, TNode>(this IReadOnlyTreeNode<T, TNode> @this, Action<IReadOnlyTreeNode<T, TNode>, int> action)
            where TNode : IReadOnlyTreeNode<T, TNode>
        {
            Check.This(@this);
            Check.ArgumentNotNull(action, "action");

            ForEachImpl(@this, 0, action);
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

        private static void ForEachImpl<T, TNode>(this IReadOnlyTreeNode<T, TNode> node, int index, Action<IReadOnlyTreeNode<T, TNode>, int> action)
            where TNode : IReadOnlyTreeNode<T, TNode>
        {
            action(node, index);

            foreach (var subNode in node)
            {
                ForEachImpl<T, TNode>(subNode, index + 1, action);
            }
        }
    }
}
