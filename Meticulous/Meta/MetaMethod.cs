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
            : base(builder, context)
        {

        }

        public override void Accept<TContext>(MetaTypeVisitor<TContext> metaTypeVisitor, TContext context)
        {
            metaTypeVisitor.VisitMethod(this, context);
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
