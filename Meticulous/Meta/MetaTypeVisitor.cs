using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public abstract class MetaTypeVisitor<TContext>
    {
        public abstract void VisitClass(MetaClass metaClass, TContext context);
        public abstract void VisitModule(MetaModule metaModule, TContext context);
        public abstract void VisitMethod(MetaMethod metaMethod, TContext context);
        public abstract void VisitField(MetaField metaMethod, TContext context);
    }

    public interface IMetaTypeVisitable
    {
        void Accept<TContext>(MetaTypeVisitor<TContext> metaTypeVisitor, TContext context);
    }
}
