using System;
using System.Linq;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.CSharp;

namespace Regulus.Protocol
{
    public class AssemblyBuilder
    {

        

        

        public void BuildFile(Assembly assembly , string library_name , string[] name_spaces , string output_path)
        {

            Dictionary<string, string> optionsDic = new Dictionary<string, string>
            {
                {"CompilerVersion", "v3.5"}
            };
            var provider = new CSharpCodeProvider(optionsDic);
            var options = new CompilerParameters
            {

                OutputAssembly = output_path,
                GenerateExecutable = false,

                ReferencedAssemblies =
                {
                    "System.Core.dll",
                    "System.xml.dll",
                    "RegulusLibrary.dll",
                    "RegulusRemoting.dll",
                    "Regulus.Serialization.dll",
                    
                    assembly.Location
               }                

            };


            var types = assembly.GetExportedTypes();
            var codes = new List<string>();
            var codeBuilder = new CodeBuilder();
            codeBuilder.ProviderEvent += (name , code) => codes.Add(code);
            codeBuilder.EventEvent += (type_name, event_name, code) => codes.Add(code);
            codeBuilder.GpiEvent += (type_name, code) => codes.Add(code);
            codeBuilder.Build(library_name, types);

            var result = provider.CompileAssemblyFromSource(options, codes.ToArray());
            if (result.Errors.Count > 0)
            {                
                throw new Exception("Gpi compile error");
            }

            
        }


        public Assembly Build(Assembly assembly, string protocol_name , string[] namespaces)
        {

            Dictionary<string, string> optionsDic = new Dictionary<string, string>
            {
                {"CompilerVersion", "v3.5"}
            };
            var provider = new CSharpCodeProvider(optionsDic);
            var options = new CompilerParameters
            {

                GenerateInMemory = true,
                GenerateExecutable = false,

                ReferencedAssemblies =
                {
                    "System.Core.dll",
                    "System.xml.dll",
                    "RegulusLibrary.dll",
                    "RegulusRemoting.dll",
                    "Regulus.Serialization.dll",                    
                    assembly.Location
               }
                , TempFiles = new TempFileCollection()
            };

            
            

            var codes = new List<string>();
            var codeBuilder = new CodeBuilder();
            codeBuilder.ProviderEvent += (name , code) => codes.Add(code);
            codeBuilder.EventEvent += (type_name, event_name, code) => codes.Add(code);
            codeBuilder.GpiEvent += (type_name, code) => codes.Add(code);
            codeBuilder.Build(protocol_name, assembly.GetExportedTypes());
            
            var result = provider.CompileAssemblyFromSource(options, codes.ToArray());
            if (result.Errors.Count > 0)
            {
                System.IO.File.WriteAllLines("dump.cs", codes.ToArray());

                throw new Exception("Prorocol compile error");
            }

            return result.CompiledAssembly;
        }
        
    }
}
