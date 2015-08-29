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

        public ExternalGroupInfo(string name)
        {
            Check.ArgumentNotEmpty(name, "name", "Group name cannot be empty");

            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }

}
