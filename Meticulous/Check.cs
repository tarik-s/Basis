using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous
{
    /// <summary>
    /// Represents a contract preconditions checker
    /// </summary>
    public static class Check
    {
        #region ArgumentNotNull

        /// <summary>
        /// Checks the first argument "this" of an extension method is not null
        /// </summary>
        /// <param name="this">The "this" parameter</param>
        /// <typeparam name="T">The "this" parameter type</typeparam>
        /// <exception cref="ArgumentNullException">Throws if the "this" argument is null</exception>
        [ContractArgumentValidator]
        public static void This<T>(T @this)
            where T : class
        {
            if (@this == null)
                throw new ArgumentNullException("this", "The first argument of extension method is null");

            Contract.EndContractBlock();
        }

        /// <summary>
        /// Checks the first parameter of a method is not null
        /// </summary>
        /// <param name="arg">The check parameter</param>
        /// <param name="paramName">The parameter name</param>
        /// <typeparam name="T">The parameter type</typeparam>
        /// <exception cref="ArgumentNullException">Throws if the argument is null</exception>
        [ContractArgumentValidator]
        public static void ArgumentNotNull<T>(T arg, string paramName)
            where T : class
        {
            if (arg == null)
                throw new ArgumentNullException(paramName);

            Contract.EndContractBlock();
        }

        /// <summary>
        /// Checks the first parameter of a method is not null
        /// </summary>
        /// <param name="arg">The parameter</param>
        /// <param name="paramName">The parameter name</param>
        /// <param name="message">The message of exception</param>
        /// <typeparam name="T">The parameter type</typeparam>
        /// <exception cref="ArgumentNullException">Throws if the argument is null</exception>
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

        /// <summary>
        /// Check the parameter value is fit in range
        /// </summary>
        /// <typeparam name="T">The parameter type</typeparam>
        /// <param name="arg">The parameter</param>
        /// <param name="paramName">The parameter name</param>
        /// <param name="lo">The lower bound</param>
        /// <param name="hi">The higher bound</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws if the argument is out of range</exception>
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

        /// <summary>
        /// Theck the parameter value is fit in range
        /// </summary>
        /// <typeparam name="T">The parameter type</typeparam>
        /// <param name="arg">The parameter</param>
        /// <param name="paramName">The parameter name</param>
        /// <param name="lo">The lower bound</param>
        /// <param name="hi">The higher bound</param>
        /// <param name="message">The message of exception</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws if the argument is out of range</exception>
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

        #endregion

        #region OperationValid

        /// <summary>
        /// Checked the condition is false otherwise throws <exception cref="InvalidOperationException"> exception</exception>
        /// </summary>
        /// <param name="condition">The condition to be checked</param>
        /// <param name="message">The exception message</param>
        public static void OperationValid(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }

        #endregion

    }

}
