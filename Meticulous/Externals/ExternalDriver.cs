using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Externals
{
    public abstract class ExternalDriver
    {
        public bool Supports(ExternalGroupAttribute groupAttribute)
        {
            return SupportsImpl(groupAttribute);
        }

        public bool Supports(Type type, Uri uri)
        {
            return SupportsImpl(type, uri);
        }

        internal void AddStaticValue(IExternal value)
        {
            HandleNewValue(value);
        }

        internal void AddDynamicValue(IExternal value)
        {
            HandleNewValue(value);
        }

        public ExternalSettings CreateSettings(string rawSettings)
        {
            return CreateSettingsImpl(rawSettings);
        }

        protected abstract void HandleNewValue(IExternal value);

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
