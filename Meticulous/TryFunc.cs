using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous
{
    public delegate bool TryFunc<TOutResult>(out TOutResult result);

    public delegate bool TryFunc<in T, TOutResult>(T arg, out TOutResult result);

    public delegate bool TryFunc<in T1, in T2, TOutResult>(T1 arg1, T2 arg2, out TOutResult result);

    public delegate bool TryFunc<in T1, in T2, in T3, TOutResult>(T1 arg1, T2 arg2, T3 arg3, out TOutResult result);

}
