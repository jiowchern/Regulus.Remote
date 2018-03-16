using System.Collections.Generic;
using NUnit.Framework;

namespace Regulus.BehaviourTree.Tests
{
    public class GetNodesTest
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
        public void GetNode1()
        {
            var test = new Test();
            var builder = new Regulus.BehaviourTree.Builder();
            var ticker = builder
                .Sequence()                    
                    .Action(test.Call1)                    
                    .Action(() => test.Call2())
                    .Action(() => test , (i) => i.Call1 ,(i) => i.Start , (i) => i.End )

                .End().Build();


            List<Infomation> nodes = new List<Infomation>();
            ticker.GetInfomation(ref nodes);
            Assert.AreEqual(0 , nodes.Count);

            ticker.Tick(0);
            nodes.Clear();
            ticker.GetInfomation(ref nodes);
            Assert.AreEqual("Call1", nodes[0].Tag);


            ticker.Tick(0);
            nodes.Clear();
            ticker.GetInfomation(ref nodes);
            Assert.AreEqual("Call2", nodes[0].Tag);

            ticker.Tick(0);
            nodes.Clear();
            ticker.GetInfomation(ref nodes);
            Assert.AreEqual("Call1", nodes[0].Tag);

        }

        private TICKRESULT Test1(float arg)
        {
            return TICKRESULT.SUCCESS;
        }
    }
}