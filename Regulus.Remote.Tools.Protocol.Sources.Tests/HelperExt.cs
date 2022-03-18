using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Regulus.Remote.Tools.Protocol.Sources.Tests
{
    public static class HelperExt
    {
        public static CSharpCompilation Compilate(params SyntaxTree[] trees)
        {
            var assemblyName = Guid.NewGuid().ToString();
            
            IEnumerable<MetadataReference> references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(Regulus.Remote.Protocol.CreaterAttribute).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Regulus.Remote.Value<>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Regulus.Remote.Property<>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Regulus.Remote.Notifier<>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Action).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Action<>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Action<,,,,,,,,>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location)
            };

            
            return CSharpCompilation.Create(assemblyName, trees, references , new CSharpCompilationOptions(outputKind  : OutputKind.DynamicallyLinkedLibrary));
        }

        public static CSharpCompilation Compilation(this SyntaxTree tree)
        {
            var assemblyName = Guid.NewGuid().ToString();
            
            IEnumerable<MetadataReference> references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(Regulus.Remote.Protocol.CreaterAttribute).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Regulus.Remote.Value<>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Regulus.Remote.Property<>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Regulus.Remote.Notifier<>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Action).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Action<>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Action<,,,,,,,,>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location)
            };
        
            return CSharpCompilation.Create(assemblyName, new []{ tree }, references);
        }

       


        public static System.Reflection.Assembly ToAssembly(this Microsoft.CodeAnalysis.CSharp.CSharpCompilation compilation)
        {
            
            Assembly assembly = null;
            using (var memoryStream = new System.IO.MemoryStream())
            {
                var emitResult = compilation.Emit(memoryStream);
                if (emitResult.Success)
                {
                    memoryStream.Seek(0, System.IO.SeekOrigin.Begin);

                    var assemblyLoadContext = System.Runtime.Loader.AssemblyLoadContext.Default;

                    assembly = assemblyLoadContext.Assemblies.FirstOrDefault(a => a.GetName()?.Name == compilation.AssemblyName);
                    if (assembly == null)
                    {
                        assembly = assemblyLoadContext.LoadFromStream(memoryStream);
                    }
                }
            }

            return assembly;
        }
    }
}