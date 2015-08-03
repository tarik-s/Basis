using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Reporting
{
    public abstract class Reporter
    {
        private static readonly NullReporter _nullReporter = new NullReporter();

        public static Reporter Null
        {
            get { return _nullReporter; }
        }

        public void ExceptionCaught(Exception exception, string message)
        {
        }



        private sealed class NullReporter : Reporter
        {
        }
    }
}
