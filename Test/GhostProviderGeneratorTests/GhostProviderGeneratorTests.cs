using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Tool;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CSharp;

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
            var g = new GhostProviderGenerator();
            var codes = g.Build(
                "GPIProvider",
                new[]
                {
                    "Regulus.Tool.GPI"
                },new [] {typeof(Regulus.Tool.GPI.GPIA) });

            Dictionary<string, string> optionsDic = new Dictionary<string, string>
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
                    "GhostProviderGeneratorTests.dll"
                }
            };
            var result = provider.CompileAssemblyFromSource(options, codes.ToArray());

            Assert.IsTrue(result.Errors.Count == 0);
        }
    }
}