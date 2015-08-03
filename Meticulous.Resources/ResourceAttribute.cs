using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Resources
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Enum)]
    public sealed class ResourceAttribute : Attribute
    {
        private readonly string _name;
        private bool? _localizable;

        public ResourceAttribute()
        {
            _name = String.Empty;
        }

        public ResourceAttribute(string name)
        {
            ResourceHelper.CheckResourceNameValid(name);

            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsLocalizable
        {
            get
            {
                if (_localizable.HasValue)
                    return _localizable.Value;

                return false;
            }
            set
            {
                _localizable = value;
            }
        }

        internal bool? Localizable
        {
            get { return _localizable; }
        }
    }
}
