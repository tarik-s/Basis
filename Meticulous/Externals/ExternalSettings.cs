using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Externals
{
    public class ExternalSettings
    {
        private readonly string _rawSettings;
        public ExternalSettings(string rawSettings)
        {
            _rawSettings = rawSettings ?? String.Empty;
        }

        public string RawSettings
        {
            get { return _rawSettings; }
        }
    }
}
