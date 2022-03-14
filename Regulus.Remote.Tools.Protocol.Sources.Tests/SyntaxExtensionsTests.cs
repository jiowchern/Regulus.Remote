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


            var syntax = symbol.ToSyntax().DescendantNodes().OfType<NamespaceDeclarationSyntax>().Any();

            NUnit.Framework.Assert.AreEqual(false, syntax);
        }


        [Test]
        public void MethodIdentifier()
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

        [Test]
        public void MethodRefParam()
        {
            var source = @"

using System;

interface IA {
    IDisposable Method1(ref IDisposable p1);
}
";

            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var symbol = com.GetSemanticModel(tree).GetDeclaredSymbol(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var syntax = symbol.ToSyntax();
            
            var parameter = syntax.DescendantNodes().OfType<ParameterSyntax>().Single();

            NUnit.Framework.Assert.AreEqual("ref", parameter.Modifiers[0].Text);
            
            NUnit.Framework.Assert.AreEqual("p1", parameter.Identifier.ToString());

        }

      


        [Test]
        public void PropertyIdentifier()
        {
            var source = @"
interface IA {
    int Property1 {get ;};
}
";

            
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            
            var symbol = com.GetSemanticModel(tree).GetDeclaredSymbol(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var syntax = symbol.ToSyntax();
            var member = syntax.DescendantNodes().OfType<PropertyDeclarationSyntax>().Single();

            var accessor = member.DescendantNodes().OfType<AccessorDeclarationSyntax>().Single();
            
             

            NUnit.Framework.Assert.AreEqual("Property1", member.Identifier.ToFullString());
            NUnit.Framework.Assert.AreEqual(SyntaxKind.GetAccessorDeclaration, accessor.Kind());
        }

        [Test]
        public void Event()
        {

        var source = @"
using System;
namespace N1
{
    public delegate IDisposable TestDelegate(int a);
}

interface IA
{
    event N1.TestDelegate Event1;
}   
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();

            var symbol = com.GetSemanticModel(tree).GetDeclaredSymbol(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var syntax = symbol.ToSyntax();

            var member = syntax.DescendantNodes().OfType<EventFieldDeclarationSyntax>().Single();
            
            NUnit.Framework.Assert.AreEqual("Event1", member.Declaration.Variables[0].Identifier.ToFullString());

            NUnit.Framework.Assert.AreEqual("N1.TestDelegate", member.Declaration.Type.ToFullString());

        }

        [Test]
        public void Index()
        {

            var source = @"
    interface IA
    {
        string this[int index]
        {
            get;
            set;
        }
    }
}   
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var nodes = tree.GetRoot().DescendantNodes().ToArray();
            var com = tree.Compilation();



            var symbol = com.GetSemanticModel(tree).GetDeclaredSymbol(tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var syntax = symbol.ToSyntax();

            var member = syntax.DescendantNodes().OfType<IndexerDeclarationSyntax>().Single();
            NUnit.Framework.Assert.AreEqual("string", member.Type.ToFullString());
            



        }


    }

    
}