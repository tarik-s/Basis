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


    class Program
    {
        static int Main(string[] args)
        {




            Console.WriteLine("Press any key...");
            Console.ReadKey();

            Environment.ExitCode = 0;
            return 0;

        }
    }
}
