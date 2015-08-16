using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Collections.Generic
{
    /// <summary>
    /// 
    /// </summary>
    public static class TreeNode
    {
        /// <summary>
        /// Creates a TreeNode root
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static TreeNode<T> Create<T>(T data)
        {
            return new TreeNode<T>(data);
        }
    }

    /// <summary>
    /// ITreeNode
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TNode">The type of the node.</typeparam>
    public interface ITreeNode<T, TNode> : IList<TNode>
        where TNode : ITreeNode<T, TNode>
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        T Data { get; set; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        TNode Parent { get; }
    }

    /// <summary>
    /// Tree node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNode<T> : ITreeNode<T, TreeNode<T>>, IReadOnlyTreeNode<T, TreeNode<T>>
    {
        #region Fields

        private static readonly List<TreeNode<T>> s_emptyNodes = new List<TreeNode<T>>();

        private T _data;
        private TreeNode<T> _parent;
        private List<TreeNode<T>> _nodes;

        #endregion

        private TreeNode(T data, TreeNode<T> parent)
        {
            _data = data;
            _parent = parent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode{T}"/> class.
        /// </summary>
        public TreeNode()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode{T}"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public TreeNode(T data)
        {
            _data = data;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TreeNode{T}"/> class.
        /// </summary>
        /// <param name="data">The data</param>
        /// <returns></returns>
        public static TreeNode<T> Create(T data)
        {
            return new TreeNode<T>(data);
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public T Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        public TreeNode<T> Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Creates a readonly representation of the tree node
        /// </summary>
        /// <returns></returns>
        public ReadOnlyTreeNode<T, TreeNode<T>> AsReadOnly()
        {
            return new ReadOnlyTreeNode<T, TreeNode<T>>(this);
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public TreeNode<T> AddChild(T data)
        {
            var child = new TreeNode<T>(data, this);
            Nodes.Add(child);
            return child;
        }

        #region IList

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TreeNode<T>> GetEnumerator()
        {
            if (_nodes == null)
                return s_emptyNodes.GetEnumerator();

            return _nodes.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Add(TreeNode<T> item)
        {
            CheckNode(item, "item");

            Nodes.Add(item);
            item._parent = this;
        }

        /// <summary>
        /// Adds a range of items.
        /// </summary>
        /// <param name="items">The items.</param>
        public void AddRange(IEnumerable<T> items)
        {
            Check.ArgumentNotNull(items, "items");

            Nodes.AddRange(items.Select(i => new TreeNode<T>(i, this)));
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            if (_nodes != null)
            {
                _nodes.ForEach(n => n._parent = null);
                _nodes.Clear();
            }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        public bool Contains(TreeNode<T> item)
        {
            if (_nodes == null)
                return false;

            return _nodes.Contains(item);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void CopyTo(TreeNode<T>[] array, int arrayIndex)
        {
            Nodes.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool Remove(TreeNode<T> item)
        {
            if (_nodes == null || item.Parent != this)
                return false;

            if (_nodes.Remove(item))
            {
                item._parent = null;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count 
        {
            get
            {
                return _nodes == null ? 0 : _nodes.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        public bool IsReadOnly 
        {
            get { return false; }
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <returns>
        /// The index of <paramref name="item" /> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(TreeNode<T> item)
        {
            if (_nodes == null)
                return -1;

            return _nodes.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        public void Insert(int index, TreeNode<T> item)
        {
            CheckNode(item, "item");

            Nodes.Insert(index, item);
            item._parent = this;
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            var node = Nodes[index];
            Nodes.RemoveAt(index);
            node._parent = null;
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public TreeNode<T> this[int index]
        {
            get { return Nodes[index]; }
            set
            {
                CheckNode(value, "value");

                var oldValue = Nodes[index];
                Nodes[index] = value;
                value._parent = this;
                if (oldValue != null)
                    oldValue._parent = null;
            }
        }

        #endregion

        private List<TreeNode<T>> Nodes
        {
            get { return _nodes ?? (_nodes = new List<TreeNode<T>>()); }
        }

        private void CheckNode(TreeNode<T> node, string nodeName)
        {
            Check.ArgumentNotNull(node, nodeName);

            if (node == this)
                throw new ArgumentException("The item belongs to other tree", "item");

            var itemParent = node.Parent;
            if (itemParent != null)
                throw new ArgumentException("The item belongs to other tree", "item");

            Check.OperationValid(itemParent != this, "The tree already contains this node");
        }
    }
    
}
