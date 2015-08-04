using System;
using System.Collections.Generic;
using System.Linq;
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



        static void Main(string[] args)
        {
            _serverVersion.Changed += delegate(object sender, EventArgs eventArgs)
            {
                
            };

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
