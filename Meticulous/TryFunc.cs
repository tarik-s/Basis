using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous
{
    /// <summary>
    /// Encapsulates a try method
    /// </summary>
    /// <typeparam name="TOutResult">The result type</typeparam>
    /// <param name="result">The result</param>
    /// <returns>Returns true if the function finished successfully otherwise false</returns>
    public delegate bool TryFunc<TOutResult>(out TOutResult result);

    /// <summary>
    /// Encapsulates a try method
    /// </summary>
    /// <typeparam name="T">The first parameter type</typeparam>
    /// <typeparam name="TOutResult">The result type</typeparam>
    /// <param name="arg">The first parameter</param>
    /// <param name="result">The result</param>
    /// <returns>Returns true if the function finished successfully otherwise false</returns>
    public delegate bool TryFunc<in T, TOutResult>(T arg, out TOutResult result);

    /// <summary>
    /// Encapsulates a try method
    /// </summary>
    /// <typeparam name="T1">The first parameter</typeparam>
    /// <typeparam name="T2">The second parameter</typeparam>
    /// <typeparam name="TOutResult">The result type</typeparam>
    /// <param name="arg1">The first parameter</param>
    /// <param name="arg2">The second parameter</param>
    /// <param name="result">The result</param>
    /// <returns>Returns true if the function finished successfully otherwise false</returns>
    public delegate bool TryFunc<in T1, in T2, TOutResult>(T1 arg1, T2 arg2, out TOutResult result);

    /// <summary>
    /// Encapsulates a try method
    /// </summary>
    /// <typeparam name="T1">The first parameter type</typeparam>
    /// <typeparam name="T2">The second parameter type</typeparam>
    /// <typeparam name="T3">The third parameter type</typeparam>
    /// <typeparam name="TOutResult">The result type</typeparam>    
    /// <param name="arg1">The first parameter type</param>
    /// <param name="arg2">The second parameter type</param>
    /// <param name="arg3">The third parameter type</param>
    /// <param name="result">The result</param>
    /// <returns>Returns true if the function finished successfully otherwise false</returns>
    public delegate bool TryFunc<in T1, in T2, in T3, TOutResult>(T1 arg1, T2 arg2, T3 arg3, out TOutResult result);
}

