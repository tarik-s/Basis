using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Externals
{
    [Flags]
    [Serializable]
    public enum ExternalPermission
    {
        None = 0,
        Read = 1,
        Write = 2,
        ReadWrite = Read | Write
    }

    public class ExternalSettings
    {
        private readonly string _rawSettings;

        public static ExternalSettings Empty  = new ExternalSettings(null);

        public ExternalSettings(string rawSettings)
        {
            _rawSettings = rawSettings ?? String.Empty;
        }

        public string RawSettings
        {
            get { return _rawSettings; }
        }

        public ExternalPermission Permissions
        {
            get { return ExternalPermission.Read; }
        }
    }
}
