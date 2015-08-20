using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Externals
{
    /// <summary>
    /// Represents base External class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class External<T> : IExternalizable
    {
        private readonly T _defaultValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="External{T}"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        protected External(T defaultValue)
        {
            _defaultValue = defaultValue;
        }

        /// <summary>
        /// Gets the default value.
        /// </summary>
        public T DefaultValue
        {
            get { return _defaultValue; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public virtual T Value
        {
            get { return _defaultValue; }
        }

        #region IExternalizable

        object IExternalizable.DefaultValue
        {
            get { return DefaultValue; }
        }

        object IExternalizable.Value
        {
            get { return Value; }
        }

        #endregion
    }
}
