using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    [DisplayName("{Name}")]
    public abstract class MetaType : IVisitableMetaType
    {
        private readonly string _name;

        protected MetaType(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public abstract MetaModule Module { get; }

        public virtual void Accept<TContext>(IMetaTypeVisitor<TContext> metaObjectVisitor, TContext context)
        {
            metaObjectVisitor.VisitType(this, context);
        }
    }

    public abstract class PlainMetaType : MetaType
    {
        private readonly CoreMetaModule _module;
        protected PlainMetaType(CoreMetaModule module, string name)
            : base(name)
        {
            _module = module;
        }

        public override MetaModule Module
        {
            get { return _module; }
        }
    }

    public sealed class VoidMetaType : PlainMetaType
    {
        internal VoidMetaType(CoreMetaModule module)
            : base(module, "void")
        {
        }
    }

    public class NumericMetaType : PlainMetaType
    {
        protected NumericMetaType(CoreMetaModule module, string name)
            : base(module, name)
        {
        }
    }

    public sealed class BooleanMetaType : NumericMetaType
    {
        public BooleanMetaType(CoreMetaModule module)
            : base(module, "bool")
        {
        }
    }


}
