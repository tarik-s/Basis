using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


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
            : base(builder)
        {
            using (context.CreateScope(this))
            {
                _module = context.Module;
                _baseClass = context.BaseClass;

                _fields = builder.BuildFields(context);
                _methods = builder.BuildMethods(context);
                _derivedClasses = builder.BuildDerivedClasses(context);
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

    public class MetaClassBuilder : MetaObjectBuilder<MetaClass>
    {
        #region Fields

        private readonly MetaClass _baseClass;
        private readonly List<MetaClassBuilder> _derivedBuilders;
        private readonly List<MetaFieldBuilder> _fieldBuilders;
        private readonly List<MetaMethodBuilder> _methodBuilders;

        #endregion

        #region Constructors

        internal MetaClassBuilder(string name, MetaClass baseClass)
            : this(name)
        {
            Check.ArgumentNotNull(baseClass, "baseClass");

            _baseClass = baseClass;
        }

        public MetaClassBuilder(string name)
            : base(MetaType.Class,  name)
        {
            _derivedBuilders = new List<MetaClassBuilder>();
            _fieldBuilders = new List<MetaFieldBuilder>();
            _methodBuilders = new List<MetaMethodBuilder>();
        }

        private MetaClassBuilder(string name, MetaClassBuilder baseClassBuilder)
            : this(name)
        {

        }

        #endregion

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

        public MetaMethodBuilder AddMethod(string name)
        {
            var methodBuilder = new MetaMethodBuilder(name);

            _methodBuilders.Add(methodBuilder);

            return methodBuilder;
        }

        public MetaFieldBuilder AddField(string name)
        {
            var fieldBuilder = new MetaFieldBuilder(name);

            _fieldBuilders.Add(fieldBuilder);

            return fieldBuilder;
        }

        #region Building

        internal ImmutableArray<MetaField> BuildFields(MetaObjectBuilderContext context)
        {
            return BuildSubObjects(_fieldBuilders, context);
        }

        internal ImmutableArray<MetaMethod> BuildMethods(MetaObjectBuilderContext context)
        {
            return BuildSubObjects(_methodBuilders, context);
        }

        internal ImmutableArray<MetaClass> BuildDerivedClasses(MetaObjectBuilderContext context)
        {
            return BuildSubObjects(_derivedBuilders, context);
        }

        internal override MetaClass Build(MetaObjectBuilderContext context)
        {
            using (context.CreateScope(_baseClass))
            {
                return new MetaClass(this, context);
            }
        }

        #endregion
    }

}
