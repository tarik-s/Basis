using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;


namespace Meticulous.Meta
{
    public class MetaClass : MetaObject, IMetaClassProxy
    {
        private readonly MetaClass _base;
        private readonly ImmutableArray<MetaInterface> _interfaces;

        private readonly ImmutableArray<MetaClass> _derivedClasses;
        private readonly ImmutableArray<MetaFunction> _methods;
        private readonly ImmutableArray<MetaField> _fields;

        internal MetaClass(MetaClassBuilder builder, MetaObjectBuilderContext context)
            : base(builder, context.Module)
        {
            _base = context.Class;
            _interfaces = context.BaseInterfaces;

            using (context.CreateScope(this))
            {
                _fields = builder.BuildFields(context);
                _methods = builder.BuildMethods(context);

                _derivedClasses = builder.BuildDerivedClasses(context);
            }
        }

        public MetaClass Base
        {
            get { return _base; }
        }

        public ImmutableArray<MetaInterface> Interfaces
        {
            get { return _interfaces; }
        }

        public ImmutableArray<MetaClass> DerivedClasses
        {
            get { return _derivedClasses; }
        }

        public IEnumerable<MetaClass> EnumerateSubclasses()
        {
            foreach (var derivedClass in _derivedClasses)
            {
                yield return derivedClass;

                foreach (var subclass in derivedClass.EnumerateSubclasses())
                {
                    yield return subclass;
                }
            }
        }

        public ImmutableArray<MetaFunction> Methods
        {
            get { return _methods; }
        }

        public ImmutableArray<MetaField> Fields
        {
            get { return _fields; }
        }

        public override void Accept<TContext>(IMetaTypeVisitor<TContext> metaTypeVisitor, TContext context)
        {
            metaTypeVisitor.VisitClass(this, context);
        }

        internal override void ResolveDeferredMembers(MetaObjectBuilderContext context)
        {
            ResolveDeferredMembers(_fields, context);
            ResolveDeferredMembers(_methods, context);
            ResolveDeferredMembers(_derivedClasses, context);
        }

        MetaClass IMetaClassProxy.Resolve(MetaObjectBuilderContext context)
        {
            return this;
        }
    }

    internal interface IMetaClassProxy
    {
        string Name { get; }

        MetaClass Resolve(MetaObjectBuilderContext context);
    }

    public class MetaClassBuilder : MetaObjectBuilder<MetaClass>, IMetaClassProxy
    {
        #region Fields

        private readonly IMetaClassProxy _base = null;
        private readonly List<MetaClassBuilder> _derivedBuilders;
        private readonly List<MetaFieldBuilder> _fieldBuilders;
        private readonly List<MetaFunctionBuilder> _methodBuilders;

        #endregion

        #region Constructors

        public MetaClassBuilder(string name, MetaClass baseClass)
            : this(name, baseClass, null)
        {
            Check.ArgumentNotNull(baseClass, "baseClass");
        }

        public MetaClassBuilder(string name)
            : this(name, null, null)
        {
            
        }

        internal MetaClassBuilder(string name, IMetaClassProxy baseClassProxy, MetaObjectBuilder parentBuilder)
            : base(name, parentBuilder)
        {
            _base = baseClassProxy;

            _derivedBuilders = new List<MetaClassBuilder>();
            _fieldBuilders = new List<MetaFieldBuilder>();
            _methodBuilders = new List<MetaFunctionBuilder>();
        }

        #endregion

        public MetaClassBuilder AddDerivedClass(string name)
        {
            var builder = new MetaClassBuilder(name, this, this);

            _derivedBuilders.Add(builder);

            return builder;
        }

        public MetaFunctionBuilder AddMethod(string name)
        {
            var methodBuilder = new MetaFunctionBuilder(name, this);

            _methodBuilders.Add(methodBuilder);

            return methodBuilder;
        }

        public MetaFieldBuilder AddField(string name)
        {
            var fieldBuilder = new MetaFieldBuilder(name, this);

            _fieldBuilders.Add(fieldBuilder);

            return fieldBuilder;
        }

        #region Building

        internal ImmutableArray<MetaField> BuildFields(MetaObjectBuilderContext context)
        {
            return BuildSubObjects(_fieldBuilders, context);
        }

        internal ImmutableArray<MetaFunction> BuildMethods(MetaObjectBuilderContext context)
        {
            return BuildSubObjects(_methodBuilders, context);
        }

        internal ImmutableArray<MetaClass> BuildDerivedClasses(MetaObjectBuilderContext context)
        {
            return BuildSubObjects(_derivedBuilders, context);
        }

        internal override MetaClass Build(MetaObjectBuilderContext context)
        {
            var baseClass = _base.Resolve(context);
            using (context.CreateScope(baseClass))
            {
                return new MetaClass(this, context);
            }
        }

        #endregion

        MetaClass IMetaClassProxy.Resolve(MetaObjectBuilderContext context)
        {
            throw new NotImplementedException();
        }
    }

}
