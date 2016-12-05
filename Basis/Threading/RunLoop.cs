using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meticulous.Threading
{
    /// <summary>
    /// Represent a run loop
    /// </summary>
    public sealed class RunLoop : IDisposable
    {
        [ThreadStatic]
        private static RunLoop _currentLoop;

        private static RunLoop _mainLoop;

        private readonly int _threadId;
        private readonly AutoResetEvent _signal;
        private ExecutionQueueProcessorImpl _processor;
        private readonly ExecutionQueue _queue;
        private int _exitCode;

        private RunLoop()
        {
            _threadId = Thread.CurrentThread.ManagedThreadId;
            _signal = new AutoResetEvent(false);
            _queue = new ExecutionQueue(action => new ExecutionQueueProcessorImpl(this, action));
        }

        /// <summary>
        /// Gets the current thread run loop.
        /// </summary>
        public static RunLoop Current
        {
            get { return _currentLoop; }
        }

        /// <summary>
        /// Gets the current thread main run loop.
        /// </summary>
        public static RunLoop MainLoop
        {
            get { return _mainLoop; }
        }

        /// <summary>
        /// Gets the thread identifier the loop is running on.
        /// </summary>
        public int ThreadId
        {
            get { return _threadId; }
        }

        /// <summary>
        /// Stops the loop with the specified exit code.
        /// </summary>
        /// <param name="exitCode">The exit code.</param>
        public void Stop(int exitCode)
        {
            _exitCode = exitCode;
            _queue.Stop();
        }

        /// <summary>
        /// Stops the loop.
        /// </summary>
        public void Stop()
        {
            Stop(0);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _queue.Dispose();
            _signal.Dispose();
        }

        /// <summary>
        /// Runs the loop on the current thread
        /// </summary>
        /// <returns>Returns the exit code the loop was stopped with.</returns>
        public static int Run()
        {
            return Run(null);
        }

        /// <summary>
        /// Runs the loop on the current thread
        /// </summary>
        /// <param name="initialAction">The initial action.</param>
        /// <returns>Returns the exit code the loop was stopped with.</returns>
        public static int Run(Action initialAction)
        {
            return RunImpl(initialAction, false);
        }

        /// <summary>
        /// Runs the main loop on the current thread.
        /// </summary>
        /// <param name="initialAction">The initial action.</param>
        /// <returns>Returns the exit code the loop was stopped with.</returns>
        public static int RunMain(Action initialAction)
        {
            Check.OperationValid(_mainLoop == null, "Main loop is already started");
            Check.OperationValid(_currentLoop == null, "Main loop cannot run within another loop");

            return RunImpl(initialAction, true);
        }

        private static int RunImpl(Action initialAction, bool isMain)
        {
            var loop = new RunLoop();
            var oldCtx = SynchronizationContext.Current;
            var oldLoop = _currentLoop;
            var newCtx = new SynchronizationContextImpl(loop);
            try
            {
                SynchronizationContext.SetSynchronizationContext(newCtx);
                _currentLoop = loop;
                if (isMain)
                    _mainLoop = loop;
                if (initialAction != null)
                    loop._queue.Post(initialAction);
                return loop.RunImpl();
            }
            finally
            {
                _currentLoop = oldLoop;
                SynchronizationContext.SetSynchronizationContext(oldCtx);
                loop.Dispose();
            }
        }

        private int RunImpl()
        {
            var handles = new WaitHandle[] {_queue.StopHandle, _signal};
            for (;;)
            {
                var index = WaitHandle.WaitAny(handles);
                if (index != 1)
                    break;
                _processor.Pump();
            }
            return _exitCode;
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
