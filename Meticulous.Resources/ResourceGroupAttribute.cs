using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Resources
{
    /// <summary>
    /// Resource group attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly)]
    public sealed class ResourceGroupAttribute : Attribute
    {
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceGroupAttribute"/> class.
        /// </summary>
        public ResourceGroupAttribute()
        {
            _name = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceGroupAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ResourceGroupAttribute(string name)
        {
            ResourceHelper.CheckResourceGroupNameValid(name);

            _name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
    }
}
