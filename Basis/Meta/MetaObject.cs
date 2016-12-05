using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Meticulous;
using Meticulous.Meta;
using Meticulous.Patterns;
using Meticulous.Threading;

namespace Meticulous.Meta
{ 
    [DebuggerDisplay("{Name}")]
    public abstract class MetaObject : MetaType
    {
        private readonly MetaModule _module;
        protected MetaObject(string name, MetaModule module)
            : base(name)
        {
            _module = module;
        }

        internal MetaObject(MetaObjectBuilder builder, MetaModule module)
            : this(builder.Name, module)
        {
            
        }

        internal abstract void ResolveDeferredMembers(MetaObjectBuilderContext context);

        public override MetaModule Module
        {
            get { return _module; }
        }

        internal static void ResolveDeferredMembers(IEnumerable<MetaObject> members, MetaObjectBuilderContext context)
        {
            foreach (var member in members)
            {
                member.ResolveDeferredMembers(context);
            }
        }
    }

    public abstract class MetaObjectBuilder : IBuilder<MetaObject>
    {
        private static long s_id = 0;
        private readonly long _id;
        private readonly long _rootId = 0;
        private readonly string _name;

        protected MetaObjectBuilder(string name)
        {
            _id = Interlocked.Increment(ref s_id);
            _name = name;
        }

        internal MetaObjectBuilder(string name, MetaObjectBuilder parentBuilder)
            : this(name)
        {
            if (parentBuilder != null)
            {
                _rootId = parentBuilder.RootId;
                if (_rootId == 0)
                    _rootId = parentBuilder.Id;
            }
        }

        internal long Id
        {
            get { return _id; }
        }

        internal long RootId
        {
            get { return _rootId; }
        }


        public string Name
        {
            get { return _name; }
        }

        MetaObject IBuilder<MetaObject>.Build()
        {
            return BuildCore();
        }

        protected abstract MetaObject BuildCore();

        internal void CheckRoot(MetaObjectBuilder builder)
        {
            Check.ArgumentNotNull(builder, "builder");
            if (_rootId == 0)
                throw new InvalidOperationException("Rootless builder");

            if (builder.RootId == 0)
                throw new ArgumentException("Rootless builder");

            Check.OperationValid(builder.RootId == _rootId, "Builders belong to different roots");
        }
    }

    public abstract class MetaObjectBuilder<TMetaObject> : MetaObjectBuilder, IBuilder<TMetaObject>
        where TMetaObject : MetaObject
    {
        //protected MetaObjectBuilder(string name)
        //    : base(name)
        //{

        //}

        internal MetaObjectBuilder(string name, MetaObjectBuilder parentBuilder)
            : base(name, parentBuilder)
        {
            
        }

        internal abstract TMetaObject Build(MetaObjectBuilderContext context);

        public TMetaObject Build()
        {
            var ctx = new MetaObjectBuilderContext(this);
            var obj = Build(ctx);
            obj.ResolveDeferredMembers(ctx);
            return obj;
        }

        protected override MetaObject BuildCore()
        {
            return (TMetaObject)Build();
        }

        internal static ImmutableArray<T> BuildSubObjects<T>(IEnumerable<MetaObjectBuilder<T>> builders, MetaObjectBuilderContext context)
            where T : MetaObject
        {
            return builders.Select(b => b.Build(context)).ToImmutableArray();
        }
    }

    internal class MetaObjectBuilderContext : IMetaTypeVisitor<bool>
    {
        private static long s_id = 0;

        private readonly long _id;
        private readonly MetaObjectBuilder _rootBuilder;

        private MetaModule _module;
        private MetaClass _class;
        private MetaFunction _function;
        private MetaParameter _parameter;
        private MetaField _field;

        private readonly Stack<MetaInterface> _interfaces;
        private readonly Stack<ImmutableArray<MetaInterface>> _baseInterfaces;

        private readonly Dictionary<string, MetaInterface> _registeredInterfaces;


        public MetaObjectBuilderContext(MetaObjectBuilder rootBuilder)
        {
            _id = Interlocked.Increment(ref s_id);

            _rootBuilder = rootBuilder;
            _baseInterfaces = new Stack<ImmutableArray<MetaInterface>>();
            _interfaces = new Stack<MetaInterface>();

            _registeredInterfaces = new Dictionary<string, MetaInterface>();
        }

        public long Id
        {
            get { return _id; }
        }

        public IDisposable CreateScope<TObject>(TObject obj)
            where TObject : MetaObject
        {
            return new Scope<TObject>(this, obj);
        }

        public MetaObjectBuilder RootBuilder
        {
            get { return _rootBuilder; }
        }

        public MetaModule Module
        {
            get { return _module; }
        }

        public MetaClass Class
        {
            get { return _class; }
        }

        public ImmutableArray<MetaInterface> BaseInterfaces
        {
            get
            {
                if (_baseInterfaces.Count == 0)
                    return ImmutableArray<MetaInterface>.Empty;

                var baseInterfaces = _baseInterfaces.Peek();
                return baseInterfaces;
            }
        }

        public void PushBaseInterfaces(ImmutableArray<IMetaInterfaceProxy> interfaceProxies)
        {
            var interfaces = interfaceProxies.Select(ip => ip.Resolve(this)).ToImmutableArray();
            _baseInterfaces.Push(interfaces);
        }
        
        public void PopBaseInterfaces()
        {
            _baseInterfaces.Pop();
        }

        public MetaInterface Interface
        {
            get
            {
                if (_interfaces.Count == 0)
                    return null;

                var result = _interfaces.Peek();
                return result;
            }
        }

        public MetaInterface FindInterface(string name)
        {
            MetaInterface result;
            if (_registeredInterfaces.TryGetValue(name, out result))
                return result;

            return null;
        }

        public MetaFunction Function
        {
            get { return _function; }
        }

        public MetaParameter Parameter
        {
            get { return _parameter; }
        }

        private void Add(MetaObject obj)
        {
            obj.Accept(this, true);
        }

        private void Remove(MetaObject obj)
        {
            obj.Accept(this, false);
        }

        #region Visitor

        public void VisitModule(MetaModule module, bool context)
        {
            Debug.Assert((context && _module == null) || (!context && _module == module));
            _module = context ? module : null;
        }

        public void VisitClass(MetaClass @class, bool context)
        {
            Debug.Assert((context && _class == @class.Base) || (!context && _class == @class));
            _class = context ? @class : @class.Base;
        }

        public void VisitInterface(MetaInterface @interface, bool context)
        {
            if (context)
            {
                if (@interface.Module == _module)
                    _registeredInterfaces[@interface.Name] = @interface;

                _interfaces.Push(@interface);
            }
            else
                _interfaces.Pop();
        }

        public void VisitFunction(MetaFunction function, bool context)
        {
            Debug.Assert((context && _function == null) || (!context && _function == function));
            _function = context ? function : null;
        }

        public void VisitParameter(MetaParameter parameter, bool context)
        {
            Debug.Assert((context && _parameter == null) || (!context && _parameter == parameter));
            _parameter = context ? parameter : null;
        }

        public void VisitField(MetaField field, bool context)
        {
            Debug.Assert((context && _field == null) || (!context && _field == field));
            _field = context ? field : null;
        }

        public void VisitPlainType(PlainMetaType type, bool context)
        {
            throw new NotImplementedException();
        }

        #endregion

        private sealed class Scope<TObject> : IDisposable
            where TObject : MetaObject
        {
            private readonly MetaObjectBuilderContext _ctx;
            private readonly TObject _obj;

            public Scope(MetaObjectBuilderContext ctx, TObject obj)
            {
                _obj = obj;
                _ctx = ctx;

                if (_obj != null)
                    _ctx.Add(obj);
            }

            public void Dispose()
            {
                if (_obj != null)
                    _ctx.Remove(_obj);
            }
        }

    }


}
