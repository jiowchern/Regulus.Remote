using System;
using System.Linq;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

using Microsoft.CSharp;

namespace Regulus.Protocol
{
    public class AssemblyBuilder
    {

        private static string[] _GetReferencedAssemblies(Assembly asm)
        {
            var assemblyNames = asm
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .Where( a=> a.Location.Contains("mscorlib") == false)
                .Select(a => a.Location)
                .ToList();

            assemblyNames.Add(asm.Location);

            return assemblyNames.ToArray();
        }
        public void Build(Assembly assembly, string protocol_name , string out_path)
        {
            var codes = AssemblyBuilder._BuildCode(assembly, protocol_name);

            Dictionary<string, string> optionsDic = new Dictionary<string, string>
            {
                {"CompilerVersion", "v3.5"}
            };

            

            var provider = new CSharpCodeProvider(optionsDic);
            var options = new CompilerParameters
            {

                GenerateInMemory = false,
                GenerateExecutable = false,
                OutputAssembly = out_path,
                TempFiles = new TempFileCollection()

            };
            options.ReferencedAssemblies.Add("RegulusLibrary.dll");
            options.ReferencedAssemblies.Add("RegulusRemoting.dll");
            options.ReferencedAssemblies.Add("Regulus.Serialization.dll");
            options.ReferencedAssemblies.AddRange(_GetReferencedAssemblies(assembly));

            var result = provider.CompileAssemblyFromSource(options, codes.ToArray());
            if (result.Errors.Count > 0)
            {

                foreach (var error in result.Errors)
                {
                   Regulus.Utility.Log.Instance.WriteInfo(error.ToString()); 
                }
                
                System.IO.File.WriteAllLines("dump.cs", codes.ToArray());

                throw new Exception("Prorocol compile error");
            }
        }
        public Assembly Build(Assembly assembly, string protocol_name )
        {


            var codes = AssemblyBuilder._BuildCode(assembly, protocol_name);

            Dictionary<string, string> optionsDic = new Dictionary<string, string>
            {
                {"CompilerVersion", "v3.5"}
            };
            var provider = new CSharpCodeProvider(optionsDic);
            var options = new CompilerParameters
            {

                GenerateInMemory = true,
                GenerateExecutable = false, TempFiles = new TempFileCollection()
            };
            options.ReferencedAssemblies.Add("RegulusLibrary.dll");
            options.ReferencedAssemblies.Add("RegulusRemoting.dll");
            options.ReferencedAssemblies.Add("Regulus.Serialization.dll");
            options.ReferencedAssemblies.AddRange(_GetReferencedAssemblies(assembly));


            var result = provider.CompileAssemblyFromSource(options, codes.ToArray());
            if (result.Errors.Count > 0)
            {
                foreach (var error in result.Errors)
                {
                    Regulus.Utility.Log.Instance.WriteInfo(error.ToString());
                }

                System.IO.File.WriteAllLines("dump.cs", codes.ToArray());

                throw new Exception("Prorocol compile error");
            }

            return result.CompiledAssembly;
        }

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
    }
}
