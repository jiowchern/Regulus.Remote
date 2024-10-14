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
                new EssentialReference(compilation,null);
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

            await new GhostTest( syntaxBuilder.Tree).RunAsync();
        }

        [Test]
        public async Task SerializableTypesTest()
        {
            var source = @"

namespace NS1
{
public enum ENUM1
{
    ITEM1
}
    
    public interface IB {
        Regulus.Remote.Value<ENUM1[]> Method0(System.Int32 p1,int p2);
        
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

            await new GhostTest( syntaxBuilder.Tree).RunAsync();
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

namespace NS2
{
    public class Test
    {
    }

}
namespace NS
{
    
    public interface IA {
      void Method1(int a,int b);
      event System.Action<NS2.Test> Event1;
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
            await new GhostTest( syntaxBuilder.Tree).RunAsync();
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
        
        public void GhostBuilderUnprocessedsTest()
        {
            var source = @"
using System;
using Regulus.Remote;
namespace NS1
{    
    delegate void GhostTest(int val);
    public interface IA 
    {
        int UnprocessedMethod();
        Value<int> ProcessedMethod();
        string this[int index]
        {
            get;
            set;
        }

        event GhostTest Event1;
        event System.Action Event2;

        int Property1 {get;set;}
        Regulus.Remote.Property<int> Property2 {get;set;}
        
    }
}

";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            SyntaxModifier modifier = SyntaxModifier.Create(com);
            var builder = new GhostBuilder(modifier, com.FindAllInterfaceSymbol());            

            var cnt = builder.ClassAndTypess.First();
            var methods = cnt.GetSyntaxs<MethodDeclarationSyntax>().ToArray();
            var indexs = cnt.GetSyntaxs<IndexerDeclarationSyntax>().ToArray();
            var events = cnt.GetSyntaxs<EventDeclarationSyntax>().ToArray();
            var propertys = cnt.GetSyntaxs<PropertyDeclarationSyntax>().ToArray();
            NUnit.Framework.Assert.AreEqual(1, methods.Count());
            NUnit.Framework.Assert.AreEqual(2, indexs.Count());
            NUnit.Framework.Assert.AreEqual(2, events.Count());
            NUnit.Framework.Assert.AreEqual(3, propertys.Count());


            var dialogProvider = new DialogProvider();
            var dialogs = dialogProvider.Unsupports(builder.ClassAndTypess).ToArray();
            NUnit.Framework.Assert.AreEqual(8, dialogs.Count());
        }


        [Test]
        public void GhostBuilderCompileTest()
        {
            var source = @"
using System;
namespace NS1
{    
    public struct Effect{
        public int Value;
    }
    public struct Item{
        public Effect[] Effects;
    }
    public interface IB 
    {
        void IBM1();
        event Action Event2;
        event Action<Item[]> Event3;
    }

    public interface IA :IB 
    {
        void M123(int a);

        int NoSupple(int a);
     
        event Action<int> Event1;

        void MethodRefTest(ref int aa);
    }
}

";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            SyntaxModifier modifier = SyntaxModifier.Create(com);
            var builder = new GhostBuilder(modifier , com.FindAllInterfaceSymbol());
            var trees = builder.Ghosts.Union(builder.EventProxys).Select( c=> CSharpSyntaxTree.ParseText(c.NormalizeWhitespace().ToFullString()));

            var ghostCom = HelperExt.Compile(trees.Union(new[] { tree }));

            var asm = ghostCom.ToAssembly();

            var eTypes = asm.GetTypes();
            var cia = (from t in eTypes where t.FullName == "CNS1_IA" select t).Single();
            var ciaCons = cia.GetConstructor(new[] { typeof(long), typeof(bool) });


            var instance = ciaCons.Invoke(new object[] { 1, false });
            var ghost = instance as Regulus.Remote.IGhost;


            System.Collections.Generic.List<object> values = new System.Collections.Generic.List<object>();
            ghost.CallMethodEvent += (mi, args, ret) => values.Add(args[0])  ;


            var m123 = cia.GetMethod("NS1.IA.M123", BindingFlags.Instance | BindingFlags.NonPublic);
            m123.Invoke(ghost, new object[] { 1 });

            var methodRefTest = cia.GetMethod("NS1.IA.MethodRefTest", BindingFlags.Instance | BindingFlags.NonPublic);
            methodRefTest.Invoke(ghost, new object[] { 2 });



            var noSupple = cia.GetMethod("NS1.IA.NoSupple", BindingFlags.Instance | BindingFlags.NonPublic);

            bool hasException = false;
            try
            {
                noSupple.Invoke(ghost, new object[] { 1 });
            }
            catch (System.Reflection.TargetInvocationException tie)
            {

                hasException = tie.InnerException.GetType() == typeof(Regulus.Remote.Exceptions.NotSupportedException);
            }
            NUnit.Framework.Assert.True(hasException);
            NUnit.Framework.Assert.AreEqual(1, values[0]);
            NUnit.Framework.Assert.AreEqual(2, values[1]);
            
        }

        [Test]
        public async Task ProjectSourceBuilderTest()
        {

            var source = @"
using System;
using Regulus.Remote;

namespace NS1
{    
    public struct Struct2
    {
        public string Field1;
    }

    public struct Struct1
    {
        public Struct2 Field1;
    }
    public delegate void NoSuppleDelegate(int a);
    public interface IB
    {
        event System.Action<int> Event22;
        int NoSuppleProperty {get; set; }
    }
    public interface IA  :IB
    {
        Value<Struct1[]> M123(int a);

        int NoSupple(int a);
        Regulus.Remote.Property<int> Property1{get;}
        Regulus.Remote.Notifier<IB> Property2{get;}
        event System.Action<int> Event1;
        event NoSuppleDelegate NoSuppleEvent;        
        
    }

    
}

 public static partial class ProtocolProvider 
 {
        public static Regulus.Remote.IProtocol CreateCase1()
        {
            Regulus.Remote.IProtocol protocol = null;
            _CreateCase1(ref protocol);
            return protocol;
        }

        [Regulus.Remote.Protocol.Creater]
        static partial void _CreateCase1(ref Regulus.Remote.IProtocol protocol);    
}
";

            
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();

            var builder = new ProjectSourceBuilder(new EssentialReference(com,null));

            
            var ghostCom = HelperExt.Compile(builder.Sources.Union(new[] { tree }));

            var asm = ghostCom.ToAssembly();


            var eTypes = asm.GetTypes();
            var cia = (from t in eTypes where t.Name == "CNS1_IA" select t).Single();
            var ciaCons = cia.GetConstructor(new[] { typeof(long), typeof(bool) });


            var instance = ciaCons.Invoke( new object[] { 1, false });
            var ghost = instance as Regulus.Remote.IGhost;
            object arg1 = null;
            ghost.CallMethodEvent += (mi, args, ret) => arg1 = args[0];

            
            var m123 = cia.GetMethod("NS1.IA.M123", BindingFlags.Instance | BindingFlags.NonPublic);
            m123.Invoke(ghost, new object[] { 1});


            var noSupple = cia.GetMethod("NS1.IA.NoSupple", BindingFlags.Instance | BindingFlags.NonPublic);

            

            ThrowTest(()=> noSupple.Invoke(ghost, new object[] { 1 }));
            
            NUnit.Framework.Assert.AreEqual(1, arg1);
        }

        void ThrowTest(System.Action action)
        {
            NUnit.Framework.Assert.Throws<Regulus.Remote.Exceptions.NotSupportedException>(() => Throw(action));
        }
        void Throw(System.Action action)
        {
            try
            {
                action();
            }
            catch (System.Reflection.TargetInvocationException tie)
            {

                throw tie.InnerException as Regulus.Remote.Exceptions.NotSupportedException;
            }
        }

       
    }

    
}