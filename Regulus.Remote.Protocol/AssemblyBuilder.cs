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


        public AssemblyBuilder(Essential essential)
        {
            var types = essential.Common.GetExportedTypes();
            var baseRefs = new RefAssemblyFinder(types);

            var assmebyes = new HashSet<Assembly>(essential.Assemblys.Union(baseRefs.Assemblys));

            var refs = new List<MetadataReference>(assmebyes.Select(assembly => MetadataReference.CreateFromFile(assembly.Location)));
            
            _Refs = refs.ToArray();
            

            _Codes = _BuildCode(types, _CreateProtoclName()).ToArray();
        }
        

        public Assembly Create()
        {
            var tree = _Codes.Select(code => CSharpSyntaxTree.ParseText(code));
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithPlatform(Platform.AnyCpu).WithOptimizationLevel(OptimizationLevel.Release);                        
            var compilation = CSharpCompilation.Create(_CreateProtoclName(), tree, _Refs , options);
            
            var stream = new System.IO.MemoryStream();
            var emitResult = compilation.Emit(stream);

            if (emitResult.Success)
            {
                
                stream.Seek(0, System.IO.SeekOrigin.Begin);

                return Assembly.Load(stream.ToArray());                
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
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithPlatform(Platform.AnyCpu).WithOptimizationLevel(OptimizationLevel.Release);
            var compilation = CSharpCompilation.Create(_CreateProtoclName(), tree, _Refs, options);
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
            var name = $"Regulus.Protocol.Temp.C{guidNumberString}";
            return name;
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
