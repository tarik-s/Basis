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

        #region Built-in types

        public static VoidMetaType Void
        {
            get { return MetaModule.Core.Void; }
        }

        public static BooleanMetaType Boolean
        {
            get { return MetaModule.Core.Boolean; }
        }

        public static IntegerMetaType Integer
        {
            get { return MetaModule.Core.Integer; }
        }

        public static FloatMetaType Float
        {
            get { return MetaModule.Core.Float; }
        }

        public static StringMetaType String
        {
            get { return MetaModule.Core.String; }
        }

        #endregion

        public string Name
        {
            get { return _name; }
        }

        public abstract MetaModule Module { get; }

        public abstract void Accept<TContext>(IMetaTypeVisitor<TContext> metaObjectVisitor, TContext context);
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

        public override void Accept<TContext>(IMetaTypeVisitor<TContext> metaObjectVisitor, TContext context)
        {
            metaObjectVisitor.VisitPlainType(this, context);
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
        internal BooleanMetaType(CoreMetaModule module)
            : base(module, "bool")
        {
        }
    }

    public sealed class IntegerMetaType : NumericMetaType
    {
        internal IntegerMetaType(CoreMetaModule module)
            : base(module, "int")
        {
        }
    }

    public sealed class FloatMetaType : NumericMetaType
    {
        internal FloatMetaType(CoreMetaModule module)
            : base(module, "float")
        {
        }
    }

    public sealed class StringMetaType : PlainMetaType
    {
        internal StringMetaType(CoreMetaModule module)
            : base(module, "string")
        {
        }
    }
}
