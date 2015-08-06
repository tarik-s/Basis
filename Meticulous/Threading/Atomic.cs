using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meticulous.Threading
{
    #region IAtomic interface

    public interface IAtomic<T>
    {
        T Value { get; set; }

        bool TrySet(T value);

        T Exchange(T value);
    }

    #endregion

    public static class Atomic
    {
        public static IAtomic<T> Create<T>(T value)
        {
            var type = typeof (T);
            if (type == typeof (bool))
                return (IAtomic<T>)CreateImpl(typeof(AtomicBoolean), value);

            if (type == typeof (int))
                return (IAtomic<T>) CreateImpl(typeof (AtomicInteger), value);

            return (IAtomic<T>) CreateImpl(typeof (Atomic<T>), value);
        }

        private static object CreateImpl(Type type, object value)
        {
            Type[] args = { value.GetType() };
            var ctor = type.GetConstructor(args);
            if (ctor == null)
                throw new MissingMethodException(type.Name, ".ctor");

            return ctor.Invoke(new []{value});
        }
    }
    
    #region Atomic<T>

    public sealed class Atomic<T> : IAtomic<T>
    {
        #region Fields

        private static readonly IEqualityComparer<T> s_comparer;

        private readonly ReaderWriterLockSlim _lock;
        private readonly IEqualityComparer<T> _comparer;
        private T _value;

        #endregion

        #region Construction

        static Atomic()
        {
            s_comparer = EqualityComparer<T>.Default;
        }

        public Atomic()
            : this(default(T))
        {
        }

        public Atomic(T value)
            : this(value, s_comparer)
        {
            
        }

        public Atomic(T value, IEqualityComparer<T> comparer)
        {
            Check.ArgumentNotNull(comparer, "comparer");

            _value = value;
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _comparer = comparer;
        }

        public static explicit operator T(Atomic<T> value)
        {
            if (value == null)
                return default(T);

            return value.Value;
        }

        public static implicit operator Atomic<T>(T value)
        {
            return new Atomic<T>(value);
        }

        #endregion

        #region IAtomic

        public T Value
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _value;
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    _value = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        public bool TrySet(T value)
        {
            _lock.EnterWriteLock();
            try
            {
                if (_comparer.Equals(value, _value))
                    return false;

                _value = value;
                return true;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public T Exchange(T value)
        {
            _lock.EnterWriteLock();
            try
            {
                var oldValue = _value;
                _value = value;
                return oldValue;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        #endregion
    }

    #endregion
    

    #region AtomicBoolean

    public struct AtomicBooleanValue : IAtomic<bool>
    {
        private int _value;

        public AtomicBooleanValue(bool initialValue)
        {
            _value = AtomicHelper.MakeInt(initialValue);
        }

        public static explicit operator bool(AtomicBooleanValue value)
        {
            return value.Value;
        }

        public static implicit operator AtomicBooleanValue(bool value)
        {
            return new AtomicBooleanValue(value);
        }

        public bool Value
        {
            get { return AtomicHelper.ReadIntAsBool(ref _value); }
            set { Exchange(value); }
        }

        public bool Exchange(bool value)
        {
            return AtomicHelper.SetIntAsBool(ref _value, value);
        }

        public bool TrySet(bool value)
        {
            var result = Exchange(value);
            return result != value;
        }
    }

    public sealed class AtomicBoolean : IAtomic<bool>
    {
        private AtomicBooleanValue _value;

        #region Construction

        public AtomicBoolean()
        {
        }

        public AtomicBoolean(bool initialValue)
        {
            _value.Exchange(initialValue);
        }

        public static explicit operator bool(AtomicBoolean value)
        {
            if (value == null)
                return false;

            return value.Value;
        }

        public static implicit operator AtomicBoolean(bool value)
        {
            return new AtomicBoolean(value);
        }

        #endregion

        #region IAtomic

        public bool Value
        {
            get { return _value.Value; }
            set { _value.Exchange(value); }
        }

        public bool Exchange(bool value)
        {
            return _value.Exchange(value);
        }

        public bool TrySet(bool value)
        {
            return _value.TrySet(value);
        }

        #endregion
    }

    #endregion


    #region AtomicInteger

    public struct AtomicIntegerValue : IAtomic<int>
    {
        private int _value;

        #region Construction

        public AtomicIntegerValue(int initialValue)
        {
            _value = initialValue;
        }

        public static explicit operator int(AtomicIntegerValue value)
        {
            return value.Value;
        }

        public static implicit operator AtomicIntegerValue(int value)
        {
            return new AtomicIntegerValue(value);
        }

        #endregion

        #region IAtomic

        public int Value
        {
            get { return Interlocked.CompareExchange(ref _value, 0, 0); }
            set { Exchange(value); }
        }

        public int Exchange(int value)
        {
            return Interlocked.Exchange(ref _value, value);
        }

        public bool TrySet(int value)
        {
            var result = Exchange(value);
            return result != value;
        }

        #endregion
    }

    public sealed class AtomicInteger : IAtomic<int>
    {
        private AtomicIntegerValue _value;

        #region Construction

        public AtomicInteger()
        {
        }

        public AtomicInteger(int initialValue)
        {
            _value.Exchange(initialValue);
        }


        public static explicit operator int(AtomicInteger value)
        {
            if (value == null)
                return 0;

            return value.Value;
        }

        public static implicit operator AtomicInteger(int value)
        {
            return new AtomicInteger(value);
        }

        #endregion

        #region IAtomic

        public int Value
        {
            get { return _value.Value; }
            set { _value.Exchange(value); }
        }

        public int Exchange(int value)
        {
            return _value.Exchange(value);
        }

        public bool TrySet(int value)
        {
            return _value.TrySet(value);
        }

        #endregion
    }

    #endregion


    #region AtomicReference

    public struct AtomicReferenceValue<T> : IAtomic<T>
        where T : class 
    {
        private T _value;

        #region Construction

        public AtomicReferenceValue(T initialValue)
        {
            _value = initialValue;
        }

        public static explicit operator T(AtomicReferenceValue<T> value)
        {
            return value.Value;
        }

        public static implicit operator AtomicReferenceValue<T>(T value)
        {
            return new AtomicReferenceValue<T>(value);
        }

        #endregion

        #region IAtomic

        public T Value
        {
            get { return Interlocked.CompareExchange(ref _value, null, null); }
            set { Exchange(value); }
        }

        public T Exchange(T value)
        {
            return Interlocked.Exchange(ref _value, value);
        }

        public bool TrySet(T value)
        {
            var result = Exchange(value);
            return !ReferenceEquals(result, value);
        }

        #endregion
    }

    public sealed class AtomicReference<T> : IAtomic<T>
        where T : class
    {
        private AtomicReferenceValue<T> _value;

        #region Construction

        public AtomicReference()
        {
        }

        public AtomicReference(T initialValue)
        {
            _value.Exchange(initialValue);
        }


        public static explicit operator T(AtomicReference<T> value)
        {
            if (value == null)
                return default(T);

            return value.Value;
        }

        public static implicit operator AtomicReference<T>(T value)
        {
            return new AtomicReference<T>(value);
        }

        #endregion

        #region IAtomic

        public T Value
        {
            get { return _value.Value; }
            set { _value.Exchange(value); }
        }

        public T Exchange(T value)
        {
            return _value.Exchange(value);
        }

        public bool TrySet(T value)
        {
            return _value.TrySet(value);
        }

        #endregion
    }

    #endregion



    internal static class AtomicHelper
    {
        public static int True = 1;
        public static int False = 0;

        public static int MakeInt(bool value)
        {
            if (value)
                return True;

            return False;
        }

        public static bool MakeBool(int value)
        {
            CheckIntBool(value, "value");

            if (value == True)
                return true;

            return false;
        }

        public static bool ReadIntAsBool(ref int value)
        {
            var result = ReadInt(ref value);
            return MakeBool(result);
        }

        public static bool SetIntAsBool(ref int value, bool newValue)
        {
            var intValue = MakeInt(newValue);
            var result = Interlocked.Exchange(ref value, intValue);
            return MakeBool(result);
        }

        public static int ReadInt(ref int value)
        {
            return Interlocked.CompareExchange(ref value, 0, 0);
        }

        [Conditional("DEBUG")]
        private static void CheckIntBool(int value, string paramName)
        {
            if (value != True && value != False)
                throw new ArgumentException("Argument MUST be 0 or 1", paramName);
        }


    }
}
