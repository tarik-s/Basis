using System;
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
    class Base
    {
        public static string MakeString(Base b)
        {
            return null;
        }
    }


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

        public override void VisitField(MetaField metaMethod, StringBuilder context)
        {
            throw new NotImplementedException();
        }
    }

    internal class Program
    {
        static Func<object, string> CreateFastConverter()
        {
            var mi = typeof(Base).GetMethod("MakeString");

            var objArg = Expression.Parameter(typeof(Object), "obj");


            var arg = Expression.Convert(objArg, typeof(Base));
            //var arg = Expression.TypeAs(objArg, typeof(Base));
            var callExpr = Expression.Call(mi, arg);
                        
            var lambdaExpr = Expression.Lambda<Func<object, string>>(callExpr, objArg);

            Console.WriteLine(lambdaExpr);

            var lambda = lambdaExpr.Compile();
            return lambda;
        }

        static Func<object, string> CreateDirectConverter()
        {
            return obj => Base.MakeString((Base)obj);
        }


        static Func<object, string> CreateSlowConverter()
        {
            var mi = typeof(Base).GetMethod("MakeString");

            return b => (string) mi.Invoke(null, new object[] { b });
        }

        private static TimeSpan TestConverter(Func<Base, string> converter)
        {
            var b = new Base();
            var sw = Stopwatch.StartNew();


            for (int i = 0; i < 10000000; ++i)
            {
                converter(b);
            }

            sw.Stop();
            return sw.Elapsed;
        }

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

            var slowCvt = CreateSlowConverter();
            var fastCvt = CreateFastConverter();
            var directCvt = CreateDirectConverter();

            var slowTest = TestConverter(slowCvt);
            var fastTest = TestConverter(fastCvt);
            var directTest = TestConverter(directCvt);

            Console.WriteLine("Slow  : " + slowTest.Ticks);
            Console.WriteLine("Fast  : " + fastTest.Ticks);
            Console.WriteLine("Direct: " + directTest.Ticks);

            Console.ReadKey();
            return 1;

            var result = RunLoop.RunMain(MainImpl);
            //var q = ExecutionQueue.Create(ExecutionQueueProcessorType.ThreadPool);

            Console.WriteLine("Finshed with code: " + result);
            Console.WriteLine("Press any key...");
            Console.ReadKey();

            Environment.ExitCode = result;
            return result;

        }

        private static async Task TestAsync()
        {
            Console.WriteLine("#1: " + Thread.CurrentThread.ManagedThreadId);
            await Task.Delay(1000);
            Console.WriteLine("#2: " + Thread.CurrentThread.ManagedThreadId);
            RunLoop.MainLoop.Stop(123);
            await Task.Delay(1000);
            Console.WriteLine("#3: " + Thread.CurrentThread.ManagedThreadId);
        }


        private static void MainImpl()
        {
            TestAsync().Wait();
        }
    }
}
