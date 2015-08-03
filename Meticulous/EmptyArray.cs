using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous
{
    // Copied from System.EmptyArray beause the last one has internal protection level
    public static class EmptyArray<T>
    {
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        private static readonly T[] _value = { };

        public static T[] Value
        {
            get
            {
                return _value;
            }
        }
    }
}
