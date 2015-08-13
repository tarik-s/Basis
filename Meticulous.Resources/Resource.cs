using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Resources
{
    /// <summary>
    /// Resource class
    /// </summary>
    /// <typeparam name="T">Type of resource</typeparam>
    public sealed class Resource<T>
    {
        private readonly T _defaultValue;
        private TryFunc<CultureInfo, T> _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource{T}"/> class.
        /// </summary>
        public Resource()
            : this(default(T))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource{T}"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        public Resource(T defaultValue)
        {
            _defaultValue = defaultValue;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public T Value
        {
            get { return Get(CultureInfo.CurrentCulture); }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns></returns>
        public T Get(CultureInfo cultureInfo)
        {
            if (_provider != null)
            {
                T result;
                if (_provider(cultureInfo, out result))
                    return result;
            }
            return _defaultValue;
        }

        /// <summary>
        /// Gets the default value.
        /// </summary>
        public T DefaultValue
        {
            get { return _defaultValue; }
        }

        internal void SetProvider(TryFunc<CultureInfo, T> provider)
        {
            _provider = provider;
        }
    }
}
