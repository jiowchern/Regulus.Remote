using NUnit.Framework;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Regulus.Remote.Tools.Protocol.Sources.Extensions;

namespace Regulus.Remote.Tools.Protocol.Sources.Tests
{
    public class SyntaxExtensionsTests
    {
        [Test]
        public void Name()
        {
            var source = @"
namespace N1
{
    namespace N2
    {
        interface IA 
        {    
        }
    }
    
}

";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var symbol = com.GetSemanticModel(tree).GetDeclaredSymbol(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var syntax = symbol.ToSyntax().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single();
            var type = syntax;

            NUnit.Framework.Assert.AreEqual("IA", type.Identifier.ToFullString());
        }

        [Test]
        public void Namespace()
        {
            var source = @"
namespace N1
{
    namespace N2
    {
        interface IA 
        {    
        }
    }
    
}

";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var symbol = com.GetSemanticModel(tree).GetDeclaredSymbol(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            
            var syntax = symbol.ToSyntax().DescendantNodes().OfType<NamespaceDeclarationSyntax>().Single();

            NUnit.Framework.Assert.AreEqual("N1.N2", syntax.Name.ToFullString());
        }

        [Test]
        public void NamespaceNoName()
        {
            var source = @"
interface IA 
        {    
        }
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var symbol = com.GetSemanticModel(tree).GetDeclaredSymbol(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());


            var syntax = symbol.ToSyntax().DescendantNodes().OfType<NamespaceDeclarationSyntax>().Single();

            NUnit.Framework.Assert.AreEqual("", syntax.Name.ToFullString());
        }


        [Test]
        public void Method1()
        {
            var source = @"

interface IA {
    void Method1();
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var symbol = com.GetSemanticModel(tree).GetDeclaredSymbol(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var syntax = symbol.ToSyntax();
            var method = syntax.DescendantNodes().OfType<MethodDeclarationSyntax>().Single();

            NUnit.Framework.Assert.AreEqual("Method1" , method.Identifier.ToString());
        }

        [Test]
        public void MethodParam()
        {
            var source = @"

interface IA {
    void Method1(int p1);
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
  
            var com = tree.Compilation();
            var symbol = com.GetSemanticModel(tree).GetDeclaredSymbol(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var syntax = symbol.ToSyntax();
  
            var parameter = syntax.DescendantNodes().OfType<ParameterSyntax>().Single();

            NUnit.Framework.Assert.AreEqual("p1", parameter.Identifier.ToString());
        }

        [Test]
        public void MethodReturn()
        {
            var source = @"

using System;

interface IA {
    IDisposable Method1(int p1);
}
";
            
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var symbol = com.GetSemanticModel(tree).GetDeclaredSymbol(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var syntax = symbol.ToSyntax();
            var method = syntax.DescendantNodes().OfType<MethodDeclarationSyntax>().Single();
            
            NUnit.Framework.Assert.AreEqual("System.IDisposable", method.ReturnType.ToFullString());
        }
    }
}