using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

using Regulus.Remote.Tools.Protocol.Sources.Extensions;

namespace Regulus.Remote.Tools.Protocol.Sources.Tests
{
    public class GhostBuilderTests
    {
        [Test]
        public void EssentialReferenceMissingTest()
        {
            var source = @"

namespace NS1
{
    public interface IB {
      void Method1();
    }
    namespace NS2
    {
        public interface IA : IB {
          void Method1();
        }
    }
    
}
";
            var syntaxBuilder =
                new Regulus.Remote.Tools.Protocol.Sources.SyntaxTreeBuilder(SourceText.From(source,
                    System.Text.Encoding.UTF8));

            var assemblyName = "TestProject";
            System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.MetadataReference> references = new Microsoft.CodeAnalysis.MetadataReference[]
            {                

            };
            CSharpCompilation compilation = CSharpCompilation.Create(assemblyName, new[] { syntaxBuilder.Tree }, references);


            try
            {
                new EssentialReference(compilation);
            }
            catch (MissingTypeException me)
            {

                NUnit.Framework.Assert.Pass();
                return;
            }

            NUnit.Framework.Assert.Fail();


        }
        [Test]
        public async Task InterfaceInheritMethodTest()
        {
            var source = @"

namespace NS1
{
    public interface IB {
      void Method1();
    }
    namespace NS2
    {
        public interface IA : IB {
          void Method1();
        }
    }
    
}
";
            var syntaxBuilder =
                new Regulus.Remote.Tools.Protocol.Sources.SyntaxTreeBuilder(SourceText.From(source,
                    System.Text.Encoding.UTF8));

            await new GhostTest(syntaxBuilder.Tree).RunAsync();
        }
        [Test]
        public async Task InterfaceInheritEventTest()
        {
            var source = @"

namespace NS1
{
    public interface IC {
        event System.Action<string> Event1;
    }
    public interface IB : IC{
        event System.Action<string> Event1;
    }
    namespace NS2
    {
        public interface IA : IB {
          event System.Action<string> Event1;
        }
    }
    
}
";
            var syntaxBuilder =
                new Regulus.Remote.Tools.Protocol.Sources.SyntaxTreeBuilder(SourceText.From(source,
                    System.Text.Encoding.UTF8));

            await new GhostTest(syntaxBuilder.Tree).RunAsync();
        }
        [Test]
        public async Task InterfaceInheritPropertyTest()
        {
            var source = @"

namespace NS1
{
    public interface IB {
        Regulus.Remote.Property<int> Property1{get;}
        Regulus.Remote.Notifier<NS2.IA> Property2{get;}
    }
    namespace NS2
    {
        public interface IA : IB {
            Regulus.Remote.Property<int> Property1{get;}
            Regulus.Remote.Notifier<NS2.IA> Property2{get;}
        }
    }
    
}
";
            var syntaxBuilder =
                new Regulus.Remote.Tools.Protocol.Sources.SyntaxTreeBuilder(SourceText.From(source,
                    System.Text.Encoding.UTF8));

            await new GhostTest(syntaxBuilder.Tree).RunAsync();
        }

        [Test]
        public async Task InterfaceTest()
        {
            var source = @"

namespace NS1
{
namespace NS2
{
public interface IA {
      
    }
}
    
}
";
            var syntaxBuilder =
                new Regulus.Remote.Tools.Protocol.Sources.SyntaxTreeBuilder(SourceText.From(source,
                    System.Text.Encoding.UTF8));

            await new GhostTest(syntaxBuilder.Tree).RunAsync();


        }
        [Test]
        public async Task InterfaceMethod1ParamTest()
        {
            var source = @"

namespace NS
{
    public interface IA {
      void Method1(int a,int b);
    }
}
";
            var syntaxBuilder =
                new Regulus.Remote.Tools.Protocol.Sources.SyntaxTreeBuilder(SourceText.From(source,
                    System.Text.Encoding.UTF8));
            await new GhostTest(syntaxBuilder.Tree).RunAsync();
        }

        [Test]
        public async Task InterfacePropertyTest()
        {
            var source = @"

namespace NS
{
    public interface IA {
      Regulus.Remote.Property<int> Property1 {get;}
      //  Regulus.Remote.Notifier<int> Property2 {get;}
    }
}
";
           
            var syntaxBuilder =
                new Regulus.Remote.Tools.Protocol.Sources.SyntaxTreeBuilder(SourceText.From(source,
                    System.Text.Encoding.UTF8));
            await new GhostTest(syntaxBuilder.Tree).RunAsync();
        }

        [Test]
        public async Task InterfacePropertyNotifierTest()
        {
            var source = @"
public interface IAA {
}
namespace NS
{


    public interface IA {
    
        Regulus.Remote.Notifier<IAA> Property2 {get;}
    }
}
";

            var syntaxBuilder =
                new Regulus.Remote.Tools.Protocol.Sources.SyntaxTreeBuilder(SourceText.From(source,
                    System.Text.Encoding.UTF8));
            await new GhostTest(syntaxBuilder.Tree).RunAsync();
        }

        [Test]
        public async Task InterfaceEventIntTest()
        {
            var source = @"

namespace NS
{
    public interface IA {
      event System.Action<int> Event1;
    }
}
";
            var syntaxBuilder =
                new Regulus.Remote.Tools.Protocol.Sources.SyntaxTreeBuilder(SourceText.From(source,
                    System.Text.Encoding.UTF8));
            await new GhostTest(syntaxBuilder.Tree).RunAsync();
        }

        [Test]
        public async Task InterfaceEventIntStringTest()
        {
            var source = @"

namespace NS
{
    public interface IA {
      event System.Action<int,string> Event1;
    }
}
";
            var syntaxBuilder =
                new Regulus.Remote.Tools.Protocol.Sources.SyntaxTreeBuilder(SourceText.From(source,
                    System.Text.Encoding.UTF8));
            await new GhostTest(syntaxBuilder.Tree).RunAsync();
        }
        [Test]
        public async Task InterfaceMethod1Test()
        {
            var source = @"

namespace NS
{
    public interface IA {
      void Method1();
    }
}
";
            var syntaxBuilder =
                new Regulus.Remote.Tools.Protocol.Sources.SyntaxTreeBuilder(SourceText.From(source,
                    System.Text.Encoding.UTF8));
            await new GhostTest(syntaxBuilder.Tree).RunAsync();
        }

        [Test]
        public async Task InterfaceMethod1ReturnIntTest()
        {
            var source = @"

namespace NS
{
    public interface IA {
      Regulus.Remote.Value<bool> Method1();
    }
}
";
            var syntaxBuilder =
                new Regulus.Remote.Tools.Protocol.Sources.SyntaxTreeBuilder(SourceText.From(source,
                    System.Text.Encoding.UTF8));
            await new GhostTest(syntaxBuilder.Tree).RunAsync();
        }

        [Test]
        public async Task NoNamespaceInterfaceTest()
        {

            var source = @"

namespace NS1{}
    public interface IA {
      
    }

";
            var syntaxBuilder =
                new Regulus.Remote.Tools.Protocol.Sources.SyntaxTreeBuilder(SourceText.From(source,
                    System.Text.Encoding.UTF8));

            await new GhostTest(syntaxBuilder.Tree).RunAsync();
        }

        [Test]
        public async Task TwoInterfaceTest()
        {

            var source = @"

namespace NS1{}
    public interface IA {
      
    }

public interface IB {
      
    }

";
            var syntaxBuilder =
                new Regulus.Remote.Tools.Protocol.Sources.SyntaxTreeBuilder(SourceText.From(source,
                    System.Text.Encoding.UTF8));

            await new GhostTest(syntaxBuilder.Tree).RunAsync();
        }



        [Test]
        public async Task GhostCompileTest()
        {

            var source = @"
using System;
namespace NS1
{    
    public interface IA 
    {
        void M123(int a);
    }
}

";
            
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();

            var interfaces = new System.Collections.Generic.HashSet<INamedTypeSymbol>((from syntaxTree in com.SyntaxTrees
                                                                                       let model = com.GetSemanticModel(syntaxTree)
                                                                                       from interfaneSyntax in syntaxTree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>()
                                                                                       let symbol = model.GetDeclaredSymbol(interfaneSyntax)
                                                                                       where symbol.IsGenericType == false
                                                                                       select symbol).SelectMany(s => s.AllInterfaces.Union(new[] { s })));


            var builders = new System.Collections.Generic.Dictionary<INamedTypeSymbol, InterfaceInheritor>(from i in interfaces
                                                                                            select new System.Collections.Generic.KeyValuePair<INamedTypeSymbol, InterfaceInheritor>(i, new InterfaceInheritor(i.ToInferredInterface())));
            var trees = new System.Collections.Generic.List<SyntaxTree>();
            foreach (var i in interfaces)
            {
                var name = $"C{i.ToDisplayString().Replace('.','_')}";
                var type = SyntaxFactory.ClassDeclaration(name);

                foreach (var i2 in i.AllInterfaces.Union(new[] { i }))
                {

                    var builder = builders[i2];
                    type = builder.Inherite(type);
                }

                

                var modifier = new SyntaxModifier(type);
                type = modifier.Type;

                type = type.ImplementRegulusRemoteIGhost();

                
                trees.Add(CSharpSyntaxTree.ParseText(type.NormalizeWhitespace().ToFullString()));
            }

            
            var ghostCom = HelperExt.Compilate(trees.Union(new[] { tree }).ToArray());

            var asm = ghostCom.ToAssembly();


            var eTypes = asm.GetTypes();
            var cia = (from t in eTypes where t.FullName == "CNS1_IA" select t).Single();
            var ciaCons = cia.GetConstructor(new[] { typeof(long), typeof(bool) });


            var instance = ciaCons.Invoke( new object[] { 1, false });
            var ghost = instance as Regulus.Remote.IGhost;
            object arg1 = null;
            ghost.CallMethodEvent += (mi, args, ret) => arg1 = args[0];

            
            var method = cia.GetMethod("NS1.IA.M123", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(ghost, new object[] { 1});


            NUnit.Framework.Assert.AreEqual(1, arg1);
        }
    }
}