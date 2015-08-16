using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Meticulous;

namespace Meticulous.Collections.Generic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public interface IReadOnlyTreeNode<T, out TNode> : IReadOnlyList<TNode>
        where TNode : IReadOnlyTreeNode<T, TNode>
    {
        /// <summary>
        /// 
        /// </summary>
        T Data { get; }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="node"></param>
        public ReadOnlyTreeNode(ITreeNode<T, TNode> node)
        {
            _node = node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ReadOnlyTreeNode<T, TNode>> GetEnumerator()
        {
            if (_node == null)
                return ((IReadOnlyList<ReadOnlyTreeNode<T, TNode>>) EmptyArray<ReadOnlyTreeNode<T, TNode>>.Value).GetEnumerator();

            return new NodeEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <param name="index"></param>
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
        /// 
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
        /// 
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
