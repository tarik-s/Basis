using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Resources
{
    public sealed class Resource<T>
    {
        private readonly T _defaultValue;
        private TryFunc<CultureInfo, T> _provider;

        public Resource()
            : this(default(T))
        {
        }

        public Resource(T defaultValue)
        {
            _defaultValue = defaultValue;
        }

        public T Value
        {
            get { return Get(CultureInfo.CurrentCulture); }
        }

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

        public T DefaultValue
        {
            get { return _defaultValue; }
        }
    }
}
