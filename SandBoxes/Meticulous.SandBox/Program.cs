using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticulous.Threading;

namespace Meticulous.SandBox
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var q = ExecutionQueue.Create())
            {
                q.Post(() =>
                {
                    q.Stop();
                });

                q.Wait();
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
