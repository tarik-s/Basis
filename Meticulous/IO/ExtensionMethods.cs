using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meticulous.IO
{
    public static class ExtensionMethods
    {
        #region GetContentsInfo

        public static DirectoryContentsInfo GetContentsInfo(this DirectoryInfo @this)
        {
            Check.This(@this);

            return DirectoryContentsInfo.Create(@this, CancellationToken.None, ExceptionHandler.NeverHandling);
        }

        public static DirectoryContentsInfo GetContentsInfo(this DirectoryInfo @this, CancellationToken cancellationToken)
        {
            Check.This(@this);

            return DirectoryContentsInfo.Create(@this, cancellationToken, ExceptionHandler.NeverHandling);
        }

        public static DirectoryContentsInfo GetContentsInfo(this DirectoryInfo @this, ExceptionHandler exceptionHandler)
        {
            Check.This(@this);

            return DirectoryContentsInfo.Create(@this, exceptionHandler);
        }

        public static DirectoryContentsInfo GetContentsInfo(this DirectoryInfo @this, CancellationToken cancellationToken, ExceptionHandler exceptionHandler)
        {
            Check.This(@this);

            return DirectoryContentsInfo.Create(@this, cancellationToken, exceptionHandler);
        }

        #endregion GetContentsInfo
    }
}
