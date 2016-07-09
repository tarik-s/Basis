using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Threading.Tasks
{
    [Flags]
    public enum AsyncValueCreationOptions
    {
        Default = 0x0,
        RememberFailedResult = 0x1
    }

    public sealed class AsyncValue<T>
    {
        private readonly object _lock = new object();

        private Func<T> _factory;
        private Func<Task<T>> _asyncFactory;

        private Lazy<Task<T>> _task;
        private AsyncValueCreationOptions _creationOptions;

        public AsyncValue(Func<T> factory, AsyncValueCreationOptions creationOptions)
        {
            Check.ArgumentNotNull(factory, "factory");

            ResetImpl_Unsafe(factory, null, creationOptions);
        }

        public AsyncValue(Func<Task<T>> factory, AsyncValueCreationOptions creationOptions)
        {
            Check.ArgumentNotNull(factory, "factory");

            ResetImpl_Unsafe(null, factory, creationOptions);
        }

        public TaskAwaiter<T> GetAwaiter()
        {
            return LazyTask.Value.GetAwaiter();
        }

        public void Start()
        {
            var unused = LazyTask.Value;
        }

        public void Reset()
        {
            ResetImpl(null, null, _creationOptions);
        }

        public void Reset(Func<T> factory)
        {
            Check.ArgumentNotNull(factory, "factory");

            ResetImpl(factory, null, _creationOptions);
        }
        public void Reset(Func<Task<T>> factory)
        {
            Check.ArgumentNotNull(factory, "factory");

            ResetImpl(null, factory, _creationOptions);
        }


        private void ResetImpl(Func<T> factory, Func<Task<T>> asyncFactory, AsyncValueCreationOptions creationOptions)
        {
            lock (_lock)
            {
                ResetImpl_Unsafe(factory, asyncFactory, creationOptions);
            }
        }

        private void ResetImpl_Unsafe(Func<T> factory, Func<Task<T>> asyncFactory, AsyncValueCreationOptions creationOptions)
        {
            if (factory != null && asyncFactory != null)
            {
                throw new InvalidOperationException("Ambiguous arguments");
            }

            if (factory != null)
            {
                _factory = factory;
                _asyncFactory = null;
                _creationOptions = creationOptions;
                _task = new Lazy<Task<T>>(() => WrapTask(() => Task.Run(factory)));
            }
            else if (asyncFactory != null)
            {
                _asyncFactory = asyncFactory;
                _factory = null;
                _creationOptions = creationOptions;
                _task = new Lazy<Task<T>>(() => WrapTask(() => Task.Run(asyncFactory)));
            }
            else
            {
                ResetImpl_Unsafe(_factory, _asyncFactory, _creationOptions); // Call reset with previous factories
            }
        }

        private async Task<T> WrapTask(Func<Task<T>> func)
        {
            try
            {
                var result = await func();
                return result;
            }
            catch (Exception)
            {
                if (!_creationOptions.HasFlag(AsyncValueCreationOptions.RememberFailedResult))
                    Reset();

                throw;
            }
        }

        private Lazy<Task<T>> LazyTask
        {
            get
            {
                lock (_lock)
                {
                    return _task;
                }
            }
        }
    }

    public static class AsyncValue
    {
        public static AsyncValue<T> Create<T>(Func<Task<T>> factory, AsyncValueCreationOptions creationOptions)
        {
            return new AsyncValue<T>(factory, creationOptions);
        }

        public static AsyncValue<T> Create<T>(Func<Task<T>> factory)
        {
            return new AsyncValue<T>(factory, AsyncValueCreationOptions.Default);
        }

        public static AsyncValue<T> Create<T>(Func<T> factory, AsyncValueCreationOptions creationOptions)
        {
            return new AsyncValue<T>(factory, creationOptions);
        }

        public static AsyncValue<T> Create<T>(Func<T> factory)
        {
            return new AsyncValue<T>(factory, AsyncValueCreationOptions.Default);
        }

        public static AsyncValueBuilder CreateBuilder()
        {
            return new AsyncValueBuilder();
        }

        public static AsyncValueBuilder CreateBuilder(AsyncValueCreationOptions options)
        {
            return new AsyncValueBuilder(options);
        }
    }

    public class AsyncValueBuilder
    {
        private AsyncValueCreationOptions _options;

        public AsyncValueBuilder()
            : this(AsyncValueCreationOptions.Default)
        {
        }

        public AsyncValueBuilder(AsyncValueCreationOptions options)
        {
            _options = options;
        }

        public AsyncValueCreationOptions CreationOptions
        {
            get { return _options; }
        }

        public AsyncValueBuilder SetOptions(AsyncValueCreationOptions options)
        {
            _options = options;
            return this;
        }

        public AsyncValue<T> Build<T>(Func<Task<T>> factory)
        {
            return AsyncValue.Create(factory, _options);
        }

        public AsyncValue<T> Build<T>(Func<T> factory)
        {
            return AsyncValue.Create(factory, _options);
        }
    }

    public class AsyncValueBuilder<T> : AsyncValueBuilder
    {
        public AsyncValueBuilder()
            : base(AsyncValueCreationOptions.Default)
        {
        }

        public AsyncValueBuilder(AsyncValueCreationOptions options)
            : base(options)
        {

        }

        public AsyncValue<T> Build(Func<Task<T>> factory)
        {
            return base.Build(factory);
        }

        public AsyncValue<T> Build(Func<T> factory)
        {
            return base.Build(factory);
        }
    }
}
