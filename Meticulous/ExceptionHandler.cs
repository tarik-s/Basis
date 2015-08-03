using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous
{
    public abstract class ExceptionHandler
    {
        #region Fields

        private static readonly ExceptionHandler _alwaysHandling = new DummyHandler(true);
        private static readonly ExceptionHandler _neverHandling = new DummyHandler(false);

        #endregion
        
        public static ExceptionHandler AlwaysHandling
        {
            get { return _alwaysHandling; }
        }

        public static ExceptionHandler NeverHandling
        {
            get { return _neverHandling; }
        }

        public static ExceptionHandler Create(Func<Exception, object, bool> handler)
        {
            Check.ArgumentNotNull(handler, "handler");

            return new DefaultHandler(handler);
        }

        public static ExceptionHandler Create(Func<Exception, bool> handler)
        {
            Check.ArgumentNotNull(handler, "handler");

            return new DefaultHandler((e, _) => handler(e));
        }

        public bool HandleException(Exception exception)
        {
            return HandleExceptionImpl(exception, null);
        }

        public bool HandleException(Exception exception, object state)
        {
            return HandleExceptionImpl(exception, state);
        }

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
