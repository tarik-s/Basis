using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Externals
{
    public class ExternalMemberInfo
    {
        private readonly string _name;
        public ExternalMemberInfo(string name)
        {
            Check.ArgumentNotEmpty(name, "name", "Member name cannot be empty");

            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

    }
}
