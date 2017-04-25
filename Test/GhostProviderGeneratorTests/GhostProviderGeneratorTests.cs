using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Tool;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CSharp;

using Regulus.Framework;
using Regulus.Remoting;

namespace Regulus.Tool.GPI
{
    public interface GPIA
    {
        void Method();
        Guid Id { get; }
        Regulus.Remoting.Value<bool> MethodReturn();

        event Action<float, string> OnCallEvent;
    }
    
}
namespace Regulus.Tool.Tests
{
    [TestClass()]
    public class GhostProviderGeneratorTests
    {
        [TestMethod()]
        public void BuildTest()
        {
            var codes = new List<string>();
            var g = new Regulus.Protocol.CodeBuilder();
            g.ProviderEvent += (name,code) => codes.Add(code);
            g.EventEvent += (type_name, event_name, code) => codes.Add(code);
            g.GpiEvent += (type_name, code) => codes.Add(code);
            g.Build("GPIProvider",new[]{"Regulus.Tool.GPI"},new [] {typeof(Regulus.Tool.GPI.GPIA) });

            var optionsDic = new Dictionary<string, string>
            {
                {"CompilerVersion", "v4.0"}
            };
            var provider = new CSharpCodeProvider(optionsDic);
            var options = new CompilerParameters
            {
                GenerateInMemory = true
                ,GenerateExecutable = false,
                ReferencedAssemblies =
                {                    
                    "System.Core.dll",                    
                    "RegulusLibrary.dll",
                    "RegulusRemoting.dll",
                    "protobuf-net.dll",
                    "Regulus.Serialization.dll",
                    "GhostProviderGeneratorTests.dll"
                }
            };
            var result = provider.CompileAssemblyFromSource(options, codes.ToArray());

            Assert.IsTrue(result.Errors.Count == 0);
        }


        [TestMethod()]
        public void BuildGetEventHandler()
        {
            bool onevent = false;
            
            Delegate function = new Action<int , float , string>((i,f,s) => { onevent = true; });
            function.Method.Invoke(function.Target,new object[]{10,100f,"1000"}); 

            Assert.AreEqual( true , onevent);
        }

        
    }
}