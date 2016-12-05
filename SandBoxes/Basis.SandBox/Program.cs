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
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using Meticulous.Collections.Generic;
using Meticulous.Externals;
using Meticulous.Threading;
using Meticulous.IO;
using Meticulous.Meta;
using NLog;

[assembly: ExternalGroup("Asm")]

namespace Meticulous.SandBox
{

    internal class MetaTypePrinter : IMetaTypeVisitor<StringBuilder>
    {
        public string Visit(MetaModule module)
        {
            var sb = new StringBuilder();
            VisitModule(module, sb);

            return sb.ToString();
        }

        public void VisitClass(MetaClass metaClass, StringBuilder context)
        {
            var baseName = metaClass.Base != null ? " : " + metaClass.Base.Name : String.Empty;
            context.AppendLine("  " + metaClass.Name + baseName);

            foreach (var field in metaClass.Fields)
            {
                field.Accept(this, context);
            }

            foreach (var method in metaClass.Methods)
            {
                method.Accept(this, context);
            }

            foreach (var derivedClass in metaClass.DerivedClasses)
            {
                derivedClass.Accept(this, context);
            }
        }

        public void VisitInterface(MetaInterface @interface, StringBuilder context)
        {
            throw new NotImplementedException();
        }

        public void VisitModule(MetaModule metaModule, StringBuilder context)
        {
            context.AppendLine(metaModule.Name);
            foreach (IVisitableMetaType metaType in metaModule.Types)
            {
                metaType.Accept(this, context);
            }
            foreach (IVisitableMetaType module in metaModule.References)
            {
                module.Accept(this, context);
            }
        }

        public void VisitFunction(MetaFunction metaFunction, StringBuilder context)
        {
            context.Append("    -" + metaFunction.Name + "(");

            foreach (var parameter in metaFunction.Parameters)
            {
                parameter.Accept(this, context);
                context.Append(", ");
            }

            context.AppendLine("): " + metaFunction.ReturnType.Name + ";");
        }

        public void VisitParameter(MetaParameter metaParameter, StringBuilder context)
        {
            context.Append(metaParameter.Name + " : " + metaParameter.Type.Name);
        }

        public void VisitField(MetaField field, StringBuilder context)
        {
            context.AppendLine("    -" + field.Name + " : " + field.Type.Name + ";");
        }

        public void VisitPlainType(PlainMetaType type, StringBuilder context)
        {
            context.AppendLine("  " + type.Name + ";");
        }
    }


    internal class DummyDriver : ExternalDriver
    {
        protected override void ReadValue(IExternal value)
        {
            value.Value = "patched value";
        }

        protected override bool SupportsImpl(Type type, Uri uri)
        {
            if (uri.Scheme == "dummy")
                return true;

            return false;
        }
    }

    

    [ExternalGroup("Program")]
    internal class Program
    {
        [External("dummy://test")]
        private static string s_remoteName = "test";

        [External("dummy://testText")] private static External<string> s_remoteText;// = "test";

        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private class Context
        {
            private readonly Random _rnd;
            private readonly Size _size;

            private int _depth;
            private readonly int _maxDepth;

            public Context(Size size)
            {
                _rnd = new Random();
                _size = size;
                _maxDepth = _size.Height*_size.Width;
            }

            public Point[] GetPossibleMovements(int[,] ar, Point pos)
            {
                var moves = new List<Point>();
                
                var leftMove = new Point(pos.X - 1, pos.Y);
                if (leftMove.X >= 0 && ar[leftMove.X, leftMove.Y] == 0)
                    moves.Add(leftMove);

                var rightMove = new Point(pos.X + 1, pos.Y);
                if (rightMove.X < _size.Width && ar[rightMove.X, rightMove.Y] == 0)
                    moves.Add(rightMove);

                var upMove = new Point(pos.X, pos.Y - 1);
                if (upMove.Y >= 0 && ar[upMove.X, upMove.Y] == 0)
                    moves.Add(upMove);

                var downMove = new Point(pos.X, pos.Y + 1);
                if (downMove.Y < _size.Height && ar[downMove.X, downMove.Y] == 0)
                    moves.Add(downMove);

                return moves.OrderBy(x => _rnd.Next()).ToArray();
            }

            public bool IsDone
            {
                get { return _depth == _maxDepth; }
            }

            public void Apply(int[,] ar, Point newPos)
            {
                var newChar = NextChar();
                ar[newPos.X, newPos.Y] = newChar;
                ++_depth;
            }

            public void Restore(int[,] ar, Point oldPos)
            {
                ar[oldPos.X, oldPos.Y] = 0;
                --_depth;
            }

            private int NextChar()
            {
                return _depth + 1;
            }

            private void BackChar()
            {
            }
        }

        private static void FillArray(int[,] ar, int width, int height, int[] words)
        {
            var ctx = new Context(new Size(width, height));
            
            FillArrayImpl(ar, ctx, new Point(0, 0));
        }

        private static bool FillArrayImpl(int[,] ar, Context ctx, Point pos)
        {
            ctx.Apply(ar, pos);
            if (ctx.IsDone)
                return true;

            var moves = ctx.GetPossibleMovements(ar, pos);
            foreach (var move in moves)
            {
                if (FillArrayImpl(ar, ctx, move))
                    return true;
            }
            ctx.Restore(ar, pos);
            return false;
        }


        private static void PrintArray(int[,] ar, int width, int height)
        {
            for (int i = 0; i < width; ++i)
            {
                Console.Write("[");
                for (int j = 0; j < height; ++j)
                {
                    var value = ar[i, j];
                    Console.Write(value);
                    if (j != height - 1)
                        Console.Write(", ");
                }
                Console.WriteLine("]");
            }
        }

        private static int Main(string[] args)
        {
            var uri = new Uri("res://path/test?hello=123&w=23|{readonly}");
            var mgr = ExternalManager.Instance;
            mgr.Setup(ImmutableArray.Create(new DummyDriver()));

            Console.WriteLine(s_remoteName);
            Console.WriteLine(s_remoteText);


            //s_logger.Info("hello world");

            ////Parameter p;
            //var count = 7;
            //var ar = new int[count, count];

            //var words = new int[count];
            //for (var i = 0; i < words.Length; ++i)
            //{
            //    words[i] = i + 1;
            //}

            //PrintArray(ar, count, count);

            //Console.WriteLine();

            //FillArray(ar, count, count, words);

            //PrintArray(ar, count, count);

            //Console.ReadKey();
            ////return 0;


            //Assembly.GetAssembly();

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

                        }).AddMethod("Delete", deleteMethod =>
                        {
                            deleteMethod.ReturnType.SetType(MetaType.Void);
                        });
                    });
                })
                .AddDerivedClass("JsDirectory", dirBuilder =>
                {
                    
                });

            var module = moduleBuilder.Build();

            var printer = new MetaTypePrinter();

            var tree = printer.Visit(module);

            Console.Write(tree);

            Console.WriteLine("Press any key...");
            Console.ReadKey();

            Environment.ExitCode = 0;
            return 0;

        }
    }
}
