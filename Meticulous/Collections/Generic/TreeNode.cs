using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Collections.Generic
{
    /// <summary>
    /// Readonly node interface 
    /// </summary>
    public interface IReadOnlyTreeNode<out T> : IReadOnlyList<IReadOnlyTreeNode<T>>
    {
        /// <summary>
        /// Gets the node attributes
        /// </summary>
        T Data { get; }
    }

    /// <summary>
    /// TreeNode
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNode<T> : IList<TreeNode<T>>
    {
        private static readonly List<TreeNode<T>> s_emptyNodes = new List<TreeNode<T>>();

        private T _data;
        private List<TreeNode<T>> _nodes;


        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode{T}"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public TreeNode(T data)
        {
            _data = data;
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
        /// Adds the child.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public TreeNode<T> AddChild(T data)
        {
            var child = new TreeNode<T>(data);
            Nodes.Add(child);
            return child;
        }

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
            Nodes.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            if (_nodes != null)
                _nodes.Clear();
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
            if (_nodes == null)
                return false;

            return _nodes.Remove(item);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count 
        {
            get
            {
                if (_nodes == null)
                    return 0;

                return _nodes.Count;
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
        /// <exception cref="System.NotImplementedException"></exception>
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
        /// <exception cref="System.NotImplementedException"></exception>
        public void Insert(int index, TreeNode<T> item)
        {
            Nodes.Insert(index, item);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void RemoveAt(int index)
        {
            Nodes.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public TreeNode<T> this[int index]
        {
            get { return Nodes[index]; }
            set { Nodes[index] = value; }
        }


        private List<TreeNode<T>> Nodes
        {
            get
            {
                if (_nodes == null)
                    _nodes = new List<TreeNode<T>>();

                return _nodes;
            }
        }
    }
    
}
