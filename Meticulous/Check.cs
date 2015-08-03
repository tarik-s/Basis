using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous
{
    public static class Check
    {
        #region Consts

        //private static readonly string _invalidArgument = "Invalid argument";

        #endregion

        public static void This<T>(T @this)
            where T : class
        {
            ArgumentNotNullImpl(@this, "this", "The first argument of extension method is null");
        }

        public static void ArgumentNotNull<T>(T arg, string paramName)
            where T : class
        {
            ArgumentNotNullImpl(arg, paramName, null);
        }

        public static void ArgumentNotNull<T>(T arg, string paramName, string message)
            where T : class
        {
            ArgumentNotNullImpl(arg, paramName, message);
        }


        public static void OperationValid(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }

        #region Impl

        private static void ArgumentNotNullImpl<T>(T arg, string paramName, string message)
            where T : class
        {
            if (arg != null)
                return;

            if (message == null)
                throw new ArgumentNullException(paramName);

            throw new ArgumentNullException(paramName, message);
        }

        #endregion
    }

    public static class CheckDebug
    {
        [Conditional("DEBUG")]
        public static void This<T>(T @this)
            where T : class
        {
            Check.This(@this);
        }

        [Conditional("DEBUG")]
        public static void ArgumentNotNull<T>(T arg, string paramName)
            where T : class
        {
            Check.ArgumentNotNull(arg, paramName);
        }

        [Conditional("DEBUG")]
        public static void ArgumentNotNull<T>(T arg, string paramName, string message)
            where T : class
        {
            Check.ArgumentNotNull(arg, paramName, message);
        }
    }
}
