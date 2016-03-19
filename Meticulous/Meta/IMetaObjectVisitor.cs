using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public interface IMetaObjectVisitor<in TContext>
    {
        void VisitModule(MetaModule module, TContext context);
        void VisitClass(MetaClass @class, TContext context);
        void VisitMethod(MetaMethod method, TContext context);
        void VisitParameter(MetaParameter parameter, TContext context);
        void VisitField(MetaField field, TContext context);
        void VisitType(MetaType type, TContext context);
    }

    public interface IVisitableMetaObject
    {
        void Accept<TContext>(IMetaObjectVisitor<TContext> visitor, TContext context);
    }
}
