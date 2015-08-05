using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meticulous.Threading
{
    public interface IAtomicValue<T>
    {
        T Value { get; set; }

        bool TrySet(T value);

        T Exchange(T value);
    }

    public sealed class Atomic<T> : IAtomicValue<T>
    {
        private readonly ReaderWriterLockSlim _lock;
        private T _value;

        public Atomic(T value)
        {
            _value = value;
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }

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
    }

    #region AtomicBoolean

    public struct AtomicBooleanValue : IAtomicValue<bool>
    {
        private int _value;

        public AtomicBooleanValue(bool initialValue)
        {
            _value = AtomicHelper.MakeInt(initialValue);
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

    public sealed class AtomicBoolean : IAtomicValue<bool>
    {
        private AtomicBooleanValue _value;

        public AtomicBoolean()
        {
        }

        public AtomicBoolean(bool initialValue)
        {
            _value.Exchange(initialValue);
        }

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
