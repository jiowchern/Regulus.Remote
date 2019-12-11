using System;
using System.Linq;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

using Microsoft.CSharp;

namespace Regulus.Remote.Protocol
{
    public class AssemblyBuilder
    {        
        readonly CSharpCodeProvider _Provider;
        readonly string[] _Codes;
        readonly string[] _Refs;


        public AssemblyBuilder(params Type[] types)
        {
            var refAsms = types.Select( type => type.Assembly ).ToList();
            var baseRefs = new BaseRemoteAssemblys();
            refAsms.Add(baseRefs.Library);
            refAsms.Add(baseRefs.Remote);
            refAsms.Add(baseRefs.Serialization);
            var locations = new HashSet<string>();
            foreach (var refAsm in refAsms)
            {
                foreach (var referencedAssembly in _GetReferencedAssemblies(refAsm))
                {
                    locations.Add(referencedAssembly);
                }
            }
            _Refs = locations.ToArray();

            Dictionary<string, string> optionsDic = new Dictionary<string, string>
            {
                {"CompilerVersion", "v4.0"}
            };

            _Provider = new CSharpCodeProvider(optionsDic);
            _Codes = _BuildCode(types, _CreateProtoclName()).ToArray();
        }
        

        public Assembly Create()
        {
            var options = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,                
                TempFiles = new TempFileCollection()
            };

            options.ReferencedAssemblies.AddRange(_Refs);
            var result = _Provider.CompileAssemblyFromSource(options, _Codes);
            
            if (result.Errors.Count > 0)
            {
                foreach (var error in result.Errors)
                {
                    Regulus.Utility.Log.Instance.WriteInfo(error.ToString());
                }

                System.IO.File.WriteAllLines("dump.cs", _Codes.ToArray());

                throw new Exception("Protocol compile error.");
            }

            return result.CompiledAssembly;
        }

        public void CreateFile(string path)
        {
            var options = new CompilerParameters
            {
                OutputAssembly = path,
                GenerateInMemory = false,
                GenerateExecutable = false,
                TempFiles = new TempFileCollection()
            };

            options.ReferencedAssemblies.AddRange(_Refs);
            var result = _Provider.CompileAssemblyFromSource(options, _Codes);

            if (result.Errors.Count > 0)
            {
                foreach (var error in result.Errors)
                {
                    Regulus.Utility.Log.Instance.WriteInfo(error.ToString());
                }

                System.IO.File.WriteAllLines("dump.cs", _Codes.ToArray());

                throw new Exception("Protocol compile error.");
            }

            
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
