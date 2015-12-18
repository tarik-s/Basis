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

        internal MetaClass(MetaClassBuilder builder, MetaObjectBuilderContext context, MetaModule module, MetaClass baseClass)
            : base(builder, context)
        {
            _module = module;

            _baseClass = baseClass;
            _fields = builder.BuildFields(this);
            _methods = builder.BuildMethods(this);

            _derivedClasses = builder.BuildDerivedClasses(context, module, this);
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

        public MetaModule Module
        {
            get { return _module; }
        }

        public override void Accept<TContext>(MetaTypeVisitor<TContext> metaTypeVisitor, TContext context)
        {
            metaTypeVisitor.VisitClass(this, context);
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

        internal ImmutableArray<MetaField> BuildFields(MetaClass thisClass)
        {
            return ImmutableArray<MetaField>.Empty;
        }

        internal ImmutableArray<MetaMethod> BuildMethods(MetaClass thisClass)
        {
            return ImmutableArray<MetaMethod>.Empty;
        }

        internal ImmutableArray<MetaClass> BuildDerivedClasses(MetaObjectBuilderContext context, MetaModule module, MetaClass thisClass)
        {
            return _derivedBuilders.Select(b => new MetaClass(b, context, module, thisClass)).ToImmutableArray();
        }

        internal MetaClass Build(MetaObjectBuilderContext context, MetaModule module)
        {
            return new MetaClass(this, context, module, _baseClass);
        }
    }

}
