using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Threading.Tasks
{
    public sealed class AsyncLazy<T>
    {
        private readonly Lazy<Task<T>> _task;

        public AsyncLazy(Func<T> factory)
        {
            _task = new Lazy<Task<T>>(() => Task.Run(factory));
        }

        public AsyncLazy(Func<Task<T>> factory)
        {
            _task = new Lazy<Task<T>>(() => Task.Run(factory));
        }

        public TaskAwaiter<T> GetAwaiter()
        {
            return _task.Value.GetAwaiter();
        }

        public Task<T> GetValueAsync()
        {
            return _task.Value;
        }

        public void Start()
        {
            var unused = _task.Value;
        }
    }
}
