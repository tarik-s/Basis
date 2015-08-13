using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Resources
{
    /// <summary>
    /// Resource attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Enum)]
    public sealed class ResourceAttribute : Attribute
    {
        private readonly string _name;
        private bool? _localizable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAttribute"/> class.
        /// </summary>
        public ResourceAttribute()
        {
            _name = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ResourceAttribute(string name)
        {
            ResourceHelper.CheckResourceNameValid(name);

            _name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is localizable.
        /// </summary>
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
