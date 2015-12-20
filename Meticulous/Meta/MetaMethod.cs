using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public class MetaMethod : MetaObject
    {
        private readonly ImmutableArray<MetaParameter> _parameters;

        internal MetaMethod(MetaMethodBuilder builder, MetaObjectBuilderContext context)
            : base(MetaType.Method, builder.Name)
        {
            using (context.CreateScope(this))
            {
                _parameters = builder.BuildParameters(context);
            }
        }

        public ImmutableArray<MetaParameter> Parameters
        {
            get { return _parameters; }
        }


        public override void Accept<TContext>(MetaObjectVisitor<TContext> metaObjectVisitor, TContext context)
        {
            metaObjectVisitor.VisitMethod(this, context);
        }
    }

    public class MetaMethodBuilder : MetaObjectBuilder<MetaMethod>
    {
        private readonly List<MetaParameterBuilder> _parameters;

        public MetaMethodBuilder(string name)
            : base(name)
        {
            _parameters = new List<MetaParameterBuilder>();
        }


        #region Buildings

        internal override MetaMethod Build(MetaObjectBuilderContext context)
        {
            return new MetaMethod(this, context);
        }

        internal ImmutableArray<MetaParameter> BuildParameters(MetaObjectBuilderContext context)
        {
            return _parameters.Select(pb => pb.Build(context)).ToImmutableArray();
        }

        #endregion

    }
}
