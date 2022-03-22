using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Regulus.Remote.Tools.Protocol.Sources.Tests
{    
    public class SyntaxModifierTests
    {
        
        [Test]
        public void ZeroProperty()
        {
            var source = @"
interface IA {
    System.Int32 Property1 {get;}
    Regulus.Remote.Property<System.Int32> Property2 {set;}
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var root = com.SyntaxTrees[0].GetRoot();
            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);

            var modifier = SyntaxModifier.Create(com).Mod(cia);
            

            var exps = modifier.Type.DescendantNodes().OfType<BlockSyntax>().ToArray();
            NUnit.Framework.Assert.AreEqual(0, modifier.TypesOfSerialization.Count());
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exps[0]));
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exps[1]));
            NUnit.Framework.Assert.AreEqual(0, modifier.Type.DescendantNodes().OfType<FieldDeclarationSyntax>().Count());


        }


        [Test]
        public void PropertyPropertyTrue()
        {
            var source = @"
interface IA {
    Regulus.Remote.Property<System.Int32> Property1 {get;}
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var root = com.SyntaxTrees[0].GetRoot();
            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);

            var modifier = SyntaxModifier.Create(com).Mod(cia);

            var exp = modifier.Type.DescendantNodes().OfType<BlockSyntax>().Single();
            NUnit.Framework.Assert.AreEqual(1, modifier.TypesOfSerialization.Count());
            NUnit.Framework.Assert.False(builder.Expression.IsEquivalentTo(exp));
            NUnit.Framework.Assert.AreEqual(1, modifier.Type.DescendantNodes().OfType<FieldDeclarationSyntax>().Count());


        }

        [Test]
        public void PropertyPropertyFail()
        {
            var source = @"
interface IA {
    Regulus.Remote.Property<IA> Property1 {get;}
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var root = com.SyntaxTrees[0].GetRoot();
            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);

            var modifier = SyntaxModifier.Create(com).Mod(cia);

            var exp = modifier.Type.DescendantNodes().OfType<BlockSyntax>().Single();
            NUnit.Framework.Assert.AreEqual(0, modifier.TypesOfSerialization.Count());
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exp));
            NUnit.Framework.Assert.AreEqual(0, modifier.Type.DescendantNodes().OfType<FieldDeclarationSyntax>().Count());


        }

        [Test]
        public void PropertyNotifierFail()
        {
            var source = @"
interface IA {
    Regulus.Remote.Notifier<System.Int32> Property1 {get;}
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var root = com.SyntaxTrees[0].GetRoot();
            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);

            var modifier = SyntaxModifier.Create(com).Mod(cia);

            var exp = modifier.Type.DescendantNodes().OfType<BlockSyntax>().Single();
            NUnit.Framework.Assert.AreEqual(0, modifier.TypesOfSerialization.Count());
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exp));
            NUnit.Framework.Assert.AreEqual(0, modifier.Type.DescendantNodes().OfType<FieldDeclarationSyntax>().Count());


        }

        [Test]
        public void PropertyNotifierSuccess()
        {
            var source = @"
interface IA {
    Regulus.Remote.Notifier<IA> Property1 {get;}
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var root = com.SyntaxTrees[0].GetRoot();
            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);

            var modifier = SyntaxModifier.Create(com).Mod(cia);

            var exp = modifier.Type.DescendantNodes().OfType<BlockSyntax>().Single();
            NUnit.Framework.Assert.AreEqual(0, modifier.TypesOfSerialization.Count());
            NUnit.Framework.Assert.False(builder.Expression.IsEquivalentTo(exp));
            NUnit.Framework.Assert.AreEqual(1, modifier.Type.DescendantNodes().OfType<FieldDeclarationSyntax>().Count());


        }


        [Test]
        public void ZeroEvent()
        {
            var source = @"
interface IA {
    event System.Func<int> Event1;
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var root = com.SyntaxTrees[0].GetRoot();
            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);

            var modifier = SyntaxModifier.Create(com).Mod(cia);

            var exps = modifier.Type.DescendantNodes().OfType<BlockSyntax>().ToArray();
            NUnit.Framework.Assert.AreEqual(0, modifier.TypesOfSerialization.Count());
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exps[0]));
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exps[1]));
        }
        [Test]
        public void EventActionInt()
        {
            var source = @"
interface IA {
    event System.Action<int> Event1;
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var root = com.SyntaxTrees[0].GetRoot();
            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);

            var modifier = new SyntaxModifier(
                    new BlockModifiers.MethodVoid(),
                    new BlockModifiers.MethodRegulusRemoteValue(),
                    new BlockModifiers.EventSystemAction(),
                    new BlockModifiers.PropertyRegulusRemoteBlock(com),
                    new Modifiers.EventFieldDeclarationSyntax(),
                    new Modifiers.PropertyFieldDeclarationSyntax()
                ).Mod(cia);

            var exps = modifier.Type.DescendantNodes().OfType<BlockSyntax>().ToArray();
            NUnit.Framework.Assert.AreEqual(1, modifier.TypesOfSerialization.Count());
            NUnit.Framework.Assert.False(builder.Expression.IsEquivalentTo(exps[0]));
            NUnit.Framework.Assert.False(builder.Expression.IsEquivalentTo(exps[1]));

        }
        [Test]
        public void EventAction()
        {
            var source = @"
interface IA {
    event System.Action Event1;
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var root = com.SyntaxTrees[0].GetRoot();
            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);

            var modifier = SyntaxModifier.Create(com).Mod(cia);

            var exps = modifier.Type.DescendantNodes().OfType<BlockSyntax>().ToArray();
            NUnit.Framework.Assert.AreEqual(0, modifier.TypesOfSerialization.Count());
            NUnit.Framework.Assert.False(builder.Expression.IsEquivalentTo(exps[0]));
            NUnit.Framework.Assert.False(builder.Expression.IsEquivalentTo(exps[1]));

            NUnit.Framework.Assert.AreEqual(1, modifier.Type.DescendantNodes().OfType<FieldDeclarationSyntax>().Count());
            

        }

        [Test]
        public void ZeroMethod()
        {
            var source = @"
interface IA {
    int Method1();
    void Method2(out int val);
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var root = com.SyntaxTrees[0].GetRoot();
            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);

            var modifier = SyntaxModifier.Create(com).Mod(cia);
            cia = modifier.Type;

            var exps = cia.DescendantNodes().OfType<BlockSyntax>().ToArray();
            NUnit.Framework.Assert.AreEqual(0, modifier.TypesOfSerialization.Count());
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exps[0]));
            NUnit.Framework.Assert.True(builder.Expression.IsEquivalentTo(exps[1]));
        }
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

            var modifier = SyntaxModifier.Create(com).Mod(cia);
            cia = modifier.Type;

            var exp = cia.DescendantNodes().OfType<BlockSyntax>().Single();
            NUnit.Framework.Assert.AreEqual(0, modifier.TypesOfSerialization.Count());            
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

            var modifier = SyntaxModifier.Create(com).Mod(cia); 
            cia = modifier.Type;

            var exp = cia.DescendantNodes().OfType<BlockSyntax>().Single();
            NUnit.Framework.Assert.AreEqual(0, modifier.TypesOfSerialization.Count());
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

            var modifier = SyntaxModifier.Create(com).Mod(cia); 
            cia = modifier.Type;

            var exp = cia.DescendantNodes().OfType<BlockSyntax>().Single();
            NUnit.Framework.Assert.AreEqual(1, modifier.TypesOfSerialization.Count());
            NUnit.Framework.Assert.False(builder.Expression.IsEquivalentTo(exp));
        }

        [Test]
        public void MethodValueParamInt()
        {
            var source = @"

interface IA {
    Regulus.Remote.Value<int> Method1(int i);
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var com = tree.Compilation();
            var root = com.SyntaxTrees[0].GetRoot();
            var builder = new Regulus.Remote.Tools.Protocol.Sources.InterfaceInheritor(root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Single());

            var cia = SyntaxFactory.ClassDeclaration("CIA");
            cia = builder.Inherite(cia);

            var modifier = SyntaxModifier.Create(com).Mod(cia);
            cia = modifier.Type;

            var exp = cia.DescendantNodes().OfType<BlockSyntax>().Single();
            NUnit.Framework.Assert.AreEqual(2, modifier.TypesOfSerialization.Count());
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

            var modifier = SyntaxModifier.Create(com).Mod(cia);
            cia = modifier.Type;

            var exp = cia.DescendantNodes().OfType<BlockSyntax>().Single();
            NUnit.Framework.Assert.AreEqual(1, modifier.TypesOfSerialization.Count());
            NUnit.Framework.Assert.False(builder.Expression.IsEquivalentTo(exp));

            var method  = cia.DescendantNodes().OfType<MethodDeclarationSyntax>().Single();
            string paramName = method.ParameterList.Parameters.Single().Identifier.ToString();

            NUnit.Framework.Assert.AreEqual("_1" , paramName);

        }

        [Test]
        public void ImplementRegulusRemoteIGhost()
        {
            var cia = SyntaxFactory.ClassDeclaration("C123");
            cia = cia.ImplementRegulusRemoteIGhost();

            var member = cia.DescendantNodes().OfType<ConstructorDeclarationSyntax>().Single();

            NUnit.Framework.Assert.AreEqual("C123" , member.Identifier.ToString());
            
        }

        [Test]
        public void CreateRegulusRemoteIEventProxyCreater()
        {
            var source = @"

class CIA : NS1.IA {
    event System.Action<Regulus.Int,TT<Test.FLoat>> NS1.IA.Event1
    {
        add{}
        remove{}
    }
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var efd = tree.GetRoot().DescendantNodes().OfType<EventDeclarationSyntax>().Single();
            var cefd = efd.CreateRegulusRemoteIEventProxyCreater();
            NUnit.Framework.Assert.AreEqual("CNS1_IA_Event1", cefd.Identifier.ValueText);

        }

        [Test]
        public void CreateRegulusRemoteIEventProxyCreaterNoTypes()
        {
            var source = @"
class CIA : NS1.IA {
    event System.Action NS1.IA.Event1
    {
        add{}
        remove{}
    }
}
";
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(source);
            var efd = tree.GetRoot().DescendantNodes().OfType<EventDeclarationSyntax>().Single();
            var cefd = efd.CreateRegulusRemoteIEventProxyCreater();
            NUnit.Framework.Assert.AreEqual("CNS1_IA_Event1", cefd.Identifier.ValueText);

        }
    }
 
    
}