using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using System.Linq;

namespace Regulus.Remote.Tools.Protocol.Sources.Tests
{
    public class InheritanceBuilderTests
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
            
            var builder = new Regulus.Remote.Tools.Protocol.Sources.InheritanceBuilder(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);
            var member = cia.DescendantNodes().OfType<MethodDeclarationSyntax>().Single();            
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
            

            var builder = new Regulus.Remote.Tools.Protocol.Sources.InheritanceBuilder(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);
            var member = cia.DescendantNodes().OfType<IndexerDeclarationSyntax>().Single();

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
            

            var builder = new Regulus.Remote.Tools.Protocol.Sources.InheritanceBuilder(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());
            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);
            var member = cia.DescendantNodes().OfType<PropertyDeclarationSyntax>().Single();
            
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
            

            var builder = new Regulus.Remote.Tools.Protocol.Sources.InheritanceBuilder(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);
            var member = cia.DescendantNodes().OfType<EventDeclarationSyntax>().Single();            
            NUnit.Framework.Assert.AreEqual("IA", member.ExplicitInterfaceSpecifier.Name.ToFullString());           

        }



        [Test]
        public void ToInterfaceDeclarationTest()
        {
            var source = @"

public interface IA {
    void Method1();
}
";
          /*  var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var s = com.GetSemanticModel(tree).GetDeclaredSymbol(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());
            INamedTypeSymbol symbol = s as INamedTypeSymbol;
            var syntax = symbol.ToInterfaceDeclaration();

            var tree2 = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.Create(syntax);
            var com2 = tree.Compilation();

            var assembly = com2.ToAssembly();

            var cia = (from type in assembly.GetExportedTypes() where type.Name == "CIA" select type).Single();

            try
            {
                cia.GetMethod("Method1").Invoke(null, null);
            }
            catch (Regulus.Remote.Exceptions.NotSupportedException nse)
            {
                NUnit.Framework.Assert.Pass();
                return;
            }
            NUnit.Framework.Assert.Fail();*/

        }
    }
}