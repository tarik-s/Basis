using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public abstract class MetaType : MetaObject
    {
        private readonly static VoidMetaType s_void = new VoidMetaType(null);

        private readonly MetaModule _modeule;

        protected MetaType(MetaModule modeule, string name)
            : base(name)
        {
            _modeule = modeule;
        }

        public MetaModule Module
        {
            get { return _modeule; }
        }

        public static VoidMetaType Void
        {
            get { return s_void; }
        }

        public override void Accept<TContext>(IMetaObjectVisitor<TContext> metaObjectVisitor, TContext context)
        {
            metaObjectVisitor.VisitType(this, context);
        }
    }

    public abstract class PlainMetaType : MetaType
    {
        protected PlainMetaType(MetaModule module, string name)
            : base(module, name)
        {
        }
    }

    public sealed class VoidMetaType : PlainMetaType
    {
        internal VoidMetaType(MetaModule module)
            : base(module, "void")
        {
        }
    }

    public class NumericMetaType : PlainMetaType
    {
        protected NumericMetaType(MetaModule module, string name)
            : base(module, name)
        {
        }
    }

    public class ObjectMetaType : MetaType // Should be a proxy
    {
        protected ObjectMetaType(MetaModule module, string name)
            : base(module, name)
        {
        }
    }
}
