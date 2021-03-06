﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Meticulous;

namespace Meticulous.Collections.Generic
{
    /// <summary>
    /// IReadOnlyTreeNode
    /// </summary>
    /// <typeparam name="T">Type of data</typeparam>
    /// <typeparam name="TNode">The type of the node.</typeparam>
    public interface IReadOnlyTreeNode<out T, out TNode> : IReadOnlyList<TNode>
        where TNode : IReadOnlyTreeNode<T, TNode>
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        T Data { get; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        TNode Parent { get; }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public struct ReadOnlyTreeNode<T, TNode> : IReadOnlyTreeNode<T, ReadOnlyTreeNode<T, TNode>>
        where TNode : class, ITreeNode<T, TNode>
    {
        private readonly ITreeNode<T, TNode> _node;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyTreeNode{T, TNode}"/> struct.
        /// </summary>
        /// <param name="node">The node.</param>
        public ReadOnlyTreeNode(ITreeNode<T, TNode> node)
        {
            _node = node;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ReadOnlyTreeNode<T, TNode>> GetEnumerator()
        {
            if (_node == null)
                return ((IReadOnlyList<ReadOnlyTreeNode<T, TNode>>) EmptyArray<ReadOnlyTreeNode<T, TNode>>.Value).GetEnumerator();

            return new NodeEnumerator(this);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        public int Count
        {
            get
            {
                if (_node != null)
                    return _node.Count;

                return 0;
            }
        }

        /// <summary>
        /// Gets the <see cref="Meticulous.Collections.Generic.ReadOnlyTreeNode{T,TNode}"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="Meticulous.Collections.Generic.ReadOnlyTreeNode{T,TNode}"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public ReadOnlyTreeNode<T, TNode> this[int index]
        {
            get
            {
                if (_node == null)
                    throw new ArgumentOutOfRangeException("index");

                return new ReadOnlyTreeNode<T, TNode>(_node[index]);
            }
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        public T Data
        {
            get
            {
                if (_node == null)
                    return default(T);

                return _node.Data;
            }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        public ReadOnlyTreeNode<T, TNode> Parent
        {
            get { return new ReadOnlyTreeNode<T, TNode>(_node.Parent); }
        }

        private sealed class NodeEnumerator : IEnumerator<ReadOnlyTreeNode<T, TNode>>
        {
            private readonly IEnumerator<ITreeNode<T, TNode>> _enumerator;

            public NodeEnumerator(ReadOnlyTreeNode<T, TNode> node)
            {
                _enumerator = node._node.GetEnumerator();
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator.Reset();
            }

            public ReadOnlyTreeNode<T, TNode> Current
            {
                get
                {
                    var current = _enumerator.Current;
                    return new ReadOnlyTreeNode<T, TNode>(current);
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}
