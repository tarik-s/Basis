using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public class MetaClass : MetaObject
    {
        private readonly MetaModule _module;
        private readonly MetaClass _baseClass;

        private readonly ImmutableArray<MetaClass> _derivedClasses;
        private readonly ImmutableArray<MetaMethod> _methods;
        private readonly ImmutableArray<MetaField> _fields;

        internal MetaClass(MetaClassBuilder builder, MetaObjectBuilderContext context)
            : base(MetaType.Class, builder)
        {
            using (context.CreateScope(this))
            {
                _module = context.Module;
                _baseClass = context.BaseClass;

                _derivedClasses = builder.BuildDerivedClasses(context);

                _fields = builder.BuildFields(context);
                _methods = builder.BuildMethods(context);
            }
        }

        public MetaModule Module
        {
            get { return _module; }
        }

        public MetaClass BaseClass
        {
            get { return _baseClass; }
        }

        public ImmutableArray<MetaClass> DerivedClasses
        {
            get { return _derivedClasses; }
        }

        public ImmutableArray<MetaMethod> Methods
        {
            get { return _methods; }
        }

        public ImmutableArray<MetaField> Fields
        {
            get { return _fields; }
        }

        public override void Accept<TContext>(MetaObjectVisitor<TContext> metaObjectVisitor, TContext context)
        {
            metaObjectVisitor.VisitClass(this, context);
        }
    }

    public class MetaClassBuilder : MetaObjectBuilder
    {
        private readonly MetaClass _baseClass;

        private readonly List<MetaClassBuilder> _derivedBuilders;
        private readonly List<MetaFieldBuilder> _fieldBuilders;

        internal MetaClassBuilder(string name, MetaClass baseClass)
            : this(name)
        {
            Check.ArgumentNotNull(baseClass, "baseClass");

            _baseClass = baseClass;
        }

        internal MetaClassBuilder(string name)
            : base(name)
        {
            _derivedBuilders = new List<MetaClassBuilder>();
            _fieldBuilders = new List<MetaFieldBuilder>();
        }

        private MetaClassBuilder(string name, MetaClassBuilder baseClassBuilder)
            : this(name)
        {
            
        }

        public MetaClass BaseClass
        {
            get { return _baseClass; }
        }

        public MetaClassBuilder AddDerivedClass(string name)
        {
            var builder = new MetaClassBuilder(name, this);

            _derivedBuilders.Add(builder);

            return builder;
        }

        public MetaFieldBuilder AddField(string name)
        {
            var fieldBuilder = new MetaFieldBuilder(name);

            _fieldBuilders.Add(fieldBuilder);

            return fieldBuilder;
        }

        internal ImmutableArray<MetaField> BuildFields(MetaObjectBuilderContext context)
        {
            return ImmutableArray<MetaField>.Empty;
        }

        internal ImmutableArray<MetaMethod> BuildMethods(MetaObjectBuilderContext context)
        {
            return ImmutableArray<MetaMethod>.Empty;
        }

        internal ImmutableArray<MetaClass> BuildDerivedClasses(MetaObjectBuilderContext context)
        {
            return _derivedBuilders.Select(b => new MetaClass(b, context)).ToImmutableArray();
        }

        internal MetaClass Build(MetaObjectBuilderContext context)
        {
            if (_baseClass == null)
                return new MetaClass(this, context);

            using (context.CreateScope(_baseClass))
            {
                return new MetaClass(this, context);
            }
        }
    }

}
