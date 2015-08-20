using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Externals
{
    /// <summary>
    /// 
    /// </summary>
    public interface IExternalizable
    {
        /// <summary>
        /// Gets the default value.
        /// </summary>
        object DefaultValue { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        object Value { get; }

    }
}
