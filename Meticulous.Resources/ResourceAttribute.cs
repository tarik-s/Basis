using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticulous.Externals;

namespace Meticulous.Resources
{
    /// <summary>
    /// Resource attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Enum)]
    public sealed class ResourceAttribute : ExternalAttribute
    {
        private bool? _localizable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAttribute"/> class.
        /// </summary>
        public ResourceAttribute()
            : base(String.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ResourceAttribute(string name)
            : base(name)
        {
            ResourceHelper.CheckResourceNameValid(name);
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
