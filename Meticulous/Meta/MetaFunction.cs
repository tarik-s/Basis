using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public class MetaFunction : MetaObject
    {
        private readonly MetaType _returnType;
        private readonly ImmutableArray<MetaParameter> _parameters;

        internal MetaFunction(MetaFunctionBuilder builder, MetaObjectBuilderContext context)
            : base(builder, context.Module)
        {
            _returnType = builder.ReturnType;

            using (context.CreateScope(this))
            {
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
        
        public override void Accept<TContext>(IMetaTypeVisitor<TContext> metaTypeVisitor, TContext context)
        {
            metaTypeVisitor.VisitMethod(this, context);
        }
    }

    public class MetaFunctionBuilder : MetaObjectBuilder<MetaFunction>
    {
        private MetaType _returnType;
        private readonly List<MetaParameterBuilder> _parameters;

        public MetaFunctionBuilder(string name)
            : base(name)
        {
            _returnType = MetaModule.Core.Void;
            _parameters = new List<MetaParameterBuilder>();
        }


        #region Buildings

        public MetaType ReturnType
        {
            get { return _returnType; }
            set { _returnType = value; }
        }

        internal override MetaFunction Build(MetaObjectBuilderContext context)
        {
            return new MetaFunction(this, context);
        }

        internal ImmutableArray<MetaParameter> BuildParameters(MetaObjectBuilderContext context)
        {
            return _parameters.Select(pb => pb.Build(context)).ToImmutableArray();
        }

        #endregion

    }
}
