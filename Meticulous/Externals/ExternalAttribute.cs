using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Externals
{
    /// <summary>
    /// External attribute
    /// </summary>
    public abstract class ExternalAttribute
    {
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected ExternalAttribute(string name)
        {
            Check.ArgumentNotNull(name, "name");

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
