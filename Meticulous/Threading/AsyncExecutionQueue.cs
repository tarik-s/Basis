using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meticulous.Threading
{
    public sealed class AsyncExecutionQueue
    {
        #region Fields

        private readonly object _lock = new object();
        private readonly Timer _timer;
        private readonly Func<Task<TimeSpan?>> _timerFunction;

        private TimeSpan _interval;
        private Task _queue;

        #endregion

        #region Construction

        private AsyncExecutionQueue(Func<Task<TimeSpan?>> timerFunction, TimeSpan interval, TimeSpan delayBeforeStart)
        {
            _interval = interval;
            _queue = Task.FromResult(0);
            _timerFunction = timerFunction;
            if (_timerFunction != null)
            {
                _timer = new Timer(OnTimer);
                if (delayBeforeStart == TimeSpan.Zero)
                    Schedule(() => OnTimerImpl());
                else
                    _timer.Change(delayBeforeStart, Timeout.InfiniteTimeSpan);
            }
        }

        public AsyncExecutionQueue()
            : this(null, Timeout.InfiniteTimeSpan, TimeSpan.Zero)
        {
        }

        public static AsyncExecutionQueue Create()
        {
            return new AsyncExecutionQueue();
        }

        public static AsyncExecutionQueue Create(Action timerFunction, TimeSpan interval)
        {
            Check.ArgumentNotNull(timerFunction, "timerFunction");

            return Create(() =>
            {
                timerFunction();
                return Task.FromResult(0);
            }, interval);
        }

        public static AsyncExecutionQueue Create(Func<Task> timerFunction, TimeSpan interval)
        {
            return Create(async () =>
            {
                await timerFunction();
                return (TimeSpan?)null;
            }, interval, TimeSpan.Zero);
        }

        public static AsyncExecutionQueue Create(Func<Task<TimeSpan?>> timerFunction, TimeSpan interval, TimeSpan delayBeforeStart)
        {
            Check.ArgumentNotNull(timerFunction, "timerFunction");

            Check.ArgumentInRange(interval, "interval", i => i > TimeSpan.Zero);
            Check.ArgumentInRange(delayBeforeStart, "delayBeforeStart", i => i >= TimeSpan.Zero);

            return new AsyncExecutionQueue(timerFunction, interval, delayBeforeStart);
        }

        #endregion

        public event EventHandler<ExceptionEventArgs> ExceptionOccurred;

        public Task Schedule(Action action)
        {
            Check.ArgumentNotNull(action, "action");

            lock (_lock)
            {
                _queue = _queue.ContinueWith(_ => action());
                return _queue;
            }
        }

        public Task<TResult> Schedule<TResult>(Func<TResult> function)
        {
            Check.ArgumentNotNull(function, "function");

            lock (_lock)
            {
                var result = _queue.ContinueWith(_ => function());
                _queue = result;
                return result;
            }
        }

        public Task Schedule(Func<Task> continuationFunction)
        {
            Check.ArgumentNotNull(continuationFunction, "continuationFunction");

            lock (_lock)
            {
                _queue = _queue.ContinueWith(_ => continuationFunction()).Unwrap();
                return _queue;
            }
        }

        public Task<TResult> Schedule<TResult>(Func<Task<TResult>> continuationFunction)
        {
            Check.ArgumentNotNull(continuationFunction, "continuationFunction");

            lock (_lock)
            {
                var result = _queue.ContinueWith(_ => continuationFunction()).Unwrap();
                _queue = result;
                return result;
            }
        }

        private void OnTimer(object _)
        {
            Schedule(() => OnTimerImpl());
        }

        private async Task OnTimerImpl()
        {
            try
            {
                var interval = await _timerFunction();
                if (interval.HasValue)
                    _interval = interval.Value;
            }
            catch (Exception e)
            {
                ExceptionEventArgs.Create(e).Raise(this, ref ExceptionOccurred);
            }

            _timer.Change(_interval, Timeout.InfiniteTimeSpan);
        }
    }
}
