using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Externals
{
    /// <summary>
    /// External memer attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public abstract class ExternalMemberAttribute : Attribute
    {
        private readonly string _name;

        protected ExternalMemberAttribute()
        {
            _name = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalMemberAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected ExternalMemberAttribute(string name)
        {
            Check.ArgumentNotEmpty(name, "name"); 

            _name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        protected internal ExternalMemberInfo CreateInfo(MemberInfo memberInfo)
        {
            Check.ArgumentNotNull(memberInfo, "memberInfo");

            return CreateInfoImpl(memberInfo);
        }

        protected abstract ExternalMemberInfo CreateInfoImpl(MemberInfo memberInfo);

    }
}
