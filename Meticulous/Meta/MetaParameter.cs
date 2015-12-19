using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public class MetaParameter : MetaObject
    {
        internal MetaParameter(MetaParameterBuilder builder, MetaObjectBuilderContext context)
            : base(MetaType.Parameter, builder.Name)
        {
        }

        public override void Accept<TContext>(MetaObjectVisitor<TContext> metaObjectVisitor, TContext context)
        {
            metaObjectVisitor.VisitParameter(this, context);
        }
    }

    public class MetaParameterBuilder : MetaObjectBuilder<MetaParameter>
    {
        protected MetaParameterBuilder(string name)
            : base(name)
        {

        }

        internal override MetaParameter Build(MetaObjectBuilderContext context)
        {
            return new MetaParameter(this, context);
        }
    }
}
