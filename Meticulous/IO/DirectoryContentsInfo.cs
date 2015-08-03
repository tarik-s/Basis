using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Meticulous.Patterns;

namespace Meticulous.IO
{
    public sealed class DirectoryContentsInfo
    {
        #region Fields

        private readonly int _fileCount;
        private readonly int _directoryCount;
        private readonly int _symlinkCount;
        private readonly int _totalCount;

        private readonly long _fileSize;
        private readonly long _directorySize;
        private readonly long _symlinkSize;
        private readonly long _totalSize;

        private readonly long _fileSizeOnDisk;
        private readonly long _directorySizeOnDisk;
        private readonly long _symlinkSizeOnDisk;
        private readonly long _totalSizeOnDisk;

        #endregion
        
        public static int DefaultPageSize
        {
            get { return 4 * 1024; }
        }

        public static DirectoryContentsInfo Create(DirectoryInfo directoryInfo)
        {
            Check.ArgumentNotNull(directoryInfo, "directoryInfo");

            return CreateImpl(directoryInfo, CancellationToken.None, ExceptionHandler.NeverHandling);
        }

        public static DirectoryContentsInfo Create(DirectoryInfo directoryInfo, ExceptionHandler exceptionHandler)
        {
            Check.ArgumentNotNull(directoryInfo, "directoryInfo");
            Check.ArgumentNotNull(exceptionHandler, "exceptionHandler");

            return CreateImpl(directoryInfo, CancellationToken.None, exceptionHandler);
        }

        public static DirectoryContentsInfo Create(DirectoryInfo directoryInfo, CancellationToken cancellationToken)
        {
            Check.ArgumentNotNull(directoryInfo, "directoryInfo");

            return CreateImpl(directoryInfo, cancellationToken, ExceptionHandler.NeverHandling);
        }

        public static DirectoryContentsInfo Create(DirectoryInfo directoryInfo, CancellationToken cancellationToken, 
            ExceptionHandler exceptionHandler)
        {
            Check.ArgumentNotNull(directoryInfo, "directoryInfo");
            Check.ArgumentNotNull(exceptionHandler, "exceptionHandler");

            return CreateImpl(directoryInfo, cancellationToken, exceptionHandler);
        }

        internal DirectoryContentsInfo(DirectoryContentsInfoBuilder builder)
        {
            _fileCount = builder.FileCount;
            _directoryCount = builder.DirectoryCount;
            _symlinkCount = builder.SymlinkCount;

            _totalCount = _fileCount + _directoryCount + _symlinkCount;

            _fileSize = builder.FileSize;
            _directorySize = builder.DirectorySize;
            _symlinkSize = builder.SymlinkSize;

            _totalSize = _fileSize + _directorySize + _symlinkSize;

            _fileSizeOnDisk = builder.FileSize;
            _directorySizeOnDisk = builder.DirectorySize;
            _symlinkSizeOnDisk = builder.SymlinkSize;

            _totalSizeOnDisk = _fileSizeOnDisk + _directorySizeOnDisk + _symlinkSizeOnDisk;
        }

        #region Count

        public int FileCount
        {
            get { return _fileCount; }
        }
        public int DirectoryCount
        {
            get { return _directoryCount; }
        }
        public int SymlinkCount
        {
            get { return _symlinkCount; }
        }
        public int TotalCount
        {
            get { return _totalCount; }
        }

        #endregion

        #region Size

        public long FileSize
        {
            get { return _fileSize; }
        }
        public long DirectorySize
        {
            get { return _directorySize; }
        }
        public long SymlinkSize
        {
            get { return _symlinkSize; }
        }
        public long TotalSize
        {
            get { return _totalSize; }
        }

        #endregion

        #region Size on Disk

        public long FileSizeOnDisk
        {
            get { return _fileSizeOnDisk; }
        }
        public long DirectorySizeOnDisk
        {
            get { return _directorySizeOnDisk; }
        }
        public long SymlinkSizeOnDisk
        {
            get { return _symlinkSizeOnDisk; }
        }
        public long TotalSizeOnDisk
        {
            get { return _totalSizeOnDisk; }
        }

        #endregion


        private static DirectoryContentsInfo CreateImpl(DirectoryInfo directoryInfo, CancellationToken cancellationToken,
            ExceptionHandler exceptionHandler)
        {
            var builder = new DirectoryContentsInfoBuilder();

            var exceptionThrown = false;
            try
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    foreach (var fileSystemInfo in directoryInfo.EnumerateFileSystemInfos())
                    {
                        ProcessFileSystemEntry(fileSystemInfo, builder, cancellationToken, exceptionHandler, ref exceptionThrown);

                        if (cancellationToken.IsCancellationRequested)
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                if (exceptionThrown)
                    throw;

                if (!exceptionHandler.HandleException(e))
                {
                    exceptionThrown = true;
                    throw;
                }
            }

            return builder.Build();
        }

        private static void ProcessFileSystemEntry(FileSystemInfo fileSystemInfo, DirectoryContentsInfoBuilder builder,
            CancellationToken cancellationToken, ExceptionHandler exceptionHandler, ref bool exceptionThrown)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var fi = fileSystemInfo as FileInfo;
                if (fi != null)
                {
                    var size = fi.Length;
                    builder.AddFile(size);
                }
                else
                {
                    var di = fileSystemInfo as DirectoryInfo;
                    if (di != null)
                    {
                        builder.AddDirectory();
                        foreach (var info in di.EnumerateFileSystemInfos())
                        {
                            ProcessFileSystemEntry(info, builder, cancellationToken, exceptionHandler, ref exceptionThrown);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (exceptionThrown)
                    throw;

                if (!exceptionHandler.HandleException(e))
                {
                    exceptionThrown = true;
                    throw;
                }
            }
        }

    }


    public sealed class DirectoryContentsInfoBuilder : IBuilder<DirectoryContentsInfo>
    {
        #region Fields

        private int _fileCount;
        private int _directoryCount;
        private int _symlinkCount;

        private long _fileSize;
        private long _directorySize;
        private long _symlinkSize;

        private long _fileSizeOnDisk;
        private long _directorySizeOnDisk;
        private long _symlinkSizeOnDisk;

        private readonly int _pageSize;

        #endregion

        public DirectoryContentsInfoBuilder()
            : this(DirectoryContentsInfo.DefaultPageSize)
        {
        }

        public DirectoryContentsInfoBuilder(int pageSize)
        {
            Check.ArgumentInRange(pageSize, "pageSize", 1, int.MaxValue);

            _pageSize = pageSize;
        }

        public DirectoryContentsInfo Build()
        {
            return new DirectoryContentsInfo(this);
        }

        #region Properties

        public int FileCount
        {
            get { return _fileCount; }
        }
        public int DirectoryCount
        {
            get { return _directoryCount; }
        }
        public int SymlinkCount
        {
            get { return _symlinkCount; }
        }

        public long FileSize
        {
            get { return _fileSize; }
        }
        public long DirectorySize
        {
            get { return _directorySize; }
        }
        public long SymlinkSize
        {
            get { return _symlinkSize; }
        }

        public long FileSizeOnDisk
        {
            get { return _fileSizeOnDisk; }
        }
        public long DirectorySizeOnDisk
        {
            get { return _directorySizeOnDisk; }
        }
        public long SymlinkSizeOnDisk
        {
            get { return _symlinkSizeOnDisk; }
        }

        #endregion

        #region Operations

        public void AddFile(long fileSize)
        {
            CheckSize(fileSize, "fileSize");

            ++_fileCount;
            _fileSize += fileSize;
            _fileSizeOnDisk += AlignSize(fileSize);
        }

        public void AddSymlink(long symlinkSize)
        {
            CheckSize(symlinkSize, "symlinkSize");

            ++_symlinkCount;
            _symlinkSize += symlinkSize;
            _symlinkSizeOnDisk += AlignSize(symlinkSize);
        }

        public void AddDirectory()
        {
            AddDirectory(0);
        }

        public void AddDirectory(long directorySize)
        {
            CheckSize(directorySize, "directorySize");

            ++_directoryCount;

            if (directorySize == 0)
                return;

            _directorySize += directorySize;
            _directorySizeOnDisk += AlignSize(directorySize);
        }

        private long AlignSize(long size)
        {
            var mod = size % _pageSize;
            if (mod == 0)
                return size;

            return (size / _pageSize + 1) * _pageSize;
        }

        private static void CheckSize(long size, string paramName)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException(paramName, size, "Size must not be less then zero");
        }

        #endregion
    }
}
