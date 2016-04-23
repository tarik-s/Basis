using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public interface IMetaTypeVisitor<in TContext>
    {
        void VisitModule(MetaModule module, TContext context);
        void VisitClass(MetaClass @class, TContext context);
        void VisitInterface(MetaInterface @interface, TContext context);
        void VisitFunction(MetaFunction function, TContext context);
        void VisitParameter(MetaParameter parameter, TContext context);
        void VisitField(MetaField field, TContext context);
        void VisitPlainType(PlainMetaType type, TContext context);
    }

    public interface IVisitableMetaType
    {
        void Accept<TContext>(IMetaTypeVisitor<TContext> visitor, TContext context);
    }
}
