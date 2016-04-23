using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Threading.Tasks
{
    public sealed class AsyncValue<T>
    {
        private readonly object _lock = new object();

        private Lazy<Task<T>> _task;
        private Func<T> _factory;
        private Func<Task<T>> _asyncFactory;


        public AsyncValue(Func<T> factory)
        {
            Check.ArgumentNotNull(factory, "factory");

            ResetImpl_Unsafe(factory, null);
        }

        public AsyncValue(Func<Task<T>> factory)
        {
            Check.ArgumentNotNull(factory, "factory");

            ResetImpl_Unsafe(null, factory);
        }

        public TaskAwaiter<T> GetAwaiter()
        {
            return LazyTaskImpl.Value.GetAwaiter();
        }

        public Task<T> GetValueAsync()
        {
            return LazyTaskImpl.Value;
        }

        public void Start()
        {
            var unused = LazyTaskImpl.Value;
        }

        public void Reset()
        {
            ResetImpl(null, null);
        }

        public void Reset(Func<T> factory)
        {
            Check.ArgumentNotNull(factory, "factory");

            ResetImpl(factory, null);
        }

        public void Reset(Func<Task<T>> factory)
        {
            Check.ArgumentNotNull(factory, "factory");

            ResetImpl(null, factory);
        }

        #region Private

        private void ResetImpl(Func<T> factory, Func<Task<T>> asyncFactory)
        {
            lock (_lock)
            {
                ResetImpl_Unsafe(factory, asyncFactory);
            }
        }

        private void ResetImpl_Unsafe(Func<T> factory, Func<Task<T>> asyncFactory)
        {
#if DEBUG
            if (factory != null && asyncFactory != null)
                throw new ArgumentException("One of the arguments must be null");
            if (factory == null && asyncFactory == null && _factory == null && _asyncFactory == null)
                throw new ArgumentException("All factories are null");
#endif
            if (factory != null)
                _task = new Lazy<Task<T>>(() => Task.Run(factory));
            else if (asyncFactory != null)
                _task = new Lazy<Task<T>>(() => Task.Run(asyncFactory));
            else
                ResetImpl_Unsafe(_factory, _asyncFactory);

            _factory = factory;
            _asyncFactory = asyncFactory;
        }

        private Lazy<Task<T>> LazyTaskImpl
        {
            get
            {
                lock (_lock)
                {
                    return _task;
                }
            }
        }

        #endregion
    }
}
