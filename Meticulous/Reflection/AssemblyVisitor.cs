using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Reflection
{
    public class AssemblyVisitor<TContext>
    {
        public void VisitAssembly(Assembly assembly, TContext context)
        {
            var types = assembly.GetTypes();
        }

        public void VisitType(Type type, TContext context)
        {
            
        }


    }
}
