using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Diagnostics.Contracts;
using Meticulous.Collections.Generic;
using Meticulous.Threading;
using Meticulous.IO;

namespace Meticulous.SandBox
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            var result = RunLoop.RunMain(MainImpl);
            //var q = ExecutionQueue.Create(ExecutionQueueProcessorType.ThreadPool);

            Console.WriteLine("Finshed with code: " + result);
            Console.WriteLine("Press any key...");
            Console.ReadKey();

            Environment.ExitCode = result;
            return result;

        }

        private static async Task TestAsync()
        {
            Console.WriteLine("#1: " + Thread.CurrentThread.ManagedThreadId);
            await Task.Delay(1000);
            Console.WriteLine("#2: " + Thread.CurrentThread.ManagedThreadId);
            RunLoop.MainLoop.Stop(123);
            await Task.Delay(1000);
            Console.WriteLine("#3: " + Thread.CurrentThread.ManagedThreadId);
        }


        private static void MainImpl()
        {
            TestAsync().Wait();
        }
    }
}
