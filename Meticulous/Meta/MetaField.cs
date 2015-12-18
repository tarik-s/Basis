using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public class MetaField : MetaObject
    {
        internal MetaField(MetaFieldBuilder builder, MetaObjectBuilderContext context)
            : base(builder, context)
        {

        }

        public override void Accept<TContext>(MetaTypeVisitor<TContext> metaTypeVisitor, TContext context)
        {
            metaTypeVisitor.VisitField(this, context);
        }
    }

    public class MetaFieldBuilder : MetaObjectBuilder
    {
        public MetaFieldBuilder(string name)
            : base(name)
        {

        }
    }
}
