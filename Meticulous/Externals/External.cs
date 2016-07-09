using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticulous.Threading;

namespace Meticulous.Externals
{
    /// <summary>
    /// Represents External class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class External<T> : IExternal
    {
        private readonly T _defaultValue;

        private AtomicReferenceValue<TryFunc<T>> _provider;
        private Atomic<T> _value;

        private Uri _path;
        private ExternalSettings _settings;

        internal External()
            : this(default(T))
        {
        }

        internal External(T defaultValue)
        {
            _defaultValue = defaultValue;
            _value = defaultValue;
            _provider = new AtomicReferenceValue<TryFunc<T>>(null);
        }

        public External(T defaultValue, Uri path, ExternalSettings settings)
            : this(defaultValue)
        {
            _path = path;
            _settings = settings;

            ExternalManager.Instance.AttachDynamicValue(this);
        }

        public Uri Path
        {
            get { return _path; }
        }

        public ExternalSettings Settings
        {
            get { return _settings; }
        }

        public T DefaultValue
        {
            get { return _defaultValue; }
        }

        public T Value
        {
            get
            {
                T value;
                if (TryGetValue(out value))
                    return value;

                return _defaultValue;
            }
            internal set
            {
                _provider.Exchange(null);
                _value.Exchange(value);
            }
        }

        public bool TryGetValue(out T value)
        {
            var provider = _provider.Value;
            if (provider != null)
            {
                if (provider(out value))
                    return true;

                return false;
            }
            value = _value.Value;
            return true;
        }


        public override string ToString()
        {
            var value = Value;
            if (value == null)
                return String.Empty;

            return value.ToString();
        }


        #region Casts

        public static explicit operator T (External<T> value)
        {
            if (value == null)
                return default(T);

            return value.Value;
        }

        public static implicit operator External<T>(T value)
        {
            return new External<T>(value);
        }

        #endregion

        #region IExternal

        object IExternal.DefaultValue
        {
            get { return DefaultValue; }
        }

        object IExternal.Value
        {
            get { return Value; }
            set
            {
                if (value == null)
                    Value = default(T);
                else
                    Value = (T)value;
            }
        }

        Type IExternal.UnderlyingType
        {
            get { return typeof(T); }
        }

        bool IExternal.Setup(Uri path, ExternalSettings settings)
        {
            _path = path;
            _settings = settings;
            return true;
        }

        #endregion
    }

}
