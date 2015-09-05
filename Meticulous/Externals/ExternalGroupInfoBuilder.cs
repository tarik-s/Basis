using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticulous.Patterns;

namespace Meticulous.Externals
{
    public class ExternalGroupInfoBuilder : IBuilder<ExternalGroupInfo>
    {
        private readonly string _name;
        private readonly List<ExternalGroupInfoBuilder> _childBuilders;

        public ExternalGroupInfoBuilder(string name)
        {
            Check.ArgumentNotEmpty(name, "name");

            _name = name;
            _childBuilders = new List<ExternalGroupInfoBuilder>();
        }

        public string Name
        {
            get { return _name; }
        }

        public ExternalGroupInfoBuilder[] GetChildBuilders()
        {
            return _childBuilders.ToArray();
        }

        public virtual ExternalGroupInfo Build()
        {
            return new ExternalGroupInfo(this);
        }
    }
}
