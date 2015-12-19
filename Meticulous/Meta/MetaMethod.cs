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
            : base(MetaType.Method, builder.Name)
        {
            using (context.CreateScope(this))
            {
                
            }
        }

        public override void Accept<TContext>(MetaObjectVisitor<TContext> metaObjectVisitor, TContext context)
        {
            metaObjectVisitor.VisitMethod(this, context);
        }
    }

    public class MetaMethodBuilder : MetaObjectBuilder<MetaMethod>
    {
        private readonly List<MetaParameterBuilder> _parameters;

        protected MetaMethodBuilder(string name)
            : base(name)
        {
            _parameters = new List<MetaParameterBuilder>();
        }


        internal override MetaMethod Build(MetaObjectBuilderContext context)
        {
            return new MetaMethod(this, context);
        }
    }
}
