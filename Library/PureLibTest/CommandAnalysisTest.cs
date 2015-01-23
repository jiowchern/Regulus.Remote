using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Regulus.UnitTest
{
    
    
    [TestClass]
    public class CommandTest
    {
        

        [TestMethod]
        public void TestCommandAnalysisWithParameters()
        {
            var analysis = new Regulus.Utility.Command.Analysis("login [ account ,    password, result]");            

            Assert.AreEqual("login", analysis.Command);
            Assert.AreEqual("result", analysis.Parameters[2]);
            Assert.AreEqual("account", analysis.Parameters[0]);
            Assert.AreEqual("password", analysis.Parameters[1]);
            
        }


        [TestMethod]
        public void TestCommandAnalysisNoParameters()
        {
            var analysis = new Regulus.Utility.Command.Analysis("login");

            Assert.AreEqual("login", analysis.Command);
            Assert.AreEqual(0, analysis.Parameters.Length);

        }

        
    }
}
