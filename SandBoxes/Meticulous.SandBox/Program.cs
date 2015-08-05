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



        static int Main(string[] args)
        {
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
