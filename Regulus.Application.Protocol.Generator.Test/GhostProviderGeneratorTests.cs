
using Regulus.Tool;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CSharp;

using NUnit.Framework;

using Regulus.Framework;
using Regulus.Remote;

namespace Regulus.Tool.GPI
{
    public interface GPIA
    {
        void Method();
        Guid Id { get; }
        Regulus.Remote.Value<bool> MethodReturn();

        event Action<float, string> OnCallEvent;
    }
    
}
namespace Regulus.Tool.Tests
{
    [TestFixture]
    public class GhostProviderGeneratorTests
    {
        [NUnit.Framework.Test()]
        public void BuildTest()
        {
            var builder = new Regulus.Remote.Protocol.AssemblyBuilder(Remote.Protocol.Essential.CreateFromDomain(typeof(Regulus.Tool.GPI.GPIA).Assembly));
            builder.Create();            
            Assert.Pass();
        }

        private string _GetAssemblyPath(string file_name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (System.IO.Path.GetFileName(assembly.Location) == file_name)
                    return assembly.Location;
            }

            throw new Exception("not found dll");
        }


        [NUnit.Framework.Test()]
        public void BuildGetEventHandler()
        {
            bool onevent = false;
            
            Delegate function = new Action<int , float , string>((i,f,s) => { onevent = true; });
            function.Method.Invoke(function.Target,new object[]{10,100f,"1000"}); 

            NUnit.Framework.Assert.AreEqual( true , onevent);
        }

        
    }
}