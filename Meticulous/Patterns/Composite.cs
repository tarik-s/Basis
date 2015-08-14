using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Patterns
{
    /// <summary>
    /// Represents the (readonly) Composite pattern
    /// </summary>
    /// <typeparam name="T">Type of the compositing objects</typeparam>
    public interface IComposite<T>
    {
        /// <summary>
        /// Gets the first level of components
        /// </summary>
        IReadOnlyList<T> Children { get; }

        /// <summary>
        /// Checks the object contains the specified component
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        bool Contains(T component);
    }

    /// <summary>
    /// Represents the (mutable) Composite pattern.
    /// </summary>
    /// <typeparam name="T">Type of the compositing objects</typeparam>
    public interface IMutableComposite<T> : IComposite<T>
    {
        /// <summary>
        /// Adds the component.
        /// </summary>
        /// <param name="componenet">The component to be added.</param>
        void Add(T componenet);

        /// <summary>
        /// Removes the component.
        /// </summary>
        /// <param name="component">The component to be removed.</param>
        /// <returns></returns>
        bool Remove(T component);
    }

    /// <summary>
    /// Composite extension methods
    /// </summary>
    public static class CompositeExtensionMethods
    {
        /// <summary>
        /// Returns a copy of the descedant components
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static T[] GetDescedants<T>(this IComposite<T> @this)
        {
            Check.This(@this);

            return @this.Children.ToArray();
        }
    }

    /// <summary>
    /// Base class for (readonly) composites
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Composite<T> : IComposite<T>
    {
        private IReadOnlyList<T> _children;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="children"></param>
        protected Composite(IEnumerable<T> children)
        {
            Check.ArgumentNotNull(children, "children");

            _children = children.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<T> Children
        {
            get { return _children; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool Contains(T component)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Base class for (mutable) composites
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MutableComposite<T> : IMutableComposite<T>
    {
        private List<T> _children;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="children"></param>
        protected MutableComposite(IEnumerable<T> children)
        {
            Check.ArgumentNotNull(children, "children");

            _children = new List<T>(children);
        }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<T> Children
        {
            get { return _children; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool Contains(T component)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="componenet"></param>
        public void Add(T componenet)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool Remove(T component)
        {
            throw new NotImplementedException();
        }
    }
}
