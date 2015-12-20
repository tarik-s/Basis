using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public abstract class MetaObjectVisitor<TContext>
    {
        public abstract void VisitClass(MetaClass @class, TContext context);
        public abstract void VisitModule(MetaModule module, TContext context);
        public abstract void VisitMethod(MetaMethod method, TContext context);
        public abstract void VisitParameter(MetaParameter parameter, TContext context);
        public abstract void VisitField(MetaField field, TContext context);
    }

    public interface IMetaObjectVisitable
    {
        void Accept<TContext>(MetaObjectVisitor<TContext> metaObjectVisitor, TContext context);
    }
}
