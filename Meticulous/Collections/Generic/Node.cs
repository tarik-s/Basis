using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Collections.Generic
{
    /// <summary>
    /// Readonly node interface 
    /// </summary>
    public interface IReadOnlyNode<T> : IEnumerable<IReadOnlyNode<T>>
    {
        /// <summary>
        /// Gets the node attributes
        /// </summary>
        IReadOnlyCollection<T> Attributes { get; }
    }

    /// <summary>
    /// Node interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INode<T> : IReadOnlyNode<T>
    {
        /// <summary>
        /// Adds a child node
        /// </summary>
        /// <param name="child"></param>
        void Add(T child);

        /// <summary>
        /// Removed the child node
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        bool Remove(T child);
    }


    /// <summary>
    /// Node
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Node<T>
    {

    }
    
}
