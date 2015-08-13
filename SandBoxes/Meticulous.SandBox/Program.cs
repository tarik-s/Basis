using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Diagnostics.Contracts;

using Meticulous.Threading;
using Meticulous.IO;

namespace Meticulous.SandBox
{

    public sealed class Remote<T>
    {
        private readonly Atomic<TaskCompletionSource<T>> _tcs;

        private T _value;
        

        public Remote(T value)
        {
            _value = value;
            _tcs = new Atomic<TaskCompletionSource<T>>(new TaskCompletionSource<T>());
        }

        public static implicit operator Remote<T>(T value)
        {
            return new Remote<T>(value);
        }

        public static implicit operator T(Remote<T> value)
        {
            if (value == null)
                return default(T);

            return value.Value;
        }

        public event EventHandler<EventArgs> Changed;

        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                var tcs = _tcs.Exchange(new TaskCompletionSource<T>());
                tcs.SetResult(value);
                EventArgs.Empty.Raise(this, ref Changed);
            }
        }


        public void Stop()
        {
            _tcs.Value.SetCanceled();
        }

        public async Task<T> GetNewValueAsync()
        {
            var task = _tcs.Value.Task;
            return await task.ConfigureAwait(false);
        }

        public override string ToString()
        {
            if (_value == null)
                return null;

            return _value.ToString();
        }
    }

    public class RemoteFieldAttribute : Attribute
    {
        public RemoteFieldAttribute(string name)
        {
        }
    }

    class Program
    {
        [RemoteField("ServerVersion")]
        private static readonly Remote<string> _serverVersion = default(string);

        private enum Test
        {
            Val1,
            Val2
        }

        private class A
        {
        }

        static int Main(string[] args)
        {
            Contract.ContractFailed += delegate {
                
            };

            var di = new DirectoryInfo(@"D:\");

            var dci = di.GetContentsInfo(ExceptionHandler.Create(e =>
            {
                Console.WriteLine(e.Message);
                return true;
            }));

            var val2 = new Atomic<int>(10);
            Console.WriteLine(val2.TrySet(10));
            Console.WriteLine(val2.TrySet(1));
            Console.WriteLine();

            var val3 = new Atomic<string>("hello");
            Console.WriteLine(val3.TrySet("hello"));
            Console.WriteLine(val3.TrySet(null));
            Console.WriteLine(val3.TrySet("hello"));
            Console.WriteLine();

            var val4 = new Atomic<Test>(Test.Val1);
            Console.WriteLine(val4.TrySet(Test.Val1));
            Console.WriteLine(val4.TrySet(Test.Val2));
            Console.WriteLine(val4.TrySet(Test.Val1));
            Console.WriteLine();

            var a1 = new A();
            var a2 = new A();
            var val5 = new Atomic<A>(null);
            Console.WriteLine(val5.TrySet(null));
            Console.WriteLine(val5.TrySet(a1));
            Console.WriteLine(val5.TrySet(a1));
            Console.WriteLine(val5.TrySet(a2));

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                RunLoop.MainLoop.Stop(-1);
            };

            var version = 0;

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    ++version;
                    _serverVersion.Value = version.ToString();

                    if (version > 20)
                    {
                        _serverVersion.Stop();
                        break;
                    }
                }
            });

            var result = RunLoop.RunMain(() =>
            {
                while (true)
                {
                    var newVersion = _serverVersion.GetNewValueAsync().Result;
                    Console.WriteLine(newVersion);
                }
            });


            Console.WriteLine("Press any key...");
            Console.ReadKey();

            Environment.ExitCode = result;
            return result;

        }
    }
}
