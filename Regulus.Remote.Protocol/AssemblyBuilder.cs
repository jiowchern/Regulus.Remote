using System;
using System.Linq;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace Regulus.Remote.Protocol
{
    public class AssemblyBuilder
    {
        readonly string[] _Codes;
        readonly MetadataReference[] _Refs;


        public AssemblyBuilder(params Type[] types)
        {
            var refAsms = types.Select( type => type.Assembly ).ToList();
            var baseRefs = new BaseRemoteAssemblys();
            refAsms.Add(baseRefs.Library);
            refAsms.Add(baseRefs.Remote);
            refAsms.Add(baseRefs.Serialization);
            refAsms.Add(baseRefs.BaseObject);
            var locations = new HashSet<string>();
            foreach (var refAsm in refAsms)
            {
                foreach (var referencedAssembly in _GetReferencedAssemblies(refAsm))
                {
                    locations.Add(referencedAssembly);
                }
            }
            _Refs = locations.Select( location => MetadataReference.CreateFromFile(location)) .ToArray();
            

            _Codes = _BuildCode(types, _CreateProtoclName()).ToArray();
        }
        

        public Assembly Create()
        {
            var tree = _Codes.Select(code => CSharpSyntaxTree.ParseText(code));
            var compilation = CSharpCompilation.Create(_CreateProtoclName(), tree, _Refs , new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            
            var stream = new System.IO.MemoryStream();
            var emitResult = compilation.Emit(stream);

            if (emitResult.Success)
            {
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                return System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromStream(stream);
            }

            foreach (var diagnostic in emitResult.Diagnostics)
            {
                Regulus.Utility.Log.Instance.WriteInfo(diagnostic.GetMessage());

            }
            throw new Exception("Protocol compile error.");

            
        }

        public void CreateFile(string path)
        {
            var tree = _Codes.Select(code => CSharpSyntaxTree.ParseText(code));
            var compilation = CSharpCompilation.Create(_CreateProtoclName(), tree, _Refs, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            var emitResult = compilation.Emit(path);

            if (emitResult.Success)
            {
                return ;
            }

            foreach (var diagnostic in emitResult.Diagnostics)
            {
                Regulus.Utility.Log.Instance.WriteInfo(diagnostic.GetMessage());
            }
            throw new Exception("Protocol compile error.");


        }


        private static string _CreateProtoclName()
        {
            var guidNumberString = Guid.NewGuid().ToString("N");
            var name = $"Regulus.Protocl.Temp.C{guidNumberString}";
            return name;
        }

        private static string[] _GetReferencedAssemblies(Assembly asm)
        {
            var assemblyNames = asm
                .GetReferencedAssemblies()
                .Select(Assembly.Load)                
                .Select(a => a.Location)
                .ToList();

            assemblyNames.Add(asm.Location);

            return assemblyNames.ToArray();
        }
        

        

        private static List<string> _BuildCode(Type[] types, string protocol_name)
        {
            var codes = new List<string>();
            var codeBuilder = new CodeBuilder();
            codeBuilder.ProviderEvent += (name, code) => codes.Add(code);
            codeBuilder.EventEvent += (type_name, event_name, code) => codes.Add(code);
            codeBuilder.GpiEvent += (type_name, code) => codes.Add(code);
            codeBuilder.Build(protocol_name, types);
            return codes;
        }
        
    }
}
