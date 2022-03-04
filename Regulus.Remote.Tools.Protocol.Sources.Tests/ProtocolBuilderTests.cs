using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace Regulus.Remote.Tools.Protocol.Sources.Tests
{
    public class ProtocolBuilderTests
    {
       

        [Test]
        public async Task ProtocolTest()
        {
            var source = @"
public interface IA
{
    
}
namespace NS1
{
    
    public class C1{}
    public interface IB
    {
       
    }
}

";
            var tree = CSharpSyntaxTree.ParseText(source);
           

            await new GhostTest(tree).RunAsync();
        }

        [Test]
        public async Task MemberMapInterfaceCodeBuilderTest()
        {
            var source = @"
public interface IA
    {
        
    }
namespace NS1
{
    
    public class C1{}
public interface IB
    {
       
    }
}

";
            var tree = CSharpSyntaxTree.ParseText(source);
            Compilation compilation = tree.Compilation();

            var builder = new MemberMapCodeBuilder(compilation);
            NUnit.Framework.Assert.AreEqual(@"new System.Tuple<System.Type, System.Func<Regulus.Remote.IProvider>>(typeof(global::IA),()=>new Regulus.Remote.TProvider<global::IA>()),new System.Tuple<System.Type, System.Func<Regulus.Remote.IProvider>>(typeof(global::NS1.IB),()=>new Regulus.Remote.TProvider<global::NS1.IB>())", builder.InterfacesCode);
        }

        [Test]
        public async Task MemberMapPropertyNotifierCodeBuilderTest()
        {
            var source = @"
    
    namespace NS1
    {
        
        
    
        
        public interface IB
        {
            Regulus.Remote.Property<IB> Property1{get;}
        }
    }

";
            var tree = CSharpSyntaxTree.ParseText(source);
            Compilation compilation = tree.Compilation();

            var builder = new MemberMapCodeBuilder(compilation);
            NUnit.Framework.Assert.AreEqual(@"typeof(global::NS1.IB).GetProperty(""Property1"")", builder.PropertyInfosCode);
        }

        [Test]
        public async Task MemberMapPropertyCodeBuilderTest()
        {
            var source = @"
    public interface IA
    {
        Regulus.Remote.Property<int> Property1{get;}
    }
    namespace NS1
    {
        
        
        public class C1
        {
        }
        
        public interface IB
        {
            Regulus.Remote.Property<C1> Property1{get;}
        }
    }

";
            var tree = CSharpSyntaxTree.ParseText(source);
            Compilation compilation = tree.Compilation();

            var builder = new MemberMapCodeBuilder(compilation);
            NUnit.Framework.Assert.AreEqual(@"typeof(global::IA).GetProperty(""Property1""),typeof(global::NS1.IB).GetProperty(""Property1"")", builder.PropertyInfosCode);
        }

        [Test]
        public async Task MemberMapEventCodeBuilderTest()
        {
            var source = @"
public interface IA
    {
        event System.Action Event1;
    }
namespace NS1
{
    
    public class C1{}
public interface IB
    {
        event System.Action<C1,string> Event1;
    }
}

";
            var tree = CSharpSyntaxTree.ParseText(source);
            Compilation compilation = tree.Compilation();

            var builder = new MemberMapCodeBuilder(compilation);
            NUnit.Framework.Assert.AreEqual(@"typeof(global::IA).GetEvent(""Event1""),typeof(global::NS1.IB).GetEvent(""Event1"")", builder.EventInfosCode);
        }

        [Test]
        public async Task MemberMapMethodCodeBuilderTest()
        {
            var source = @"
public interface IA
    {
        Regulus.Remote.Value<int> Method1(int a1,int a2);
    }
namespace NS1
{
    
    
public interface IB
    {
        Regulus.Remote.Value<int> Method1(int a1,int a2);
        Regulus.Remote.Value<int> Method2();
    }
}

";
            var tree = CSharpSyntaxTree.ParseText(source);
            Compilation compilation = tree.Compilation();
            //new Regulus.Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<System.Action<global::IA,int,int>>)((ins,_1,_2) => ins.Method(_1,_2))).Method,new Regulus.Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<System.Action<global::NS1.IB,int,int>>)((ins,_1,_2) => ins.Method(_1,_2))).Method
            var builder = new MemberMapCodeBuilder(compilation);
            NUnit.Framework.Assert.AreEqual("new Regulus.Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<System.Action<global::IA,int,int>>)((ins,_1,_2) => ins.Method1(_1,_2))).Method,new Regulus.Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<System.Action<global::NS1.IB,int,int>>)((ins,_1,_2) => ins.Method1(_1,_2))).Method,new Regulus.Utility.Reflection.TypeMethodCatcher((System.Linq.Expressions.Expression<System.Action<global::NS1.IB>>)((ins) => ins.Method2())).Method", builder.MethodInfosCode);
        }

        [Test]
        public async Task EventProviderCodeBuilderTest()
        {

            var source = @"
public interface IA
    {
        event System.Action<int> Event1;
        event System.Action Event2;
    }
namespace NS1
{
    
    
public interface IB
    {
        event System.Action<int> Event1;
    }
}

";
            var tree = CSharpSyntaxTree.ParseText(source);
            Compilation compilation = tree.Compilation();

            var interfaceMap = new EventProviderCodeBuilder(new GhostBuilder(compilation).Events);
            NUnit.Framework.Assert.AreEqual("new global::RegulusRemoteGhosts.IA_Event1(),new global::RegulusRemoteGhosts.IA_Event2(),new global::NS1.RegulusRemoteGhosts.IB_Event1()", interfaceMap.Code);
        }

        [Test]
        public async Task InterfaceProviderCodeBuilderTest()
        {
            var source = $@"
namespace NS1
{{
    
    public interface IA
    {{
    }}

    public interface IB
    {{
        Regulus.Remote.Property<IA> Property1 {{ get; }}
    }}
}}

";
            var tree = CSharpSyntaxTree.ParseText(source);
            Compilation compilation = tree.Compilation();
            
            var interfaceMap = new InterfaceProviderCodeBuilder(new GhostBuilder(compilation).Ghosts.Select(g => g.Syntax));
            NUnit.Framework.Assert.AreEqual("{typeof(global::NS1.IA),typeof(global::NS1.RegulusRemoteGhosts.CIA)},{typeof(global::NS1.IB),typeof(global::NS1.RegulusRemoteGhosts.CIB)}", interfaceMap.Code);
        }

        [Test]
        public async Task InterfaceInheritanceProviderCodeBuilderTest()
        {
            var source = $@"
namespace NS1
{{
    
    public interface IC
    {{
d
    }}
    public interface IA : IC
    {{
d
    }}

    public interface IB : IA
    {{
        Regulus.Remote.Property<IA> Property1 {{ get; }}
    }}
}}

";
            var tree = CSharpSyntaxTree.ParseText(source);
            Compilation compilation = tree.Compilation();

            var interfaceMap = new InterfaceProviderCodeBuilder(new GhostBuilder(compilation).Ghosts.Select(g=>g.Syntax));
            NUnit.Framework.Assert.AreEqual("{typeof(global::NS1.IC),typeof(global::NS1.RegulusRemoteGhosts.CIC)},{typeof(global::NS1.IA),typeof(global::NS1.RegulusRemoteGhosts.CIA)},{typeof(global::NS1.IB),typeof(global::NS1.RegulusRemoteGhosts.CIB)}", interfaceMap.Code);
        }

        [Test]
        public async Task SerializableExtractor3Test()
        {
            var source = @"
namespace NS1
{
    public class CReturn
    {
        public CReturn Field1;
    }
    public class CUseless
    {
        public CArg1 Field1;
    }
    public enum ENUM1
    {
        ITEM1,
    }
    public struct CArg3
    {
        public ENUM1 Field2;
        public string Field1;
    }
    public struct CArg2
    {
        public CArg2 Field3;
        public ENUM1 Field2;
        public string Field1;
    }
    public class CArg1
    {
        public CArg2 Field1;
    }
    public interface IB
    {
        event System.Action<float , CArg1,CArg3[][]> Event1;
        Regulus.Remote.Property<string> Property1 {get;}
    }
    public interface IA
    {
        Regulus.Remote.Value<CReturn> Method(CArg1 a1,int a2,System.Guid id);
        Regulus.Remote.Notifier<IB> Property1 {get;}
    }
}

";
            
            var tree = CSharpSyntaxTree.ParseText(source);
            Compilation compilation = tree.Compilation();
            var builder = new GhostBuilder(compilation);
            var symbols = new SerializableExtractor(new EssentialReference(compilation), builder.Ghosts ).Symbols.Select(s=>s.ToDisplayString());
           
            var cSymbols = new[]
            {
                "NS1.CReturn",
                "NS1.CArg1",
                "NS1.CArg2",
                "NS1.CArg3",
                "NS1.CArg3[]",
                "NS1.CArg3[][]",
                "int",
                "string",
                "System.Guid",
                "float",
                "NS1.ENUM1",
            };
           
            var count = symbols.Except(cSymbols).Count();
            NUnit.Framework.Assert.AreEqual(0, count);
        }


        [Test]
        public async Task SerializableExtractor1Test()
        {
            var source = @"
namespace NS1
{
    public struct C2
    {
        public int F1;
        public string F2;
        public float F3;
    }
    public class C1
    {
        public C2;
    }
    public interface IB{
        Regulus.Remote.Property<C1> Property1 {get;}
    }
    public interface IA
    {
        Regulus.Remote.Value<C2> Method(C2 a1 , C1 a2);
        Regulus.Remote.Notifier<IB> Property1 {get;}
    }
}

";
            var tree=CSharpSyntaxTree.ParseText(source);
            Compilation compilation =tree.Compilation();
            var builder = new GhostBuilder(compilation);
            var symbols = new SerializableExtractor(new EssentialReference(compilation), builder.Ghosts).Symbols;

            var cSymbols = new[]
            {
                
                compilation.GetTypeByMetadataName("NS1.C2"),
                compilation.GetTypeByMetadataName("NS1.C1")
            };
            var count = cSymbols.Except(symbols).Count();
            NUnit.Framework.Assert.AreEqual(0, count);

        }

        [Test]
        public async Task SerializableExtractor2Test()
        {
            var source = @"
namespace NS1
{
    public struct C2
    {
        
    }
    public class C1
    {
        public C2 Field1;
    }
    public interface IB{
        Regulus.Remote.Property<C1> Property1 {get;}
    }
    public interface IA
    {
        void Method( C1 a2);
        Regulus.Remote.Notifier<IB> Property1 {get;}
    }
}

";
            var tree = CSharpSyntaxTree.ParseText(source);
            Compilation compilation = tree.Compilation();
            var builder = new GhostBuilder(compilation);
            var  symbols = new SerializableExtractor(new EssentialReference(compilation), builder.Ghosts).Symbols;

            var cSymbols = new[]
            {
                compilation.GetTypeByMetadataName("NS1.C2"),
                compilation.GetTypeByMetadataName("NS1.C1")
            };
            var count = cSymbols.Except(symbols).Count();
            NUnit.Framework.Assert.AreEqual(0,count);

        }

        [Test]
        public void SerializableExtractor4Test()
        {
            var source = @"
namespace NS1
{
    
    public interface IB{
        Regulus.Remote.Property<int> Property1 {get;}
    }    
}
";
            var tree = CSharpSyntaxTree.ParseText(source);
            Compilation compilation = tree.Compilation();
            var builder = new GhostBuilder(compilation);

            var cSymbols = new[]
             {
                compilation.GetSpecialType(SpecialType.System_Int32)
            };

            var symbols = new SerializableExtractor(new EssentialReference(compilation), builder.Ghosts).Symbols;
            
            var count = cSymbols.Except(symbols).Count();
            NUnit.Framework.Assert.AreEqual(0, count);

        }

      


        [Test]
        public void SerializableExtractor6Test()
        {
            var source = @"
namespace Regulus.Remote.Tools.Protocol.Sources.TestCommon
{
    public static partial class ProtocolProvider 
    {
        public static Regulus.Remote.IProtocol CreateCase1()
        {
            Regulus.Remote.IProtocol protocol = null;
            _CreateCase1(ref protocol);
            return protocol;
        }

        [Remote.Protocol.Creator]
        static partial void _CreateCase1(ref Regulus.Remote.IProtocol protocol);


        public static IProtocol CreateCase2()
        {
            IProtocol protocol = null;
            _CreateCase2(ref protocol);
            return protocol;
        }

        [Remote.Protocol.Creator]
        static partial void _CreateCase2(ref IProtocol protocol);
    }
}

";
            var tree = CSharpSyntaxTree.ParseText(source);
            Compilation compilation = tree.Compilation();

            var builder = new GhostBuilder(compilation);
            var symbols = new SerializableExtractor(new EssentialReference(compilation), builder.Ghosts).Symbols;



            NUnit.Framework.Assert.AreEqual(0, symbols.Count);

        }
    }


}