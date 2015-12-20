using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using Meticulous.Patterns;

namespace Meticulous.Meta
{
    [Serializable]
    public enum MetaType
    {
        Unknown,
        Module,
        Class,
        Method,
        Parameter,
        Field
    }

    [DebuggerDisplay("{Name}")]
    public abstract class MetaObject : IMetaObjectVisitable
    {
        private readonly string _name;
        private readonly MetaType _type;

        internal MetaObject(MetaObjectBuilder builder)
        {
            _type = builder.Type;
            _name = builder.Name;
        }
        
        public MetaType Type
        {
            get { return _type; }
        }

        public string Name
        {
            get { return _name; }
        }

        public abstract void Accept<TContext>(MetaObjectVisitor<TContext> metaObjectVisitor, TContext context);
    }

    public abstract class MetaObjectBuilder : IBuilder<MetaObject>
    {
        private readonly string _name;
        private readonly MetaType _type;

        protected MetaObjectBuilder(MetaType type, string name)
        {
            Check.ArgumentNotNull(name, "name");

            _type = type;
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public MetaType Type
        {
            get { return _type; }
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
        protected MetaObjectBuilder(MetaType type, string name)
            : base(type, name)
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
        // return _classBuilders.Select(cb => cb.Build(context)).ToImmutableArray();
        internal static ImmutableArray<T> BuildSubObjects<T>(IEnumerable<MetaObjectBuilder<T>> builders, MetaObjectBuilderContext context)
            where T : MetaObject
        {
            return builders.Select(b => b.Build(context)).ToImmutableArray();
        }
    }

    internal class MetaObjectBuilderContext
    {
        private readonly MetaObjectBuilder _rootBuilder;

        private MetaModule _module;
        private List<MetaClass> _classes;
        private MetaMethod _method;
        private MetaParameter _parameter;
        private MetaField _field;


        public MetaObjectBuilderContext(MetaObjectBuilder rootBuilder)
        {
            _rootBuilder = rootBuilder;
            _classes = new List<MetaClass>();
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
            get
            {
                var count = _classes.Count;
                if (count == 0)
                    return null;

                return _classes[count - 1];
            }
        }

        public MetaClass BaseClass 
        {
            get
            {
                var count = _classes.Count;
                if (count < 2)
                    return null;

                return _classes[count - 2];
            }
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
            switch (obj.Type)
            {
                case MetaType.Module:
                    _module = (MetaModule)obj;
                    break;
                case MetaType.Class:
                    _classes.Add((MetaClass)obj);
                    break;
                case MetaType.Method:
                    _method = (MetaMethod)obj;
                    break;
                case MetaType.Parameter:
                    _parameter = (MetaParameter) obj;
                    break;
                case MetaType.Field:
                    _field = (MetaField)obj;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Remove(MetaObject obj)
        {
            const string msg = "Operation called in the wrong order";
            switch (obj.Type)
            {
                case MetaType.Module:
                    Check.OperationValid(_module == (MetaModule)obj, msg);
                    _module = null;
                    break;
                case MetaType.Class:
                    var count = _classes.Count;
                    Check.OperationValid(count > 0 && _classes[count - 1] == (MetaClass)obj, msg);
                    _classes.RemoveAt(count - 1);
                    break;
                case MetaType.Method:
                    Check.OperationValid(_method == (MetaMethod)obj, msg);
                    _method = null;
                    break;
                case MetaType.Parameter:
                    Check.OperationValid(_parameter == (MetaParameter)obj, msg);
                    _parameter = null;
                    break;
                case MetaType.Field:
                    Check.OperationValid(_field == (MetaField)obj, msg);
                    _field = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private class Scope<TObject> : IDisposable
            where TObject: MetaObject
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
