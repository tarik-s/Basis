using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public class MetaInterface : MetaObject, IMetaInterfaceProxy
    {
        private readonly ImmutableArray<MetaInterface> _baseInterfaces;
        private readonly ImmutableArray<MetaFunction> _methods;

        internal MetaInterface(MetaInterfaceBuilder builder, MetaObjectBuilderContext context) 
            : base(builder, context.Module)
        {
            _baseInterfaces = context.BaseInterfaces;

            using (context.CreateScope(this))
            {
                _methods = builder.BuildMethods(context);
            }
        }

        public ImmutableArray<MetaInterface> BaseInterfaces
        {
            get { return _baseInterfaces; }
        }

        public ImmutableArray<MetaFunction> Methods
        {
            get { return _methods; }
        }

        public override void Accept<TContext>(IMetaTypeVisitor<TContext> metaObjectVisitor, TContext context)
        {
            metaObjectVisitor.VisitInterface(this, context);
        }

        internal override void ResolveDeferredMembers(MetaObjectBuilderContext context)
        {
            ResolveDeferredMembers(_methods, context);
        }

        MetaInterface IMetaInterfaceProxy.Resolve(MetaObjectBuilderContext context)
        {
            return this;
        }
    }

    internal interface IMetaInterfaceProxy
    {
        string Name { get; }

        MetaInterface Resolve(MetaObjectBuilderContext context);
    }

    public class MetaInterfaceBuilder : MetaObjectBuilder<MetaInterface> , IMetaInterfaceProxy
    {
        private readonly List<IMetaInterfaceProxy> _baseInterfaces;

        private readonly List<MetaFunctionBuilder> _methodBuilders;

        internal MetaInterfaceBuilder(string name, MetaObjectBuilder parentBuilder)
            : base(name, parentBuilder)
        {
            _baseInterfaces = new List<IMetaInterfaceProxy>();
            _methodBuilders = new List<MetaFunctionBuilder>();
        }

        public MetaInterfaceBuilder(string name)
            : this(name, null)
        {
        }

        public void AddBaseInterface(MetaInterface @interface)
        {
            _baseInterfaces.Add(@interface);
        }

        public void AddBaseInterface(MetaInterfaceBuilder @interface)
        {
            CheckRoot(@interface);
            
            _baseInterfaces.Add(@interface);
        }

        public MetaFunctionBuilder AddMethod(string name)
        {
            var methodBuilder = new MetaFunctionBuilder(name, this);

            _methodBuilders.Add(methodBuilder);

            return methodBuilder;
        }

        internal override MetaInterface Build(MetaObjectBuilderContext context)
        {
            context.PushBaseInterfaces(_baseInterfaces.ToImmutableArray());
            try
            {
                return new MetaInterface(this, context);
            }
            finally
            {
                context.PopBaseInterfaces();
            }
            
        }

        internal ImmutableArray<MetaFunction> BuildMethods(MetaObjectBuilderContext context)
        {
            return BuildSubObjects(_methodBuilders, context);
        }

        MetaInterface IMetaInterfaceProxy.Resolve(MetaObjectBuilderContext context)
        {
            var @interface = context.FindInterface(Name);
            if (@interface == null)
                throw new KeyNotFoundException("Interface not found: " + Name);

            return @interface;
        }
    }
}
