using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous
{
    /// <summary>
    /// Event arguments.
    /// </summary>
    public class EventArgs<T> : EventArgs
    {
        private readonly T _value;

        public EventArgs(T value)
        {
            _value = value;
        }

        public static EventArgs<T> Create(T value)
        {
            return new EventArgs<T>(value);
        }

        public T Value { get { return _value; } }
    }

    /// <summary>
    /// ExceptionEventArgs
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
        private readonly Exception _exception;
        public ExceptionEventArgs(Exception exception)
        {
            Check.ArgumentNotNull(exception, "exception");

            _exception = exception;
        }

        public static ExceptionEventArgs Create(Exception exception)
        {
            return new ExceptionEventArgs(exception);
        }

        public Exception Exception
        {
            get { return _exception; }
        }

    }
}
