using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticulous.Patterns;

namespace Meticulous.Meta
{
    public class MetaModule : MetaObject
    {
        private readonly ImmutableArray<MetaClass> _classes;
        private readonly ImmutableArray<MetaModule> _references;

        internal MetaModule(MetaModuleBuilder builder, MetaObjectBuilderContext context)
            : base(builder, context)
        {
            _references = builder.GetReferences();

            _classes = builder.BuildClasses(context, this);
        }

        public ImmutableArray<MetaClass> Classes
        {
            get { return _classes; }
        }

        public ImmutableArray<MetaModule> References
        {
            get { return _references; }
        }

        public override void Accept<TContext>(MetaTypeVisitor<TContext> metaTypeVisitor, TContext context)
        {
            metaTypeVisitor.VisitModule(this, context);
        }
    }

    public class MetaModuleBuilder : MetaObjectBuilder, IBuilder<MetaModule>
    {
        private readonly List<MetaModule> _references;
        private readonly List<MetaClassBuilder> _classBuilders;

        public MetaModuleBuilder(string name)
            : base(name)
        {
            _classBuilders = new List<MetaClassBuilder>();
            _references = new List<MetaModule>();
        }

        public MetaModule Build()
        {
            var ctx = new MetaObjectBuilderContext(0);
            return new MetaModule(this, ctx);
        }

        public MetaClassBuilder AddClass(string className)
        {
            var builder = new MetaClassBuilder(className);

            _classBuilders.Add(builder);

            return builder;
        }

        public MetaClassBuilder AddClass(string name, MetaClass baseMetaClass)
        {
            var builder = new MetaClassBuilder(name, baseMetaClass);

            _classBuilders.Add(builder);

            return builder;
        }

        internal ImmutableArray<MetaClass> BuildClasses(MetaObjectBuilderContext context, MetaModule module)
        {
            return _classBuilders.Select(cb => cb.Build(context, module)).ToImmutableArray();
        }

        #region References

        public void AddReference(MetaModule metaModule)
        {
            Check.ArgumentNotNull(metaModule, "MetaModule");

            _references.Add(metaModule);
        }

        public void AddReferences(IEnumerable<MetaModule> moduleInfos)
        {
            Check.ArgumentNotNull(moduleInfos, "moduleInfos");

            _references.AddRange(moduleInfos);
        }

        public ImmutableArray<MetaModule> GetReferences()
        {
            return _references.ToImmutableArray();
        }

        #endregion
    }

}
