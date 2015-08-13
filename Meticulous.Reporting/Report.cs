using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Reporting
{
    /// <summary>
    /// Report level
    /// </summary>
    [Serializable]
    public enum ReportLevel
    {
        /// <summary>
        /// Trace level
        /// </summary>
        Trace,

        /// <summary>
        /// Log level
        /// </summary>
        Log,

        /// <summary>
        /// Info level
        /// </summary>
        Info,

        /// <summary>
        /// Warn level
        /// </summary>
        Warn,

        /// <summary>
        /// Error level
        /// </summary>
        Error,

        /// <summary>
        /// Fatal level
        /// </summary>
        Fatal
    }

    /// <summary>
    /// Represents a static class for reporting
    /// </summary>
    public static class Report
    {
        private static Reporter _reporter = Reporter.Null;

        /// <summary>
        /// Handles the caught exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public static void ExceptionCaught(Exception exception)
        {
            _reporter.ExceptionCaught(exception, string.Empty);
        }

        /// <summary>
        /// Handles the caught exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        public static void ExceptionCaught(Exception exception, string message)
        {
            _reporter.ExceptionCaught(exception, message);
        }



    }
}
