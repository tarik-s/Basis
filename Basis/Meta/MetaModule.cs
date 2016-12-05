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
        private static readonly CoreMetaModule s_coreModule = new CoreMetaModule();

        private readonly ImmutableArray<MetaModule> _references = ImmutableArray<MetaModule>.Empty;
        private ImmutableArray<MetaType> _types = ImmutableArray<MetaType>.Empty;

        private readonly ImmutableArray<MetaClass> _rootClasses = ImmutableArray<MetaClass>.Empty;
        private readonly ImmutableArray<MetaClass> _classes= ImmutableArray<MetaClass>.Empty;

        private readonly ImmutableArray<MetaInterface> _interfaces = ImmutableArray<MetaInterface>.Empty;



        internal MetaModule(MetaModuleBuilder builder, MetaObjectBuilderContext context)
            : base(builder, null)
        {
            _references = builder.GetReferences();

            var types = new List<MetaType>();
            using (context.CreateScope(this))
            {

                _interfaces = builder.BuildInterfaces(context);

                // Build classes
                _rootClasses = builder.BuildClasses(context);

                var classes = new List<MetaClass>();
                classes.AddRange(_rootClasses);

                foreach (var @class in _rootClasses)
                {
                    classes.AddRange(@class.EnumerateSubclasses());
                }
                _classes = classes.OrderBy(c => c.Name).ToImmutableArray();

                types.AddRange(classes);
            }
            SetTypes(types);
        }

        internal MetaModule(string name)
            : base(name, null)
        {
            
        }

        public static CoreMetaModule Core
        {
            get { return s_coreModule; }
        }

        public ImmutableArray<MetaType> Types
        {
            get { return _types; }
        }

        public ImmutableArray<MetaClass> Classes
        {
            get { return _classes; }
        }

        public ImmutableArray<MetaClass> RootClasses
        {
            get { return _rootClasses; }
        }

        internal override void ResolveDeferredMembers(MetaObjectBuilderContext context)
        {
            ResolveDeferredMembers(_rootClasses, context);
        }

        public override MetaModule Module
        {
            get { return this; }
        }

        public ImmutableArray<MetaModule> References
        {
            get { return _references; }
        }

        public override void Accept<TContext>(IMetaTypeVisitor<TContext> metaTypeVisitor, TContext context)
        {
            metaTypeVisitor.VisitModule(this, context);
        }

        internal void SetTypes(IEnumerable<MetaType> types)
        {
            _types = types.OrderBy(mt => mt.Name).ToImmutableArray();
        }
    }

    public sealed class CoreMetaModule : MetaModule
    {
        private readonly VoidMetaType _void;
        private readonly BooleanMetaType _bool;
        private readonly IntegerMetaType _int;
        private readonly FloatMetaType _float;
        private readonly StringMetaType _string;

        internal CoreMetaModule()
            : base("core")
        {
            _void = new VoidMetaType(this);
            _bool = new BooleanMetaType(this);
            _int = new IntegerMetaType(this);
            _float = new FloatMetaType(this);
            _string = new StringMetaType(this);

            SetTypes(new MetaType[] {_void, _bool, _int, _float, _string});
        }

        public new VoidMetaType Void
        {
            get { return _void; }
        }

        public new BooleanMetaType Boolean
        {
            get { return _bool; }
        }

        public new IntegerMetaType Integer
        {
            get { return _int; }
        }

        public new FloatMetaType Float
        {
            get { return _float; }
        }

        public new StringMetaType String
        {
            get { return _string; }
        }
    }

    public class MetaModuleBuilder : MetaObjectBuilder<MetaModule>
    {
        #region Fields

        private readonly List<MetaModule> _references;
        private readonly List<MetaClassBuilder> _classBuilders;
        private readonly List<MetaInterfaceBuilder> _interfaceBuilders;

        #endregion
        
        public MetaModuleBuilder(string name)
            : base(name, null)
        {
            _classBuilders = new List<MetaClassBuilder>();
            _interfaceBuilders = new List<MetaInterfaceBuilder>();
            _references = new List<MetaModule>();
            _references.Add(MetaModule.Core);
        }

        #region Classes

        public MetaClassBuilder AddClass(string className)
        {
            var builder = new MetaClassBuilder(className, null, this);

            _classBuilders.Add(builder);

            return builder;
        }

        public MetaClassBuilder AddClass(string name, MetaClass baseClass)
        {
            var builder = new MetaClassBuilder(name, baseClass, this);

            _classBuilders.Add(builder);

            return builder;
        }

        #endregion

        #region Interfaces

        public MetaInterfaceBuilder AddInterface(string interfaceName)
        {
            var builder = new MetaInterfaceBuilder(interfaceName, this);

            _interfaceBuilders.Add(builder);

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

        internal ImmutableArray<MetaInterface> BuildInterfaces(MetaObjectBuilderContext context)
        {
            return BuildSubObjects(_interfaceBuilders, context);
        }

        #endregion
    }

}
