using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Meticulous.Threading;

namespace Meticulous.SandBox
{

    public sealed class Remote<T>
    {
        private T _value;

        public Remote(T value)
        {
            _value = value;
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
                EventArgs.Empty.Raise(this, ref Changed);
            }
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

            var infs = typeof (Test).GetInterfaces();

            var val = Atomic.Create(false);

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

            var result = RunLoop.Main(() =>
            {
                Console.WriteLine(_serverVersion);

                _serverVersion.Changed += delegate(object sender, EventArgs eventArgs)
                {
                    Console.WriteLine(_serverVersion);
                };


                var handler = new HttpClientHandler();
                var client = new HttpClient(handler);

                client.GetStringAsync("http://ibackuper.com/version.php").ContinueWith(t =>
                {
                    _serverVersion.Value = t.Result;
                });
            });




            

            //Console.ReadKey();
            Environment.ExitCode = result;
            return result;

            

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
