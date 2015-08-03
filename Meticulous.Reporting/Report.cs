using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Reporting
{
    [Serializable]
    public enum ReportLevel
    {
        Trace,
        Log,
        Info,
        Warn,
        Error,
        Fatal
    }

    public static class Report
    {
        private static Reporter _reporter = Reporter.Null;

        public static void ExceptionCaught(Exception exception)
        {
            _reporter.ExceptionCaught(exception, string.Empty);
        }

        public static void ExceptionCaught(Exception exception, string message)
        {
            _reporter.ExceptionCaught(exception, message);
        }



    }
}
