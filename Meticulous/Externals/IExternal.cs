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
    public interface IExternal
    {
        object DefaultValue { get; }

        object Value { get; set; }

        Type UnderlyingType { get; }

        Uri Path { get; }

        ExternalSettings Settings { get; }

        bool Setup(Uri path, ExternalSettings settings);
    }
}
