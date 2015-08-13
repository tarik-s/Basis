using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous
{
    public static class Check
    {
        #region ArgumentNotNull

        [ContractArgumentValidator]
        public static void This<T>(T @this)
            where T : class
        {
            if (@this == null)
                throw new ArgumentNullException("this", "The first argument of extension method is null");

            Contract.EndContractBlock();
        }

        [ContractArgumentValidator]
        public static void ArgumentNotNull<T>(T arg, string paramName)
            where T : class
        {
            if (arg == null)
                throw new ArgumentNullException(paramName);

            Contract.EndContractBlock();
        }

        [ContractArgumentValidator]
        public static void ArgumentNotNull<T>(T arg, string paramName, string message)
            where T : class
        {
            if (arg == null)
                throw new ArgumentNullException(paramName, message);

            Contract.EndContractBlock();
        }

        #endregion


        #region ArgumentInRange

        [ContractArgumentValidator]
        public static void ArgumentInRange<T>(T arg, string paramName, T lo, T hi, string message)
            where T: struct, IComparable<T>
        {
            if (lo.CompareTo(arg) > 0)
                throw new ArgumentOutOfRangeException(paramName, message);

            if (hi.CompareTo(arg) < 0)
                throw new ArgumentOutOfRangeException(paramName, message);

            Contract.EndContractBlock();
        }

        [ContractArgumentValidator]
        public static void ArgumentInRange<T>(T arg, string paramName, T lo, T hi)
            where T : struct, IComparable<T>
        {
            if (lo.CompareTo(arg) > 0)
                throw new ArgumentOutOfRangeException(paramName);

            if (hi.CompareTo(arg) < 0)
                throw new ArgumentOutOfRangeException(paramName);
            
            Contract.EndContractBlock();
        }

        #endregion

        #region OperationValid

        public static void OperationValid(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }

        #endregion

        //#region Impl

        //private static void ArgumentNotNullImpl<T>(T arg, string paramName, string message)
        //    where T : class
        //{
        //    if (arg != null)
        //        return;

        //    if (message == null)
        //        throw new ArgumentNullException(paramName);

        //    throw new ArgumentNullException(paramName, message);
        //}

        //private static void ArgumentInRangeImpl<T>(T arg, string paramName, T lo, T hi, string message)
        //    where T : struct, IComparable<T>
        //{
        //    if (lo.CompareTo(arg) <= 0)
        //    {
        //        if (hi.CompareTo(arg) >= 0)
        //            return;
        //    }

        //    if (message == null)
        //        throw new ArgumentOutOfRangeException(paramName);

        //    throw new ArgumentOutOfRangeException(paramName, message);
        //}

        //#endregion
    }

}
