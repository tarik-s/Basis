﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meticulous.Threading
{
    #region IAtomic interface

    /// <summary>
    /// Atomic value interface
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public interface IAtomic<T>
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        T Value { get; set; }

        /// <summary>
        /// Tries to set the new value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>True if the value has been set and the original value was different of value</returns>
        bool TrySet(T value);

        /// <summary>
        /// Exchanges the current value with the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Returns the original value</returns>
        T Exchange(T value);
    }

    #endregion

    /// <summary>
    /// Atomic static factory
    /// </summary>
    public static class Atomic
    {
        /// <summary>
        /// Creates the new instance of atomic value.
        /// </summary>
        /// <typeparam name="T">The type of value</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>Returns the new instance of atomic value</returns>
        public static IAtomic<T> Create<T>(T value)
        {
            var type = typeof(T);
            if (type == typeof(bool))
                return (IAtomic<T>)CreateImpl(typeof(AtomicBoolean), value);

            if (type == typeof(int))
                return (IAtomic<T>)CreateImpl(typeof(AtomicInteger), value);

            return (IAtomic<T>)CreateImpl(typeof(Atomic<T>), value);
        }

        private static object CreateImpl(Type type, object value)
        {
            Type[] args = { value.GetType() };
            var ctor = type.GetConstructor(args);
            if (ctor == null)
                throw new MissingMethodException(type.Name, ".ctor");

            return ctor.Invoke(new[] { value });
        }
    }

    #region Atomic<T>

    /// <summary>
    /// Generic atomic class for any type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Atomic<T> : IAtomic<T>
    {
        #region Fields

        private static readonly Func<T, T, bool> s_comparer;

        private SpinLock _lock;
        private readonly Func<T, T, bool> _equalizer;
        private T _value;

        #endregion

        #region Construction

        static Atomic()
        {
            var comparer = EqualityComparer<T>.Default;
            s_comparer = (a, b) => comparer.Equals(a, b);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Atomic{T}"/> class.
        /// </summary>
        public Atomic()
            : this(default(T))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Atomic{T}"/> class.
        /// </summary>
        /// <param name="value">The initial value.</param>
        public Atomic(T value)
            : this(value, s_comparer)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Atomic{T}"/> class.
        /// </summary>
        /// <param name="value">The initial value.</param>
        /// <param name="equalizer">The equalizer.</param>
        public Atomic(T value, Func<T, T, bool> equalizer)
        {
            Check.ArgumentNotNull(equalizer, "comparer");

            _lock = new SpinLock();
            _value = value;
            _equalizer = equalizer;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Atomic{T}"/> to T/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator T(Atomic<T> value)
        {
            if (value == null)
                return default(T);

            return value.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from T to <see cref="Atomic{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Atomic<T>(T value)
        {
            return new Atomic<T>(value);
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (_value == null)
                return String.Empty;

            return _value.ToString();
        }

        #region IAtomic

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public T Value
        {
            get
            {
                bool lockTaken = false;
                try
                {
                    _lock.Enter(ref lockTaken);
                    return _value;
                }
                finally
                {
                    if (lockTaken)
                        _lock.Exit();
                }
            }
            set
            {
                bool lockTaken = false;
                try
                {
                    _lock.Enter(ref lockTaken);
                    _value = value;
                }
                finally
                {
                    if (lockTaken)
                        _lock.Exit();
                }
            }
        }

        /// <summary>
        /// Tries to set the new value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// True if the value has been set and the original value was different of value
        /// </returns>
        public bool TrySet(T value)
        {
            bool lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);

                if (_equalizer(value, _value))
                    return false;

                _value = value;
                return true;
            }
            finally
            {
                if (lockTaken)
                    _lock.Exit();
            }
        }

        /// <summary>
        /// Exchanges the current value with the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Returns the original value
        /// </returns>
        public T Exchange(T value)
        {
            bool lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);
                var oldValue = _value;
                _value = value;
                return oldValue;
            }
            finally
            {
                if (lockTaken)
                    _lock.Exit();
            }
        }

        #endregion
    }

    #endregion


    #region AtomicBoolean

    /// <summary>
    /// Atomic boolean struct
    /// </summary>
    public struct AtomicBooleanValue : IAtomic<bool>, IEquatable<AtomicBooleanValue>
    {
        private int _value;

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicBooleanValue"/> struct.
        /// </summary>
        /// <param name="initialValue">Initial value</param>
        public AtomicBooleanValue(bool initialValue)
        {
            _value = AtomicHelper.MakeInt(initialValue);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="AtomicBooleanValue"/> to <see cref="System.Boolean"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator bool(AtomicBooleanValue value)
        {
            return value.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Boolean"/> to <see cref="AtomicBooleanValue"/>.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator AtomicBooleanValue(bool value)
        {
            return new AtomicBooleanValue(value);
        }

        #endregion

        #region IEquatable implementation

        /// <summary>
        /// Determines whether the specified <see cref="AtomicBooleanValue"/> is equal to the current <see cref="AtomicBooleanValue"/>.
        /// </summary>
        /// <param name="other">The <see cref="AtomicBooleanValue"/> to compare with the current <see cref="AtomicBooleanValue"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="AtomicBooleanValue"/> is equal to the current
        /// <see cref="AtomicBooleanValue"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(AtomicBooleanValue other)
        {
            return this.Value == other.Value;
        }

        #endregion

        /// <param name="lhs">Lhs.</param>
        /// <param name="rhs">Rhs.</param>
        public static bool operator ==(AtomicBooleanValue lhs, AtomicBooleanValue rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <param name="lhs">Lhs.</param>
        /// <param name="rhs">Rhs.</param>
        public static bool operator !=(AtomicBooleanValue lhs, AtomicBooleanValue rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="AtomicBooleanValue"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="AtomicBooleanValue"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="AtomicBooleanValue"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is AtomicBooleanValue))
                return false;

            var value = (AtomicBooleanValue)obj;
            return Value == value.Value;
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="AtomicBooleanValue"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        #region IAtomic

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AtomicBooleanValue"/> is value.
        /// </summary>
        /// <value>
        ///   <c>true</c> if value; otherwise, <c>false</c>.
        /// </value>
        public bool Value
        {
            get { return AtomicHelper.ReadIntAsBool(ref _value); }
            set { Exchange(value); }
        }

        /// <summary>
        /// Exchanges the current value with the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Returns the original value
        /// </returns>
        public bool Exchange(bool value)
        {
            return AtomicHelper.SetIntAsBool(ref _value, value);
        }

        /// <summary>
        /// Tries to set the new value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// True if the value has been set and the original value was different of value
        /// </returns>
        public bool TrySet(bool value)
        {
            var result = Exchange(value);
            return result != value;
        }

        #endregion
    }

    /// <summary>
    /// Atomic boolean value class
    /// </summary>
    public sealed class AtomicBoolean : IAtomic<bool>
    {
        private AtomicBooleanValue _value;

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicBoolean"/> class.
        /// </summary>
        public AtomicBoolean()
            : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicBoolean"/> class.
        /// </summary>
        /// <param name="initialValue">Initial value</param>
        public AtomicBoolean(bool initialValue)
        {
            _value = initialValue;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="AtomicBoolean"/> to <see cref="System.Boolean"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator bool(AtomicBoolean value)
        {
            if (value == null)
                return false;

            return value.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Boolean"/> to <see cref="AtomicBoolean"/>.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator AtomicBoolean(bool value)
        {
            return new AtomicBoolean(value);
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _value.ToString();
        }

        #region IAtomic

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public bool Value
        {
            get { return _value.Value; }
            set { _value.Exchange(value); }
        }

        /// <summary>
        /// Exchanges the current value with the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Returns the original value
        /// </returns>
        public bool Exchange(bool value)
        {
            return _value.Exchange(value);
        }

        /// <summary>
        /// Tries to set the new value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// True if the value has been set and the original value was different of value
        /// </returns>
        public bool TrySet(bool value)
        {
            return _value.TrySet(value);
        }

        #endregion
    }

    #endregion


    #region AtomicInteger

    /// <summary>
    /// Atomic Integer struct
    /// </summary>
    public struct AtomicIntegerValue : IAtomic<int>, IEquatable<AtomicIntegerValue>
    {
        private int _value;

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicIntegerValue"/> struct.
        /// </summary>
        /// <param name="initialValue">The initial value.</param>
        public AtomicIntegerValue(int initialValue)
        {
            _value = initialValue;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="AtomicIntegerValue"/> to <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator int(AtomicIntegerValue value)
        {
            return value.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="AtomicIntegerValue"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator AtomicIntegerValue(int value)
        {
            return new AtomicIntegerValue(value);
        }

        #endregion

        #region IEquatable implementation

        /// <summary>
        /// Determines whether the specified <see cref="AtomicIntegerValue"/> is equal to the current <see cref="AtomicIntegerValue"/>.
        /// </summary>
        /// <param name="other">The <see cref="AtomicIntegerValue"/> to compare with the current <see cref="AtomicIntegerValue"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="AtomicIntegerValue"/> is equal to the current
        /// <see cref="AtomicIntegerValue"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(AtomicIntegerValue other)
        {
            return this.Value == other.Value;
        }

        #endregion

        /// <param name="lhs">Lhs.</param>
        /// <param name="rhs">Rhs.</param>
        public static bool operator ==(AtomicIntegerValue lhs, AtomicIntegerValue rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <param name="lhs">Lhs.</param>
        /// <param name="rhs">Rhs.</param>
        public static bool operator !=(AtomicIntegerValue lhs, AtomicIntegerValue rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="AtomicIntegerValue"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="AtomicIntegerValue"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="AtomicIntegerValue"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is AtomicIntegerValue))
                return false;

            var value = (AtomicIntegerValue)obj;
            return Value == value.Value;
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="AtomicIntegerValue"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Value.ToString();
        }

        #region IAtomic

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public int Value
        {
            get { return Interlocked.CompareExchange(ref _value, 0, 0); }
            set { Exchange(value); }
        }

        /// <summary>
        /// Exchanges the current value with the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Returns the original value
        /// </returns>
        public int Exchange(int value)
        {
            return Interlocked.Exchange(ref _value, value);
        }

        /// <summary>
        /// Tries to set the new value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// True if the value has been set and the original value was different of value
        /// </returns>
        public bool TrySet(int value)
        {
            var result = Exchange(value);
            return result != value;
        }

        #endregion
    }

    /// <summary>
    /// Atomic interger class
    /// </summary>
    public sealed class AtomicInteger : IAtomic<int>
    {
        private AtomicIntegerValue _value;

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicInteger"/> class.
        /// </summary>
        public AtomicInteger()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicInteger"/> class.
        /// </summary>
        /// <param name="initialValue">The initial value.</param>
        public AtomicInteger(int initialValue)
        {
            _value = initialValue;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="AtomicInteger"/> to <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator int(AtomicInteger value)
        {
            if (value == null)
                return 0;

            return value.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="AtomicInteger"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator AtomicInteger(int value)
        {
            return new AtomicInteger(value);
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _value.ToString();
        }

        #region IAtomic

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public int Value
        {
            get { return _value.Value; }
            set { _value.Exchange(value); }
        }

        /// <summary>
        /// Exchanges the current value with the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Returns the original value
        /// </returns>
        public int Exchange(int value)
        {
            return _value.Exchange(value);
        }

        /// <summary>
        /// Tries to set the new value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// True if the value has been set and the original value was different of value
        /// </returns>
        public bool TrySet(int value)
        {
            return _value.TrySet(value);
        }

        #endregion
    }

    #endregion


    #region AtomicReference

    /// <summary>
    /// Atomic reference base struct
    /// </summary>
    /// <typeparam name="T">Type of reference</typeparam>
    public struct AtomicReferenceValue<T> : IAtomic<T>, IEquatable<AtomicReferenceValue<T>>
        where T : class
    {
        private T _value;

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicReferenceValue{T}"/> struct.
        /// </summary>
        /// <param name="initialValue">The initial value.</param>
        public AtomicReferenceValue(T initialValue)
        {
            _value = initialValue;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="AtomicReferenceValue{T}"/> to T/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator T(AtomicReferenceValue<T> value)
        {
            return value.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from T to <see cref="AtomicReferenceValue{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator AtomicReferenceValue<T>(T value)
        {
            return new AtomicReferenceValue<T>(value);
        }

        #endregion

        #region IEquatable implementation

        /// <summary>
        /// Determines whether the specified <see cref="AtomicReferenceValue{T}"/> is equal to the
        /// current <see cref="AtomicReferenceValue{T}"/>.
        /// </summary>
        /// <param name="other">The <see cref="AtomicReferenceValue{T}"/> to compare with the current <see cref="AtomicReferenceValue{T}"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="AtomicReferenceValue{T}"/> is equal to the
        /// current <see cref="AtomicReferenceValue{T}"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(AtomicReferenceValue<T> other)
        {
            var thisVal = this.Value;
            if (thisVal == null)
                return other.Value == null;

            return thisVal.Equals(other.Value);
        }

        #endregion

        /// <param name="lhs">Lhs.</param>
        /// <param name="rhs">Rhs.</param>
        public static bool operator ==(AtomicReferenceValue<T> lhs, AtomicReferenceValue<T> rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <param name="lhs">Lhs.</param>
        /// <param name="rhs">Rhs.</param>
        public static bool operator !=(AtomicReferenceValue<T> lhs, AtomicReferenceValue<T> rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="AtomicReferenceValue{T}"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="AtomicReferenceValue{T}"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="AtomicReferenceValue{T}"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is AtomicReferenceValue<T>))
                return false;

            var value = (AtomicReferenceValue<T>)obj;
            return Value == value.Value;
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="AtomicReferenceValue{T}"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (_value == null)
                return String.Empty;

            return _value.ToString();
        }

        #region IAtomic

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public T Value
        {
            get { return Interlocked.CompareExchange(ref _value, null, null); }
            set { Exchange(value); }
        }

        /// <summary>
        /// Exchanges the current value with the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Returns the original value
        /// </returns>
        public T Exchange(T value)
        {
            return Interlocked.Exchange(ref _value, value);
        }

        /// <summary>
        /// Tries to set the new value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// True if the value has been set and the original value was different of value
        /// </returns>
        public bool TrySet(T value)
        {
            var result = Exchange(value);
            return !ReferenceEquals(result, value);
        }

        #endregion
    }

    /// <summary>
    /// Atomic reference base class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class AtomicReference<T> : IAtomic<T>
        where T : class
    {
        private AtomicReferenceValue<T> _value;

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicReference{T}"/> class.
        /// </summary>
        public AtomicReference()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtomicReference{T}"/> class.
        /// </summary>
        /// <param name="initialValue">The initial value.</param>
        public AtomicReference(T initialValue)
        {
            _value = initialValue;
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="AtomicReference{T}"/> to T.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator T(AtomicReference<T> value)
        {
            if (value == null)
                return default(T);

            return value.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from T to <see cref="AtomicReference{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator AtomicReference<T>(T value)
        {
            return new AtomicReference<T>(value);
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _value.ToString();
        }

        #region IAtomic

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value
        {
            get { return _value.Value; }
            set { _value.Exchange(value); }
        }

        /// <summary>
        /// Exchanges the current value with the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Returns the original value
        /// </returns>
        public T Exchange(T value)
        {
            return _value.Exchange(value);
        }

        /// <summary>
        /// Tries to set the new value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// True if the value has been set and the original value was different of value
        /// </returns>
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
            #if DEBUG
            if (value != True && value != False)
                throw new ArgumentException("Value MUST be 0 or 1", "value");
            #endif

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
    }
}
