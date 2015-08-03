using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Resources
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly)]
    public sealed class ResourceGroupAttribute : Attribute
    {
        private readonly string _name;

        public ResourceGroupAttribute()
        {
            _name = String.Empty;
        }

        public ResourceGroupAttribute(string name)
        {
            ResourceHelper.CheckResourceGroupNameValid(name);

            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
