using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meticulous.IO
{
    /// <summary>
    /// IO extension methods
    /// </summary>
    public static class ExtensionMethods
    {
        #region GetContentsInfo

        /// <summary>
        /// Gets the contents information.
        /// </summary>
        /// <param name="this">The this.</param>
        /// <returns></returns>
        public static DirectoryContentsInfo GetContentsInfo(this DirectoryInfo @this)
        {
            Check.This(@this);

            return DirectoryContentsInfo.Create(@this, CancellationToken.None, ExceptionHandler.NeverHandling);
        }

        /// <summary>
        /// Gets the contents information.
        /// </summary>
        /// <param name="this">The this.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static DirectoryContentsInfo GetContentsInfo(this DirectoryInfo @this, CancellationToken cancellationToken)
        {
            Check.This(@this);

            return DirectoryContentsInfo.Create(@this, cancellationToken, ExceptionHandler.NeverHandling);
        }

        /// <summary>
        /// Gets the contents information.
        /// </summary>
        /// <param name="this">The this.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <returns></returns>
        public static DirectoryContentsInfo GetContentsInfo(this DirectoryInfo @this, ExceptionHandler exceptionHandler)
        {
            Check.This(@this);

            return DirectoryContentsInfo.Create(@this, exceptionHandler);
        }

        /// <summary>
        /// Gets the contents information.
        /// </summary>
        /// <param name="this">The this.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <returns></returns>
        public static DirectoryContentsInfo GetContentsInfo(this DirectoryInfo @this, CancellationToken cancellationToken, ExceptionHandler exceptionHandler)
        {
            Check.This(@this);

            return DirectoryContentsInfo.Create(@this, cancellationToken, exceptionHandler);
        }

        #endregion GetContentsInfo
    }
}
