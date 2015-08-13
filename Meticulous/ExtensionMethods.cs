using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meticulous
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        #region Raise

        /// <summary>
        /// Raise the specified this, sender and eventDelegate.
        /// </summary>
        /// <param name="this">This.</param>
        /// <param name="sender">Sender.</param>
        /// <param name="eventDelegate">Event delegate.</param>
        /// <typeparam name="TEventArgs">The 1st type parameter.</typeparam>
        public static void Raise<TEventArgs>(this TEventArgs @this, Object sender, ref EventHandler<TEventArgs> eventDelegate)
            where TEventArgs : EventArgs
        {
            Check.This(@this);

            // Copy a reference to the delegate field now into a temporary field for thread safety  
            var handler = Volatile.Read(ref eventDelegate);
            // If any methods registered interest with our event, notify them   
            if (handler != null)
                handler(sender, @this);
        }

        /// <summary>
        /// Raise the specified this, sender and eventDelegate.
        /// </summary>
        /// <param name="this">This.</param>
        /// <param name="sender">Sender.</param>
        /// <param name="eventDelegate">Event delegate.</param>
        /// <param name="exceptionHandler">exception handler</param>
        /// <typeparam name="TEventArgs">The 1st type parameter.</typeparam>
        public static void Raise<TEventArgs>(this TEventArgs @this, Object sender, ref EventHandler<TEventArgs> eventDelegate, ExceptionHandler exceptionHandler)
            where TEventArgs : EventArgs
        {
            Check.This(@this);

            // Copy a reference to the delegate field now into a temporary field for thread safety
            var handler = Volatile.Read(ref eventDelegate);
            // If any methods registered interest with our event, notify them
            if (handler == null)
                return;

            var exHandler = exceptionHandler ?? ExceptionHandler.NeverHandling;

            var delegates = handler.GetInvocationList();
            foreach (var delegate1 in delegates)
            {
                var @delegate = (EventHandler<TEventArgs>) delegate1;
                try
                {
                    @delegate(sender, @this);
                }
                catch (Exception e)
                {
                    if (!exHandler.HandleException(e, sender))
                        throw;
                }
            }
        }

        #endregion

        #region FitIn

        /// <summary>
        /// Fits "this" in range.
        /// </summary>
        /// <param name="this">The this.</param>
        /// <param name="lo">The lo.</param>
        /// <param name="hi">The hi.</param>
        /// <returns></returns>
        public static int FitIn(this int @this, int lo, int hi)
        {
            if (@this < lo)
                return lo;
            if (@this > hi)
                return hi;
            return @this;
        }

        /// <summary>
        /// Fits "this" in range.
        /// </summary>
        /// <param name="this">The this.</param>
        /// <param name="lo">The lo.</param>
        /// <param name="hi">The hi.</param>
        /// <returns></returns>
        public static long FitIn(this long @this, long lo, long hi)
        {
            if (@this < lo)
                return lo;
            if (@this > hi)
                return hi;
            return @this;
        }

        /// <summary>
        /// Fits "this" in range.
        /// </summary>
        /// <param name="this">The this.</param>
        /// <param name="lo">The lo.</param>
        /// <param name="hi">The hi.</param>
        /// <returns></returns>
        public static float FitIn(this float @this, float lo, float hi)
        {
            if (@this < lo)
                return lo;
            if (@this > hi)
                return hi;
            return @this;
        }

        /// <summary>
        /// Fits "this" in range.
        /// </summary>
        /// <param name="this">The this.</param>
        /// <param name="lo">The lo.</param>
        /// <param name="hi">The hi.</param>
        /// <returns></returns>
        public static double FitIn(this double @this, double lo, int hi)
        {
            if (@this < lo)
                return lo;
            if (@this > hi)
                return hi;
            return @this;
        }

        #endregion

    }
}
