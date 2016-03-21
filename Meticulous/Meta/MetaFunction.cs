using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public class MetaFunction : MetaObject
    {
        private readonly MetaParameter _returnParameter;
        private readonly ImmutableArray<MetaParameter> _parameters;

        internal MetaFunction(MetaFunctionBuilder builder, MetaObjectBuilderContext context)
            : base(builder, context.Module)
        {
            using (context.CreateScope(this))
            {
                _returnParameter = builder.BuildReturnParameter(context);
                _parameters = builder.BuildParameters(context);
            }
        }

        public MetaType ReturnType
        {
            get { return _returnParameter.Type; }
        }

        public ImmutableArray<MetaParameter> Parameters
        {
            get { return _parameters; }
        }
        
        public override void Accept<TContext>(IMetaTypeVisitor<TContext> metaTypeVisitor, TContext context)
        {
            metaTypeVisitor.VisitFunction(this, context);
        }

        internal override void ResolveDeferredMembers(MetaObjectBuilderContext context)
        {
            _returnParameter.ResolveDeferredMembers(context);
            ResolveDeferredMembers(_parameters, context);
        }
    }

    public class MetaFunctionBuilder : MetaObjectBuilder<MetaFunction>
    {
        private readonly MetaParameterBuilder _returnParameter;
        private readonly List<MetaParameterBuilder> _parameters;

        public MetaFunctionBuilder(string name)
            : base(name)
        {
            _returnParameter = MetaParameterBuilder.CreateReturnParameterBuilder();
            _parameters = new List<MetaParameterBuilder>();
        }


        #region Buildings

        internal override MetaFunction Build(MetaObjectBuilderContext context)
        {
            return new MetaFunction(this, context);
        }

        internal ImmutableArray<MetaParameter> BuildParameters(MetaObjectBuilderContext context)
        {
            return _parameters.Select(pb => pb.Build(context)).ToImmutableArray();
        }

        internal MetaParameter BuildReturnParameter(MetaObjectBuilderContext context)
        {
            return _returnParameter.Build(context);
        }

        #endregion

    }
}
