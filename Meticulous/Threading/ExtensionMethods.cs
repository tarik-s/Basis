using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meticulous.Threading
{
    public static class ExtensionMethods
    {
        public static Task<TResult> StartTask<TResult>(this SynchronizationContext @this, Func<TResult> func, 
            CancellationToken cancellationToken, TaskCreationOptions options)
        {
            Check.This(@this);
            Check.ArgumentNotNull(func, "func");

            return StartTaskImpl(@this, func, cancellationToken, options);
        }

        private static Task<TResult> StartTaskImpl<TResult>(this SynchronizationContext @this, Func<TResult> func,
            CancellationToken cancellationToken, TaskCreationOptions options)
        {
            var tcs = new TaskCompletionSource<TResult>(options);
            @this.Post(_ =>
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        tcs.SetCanceled();
                    }
                    else
                    {
                        var result = func();
                        tcs.SetResult(result);
                    }
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            }, null);

            return tcs.Task;
        }
    }
}
