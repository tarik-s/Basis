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
    /// <summary>
    /// Represents directory contents info
    /// </summary>
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

        /// <summary>
        /// Gets the default size of the page.
        /// </summary>
        public static int DefaultPageSize
        {
            get { return 4 * 1024; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DirectoryContentsInfo"/> class.
        /// </summary>
        /// <param name="directoryInfo">The directory info.</param>
        public static DirectoryContentsInfo Create(DirectoryInfo directoryInfo)
        {
            Check.ArgumentNotNull(directoryInfo, "directoryInfo");

            return CreateImpl(directoryInfo, CancellationToken.None, ExceptionHandler.NeverHandling);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DirectoryContentsInfo"/> class.
        /// </summary>
        /// <param name="directoryInfo">The directory info.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <returns></returns>
        public static DirectoryContentsInfo Create(DirectoryInfo directoryInfo, ExceptionHandler exceptionHandler)
        {
            Check.ArgumentNotNull(directoryInfo, "directoryInfo");
            Check.ArgumentNotNull(exceptionHandler, "exceptionHandler");

            return CreateImpl(directoryInfo, CancellationToken.None, exceptionHandler);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DirectoryContentsInfo"/> class.
        /// </summary>
        /// <param name="directoryInfo">The directory info.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public static DirectoryContentsInfo Create(DirectoryInfo directoryInfo, CancellationToken cancellationToken)
        {
            Check.ArgumentNotNull(directoryInfo, "directoryInfo");

            return CreateImpl(directoryInfo, cancellationToken, ExceptionHandler.NeverHandling);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DirectoryContentsInfo"/> class.
        /// </summary>
        /// <param name="directoryInfo">The directory info.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the file count.
        /// </summary>
        public int FileCount
        {
            get { return _fileCount; }
        }

        /// <summary>
        /// Gets the directory count.
        /// </summary>
        public int DirectoryCount
        {
            get { return _directoryCount; }
        }

        /// <summary>
        /// Gets the symlink count.
        /// </summary>
        public int SymlinkCount
        {
            get { return _symlinkCount; }
        }

        /// <summary>
        /// Gets the total filesystem entry count.
        /// </summary>
        public int TotalCount
        {
            get { return _totalCount; }
        }

        #endregion

        #region Size

        /// <summary>
        /// Gets the size of the file.
        /// </summary>
        public long FileSize
        {
            get { return _fileSize; }
        }


        /// <summary>
        /// Gets the size of the directory.
        /// </summary>
        public long DirectorySize
        {
            get { return _directorySize; }
        }

        /// <summary>
        /// Gets the size of the symlink.
        /// </summary>
        public long SymlinkSize
        {
            get { return _symlinkSize; }
        }

        /// <summary>
        /// Gets the total size of the file system entries.
        /// </summary>
        public long TotalSize
        {
            get { return _totalSize; }
        }

        #endregion

        #region Size on Disk

        /// <summary>
        /// Gets the file size on disk.
        /// </summary>
        public long FileSizeOnDisk
        {
            get { return _fileSizeOnDisk; }
        }

        /// <summary>
        /// Gets the directory size on disk.
        /// </summary>
        public long DirectorySizeOnDisk
        {
            get { return _directorySizeOnDisk; }
        }

        /// <summary>
        /// Gets the symlink size on disk.
        /// </summary>
        public long SymlinkSizeOnDisk
        {
            get { return _symlinkSizeOnDisk; }
        }

        /// <summary>
        /// Gets the total size of filesystem entries on disk.
        /// </summary>
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


    /// <summary>
    /// DirectoryContentsInfo Builder
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryContentsInfoBuilder"/> class.
        /// </summary>
        public DirectoryContentsInfoBuilder()
        {
            _pageSize = DirectoryContentsInfo.DefaultPageSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryContentsInfoBuilder"/> class.
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        public DirectoryContentsInfoBuilder(int pageSize)
        {
            Check.ArgumentInRange(pageSize, "pageSize", 1, int.MaxValue);

            _pageSize = pageSize;
        }

        /// <summary>
        /// Builds a new instance of the <see cref="DirectoryContentsInfo"/> class
        /// </summary>
        /// <returns></returns>
        public DirectoryContentsInfo Build()
        {
            return new DirectoryContentsInfo(this);
        }

        #region Properties

        /// <summary>
        /// Gets the file count.
        /// </summary>
        public int FileCount
        {
            get { return _fileCount; }
        }

        /// <summary>
        /// Gets the directory count.
        /// </summary>
        public int DirectoryCount
        {
            get { return _directoryCount; }
        }

        /// <summary>
        /// Gets the symlink count.
        /// </summary>
        public int SymlinkCount
        {
            get { return _symlinkCount; }
        }

        /// <summary>
        /// Gets the size of the files.
        /// </summary>
        public long FileSize
        {
            get { return _fileSize; }
        }

        /// <summary>
        /// Gets the size of the directories.
        /// </summary>
        public long DirectorySize
        {
            get { return _directorySize; }
        }

        /// <summary>
        /// Gets the size of the symlinks.
        /// </summary>
        public long SymlinkSize
        {
            get { return _symlinkSize; }
        }

        /// <summary>
        /// Gets the file size on disk.
        /// </summary>
        public long FileSizeOnDisk
        {
            get { return _fileSizeOnDisk; }
        }

        /// <summary>
        /// Gets the directory size on disk.
        /// </summary>
        public long DirectorySizeOnDisk
        {
            get { return _directorySizeOnDisk; }
        }

        /// <summary>
        /// Gets the symlink size on disk.
        /// </summary>
        public long SymlinkSizeOnDisk
        {
            get { return _symlinkSizeOnDisk; }
        }

        #endregion

        #region Operations

        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="fileSize">Size of the file.</param>
        public void AddFile(long fileSize)
        {
            Check.ArgumentInRange(fileSize, "fileSize", 0, long.MaxValue);

            ++_fileCount;
            _fileSize += fileSize;
            _fileSizeOnDisk += AlignSize(fileSize);
        }

        /// <summary>
        /// Adds the symlink.
        /// </summary>
        /// <param name="symlinkSize">Size of the symlink.</param>
        public void AddSymlink(long symlinkSize)
        {
            Check.ArgumentInRange(symlinkSize, "symlinkSize", 0, long.MaxValue);

            ++_symlinkCount;
            _symlinkSize += symlinkSize;
            _symlinkSizeOnDisk += AlignSize(symlinkSize);
        }

        /// <summary>
        /// Adds the directory.
        /// </summary>
        public void AddDirectory()
        {
            AddDirectory(0);
        }

        /// <summary>
        /// Adds the directory.
        /// </summary>
        /// <param name="directorySize">Size of the directory.</param>
        public void AddDirectory(long directorySize)
        {
            Check.ArgumentInRange(directorySize, "directorySize", 0, long.MaxValue);

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

        #endregion
    }
}
