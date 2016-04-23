using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public enum ParameterType
    {
        Input,
        Output,
        Result
    }

    public sealed class MetaParameter : MetaVariable<MetaParameter>
    {
        private readonly ParameterType _parameterType;
        internal MetaParameter(MetaParameterBuilder builder, MetaObjectBuilderContext context)
            : base(builder, context)
        {
            _parameterType = builder.ParameterType;
        }

        public override void Accept<TContext>(IMetaTypeVisitor<TContext> metaTypeVisitor, TContext context)
        {
            metaTypeVisitor.VisitParameter(this, context);
        }

        public ParameterType ParameterType
        {
            get { return _parameterType; }
        }
    }

    public sealed class MetaParameterBuilder : MetaVariableBuilder<MetaParameter>
    {
        private ParameterType _parameterType;

        internal MetaParameterBuilder(string name, MetaObjectBuilder parentBuilder)
            : base(name, parentBuilder, MetaType.Integer)
        {
            _parameterType = ParameterType.Input;
        }

        public MetaParameterBuilder(string name)
            : this(name, null)
        {
        }

        public static MetaParameterBuilder CreateReturnParameterBuilder(MetaObjectBuilder parentBuilder)
        {
            var paramBuilder = new MetaParameterBuilder("return", parentBuilder);
            paramBuilder.ParameterType = ParameterType.Result;
            return paramBuilder;
        }

        internal override MetaParameter Build(MetaObjectBuilderContext context)
        {
            return new MetaParameter(this, context);
        }

        public ParameterType ParameterType
        {
            get { return _parameterType; }
            set { _parameterType = value; }
        }
    }
}
