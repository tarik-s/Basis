using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Meticulous.Threading
{
    public enum ExecutionQueueProcessorType
    {
        Thread,
        ThreadPool
    }

    public sealed class ExecutionQueue : IDisposable
    {
        #region Fields

        private volatile State _state;
        private volatile bool _isAcquired;
        private readonly ManualResetEvent _stopEvent;
        private readonly Queue<ExecutionItem> _items;
        private readonly ExecutionQueueProcessor _processor;

        private readonly Lazy<Scheduler> _scheduler;

        #endregion

        #region Creation

        internal ExecutionQueue(Func<Action, ExecutionQueueProcessor> processorFactory)
        {
            _state = State.Working;
            _stopEvent = new ManualResetEvent(false);
            _items = new Queue<ExecutionItem>();
            _scheduler = new Lazy<Scheduler>(() => new Scheduler(this));
            _processor = processorFactory(Execute);
        }

        public static ExecutionQueue Create()
        {
            return Create(ExecutionQueueProcessorType.ThreadPool);
        }

        public static ExecutionQueue Create(ExecutionQueueProcessorType type)
        {
            return new ExecutionQueue(action => ExecutionQueueProcessor.Create(type, action));
        }

        #endregion

        public event EventHandler<ExecutionQueueUnhandledExceptionEventArgs> UnhandledException;


        #region Post

        public void Post(Action action)
        {
            Post(action, CancellationToken.None);
        }

        public void Post(Action action, CancellationToken token)
        {
            Check.ArgumentNotNull(action, "action");
            CheckWorking();

            var item = new ExecutionItem(ExecutionItemOrigin.Post, action, token);
            Enqueue(item);
        }

        #endregion

        #region Send

        public void Send(Action action)
        {
            Send(action, CancellationToken.None);
        }

        public void Send(Action action, CancellationToken token)
        {
            Check.ArgumentNotNull(action, "action");
            CheckWorking();

            token.ThrowIfCancellationRequested();

            var completionEvent = new ManualResetEvent(false);
            var item = new ExecutionItem(ExecutionItemOrigin.Send, action, token) { CompletionEvent = completionEvent };

            Enqueue(item);

            completionEvent.WaitOne();

            token.ThrowIfCancellationRequested();

            var exception = item.Exception;
            if (exception != null)
                throw new AggregateException(exception);
        }

        #endregion

        #region StartTask

        public Task StartTask(Action action)
        {
            return StartTask(action, CancellationToken.None);
        }

        public Task StartTask(Action action, CancellationToken token)
        {
            Check.ArgumentNotNull(action, "action");
            CheckWorking();

            var scheduler = _scheduler.Value;
            var factory = Task.Factory;
            return factory.StartNew(action, token, factory.CreationOptions, scheduler);
        }

        public Task<TResult> StartTask<TResult>(Func<TResult> func)
        {
            return StartTask(func, CancellationToken.None);
        }

        public Task<TResult> StartTask<TResult>(Func<TResult> func, CancellationToken token)
        {
            Check.ArgumentNotNull(func, "func");
            CheckWorking();

            var scheduler = _scheduler.Value;
            var factory = Task<TResult>.Factory;
            return factory.StartNew(func, token, factory.CreationOptions, scheduler);
        }

        #endregion

        public bool Stop()
        {
            CheckNotDisposed();

            lock (_items)
            {
                if (_state != State.Working)
                    return false;

                var item = new ExecutionItem(ExecutionItemOrigin.Stop, () =>
                {
                    _state = State.Stopped;
                    _stopEvent.Set();
                    _processor.Stop();
                });
                Enqueue(item);
                _state = State.Stopping;
            }

            return true;
        }

        public void Wait()
        {
            Wait(Timeout.InfiniteTimeSpan);
        }

        public bool Wait(TimeSpan timeout)
        {
            CheckNotDisposed();

            return _stopEvent.WaitOne(timeout);
        }

        #region IDisposable

        public void Dispose()
        {
            if (_state == State.Disposed)
                return;

            Stop();
            Wait();

            _state = State.Disposed;
            _stopEvent.Dispose();
        }

        #endregion


        private void Enqueue(ExecutionItem item)
        {
            var shouldExecute = false;
            //var count = 0;

            lock (_items)
            {
                CheckWorking(); // Ensure the queue has not been stopped
                _items.Enqueue(item);
                if (!_isAcquired)
                {
                    _isAcquired = true;
                    shouldExecute = true;
                }
                //count = _items.Count;
            }

            //if (count > _criticalItemCount)
            //{
            //    NotifyLimitReached();
            //    Thread.Sleep(100);
            //}

            if (shouldExecute)
                _processor.Pulse();
        }

        private void Execute()
        {
            for (; ; )
            {
                ExecutionItem[] items;
                lock (_items)
                {
                    items = _items.ToArray();
                    _items.Clear();
                }

                foreach (var item in items)
                {
                    Execute(item);
                }

                lock (_items)
                {
                    if (_items.Count != 0)
                        continue;

                    _isAcquired = false;
                    return;
                }
            }
        }

        private void Execute(ExecutionItem item)
        {
            try
            {
                if (!item.Token.IsCancellationRequested)
                    item.Action();
            }
            catch (Exception e)
            {
                item.Exception = e;
                if (item.Origin == ExecutionItemOrigin.Post)
                    RaiseUnhandledException(e);
            }
            finally
            {
                var evt = item.CompletionEvent;
                if (evt != null)
                    evt.Set();
            }
        }

        private void RaiseUnhandledException(Exception exception)
        {
            var args = new ExecutionQueueUnhandledExceptionEventArgs(exception);
            args.Raise(this, ref UnhandledException);
        }

        private void CheckNotDisposed()
        {
            if (_state == State.Disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        private void CheckWorking()
        {
            CheckNotDisposed();
            Check.OperationValid(_state == State.Working, "Queue is already stopp(ing)/(ed)");
        }

        #region Private classes

        [Serializable]
        private enum State
        {
            Working,
            Stopping,
            Stopped,
            Disposed
        }

        [Serializable]
        private enum ExecutionItemOrigin
        {
            Post,
            Send,
            Task,
            Stop
        }

        private sealed class ExecutionItem
        {
            private readonly Action _action;
            private readonly CancellationToken _token;
            private readonly ExecutionItemOrigin _origin;
            public ExecutionItem(ExecutionItemOrigin origin, Action action)
                : this(origin, action, CancellationToken.None)
            {
            }

            public ExecutionItem(ExecutionItemOrigin origin, Action action, CancellationToken token)
            {
                _action = action;
                _token = token;
                _origin = origin;
            }

            public Action Action
            {
                get { return _action; }
            }

            public ExecutionItemOrigin Origin
            {
                get { return _origin; }
            }

            public CancellationToken Token
            {
                get { return _token; }
            }

            public Exception Exception { get; set; }

            public ManualResetEvent CompletionEvent { get; set; }

        }

        private sealed class Scheduler : TaskScheduler
        {
            private readonly ExecutionQueue _queue;

            public Scheduler(ExecutionQueue queue)
            {
                Check.ArgumentNotNull(queue, "queue");

                _queue = queue;
            }

            public override int MaximumConcurrencyLevel
            {
                get { return 1; }
            }

            protected override void QueueTask(Task task)
            {
                var item = new ExecutionItem(ExecutionItemOrigin.Task, () => ExecuteTask(task));
                _queue.Enqueue(item);
            }

            private void ExecuteTask(Task task)
            {
                base.TryExecuteTask(task);
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                return false;
            }

            protected override IEnumerable<Task> GetScheduledTasks()
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }

    public sealed class ExecutionQueueUnhandledExceptionEventArgs : EventArgs
    {
        private readonly Exception _exception;
        public ExecutionQueueUnhandledExceptionEventArgs(Exception exception)
        {
            _exception = exception;
        }

        public Exception Exception
        {
            get { return _exception; }
        }
    }

    internal abstract class ExecutionQueueProcessor
    {
        private readonly Action _action;
        public static ExecutionQueueProcessor Create(ExecutionQueueProcessorType type, Action action)
        {
            if (type == ExecutionQueueProcessorType.Thread)
                return new ThreadExecutionQueueProcessor(action);

            if (type == ExecutionQueueProcessorType.ThreadPool)
                return new ThreadPoolExecutionQueueProcessor(action);

            throw new ArgumentException("Invalid processor type", "type");
        }

        protected ExecutionQueueProcessor(Action action)
        {
            Check.ArgumentNotNull(action, "action");

            _action = action;
        }

        public abstract void Pulse();

        public virtual void Stop()
        {
        }

        protected void Execute()
        {
            _action();
        }

        private sealed class ThreadExecutionQueueProcessor : ExecutionQueueProcessor
        {
            private readonly Thread _thread;
            private readonly AutoResetEvent _signal;
            private volatile bool _isStopped;
            public ThreadExecutionQueueProcessor(Action action)
                : base(action)
            {
                _thread = new Thread(Main);
                _signal = new AutoResetEvent(false);
            }

            public override void Pulse()
            {
                _signal.Set();
            }

            public override void Stop()
            {
                _isStopped = true;
                _signal.Set();
            }

            private void Main()
            {
                while (_signal.WaitOne())
                {
                    if (_isStopped)
                        return;

                    Execute();
                }
            }
        }

        private sealed class ThreadPoolExecutionQueueProcessor : ExecutionQueueProcessor
        {
            public ThreadPoolExecutionQueueProcessor(Action action)
                : base(action)
            {
            }

            public override void Pulse()
            {
                ThreadPool.QueueUserWorkItem(_ => Execute());
            }

            public override void Stop()
            {

            }
        }
    }


}
