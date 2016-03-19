using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using Meticulous;
using Meticulous.Meta;
using Meticulous.Patterns;

namespace Meticulous.Meta
{ 
    [DebuggerDisplay("{Name}")]
    public abstract class MetaObject : IVisitableMetaObject
    {
        private readonly string _name;

        protected MetaObject(string name)
        {
            _name = name;
        }

        internal MetaObject(MetaObjectBuilder builder)
        {
            _name = builder.Name;
        }
        
        public string Name
        {
            get { return _name; }
        }

        public abstract void Accept<TContext>(IMetaObjectVisitor<TContext> metaObjectVisitor, TContext context);
    }

    public abstract class MetaObjectBuilder : IBuilder<MetaObject>
    {
        private readonly string _name;

        protected MetaObjectBuilder(string name)
        {
            _name = name;
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
    }

    public abstract class MetaObjectBuilder<TMetaObject> : MetaObjectBuilder, IBuilder<TMetaObject>
        where TMetaObject : MetaObject
    {
        protected MetaObjectBuilder(string name)
            : base(name)
        {

        }

        internal abstract TMetaObject Build(MetaObjectBuilderContext context);

        public TMetaObject Build()
        {
            var ctx = new MetaObjectBuilderContext(this);
            return Build(ctx);
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

    internal class MetaObjectBuilderContext : IMetaObjectVisitor<bool>
    {
        private readonly MetaObjectBuilder _rootBuilder;

        private MetaModule _module;
        private MetaClass _class;
        private MetaMethod _method;
        private MetaParameter _parameter;
        private MetaField _field;


        public MetaObjectBuilderContext(MetaObjectBuilder rootBuilder)
        {
            _rootBuilder = rootBuilder;
        }

        public MetaObjectBuilderContext Copy()
        {
            var ctx = new MetaObjectBuilderContext(_rootBuilder)
            {
                _class = _class,
                _field = _field,
                _module = _module,
                _method = _method,
                _parameter = _parameter
            };
           
            return ctx;
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

        public MetaMethod Method
        {
            get { return _method; }
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
            Debug.Assert((context && _class == null) || (!context && _class == @class));
            _class = context ? @class : null;
        }

        public void VisitMethod(MetaMethod method, bool context)
        {
            Debug.Assert((context && _method == null) || (!context && _method == method));
            _method = context ? method : null;
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

        public void VisitType(MetaType type, bool context)
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
