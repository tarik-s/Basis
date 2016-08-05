using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Externals
{
    public abstract class ExternalDriver
    {
        private readonly ConcurrentDictionary<string, IExternal> _staticVariables;
        private readonly ConcurrentDictionary<string, WeakReference<IExternal>> _dynamicVariables;

        protected ExternalDriver()
        {
            _staticVariables = new ConcurrentDictionary<string, IExternal>();
            _dynamicVariables = new ConcurrentDictionary<string, WeakReference<IExternal>>();
        }

        public bool Supports(ExternalGroupAttribute groupAttribute)
        {
            return SupportsImpl(groupAttribute);
        }

        public bool Supports(Type type, Uri uri)
        {
            return SupportsImpl(type, uri);
        }

        internal void ReadStaticValue(IExternal value)
        {
            ReadValue(value);
        }

        internal void ReadDynamicValue(IExternal value)
        {
            ReadValue(value);
        }

        public ExternalSettings CreateSettings(string rawSettings)
        {
            return CreateSettingsImpl(rawSettings);
        }

        protected abstract void ReadValue(IExternal value);

        protected virtual ExternalSettings CreateSettingsImpl(string rawSettings)
        {
            return new ExternalSettings(rawSettings);
        }

        protected abstract bool SupportsImpl(Type type, Uri uri);

        protected virtual bool SupportsImpl(ExternalGroupAttribute groupAttribute)
        {
            var attrType = groupAttribute.GetType();
            if (attrType.IsBaseOrTypeOf(typeof (ExternalGroupAttribute)))
                return true;

            return false;
        }
    }
}
