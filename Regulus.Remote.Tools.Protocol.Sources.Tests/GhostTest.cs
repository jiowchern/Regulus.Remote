using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;

namespace Regulus.Remote.Tools.Protocol.Sources.Tests
{
    public class GhostTest
    {
        private readonly SyntaxTree[] _Souls;
      
        private readonly IEnumerable<SyntaxTree> _Sources;

        public GhostTest(params SyntaxTree[] souls)
        {

            _Souls = souls;
            var assemblyName = "TestProject";
            IEnumerable<MetadataReference> references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(Regulus.Remote.Property<>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Regulus.Remote.Value<>).GetTypeInfo().Assembly.Location),

            };
            CSharpCompilation compilation =  CSharpCompilation.Create(assemblyName, souls, references) ;
            _Sources = new ProjectSourceBuilder(compilation).Sources;
        }

      

        public async Task RunAsync()
        {
           
            var test = new CSharpSourceGeneratorVerifier<SourceGenerator>.Test
            {
                TestState =
                {
                    ReferenceAssemblies = ReferenceAssemblies.Default.AddPackages(ImmutableArray.Create(
                        new PackageIdentity("Regulus.Remote", "0.1.10.1"),
                        new PackageIdentity("Regulus.Serialization", "0.1.10.0"))),
                    
                },
            
            
            };
           
            foreach (var syntaxTree in _Sources)
            {
               
                test.TestState.GeneratedSources.Add((typeof(SourceGenerator), syntaxTree.FilePath, await syntaxTree.GetTextAsync( )));
            }
          
            foreach (var syntaxTree in _Souls)
            {
                test.TestState.Sources.Add(await syntaxTree.GetTextAsync( ));
            }
            
            await test.RunAsync();
        }
    }
}