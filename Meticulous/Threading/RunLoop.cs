using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meticulous.Threading
{
    public sealed class RunLoop : IDisposable
    {
        [ThreadStatic]
        private static RunLoop _currentLoop;

        private readonly int _threadId;

        private readonly AutoResetEvent _signal;
        private ExecutionQueueProcessorImpl _processor;
        private readonly ExecutionQueue _queue;

        private volatile bool _isStopped;

        private RunLoop()
        {
            _threadId = Thread.CurrentThread.ManagedThreadId;
            _signal = new AutoResetEvent(false);
            _queue = new ExecutionQueue(action => new ExecutionQueueProcessorImpl(this, action));
        }

        public static RunLoop Current
        {
            get { return _currentLoop; }
        }

        public int ThreadId
        {
            get { return _threadId; }
        }

        public void Stop()
        {
            _isStopped = true;
            _signal.Set();
        }

        public void Dispose()
        {
            _queue.Dispose();
        }

        public static void Run()
        {
            Run(null);
        }

        public static void Run(Action initialAction)
        {
            var loop = new RunLoop();
            var oldCtx = SynchronizationContext.Current;
            var oldLoop = _currentLoop;
            var newCtx = new SynchronizationContextImpl(loop);
            try
            {
                SynchronizationContext.SetSynchronizationContext(newCtx);
                _currentLoop = loop;
                if (initialAction != null)
                    loop._queue.Post(initialAction);
                loop.RunImpl();
            }
            finally
            {
                _currentLoop = oldLoop;
                SynchronizationContext.SetSynchronizationContext(oldCtx);
            }
        }

        private void RunImpl()
        {
            while (_signal.WaitOne())
            {
                if (_isStopped)
                    return;

                _processor.Pump();
            }
        }



        private sealed class ExecutionQueueProcessorImpl : ExecutionQueueProcessor
        {
            private readonly RunLoop _loop;
            public ExecutionQueueProcessorImpl(RunLoop runLoop, Action action)
                : base(action)
            {
                _loop = runLoop;
                _loop._processor = this;
            }

            public override void Pulse()
            {
                _loop._signal.Set();
            }

            public void Pump()
            {
                Execute();
            }
        }

        private sealed class SynchronizationContextImpl : SynchronizationContext
        {
            private readonly RunLoop _loop;

            public SynchronizationContextImpl(RunLoop loop)
            {
                _loop = loop;
            }

            public override SynchronizationContext CreateCopy()
            {
                return new SynchronizationContextImpl(_loop);
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                _loop._queue.Post(() =>
                {
                    d(state);
                });
            }

            public override void Send(SendOrPostCallback d, object state)
            {
                if (_loop.ThreadId == Thread.CurrentThread.ManagedThreadId)
                {
                    d(state);
                    return;
                }

                _loop._queue.Send(() =>
                {
                    d(state);
                });
            }
        }
    }
}
