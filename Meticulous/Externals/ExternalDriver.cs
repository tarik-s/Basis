using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Externals
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ExternalDriver
    {
        public bool Supports(ExternalGroupAttribute groupAttribute)
        {
            Check.ArgumentNotNull(groupAttribute, "groupAttribute");

            return SupportsImpl(groupAttribute);
        }

        public bool Supports(ExternalMemberAttribute memberAttribute)
        {
            Check.ArgumentNotNull(memberAttribute, "memberAttribute");

            return SupportsImpl(memberAttribute);
        }

        public ExternalGroupInfoBuilder CreateGroupInfoBuilder(Type classType, ExternalGroupAttribute groupAttribute)
        {
            Check.ArgumentNotNull(classType, "classType");
            Check.ArgumentNotNull(groupAttribute, "groupAttribute");

            return CreateGroupInfoBuilderImpl(classType, groupAttribute);
        }

        public ExternalMemberInfo CreateMemberInfo(MemberInfo memberInfo, ExternalMemberAttribute memberAttribute)
        {
            Check.ArgumentNotNull(memberInfo, "memberInfo");
            Check.ArgumentNotNull(memberAttribute, "memberAttribute");

            return CreateMemberInfoImpl(memberInfo, memberAttribute);
        }

        protected abstract bool SupportsImpl(ExternalMemberAttribute memberAttribute);

        protected virtual ExternalMemberInfo CreateMemberInfoImpl(MemberInfo memberInfo, ExternalMemberAttribute memberAttribute)
        {
            return memberAttribute.CreateInfo(memberInfo);
        }

        protected virtual ExternalGroupInfoBuilder CreateGroupInfoBuilderImpl(Type classType, ExternalGroupAttribute groupAttribute)
        {
            return groupAttribute.CreateGroupInfoBuilder(classType);
        }

        protected virtual bool SupportsImpl(ExternalGroupAttribute groupAttribute)
        {
            if (groupAttribute.GetType().IsSubclassOf(typeof(ExternalGroupAttribute)))
                return false;

            return true;
        }
    }
}
