using System;
using System.Linq;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;

using Microsoft.CSharp;

namespace Regulus.Protocol
{
    public class AssemblyBuilder
    {

        public void BuildFile(Assembly assembly, string protocol_name, string output_path)
        {

            var options = new CompilerParameters
            {
                IncludeDebugInformation = true,
                GenerateInMemory = false,
                GenerateExecutable = false,
                OutputAssembly = output_path,
                TempFiles = new TempFileCollection(@".\out", true)
            };


            _Build(assembly, protocol_name, options);

        }

        public Assembly BuildMemory(Assembly assembly, string protocol_name)
        {

            var options = new CompilerParameters
            {
                IncludeDebugInformation = true,
                GenerateInMemory = true,
                GenerateExecutable = false,
                //TempFiles = new TempFileCollection(@".\out", true)
            };


            return _Build(assembly, protocol_name, options);

        }

        private Assembly _Build(Assembly assembly, string protocol_name, CompilerParameters options)
        {
            options.ReferencedAssemblies.Add("RegulusLibrary.dll");
            options.ReferencedAssemblies.Add("RegulusRemoting.dll");
            options.ReferencedAssemblies.Add("Regulus.Serialization.dll");
            var locations = _GetReferencedAssemblies(assembly).ToArray();
            options.ReferencedAssemblies.AddRange(locations);

            var optionsDic = new Dictionary<string, string> {{"CompilerVersion", "v4.0"}};

            var provider = new CSharpCodeProvider(optionsDic);

            var codes = AssemblyBuilder._BuildCode(assembly, protocol_name);
            var result = provider.CompileAssemblyFromSource(options, codes.ToArray());

            if (result.Errors.Count <= 0)
            {
                return result.CompiledAssembly;
            }

            File.WriteAllLines("dump.cs", codes.ToArray());
            File.WriteAllLines("error.log", locations.Union(_GetErrors(result.Errors)).ToArray());

            throw new Exception("Protocol compile error");
        }

        private static IEnumerable<string> _GetReferencedAssemblies(Assembly asm)
        {
            HashSet<string> locations = new HashSet<string>();
            _ReferencedAssemblies(asm, locations);
            return locations.Union(new[] {asm.Location});
        }

        private static void _ReferencedAssemblies(Assembly asm, HashSet<string> locations)
        {
            foreach (var refAsm in asm.GetReferencedAssemblies())
            {
                var loaded = Assembly.Load(refAsm);
                if (loaded.Location.Contains("mscorlib"))
                    continue;

                if (locations.Contains(loaded.Location))
                    continue;

                locations.Add(loaded.Location);
                _ReferencedAssemblies(loaded, locations);
            }
        }
        



        // static build in memory
        private static List<string> _BuildCode(Assembly assembly, string protocol_name)
        {
            var codes = new List<string>();
            var codeBuilder = new CodeBuilder();
            
            codeBuilder.ProviderEvent += (name, code) => codes.Add(code);
            codeBuilder.EventEvent += (type_name, event_name, code) => codes.Add(code);
            codeBuilder.GpiEvent += (type_name, code) => codes.Add(code);
            codeBuilder.Build(protocol_name, assembly.GetExportedTypes());
            return codes;
        }

        private IEnumerable<string> _GetErrors(IEnumerable errors)
        {
            foreach (var error in errors)
            {
                yield return error.ToString();
            }
        }
    }
}
