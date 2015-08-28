using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Externals
{
    /// <summary>
    /// 
    /// </summary>
    public class ExternalManager
    {
        //private readonly ExternalDriver[] _drivers;

        /// <summary>
        /// 
        /// </summary>
        public ExternalManager()
        {
            AppDomain.CurrentDomain.AssemblyLoad += HandleCurrentDomainAssemblyLoad;
        }

        private void HandleCurrentDomainAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            
        }
    }
}
