using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public class MetaMethod : MetaObject
    {
        internal MetaMethod(MetaMethodBuilder builder, MetaObjectBuilderContext context)
            : base(MetaType.Method, builder, context)
        {

            context.Remove(this);
        }

        public override void Accept<TContext>(MetaObjectVisitor<TContext> metaObjectVisitor, TContext context)
        {
            metaObjectVisitor.VisitMethod(this, context);
        }
    }

    public class MetaMethodBuilder : MetaObjectBuilder
    {
        protected MetaMethodBuilder(string name)
            : base(name)
        {

        }
    }
}
