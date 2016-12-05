using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Externals
{
    internal sealed class ExternalMemberWrapper : IExternalCore
    {
        private readonly object _defaultValue;
        private readonly MemberWrapper _wrapper;

        private Uri _path;
        private ExternalSettings _settings;

        public ExternalMemberWrapper(MemberWrapper wrapper)
        {
            _wrapper = wrapper;
            _defaultValue = _wrapper.GetValue();
            _settings = ExternalSettings.Empty;
        }

        public static ExternalMemberWrapper Create(MemberInfo memberInfo)
        {
            var wrapperCore = MemberWrapper.Create(memberInfo, null);
            return new ExternalMemberWrapper(wrapperCore);
        }

        public object DefaultValue 
        {
            get { return _defaultValue; }
        }

        public object Value
        {
            get { return _wrapper.GetValue(); }
            set { _wrapper.SetValue(value); }
        }

        public Type UnderlyingType
        {
            get { return _wrapper.Type; }
        }

        public Uri Path
        {
            get { return _path; }
        }

        public ExternalSettings Settings
        {
            get { return _settings; }
        }

        public bool Setup(Uri path, ExternalSettings settings)
        {
            _path = path;
            _settings = settings;
            return true;
        }
    }

    public abstract class MemberWrapper
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

        public abstract Type Type { get; }

        public abstract bool IsStatic { get; }

        #region Implementations

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

            public override Type Type
            {
                get { return _fieldInfo.FieldType; }
            }

            public override bool IsStatic
            {
                get { return _fieldInfo.IsStatic; }
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

            public override Type Type
            {
                get { return _propertyInfo.PropertyType; }
            }

            public override bool IsStatic
            {
                get
                {
                    var getMethod = _propertyInfo.GetMethod;
                    if (getMethod != null)
                        return getMethod.IsStatic;

                    var setMethod = _propertyInfo.SetMethod;
                    Check.OperationValid(setMethod != null, "Property doesn't have getter or setter");
                    return setMethod.IsStatic;
                }
            }
        }

        #endregion
    }


}
