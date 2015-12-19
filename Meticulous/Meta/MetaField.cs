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
            : base(MetaType.Field, builder, context)
        {

            context.Remove(this);
        }

        public override void Accept<TContext>(MetaObjectVisitor<TContext> metaObjectVisitor, TContext context)
        {
            metaObjectVisitor.VisitField(this, context);
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
