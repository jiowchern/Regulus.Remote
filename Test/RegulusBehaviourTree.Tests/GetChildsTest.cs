using System.Collections.Generic;
using NUnit.Framework;

namespace Regulus.BehaviourTree.Tests
{
    public class GetChildsTest
    {
        class Test
        {
            public TICKRESULT Call1(float d)
            {
                return TICKRESULT.SUCCESS;
            }

            public IEnumerable<TICKRESULT> Call2()
            {
                yield return TICKRESULT.SUCCESS;
            }

            public void Start()
            {
                
            }

            public void End()
            {

            }
        }
        [NUnit.Framework.Test()]
        public void GetChilds1()
        {
            var test = new Test();
            var builder = new Regulus.BehaviourTree.Builder();
            var ticker = builder
                .Sequence()                    
                    .Action(test.Call1)                    
                    .Action(() => test.Call2())
                    .Action(() => test , (i) => i.Call1 ,(i) => i.Start , (i) => i.End )

                .End().Build();

            var childs = ticker.GetChilds();

            Assert.AreEqual("Sequence" , ticker.Tag);

            Assert.AreEqual("Call1", childs[0].Tag);

            Assert.AreEqual("Call2", childs[1].Tag);

            Assert.AreEqual("Call1", childs[2].Tag);


        }

        private TICKRESULT Test1(float arg)
        {
            return TICKRESULT.SUCCESS;
        }
    }
}