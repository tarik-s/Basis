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
    public class ExternalGroupInfo
    {
        private readonly string _name;
        private readonly ExternalGroupInfo[] _childGroups;

        public ExternalGroupInfo(ExternalGroupInfoBuilder builder)
        {
            Check.ArgumentNotNull(builder, "builder");

            _name = builder.Name;
            _childGroups = builder.GetChildBuilders().Select(b => b.Build()).ToArray();
        }

        public string Name
        {
            get { return _name; }
        }

        public IReadOnlyList<ExternalGroupInfo> ChildGroups
        {
            get { return _childGroups; }
        }
    }

}
