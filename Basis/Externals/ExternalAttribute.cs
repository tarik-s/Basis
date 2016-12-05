using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Externals
{
    /// <summary>
    /// External memer attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ExternalAttribute : Attribute
    {
        private readonly string _path;
        private readonly string _rawSettings;

        public ExternalAttribute(string path, string rawSettings)
        {
            Check.ArgumentNotEmpty(path, "path");

            _path = path;
            _rawSettings = rawSettings ?? String.Empty;
        }

        public ExternalAttribute(string path)
            : this(path, null)
        {
        }

        public string Path
        {
            get { return _path; }
        }

        public string RawSettings
        {
            get { return _rawSettings; }
        }

    }
}
