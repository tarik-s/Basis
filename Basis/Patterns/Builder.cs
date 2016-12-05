using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Patterns
{
    /// <summary>
    /// Builder pattern interface
    /// </summary>
    /// <typeparam name="T">Built type</typeparam>
    public interface IBuilder<out T>
        where T : class
    {
        /// <summary>
        /// Builds a new instance of T
        /// </summary>
        /// <returns></returns>
        T Build();
    }
}
