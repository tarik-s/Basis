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
        private readonly MetaType _returnType;
        private readonly ImmutableArray<MetaParameter> _parameters;

        internal MetaMethod(MetaMethodBuilder builder, MetaObjectBuilderContext context)
            : base(builder)
        {
            using (context.CreateScope(this))
            {
                _returnType = builder.ReturnType;
                _parameters = builder.BuildParameters(context);
            }
        }

        public MetaType ReturnType
        {
            get { return _returnType; }
        }

        public ImmutableArray<MetaParameter> Parameters
        {
            get { return _parameters; }
        }


        public override void Accept<TContext>(IMetaObjectVisitor<TContext> metaObjectVisitor, TContext context)
        {
            metaObjectVisitor.VisitMethod(this, context);
        }
    }

    public class MetaMethodBuilder : MetaObjectBuilder<MetaMethod>
    {
        private MetaType _returnType;
        private readonly List<MetaParameterBuilder> _parameters;

        public MetaMethodBuilder(string name)
            : base(name)
        {
            _returnType = MetaType.Void;
            _parameters = new List<MetaParameterBuilder>();
        }


        #region Buildings

        public MetaType ReturnType
        {
            get { return _returnType; }
            set { _returnType = value; }
        }

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
