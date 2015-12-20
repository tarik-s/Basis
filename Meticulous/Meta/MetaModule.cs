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
        private readonly ImmutableArray<MetaClass> _rootClasses;
        private readonly ImmutableArray<MetaClass> _classes;
        private readonly ImmutableArray<MetaModule> _references;

        internal MetaModule(MetaModuleBuilder builder, MetaObjectBuilderContext context)
            : base(builder)
        {
            using (context.CreateScope(this))
            {
                _references = builder.GetReferences();
                _rootClasses = builder.BuildClasses(context);
                _classes = _rootClasses;
            }
        }

        public ImmutableArray<MetaClass> Classes
        {
            get { return _classes; }
        }

        public ImmutableArray<MetaClass> RootClasses
        {
            get { return _rootClasses; }
        }

        public ImmutableArray<MetaModule> References
        {
            get { return _references; }
        }

        public override void Accept<TContext>(MetaObjectVisitor<TContext> metaObjectVisitor, TContext context)
        {
            metaObjectVisitor.VisitModule(this, context);
        }
    }

    public class MetaModuleBuilder : MetaObjectBuilder<MetaModule>
    {
        #region Fields

        private readonly List<MetaModule> _references;
        private readonly List<MetaClassBuilder> _classBuilders;

        #endregion
        
        public MetaModuleBuilder(string name)
            : base(MetaType.Module, name)
        {
            _classBuilders = new List<MetaClassBuilder>();
            _references = new List<MetaModule>();
        }

        #region Classes

        public MetaClassBuilder AddClass(string className)
        {
            var builder = new MetaClassBuilder(className);

            _classBuilders.Add(builder);

            return builder;
        }

        public MetaClassBuilder AddClass(string name, MetaClass baseClass)
        {
            var builder = new MetaClassBuilder(name, baseClass);

            _classBuilders.Add(builder);

            return builder;
        }

        #endregion

        #region References

        public void AddReference(MetaModule metaModule)
        {
            Check.ArgumentNotNull(metaModule, "MetaModule");

            _references.Add(metaModule);
        }

        public void AddReferences(ImmutableArray<MetaModule> moduleInfos)
        {
            _references.AddRange(moduleInfos);
        }

        public ImmutableArray<MetaModule> GetReferences()
        {
            return _references.ToImmutableArray();
        }

        #endregion

        #region Building

        internal override MetaModule Build(MetaObjectBuilderContext context)
        {
            return new MetaModule(this, context);
        }

        internal ImmutableArray<MetaClass> BuildClasses(MetaObjectBuilderContext context)
        {
            return BuildSubObjects(_classBuilders, context);
        }

        #endregion
    }

}
