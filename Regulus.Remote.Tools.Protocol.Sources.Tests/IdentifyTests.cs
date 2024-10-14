using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;
using System.Linq;
using System.Reflection;

namespace Regulus.Remote.Tools.Protocol.Sources.Tests
{
    public class IdentifyTests
    {
        [Test]
        public void HasTag()
        {
            var src = @"

public interface IWithTag : Regulus.Remote.Protocolable {
      
    }        

    public interface IWithoutTag {
      
    } 
";
            var syntaxBuilder =
               new Regulus.Remote.Tools.Protocol.Sources.SyntaxTreeBuilder(SourceText.From(src,
                   System.Text.Encoding.UTF8));

            var assemblyName = "TestProject";
            System.Collections.Generic.IEnumerable<Microsoft.CodeAnalysis.MetadataReference> references = new Microsoft.CodeAnalysis.MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(Regulus.Remote.Property<>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Regulus.Remote.Value<>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Action).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Action<,,,,,,,,>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Regulus.Remote.Protocolable).GetTypeInfo().Assembly.Location),
            };
            CSharpCompilation compilation = CSharpCompilation.Create(assemblyName, new[] { syntaxBuilder.Tree }, references);

            var tag = new IdentifyFinder(compilation).Tag;
            var builder = new ProjectSourceBuilder(new EssentialReference(compilation, tag));
            var sources = builder.Sources.ToArray();
            var names = from source in sources
                        let name = source.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault()?.Identifier.ToString()
                        where name != null
                        select name;

            NUnit.Framework.Assert.True(names.Any(n => n == "CIWithTag"));
            NUnit.Framework.Assert.False(names.Any(n => n == "CIWithoutTag"));

        }
        
    }


}