using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    [Serializable]
    public enum MetaType
    {
        Unknown,
        Module,
        Class,
        Method,
        Field
    }

    [DebuggerDisplay("{Name}")]
    public abstract class MetaObject : IMetaObjectVisitable
    {
        private readonly string _name;
        private readonly MetaType _type;

        internal MetaObject(MetaType type, MetaObjectBuilder builder, MetaObjectBuilderContext context)
            : this(type, builder.Name)
        {
            context.Add(this);
        }

        protected MetaObject(MetaType type, string name)
        {
            _type = type;
            _name = name;
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

    public abstract class MetaObjectBuilder
    {
        private readonly string _name;

        protected MetaObjectBuilder(string name)
        {
            Check.ArgumentNotNull(name, "name");

            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }

    internal class MetaObjectBuilderContext
    {
        private readonly long _id;

        private MetaModule _module;
        private List<MetaClass> _classes;
        private MetaMethod _method;
        private MetaField _field;


        public MetaObjectBuilderContext(long id)
        {
            _id = id;
            _classes = new List<MetaClass>();
        }

        public long Id
        {
            get { return _id; }
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

        public void Add(MetaObject obj)
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
                case MetaType.Field:
                    _field = (MetaField)obj;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Remove(MetaObject obj)
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
                case MetaType.Field:
                    Check.OperationValid(_field == (MetaField)obj, msg);
                    _field = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }


}
