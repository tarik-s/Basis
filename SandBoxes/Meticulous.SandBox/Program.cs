using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Diagnostics.Contracts;
using System.Reflection;
using Meticulous.Collections.Generic;
using Meticulous.Threading;
using Meticulous.IO;
using Meticulous.Meta;

namespace Meticulous.SandBox
{

    internal class MetaObjectPrinter : MetaObjectVisitor<StringBuilder>
    {
        public string Visit(MetaModule module)
        {
            var sb = new StringBuilder();
            VisitModule(module, sb);

            return sb.ToString();
        }

        public override void VisitClass(MetaClass metaClass, StringBuilder context)
        {
            var baseName = metaClass.BaseClass != null ? ":" + metaClass.BaseClass.Name : String.Empty;
            context.AppendLine("  " + metaClass.Name + baseName);

            foreach (var derivedClass in metaClass.DerivedClasses)
            {
                derivedClass.Accept(this, context);
            }

            foreach (var field in metaClass.Fields)
            {
                field.Accept(this, context);
            }
        }

        public override void VisitModule(MetaModule metaModule, StringBuilder context)
        {
            context.AppendLine(metaModule.Name);
            foreach (IMetaObjectVisitable metaClass in metaModule.Classes)
            {
                metaClass.Accept(this, context);
            }
            foreach (IMetaObjectVisitable module in metaModule.References)
            {
                module.Accept(this, context);
            }
        }

        public override void VisitMethod(MetaMethod metaMethod, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public override void VisitParameter(MetaParameter metaParameter, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public override void VisitField(MetaField metaMethod, StringBuilder context)
        {
            throw new NotImplementedException();
        }
    }

    internal class Program
    {
 
        private static int Main(string[] args)
        {
            var baseModuleBuilder = new MetaModuleBuilder("BaseModule");

            var classBuilder = baseModuleBuilder.AddClass("JsObject");

            var baseModule = baseModuleBuilder.Build();

            var classes = baseModule.Classes;

            var moduleBuilder = new MetaModuleBuilder("ChildModule");

            moduleBuilder.AddReference(baseModule);

            moduleBuilder.AddClass("JsFileSystemObject", classes[0])
                .AddDerivedClass("JsFile", fileClass =>
                {
                    fileClass.AddField("size", fileField =>
                    {

                    }).AddDerivedClass("JsFileLink", fileLinkClass =>
                    {
                        fileLinkClass.AddField("originalFilePath", fileLinkField =>
                        {

                        }).AddField("linkSize", fileLinkField =>
                        {

                        });
                    });
                })
                .AddDerivedClass("JsDirectory", dirBuilder =>
                {
                    
                });

            var module = moduleBuilder.Build();

            var printer = new MetaObjectPrinter();

            var tree = printer.Visit(module);

            Console.Write(tree);

            Console.WriteLine("Press any key...");
            Console.ReadKey();

            Environment.ExitCode = 0;
            return 0;

        }
    }
}
