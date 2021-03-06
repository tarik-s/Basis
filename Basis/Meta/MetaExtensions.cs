﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticulous.Meta
{
    public static class MetaExtensions
    {
        public static MetaClassBuilder AddDerivedClass(this MetaClassBuilder @this, string name, Action<MetaClassBuilder> handler)
        {
            Check.This(@this);

            var builder = @this.AddDerivedClass(name);

            if (handler != null)
                handler(builder);

            return @this;
        }

        public static MetaClassBuilder AddMethod(this MetaClassBuilder @this, string name, Action<MetaFunctionBuilder> handler)
        {
            Check.This(@this);

            var builder = @this.AddMethod(name);

            if (handler != null)
                handler(builder);

            return @this;
        }

        public static MetaClassBuilder AddField(this MetaClassBuilder @this, string name, Action<MetaFieldBuilder> handler)
        {
            Check.This(@this);

            var builder = @this.AddField(name);

            if (handler != null)
                handler(builder);

            return @this;
        }
    }
}
