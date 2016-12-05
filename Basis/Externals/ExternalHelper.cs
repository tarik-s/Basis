using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Externals
{
    public static class External
    {
        public static External<T> Create<T>(T value, Uri path, ExternalSettings settings)
        {
            return new External<T>(value, path, settings);
        }
    }

    internal static class ExternalHelper
    {
        public static Type GetMemberType(MemberInfo memberInfo)
        {
            var pi = memberInfo as PropertyInfo;
            if (pi != null)
                return pi.PropertyType;

            var fi = memberInfo as FieldInfo;
            if (fi != null)
                return fi.FieldType;

            throw new ArgumentException("Invalid paramerter", "memberInfo");
        }
    }
}
