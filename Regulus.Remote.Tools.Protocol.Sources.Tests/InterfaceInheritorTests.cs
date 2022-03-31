using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using System.Linq;

namespace Regulus.Remote.Tools.Protocol.Sources.Tests
{
    public class InterfaceInheritorTests
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
            
            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            
            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);
            var member = cia.DescendantNodes().OfType<MethodDeclarationSyntax>().Single();            
            NUnit.Framework.Assert.AreEqual("IA", member.ExplicitInterfaceSpecifier.Name.ToFullString());

            var exp = member.DescendantNodes().OfType<BlockSyntax>().Single();            
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exp));
        }

        [Test]
        public void NamespaceMethod()
        {
            var source = @"
namespace N2
{
    namespace N1
    {
        interface IA {
            void Method1();
        }
    }
}


";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);

            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);
            var member = cia.DescendantNodes().OfType<MethodDeclarationSyntax>().Single();
            NUnit.Framework.Assert.AreEqual("N2.N1.IA", member.ExplicitInterfaceSpecifier.Name.ToFullString());

            var exp = member.DescendantNodes().OfType<BlockSyntax>().Single();
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exp));
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

            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);
            var member = cia.DescendantNodes().OfType<IndexerDeclarationSyntax>().Single();

            NUnit.Framework.Assert.AreEqual("IA", member.ExplicitInterfaceSpecifier.Name.ToFullString());

            var exps = member.DescendantNodes().OfType<BlockSyntax>().ToArray();
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exps[0]));
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exps[1]));

        }

        [Test]
        public void Property1()
        {
            var source = @"

interface IA {
    int Property1 {get; set;}
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            
            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());
            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);
            var member = cia.DescendantNodes().OfType<PropertyDeclarationSyntax>().Single();
            
            NUnit.Framework.Assert.AreEqual("IA", member.ExplicitInterfaceSpecifier.Name.ToFullString());
            var exps = member.DescendantNodes().OfType<BlockSyntax>().ToArray();
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exps[0]));
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exps[1]));
        }

        [Test]
        public void Base1()
        {
            var source = @"

interface IA {    
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);


            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());
            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);

            var base1 = cia.BaseList.Types[0];
            NUnit.Framework.Assert.AreEqual("IA", base1.ToFullString());
        }

        


        [Test]
        public void Property2()
        {
            var source = @"

interface IA {
    int Property1 {get; }
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);


            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());
            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);
            var member = cia.DescendantNodes().OfType<PropertyDeclarationSyntax>().Single();

            NUnit.Framework.Assert.AreEqual("IA", member.ExplicitInterfaceSpecifier.Name.ToFullString());
            var exp = member.DescendantNodes().OfType<BlockSyntax>().Single();
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exp));
            

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
            

            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);
            var member = cia.DescendantNodes().OfType<EventDeclarationSyntax>().Single();            
            NUnit.Framework.Assert.AreEqual("IA", member.ExplicitInterfaceSpecifier.Name.ToFullString());
            NUnit.Framework.Assert.AreEqual("System.Action<int> ", member.Type.ToFullString());

            var exps = member.DescendantNodes().OfType<BlockSyntax>().ToArray();
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exps[0]));
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exps[1]));

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