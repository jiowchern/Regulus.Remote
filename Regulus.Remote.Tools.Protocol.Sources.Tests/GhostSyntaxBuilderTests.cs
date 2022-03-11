using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using System.Linq;

namespace Regulus.Remote.Tools.Protocol.Sources.Tests
{
   
    public class GhostSyntaxBuilderTests
    {
     

        [Test]
        public void Method1()
        {
            var source = @"

interface IA {
    void Method1();
}
";            
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);


            var builder = new Regulus.Remote.Tools.Protocol.Sources.GhostSyntaxBuilder(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());
            var member = builder.Ghost.DescendantNodes().OfType<MethodDeclarationSyntax>().Single();            
            NUnit.Framework.Assert.AreEqual("IA", member.ExplicitInterfaceSpecifier.Name.ToFullString());

            

        }

        [Test]
        public void Index1()
        {
            var source = @"

interface IA {
    string this[int index]
        {
            get;
            set;
        }
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            

            var builder = new Regulus.Remote.Tools.Protocol.Sources.GhostSyntaxBuilder(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());
            var member = builder.Ghost.DescendantNodes().OfType<IndexerDeclarationSyntax>().Single();
            NUnit.Framework.Assert.AreEqual("IA", member.ExplicitInterfaceSpecifier.Name.ToFullString());

        }

        [Test]
        public void Property1()
        {
            var source = @"

interface IA {
    int Property1 {get;private set;}
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            

            var builder = new Regulus.Remote.Tools.Protocol.Sources.GhostSyntaxBuilder(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var member = builder.Ghost.DescendantNodes().OfType<PropertyDeclarationSyntax>().Single();
            NUnit.Framework.Assert.AreEqual("IA", member.ExplicitInterfaceSpecifier.Name.ToFullString());
            

        }

        [Test]
        public void Event1()
        {
            var source = @"


interface IA {
    event System.Action<int> Event1;
}


";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            

            var builder = new Regulus.Remote.Tools.Protocol.Sources.GhostSyntaxBuilder(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var member = builder.Ghost.DescendantNodes().OfType<EventDeclarationSyntax>().Single();
            NUnit.Framework.Assert.AreEqual("IA", member.ExplicitInterfaceSpecifier.Name.ToFullString());           

        }
    }
}