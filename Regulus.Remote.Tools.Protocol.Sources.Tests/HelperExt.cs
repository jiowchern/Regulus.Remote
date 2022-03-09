using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Regulus.Remote.Tools.Protocol.Sources.Tests
{
    public static class HelperExt
    {
    
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
            };
            return CSharpCompilation.Create(assemblyName, new []{ tree }, references);
        }
    }
}