using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous
{
    /// <summary>
    /// The base class for exception handling
    /// </summary>
    public abstract class ExceptionHandler
    {
        #region Fields

        private static readonly ExceptionHandler s_alwaysHandling = new DummyHandler(true);
        private static readonly ExceptionHandler s_neverHandling = new DummyHandler(false);

        #endregion
        
        /// <summary>
        /// Gets an instance of <see cref="ExceptionHandler"/> which handles of exceptions.
        /// </summary>
        public static ExceptionHandler AlwaysHandling
        {
            get { return s_alwaysHandling; }
        }

        /// <summary>
        /// Gets an instance of <see cref="ExceptionHandler"/> which doesn't handle any exception.
        /// </summary>
        public static ExceptionHandler NeverHandling
        {
            get { return s_neverHandling; }
        }

        /// <summary>
        /// Creates an instance of <see cref="ExceptionHandler"/> with specific handling delegate
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>Return new instance</returns>
        public static ExceptionHandler Create(Func<Exception, object, bool> handler)
        {
            Check.ArgumentNotNull(handler, "handler");

            return new DefaultHandler(handler);
        }

        /// <summary>
        /// Creates an instance of <see cref="ExceptionHandler"/>  with specific handling delegate
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns>Return new instance</returns>
        public static ExceptionHandler Create(Func<Exception, bool> handler)
        {
            Check.ArgumentNotNull(handler, "handler");

            return new DefaultHandler((e, _) => handler(e));
        }


        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>Returns true is exception is handled successfully, otherwise false</returns>
        public bool HandleException(Exception exception)
        {
            return HandleExceptionImpl(exception, null);
        }


        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="state">The state.</param>
        /// <returns>Returns true is exception is handled successfully, otherwise false</returns>
        public bool HandleException(Exception exception, object state)
        {
            return HandleExceptionImpl(exception, state);
        }


        /// <summary>
        /// Handles the exception implementation.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="state">The state.</param>
        /// <returns>Returns true is exception is handled successfully, otherwise false</returns>
        protected abstract bool HandleExceptionImpl(Exception exception, object state);

        #region Private implementations

        private sealed class DummyHandler : ExceptionHandler
        {
            private readonly bool _handlingResult;
            public DummyHandler(bool handlingResult)
            {
                _handlingResult = handlingResult;
            }

            protected override bool HandleExceptionImpl(Exception exception, object state)
            {
                return _handlingResult;
            }
        }
    
        private sealed class DefaultHandler : ExceptionHandler
        {
            private readonly Func<Exception, object, bool> _handler;
            public DefaultHandler(Func<Exception, object, bool> handler)
            {
                _handler = handler;
            }

            protected override bool HandleExceptionImpl(Exception exception, object state)
            {
                return _handler(exception, state);
            }
        }

        #endregion
    }
}
