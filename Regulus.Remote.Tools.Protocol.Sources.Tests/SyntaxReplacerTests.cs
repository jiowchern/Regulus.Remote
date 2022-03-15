using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using System.Linq;

namespace Regulus.Remote.Tools.Protocol.Sources.Tests
{
    public class SyntaxReplacerTests
    {
        [Test]
        public void MethodVoid()
        {
            var source = @"
interface IA {
    void Method1();
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var root = com.SyntaxTrees[0].GetRoot();
            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);

            var replacer =  new SyntaxReplacer(cia);
            cia = replacer.Type;

            var exp = cia.DescendantNodes().OfType<BlockSyntax>().Single();
            NUnit.Framework.Assert.AreEqual(0, replacer.TypesOfSerialization.Count());            
            NUnit.Framework.Assert.False(builder.Expression.IsEquivalentTo(exp));
        }

        [Test]
        public void MethodInt()
        {
            var source = @"
interface IA {
    int Method1();
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var root = com.SyntaxTrees[0].GetRoot();
            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);

            var replacer = new SyntaxReplacer(cia);
            cia = replacer.Type;

            var exp = cia.DescendantNodes().OfType<BlockSyntax>().Single();
            NUnit.Framework.Assert.AreEqual(0, replacer.TypesOfSerialization.Count());
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exp));
        }

        [Test]
        public void MethodValue()
        {
            var source = @"

interface IA {
    Regulus.Remote.Value<int> Method1();
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var root = com.SyntaxTrees[0].GetRoot();
            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);

            var replacer = new SyntaxReplacer(cia);
            cia = replacer.Type;

            var exp = cia.DescendantNodes().OfType<BlockSyntax>().Single();
            NUnit.Framework.Assert.AreEqual(1, replacer.TypesOfSerialization.Count());
            NUnit.Framework.Assert.False(builder.Expression.IsEquivalentTo(exp));
        }

        [Test]
        public void MethodVoidParam1()
        {
            var source = @"
interface IA {
    void Method1(int i);
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var root = com.SyntaxTrees[0].GetRoot();
            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);

            var replacer = new SyntaxReplacer(cia);
            cia = replacer.Type;

            var exp = cia.DescendantNodes().OfType<BlockSyntax>().Single();
            NUnit.Framework.Assert.AreEqual(1, replacer.TypesOfSerialization.Count());
            NUnit.Framework.Assert.False(builder.Expression.IsEquivalentTo(exp));
        }
    }
}