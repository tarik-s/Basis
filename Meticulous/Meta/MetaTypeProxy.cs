using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public abstract class MetaTypeProxy
    {
        public static MetaTypeProxy Create(MetaType type)
        {
            Check.ArgumentNotNull(type, "type");

            return new TransparentMetaTypeProxy(type);
        }

        public static MetaTypeProxy Create(MetaObjectBuilder builder)
        {
            Check.ArgumentNotNull(builder, "builder");

            return new DeferredTypeProxy(builder);
        }


        public abstract MetaType Resolve(MetaModule module);

        public abstract string Name { get; }

        private sealed class TransparentMetaTypeProxy : MetaTypeProxy
        {
            private readonly MetaType _type;

            public TransparentMetaTypeProxy(MetaType type)
            {
                _type = type;
            }

            public override string Name
            {
                get { return _type.Name; }
            }

            public override MetaType Resolve(MetaModule module)
            {
                return _type;
            }
        }

        private sealed class DeferredTypeProxy : MetaTypeProxy
        {
            private readonly string _name;

            public DeferredTypeProxy(MetaObjectBuilder builder)
            {
                _name = builder.Name;
            }

            public override string Name
            {
                get { return _name; }
            }

            public override MetaType Resolve(MetaModule module)
            {
                var types = module.Types;
                var resolvedType = types.First(t => t.Name == _name);
                return resolvedType;
            }
        }

    }

}
