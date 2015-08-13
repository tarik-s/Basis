using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Meticulous.Threading
{

    /// <summary>
    /// Execution queue processor type
    /// </summary>
    public enum ExecutionQueueProcessorType
    {
        /// <summary>
        /// All task are performed on the separate thread
        /// </summary>
        Thread,

        /// <summary>
        /// All task are performed on the thread pool
        /// </summary>
        ThreadPool
    }

    /// <summary>
    /// Represens an execution queue
    /// </summary>
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

        /// <summary>
        /// Creates a new instance of the <see cref="ExecutionQueue"/> class.
        /// </summary>
        public static ExecutionQueue Create()
        {
            return Create(ExecutionQueueProcessorType.ThreadPool);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ExecutionQueue"/> class.
        /// </summary>
        /// <param name="processorType">The processor factory.</param>
        public static ExecutionQueue Create(ExecutionQueueProcessorType processorType)
        {
            return new ExecutionQueue(action => ExecutionQueueProcessor.Create(processorType, action));
        }

        #endregion

        /// <summary>
        /// Occurs when [unhandled exception] occurred.
        /// </summary>
        public event EventHandler<ExecutionQueueUnhandledExceptionEventArgs> UnhandledException;


        #region Post

        /// <summary>
        /// Posts the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        public void Post(Action action)
        {
            Post(action, CancellationToken.None);
        }

        /// <summary>
        /// Posts the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="token">The cancellation token.</param>
        public void Post(Action action, CancellationToken token)
        {
            Check.ArgumentNotNull(action, "action");
            CheckWorking();

            var item = new ExecutionItem(ExecutionItemOrigin.Post, action, token);
            Enqueue(item);
        }

        #endregion

        #region Send

        /// <summary>
        /// Sends the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        public void Send(Action action)
        {
            Send(action, CancellationToken.None);
        }

        /// <summary>
        /// Sends the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="token">The cancellation token.</param>
        /// <exception cref="System.AggregateException"></exception>
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

        /// <summary>
        /// Starts the task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public Task StartTask(Action action)
        {
            return StartTask(action, CancellationToken.None);
        }

        /// <summary>
        /// Starts the task.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        public Task StartTask(Action action, CancellationToken token)
        {
            Check.ArgumentNotNull(action, "action");
            CheckWorking();

            var scheduler = _scheduler.Value;
            var factory = Task.Factory;
            return factory.StartNew(action, token, factory.CreationOptions, scheduler);
        }

        /// <summary>
        /// Starts the task.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        public Task<TResult> StartTask<TResult>(Func<TResult> func)
        {
            return StartTask(func, CancellationToken.None);
        }

        /// <summary>
        /// Starts the task.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="func">The function.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        public Task<TResult> StartTask<TResult>(Func<TResult> func, CancellationToken token)
        {
            Check.ArgumentNotNull(func, "func");
            CheckWorking();

            var scheduler = _scheduler.Value;
            var factory = Task<TResult>.Factory;
            return factory.StartNew(func, token, factory.CreationOptions, scheduler);
        }

        #endregion

        /// <summary>
        /// Stops this queue.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Waits the queue to be finished.
        /// </summary>
        public void Wait()
        {
            Wait(Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Waits the queue to be finished.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>Returns true if the queue has been finished, otherwise false.</returns>
        public bool Wait(TimeSpan timeout)
        {
            CheckNotDisposed();

            return _stopEvent.WaitOne(timeout);
        }

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
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

    /// <summary>
    /// ExecutionQueueUnhandledExceptionEventArgs
    /// </summary>
    public sealed class ExecutionQueueUnhandledExceptionEventArgs : EventArgs
    {
        private readonly Exception _exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecutionQueueUnhandledExceptionEventArgs"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public ExecutionQueueUnhandledExceptionEventArgs(Exception exception)
        {
            _exception = exception;
        }

        /// <summary>
        /// Gets the exception.
        /// </summary>
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
