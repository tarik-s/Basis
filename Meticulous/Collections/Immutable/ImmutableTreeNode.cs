﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticulous.Collections.Generic;
using Meticulous.Patterns;

namespace Meticulous.Collections.Immutable
{
    /// <summary>
    /// 
    /// </summary>
    public static class ImmutableTreeNode
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static ImmutableTreeNode<T> ToImmutableTreeNode<T, TNode>(this IReadOnlyTreeNode<T, TNode> @this)
            where TNode : IReadOnlyTreeNode<T, TNode>
        {
            Check.This(@this);

            var result = @this as ImmutableTreeNode<T>;
            if (result != null)
                return result;

            return MakeImmutable(@this, null);
        }




        private static ImmutableTreeNode<T> MakeImmutable<T, TNode>(this IReadOnlyTreeNode<T, TNode> node, ImmutableTreeNode<T> parent)
            where TNode : IReadOnlyTreeNode<T, TNode>
        {
            return new ImmutableTreeNode<T>(node.Data, parent, p => EnumerateChildren(node, p));
        }

        private static IEnumerable<ImmutableTreeNode<T>> EnumerateChildren<T, TNode>(IEnumerable<TNode> node, ImmutableTreeNode<T> parent)
            where TNode : IReadOnlyTreeNode<T, TNode>
        {
            return node.Select(n => MakeImmutable(n, parent));
        }
    }

    /// <summary>
    /// Immutable tree node
    /// </summary>
    public class ImmutableTreeNode<T> : ITreeNode<T, ImmutableTreeNode<T>>, IReadOnlyTreeNode<T, ImmutableTreeNode<T>>
    {
        private static readonly ImmutableTreeNode<T>[] s_emptyNodes = EmptyArray<ImmutableTreeNode<T>>.Value;
        private static readonly ImmutableTreeNode<T> s_empty = new ImmutableTreeNode<T>();

        private readonly T _data;
        private readonly ImmutableTreeNode<T>[] _nodes;
        private readonly ImmutableTreeNode<T> _parent;

        private ImmutableTreeNode()
        {
            _nodes = s_emptyNodes;
        }

        internal ImmutableTreeNode(T data, ImmutableTreeNode<T> parent, Func<ImmutableTreeNode<T>, IEnumerable<ImmutableTreeNode<T>>> childrenFactory)
        {
            _data = data;
            _parent = parent;
            _nodes = childrenFactory(this).ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        public static ImmutableTreeNode<T> Empty
        {
            get { return s_empty; }
        }

        /// <summary>
        /// Gets the node data
        /// </summary>
        public T Data
        {
            get { return _data; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ImmutableTreeNode<T> Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ImmutableTreeNode<T>> GetEnumerator()
        {
            return ((IReadOnlyList<ImmutableTreeNode<T>>)_nodes).GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        void ICollection<ImmutableTreeNode<T>>.Add(ImmutableTreeNode<T> item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        void ICollection<ImmutableTreeNode<T>>.Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ImmutableTreeNode<T> item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ImmutableTreeNode<T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<ImmutableTreeNode<T>>.Remove(ImmutableTreeNode<T> item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return _nodes.Length; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(ImmutableTreeNode<T> item)
        {
            return Array.IndexOf(_nodes, item, 0, _nodes.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        void IList<ImmutableTreeNode<T>>.Insert(int index, ImmutableTreeNode<T> item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        void IList<ImmutableTreeNode<T>>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        ImmutableTreeNode<T> IList<ImmutableTreeNode<T>>.this[int index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ImmutableTreeNode<T> this[int index]
        {
            get
            {
                Check.ArgumentInRange(index, "index", _nodes);
                return _nodes[index];
            }
        }

        /// <summary>
        ///
        /// </summary>
        T ITreeNode<T, ImmutableTreeNode<T>>.Data
        {
            get { return this.Data; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public class Builder : IBuilder<ImmutableTreeNode<T>>
        {
            private readonly TreeNode<T> _node;

            /// <summary>
            /// 
            /// </summary>
            public Builder()
            {
                _node = new TreeNode<T>();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="data"></param>
            public Builder(T data)
            {
                _node = new TreeNode<T>(data);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public ImmutableTreeNode<T> Build()
            {
                return _node.ToImmutableTreeNode();
            }
        }

    }
}
