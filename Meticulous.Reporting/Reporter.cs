using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Reporting
{
    /// <summary>
    /// Represents a reporter class
    /// </summary>
    public abstract class Reporter
    {
        private static readonly NullReporter s_nullReporter = new NullReporter();

        /// <summary>
        /// Gets the empty reporter.
        /// </summary>
        public static Reporter Null
        {
            get { return s_nullReporter; }
        }

        /// <summary>
        /// Handles the caught exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        public void ExceptionCaught(Exception exception, string message)
        {
        }

        private sealed class NullReporter : Reporter
        {
        }
    }
}
