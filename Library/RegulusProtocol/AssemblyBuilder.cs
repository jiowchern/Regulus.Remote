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

        public AssemblyBuilder(Assembly common)  
        {
            var asmRefs = new RemoteAssemblys();

            var refs = new[] { common, asmRefs.Library, asmRefs.Remote, asmRefs.Serialization };
            var locations = new HashSet<string>();
            foreach (var refAsm in refs)
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


            _Codes = _BuildCode(common, _CreateProtoclName()).ToArray();

        }
        public AssemblyBuilder(Assembly common, Assembly regulus_library, Assembly regulus_remote, Assembly regulus_serialization)
        {
            
            var refs = new[] { common , regulus_library , regulus_remote , regulus_serialization };
            var locations = new HashSet<string>();
            foreach (var refAsm in refs)
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


            _Codes = _BuildCode(common, _CreateProtoclName()).ToArray();

            
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

                throw new Exception("Prorocol compile error");
            }

            return result.CompiledAssembly;
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
                .Where( a=> a.Location.Contains("mscorlib") == false)
                .Select(a => a.Location)
                .ToList();

            assemblyNames.Add(asm.Location);

            return assemblyNames.ToArray();
        }
        public void _Build(Assembly assembly, string protocol_name , string out_path)
        {
            var codes = AssemblyBuilder._BuildCode(assembly, protocol_name);

            Dictionary<string, string> optionsDic = new Dictionary<string, string>
            {
                {"CompilerVersion", "v4.0"}
            };

            

            var provider = new CSharpCodeProvider(optionsDic);
            var options = new CompilerParameters
            {

                GenerateInMemory = false,
                GenerateExecutable = false,
                OutputAssembly = out_path,
                TempFiles = new TempFileCollection()

            };
            
            var locations = new HashSet<string>();
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;

            foreach (var referencedAssembly in _GetReferencedAssemblies(Assembly.LoadFile(System.IO.Path.Combine(baseDir, "Regulus.Library.dll"))))
            {
                locations.Add(referencedAssembly);
            }

            foreach (var referencedAssembly in _GetReferencedAssemblies(Assembly.LoadFile(System.IO.Path.Combine(baseDir, "Regulus.Remote.dll"))))
            {
                locations.Add(referencedAssembly);
            }

            foreach (var referencedAssembly in _GetReferencedAssemblies(Assembly.LoadFile(System.IO.Path.Combine(baseDir, "Regulus.Serialization.dll"))))
            {
                locations.Add(referencedAssembly);
            }

            foreach (var referencedAssembly in _GetReferencedAssemblies(assembly))
            {
                locations.Add(referencedAssembly);
            }


            options.ReferencedAssemblies.AddRange(locations.ToArray());

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
        public Assembly _Build(Assembly assembly, string protocol_name )
        {


            var codes = AssemblyBuilder._BuildCode(assembly, protocol_name);

            Dictionary<string, string> optionsDic = new Dictionary<string, string>
            {
                {"CompilerVersion", "v4.0"}
            };
            var provider = new CSharpCodeProvider(optionsDic);
            var options = new CompilerParameters
            {

                GenerateInMemory = true,
                GenerateExecutable = false, TempFiles = new TempFileCollection()
            };

            



            var locations = new HashSet<string>();
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            foreach (var referencedAssembly in _GetReferencedAssemblies(Assembly.LoadFile(System.IO.Path.Combine(baseDir, "Regulus.Library.dll"))))
            {
                locations.Add(referencedAssembly);
            }

            foreach (var referencedAssembly in _GetReferencedAssemblies(Assembly.LoadFile(System.IO.Path.Combine(baseDir, "Regulus.Remote.dll"))))            
            {
                locations.Add(referencedAssembly);
            }

            foreach (var referencedAssembly in _GetReferencedAssemblies(Assembly.LoadFile(System.IO.Path.Combine(baseDir, "Regulus.Serialization.dll"))))                
            {
                locations.Add(referencedAssembly);
            }

            foreach (var referencedAssembly in _GetReferencedAssemblies(assembly))
            {
                locations.Add(referencedAssembly);
            }


            options.ReferencedAssemblies.AddRange(locations.ToArray());


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
