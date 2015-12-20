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
            : base(builder)
        {
            using (context.CreateScope(this))
            {
            }
        }

        public override void Accept<TContext>(MetaObjectVisitor<TContext> metaObjectVisitor, TContext context)
        {
            metaObjectVisitor.VisitField(this, context);
        }
    }

    public class MetaFieldBuilder : MetaObjectBuilder<MetaField>
    {
        public MetaFieldBuilder(string name)
            : base(MetaType.Field,  name)
        {

        }

        internal override MetaField Build(MetaObjectBuilderContext context)
        {
            return new MetaField(this, context);
        }
    }
}
