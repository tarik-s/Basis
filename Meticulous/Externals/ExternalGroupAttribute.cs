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
    [AttributeUsage(AttributeTargets.Class)]
    public class ExternalGroupAttribute : Attribute
    {
        private readonly string _name;

        /// <summary>
        /// 
        /// </summary>
        public ExternalGroupAttribute()
        {
            _name = String.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public ExternalGroupAttribute(string name)
        {
            Check.ArgumentNotEmpty(name, "name");

            _name = name;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        protected internal virtual ExternalGroupInfo CreateInfo(Type classType)
        {
            Check.ArgumentNotNull(classType, "classType");
            

            return new ExternalGroupInfo(_name);
        }
    }
}
