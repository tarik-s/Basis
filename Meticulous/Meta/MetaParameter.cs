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
        public MetaParameterBuilder(string name)
            : base(name)
        {
            _parameterType = ParameterType.Input;
        }

        public static MetaParameterBuilder CreateReturnParameterBuilder()
        {
            var paramBuilder = new MetaParameterBuilder("return");
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
