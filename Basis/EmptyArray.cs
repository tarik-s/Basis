using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous
{
    /// <summary>
    /// Represents an empty array
    /// </summary>
    /// <typeparam name="T">Type of array element</typeparam>
    public static class EmptyArray<T>
    {
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        private static readonly T[] _value = { };

        /// <summary>
        /// Gets the instance of an empty array
        /// </summary>
        public static T[] Value
        {
            get { return _value; }
        }
    }
}
