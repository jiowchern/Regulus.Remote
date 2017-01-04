using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Remoting.Tests
{
    public interface IGpi
    {
        void Method();
        //Guid Id { get; }
        //Regulus.Remoting.Value<bool> MethodReturn();

        //event Action<float, string> OnCallEvent;
    }
    [TestClass()]
    public class AgentCoreTests
    {

        
    }
}