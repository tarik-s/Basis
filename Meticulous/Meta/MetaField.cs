using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public enum EncapsulationLevel
    {
        Private,
        Protected,
        Public
    }

    public sealed class MetaField : MetaVariable<MetaField>
    {
        private readonly EncapsulationLevel _encapsulationLevel;

        internal MetaField(MetaFieldBuilder builder, MetaObjectBuilderContext context)
            : base(builder, context)
        {
            using (context.CreateScope(this))
            {
            }
            _encapsulationLevel = builder.EncapsulationLevel;
        }

        public EncapsulationLevel EncapsulationLevel
        {
            get { return _encapsulationLevel; }
        }

        public override void Accept<TContext>(IMetaTypeVisitor<TContext> metaTypeVisitor, TContext context)
        {
            metaTypeVisitor.VisitField(this, context);
        }
    }

    public class MetaFieldBuilder : MetaVariableBuilder<MetaField>
    {
        private EncapsulationLevel _encapsulationLevel;

        public MetaFieldBuilder(string name)
            : base(name)
        {
            _encapsulationLevel = EncapsulationLevel.Private;
        }

        internal override MetaField Build(MetaObjectBuilderContext context)
        {
            return new MetaField(this, context);
        }

        public EncapsulationLevel EncapsulationLevel
        {
            get { return _encapsulationLevel; }
            set { _encapsulationLevel = value; }
        }
    }
}
