using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public abstract class MetaVariable<TMetaVariable> : MetaObject
        where TMetaVariable : MetaVariable<TMetaVariable>
    {
        private readonly Lazy<MetaType> _type;

        protected MetaVariable(string name, MetaType type, MetaModule module) 
            : base(name, module)
        {
            _type = new Lazy<MetaType>(() => type);
        }

        internal MetaVariable(MetaVariableBuilder<TMetaVariable> builder, MetaObjectBuilderContext context) 
            : base(builder, context.Module)
        {
            _type = new Lazy<MetaType>(builder.CreateMetaTypeFactory(context));
        }
        internal override void ResolveDeferredMembers(MetaObjectBuilderContext context)
        {
            var unused = _type.Value;
        }

        public MetaType Type
        {
            get { return _type.Value; }
        }
    }
    
    public abstract class MetaVariableBuilder<TMetaVariable> : MetaObjectBuilder<TMetaVariable>
        where TMetaVariable : MetaVariable<TMetaVariable>
    {
        private MetaTypeProxy _type;

        protected MetaVariableBuilder(string name)
            : base(name)
        {
            _type = new MetaTypeProxy();
        }

        public MetaTypeProxy Type
        {
            get { return _type; }
        }

        internal Func<MetaType> CreateMetaTypeFactory(MetaObjectBuilderContext context)
        {
            var module = context.Module;
            return () => _type.Resolve(module);
        }
    }
}
