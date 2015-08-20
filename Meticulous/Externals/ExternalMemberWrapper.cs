using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Externals
{
    internal sealed class ExternalMemberWrapper : IExternalizable
    {
        private readonly object _defaultValue;
        private readonly MemberWrapper _wrapperCore;

        private ExternalMemberWrapper(MemberWrapper wrapperCore)
        {
            _wrapperCore = wrapperCore;
            _defaultValue = _wrapperCore.GetValue();
        }

        public static ExternalMemberWrapper Create(MemberInfo memberInfo, object owner)
        {
            var wrapperCore = MemberWrapper.Create(memberInfo, owner);
            return new ExternalMemberWrapper(wrapperCore);
        }

        public object DefaultValue 
        {
            get { return _defaultValue; }
        }

        public object Value
        {
            get { return _wrapperCore.GetValue(); }
        }

        private abstract class MemberWrapper
        {
            public static MemberWrapper Create(MemberInfo memberInfo, object owner)
            {
                Check.ArgumentNotNull(memberInfo, "memberInfo");

                var fieldInfo = memberInfo as FieldInfo;
                if (fieldInfo != null)
                    return new FieldWrapper(fieldInfo, owner);

                var propertyInfo = memberInfo as PropertyInfo;
                if (propertyInfo != null)
                    return new PropertyWrapper(propertyInfo, owner);

                throw new ArgumentException("Invalid mememer info", "memberInfo");
            }

            public abstract object GetValue();

            public abstract void SetValue(object value);
        }

        private sealed class FieldWrapper : MemberWrapper
        {
            private readonly FieldInfo _fieldInfo;
            private readonly object _owner;

            public FieldWrapper(FieldInfo fieldInfo, object owner)
            {
                _fieldInfo = fieldInfo;
                _owner = owner;
            }

            public override object GetValue()
            {
                return _fieldInfo.GetValue(_owner);
            }

            public override void SetValue(object value)
            {
                _fieldInfo.SetValue(_owner, value);
            }
        }

        private sealed class PropertyWrapper : MemberWrapper
        {
            private readonly PropertyInfo _propertyInfo;
            private readonly object _owner;

            public PropertyWrapper(PropertyInfo propertyInfo, object owner)
            {
                _propertyInfo = propertyInfo;
                _owner = owner;
            }

            public override object GetValue()
            {
                return _propertyInfo.GetValue(_owner);
            }

            public override void SetValue(object value)
            {
                _propertyInfo.SetValue(_owner, value);
            }
        }
    }
}
