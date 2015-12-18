using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    [DebuggerDisplay("{Name}")]
    public abstract class MetaObject : IMetaTypeVisitable
    {
        private readonly string _name;

        internal MetaObject(MetaObjectBuilder builder, MetaObjectBuilderContext context)
        {
            context.Add(this);
            _name = builder.Name;
        }

        protected MetaObject(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public abstract void Accept<TContext>(MetaTypeVisitor<TContext> metaTypeVisitor, TContext context);
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

        public MetaObjectBuilderContext(long id)
        {
            _id = id;
        }

        public long Id
        {
            get { return _id; }
        }

        public void Add(MetaObject metaObject)
        {
        }
    }


}
