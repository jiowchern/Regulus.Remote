using System;
using System.Collections.Generic;
using System.Linq;


using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.Project.RestartProcess;
namespace RestartProcessTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void DictionaryExtenstionTestUnchecked()
        {
            var dic = new Dictionary<string ,int>();

            dic.Add("a", 1);
            dic.Add("b", 2);
            dic.Add("c", 3);
            var unckeckeds = dic.Unchecked(
                new string[]
                {
                    "a",
                    "b"
                });
            
            var data = unckeckeds.First();
            Assert.AreEqual(3 , data.Value);

        }
    }
}
