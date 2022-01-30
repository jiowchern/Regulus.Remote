using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace Regulus.Remote.Tools.Protocol.Sources.Tests
{
    public class GhostBuilderTests
    {
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
    }
}