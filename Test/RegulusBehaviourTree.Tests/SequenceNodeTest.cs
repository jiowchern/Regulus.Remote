using System;


using Regulus.BehaviourTree;

namespace Regulus.BehaviourTree.Tests
{
    /// <summary>此類別包含 SequenceNode 的參數化單元測試</summary>
    

    public partial class SequenceNodeTest
    {

        /// <summary>.ctor() 的測試虛設常式</summary>

        internal SequenceNode ConstructorTest()
        {
            SequenceNode target = new SequenceNode();            
            return target;
            // TODO: 將判斷提示加入 方法 SequenceNodeTest.ConstructorTest()
        }

        /// <summary>Tick(Single) 的測試虛設常式</summary>

        internal TICKRESULT TickTest(SequenceNode target, float delta)
        {
            TICKRESULT result = target.Tick(delta);
            return result;
            // TODO: 將判斷提示加入 方法 SequenceNodeTest.TickTest(SequenceNode, Single)
        }

        [NUnit.Framework.Test()]
        public void TestTick1()
        {
            SequenceNode target = new SequenceNode();
            target.Add(new ActionNode<NumberTestNode>(()=>new NumberTestNode(3)
                , n => n.Tick
                , n => n.Start
                , n => n.End
                ));

            /*target.Add(new ActionNode<NumberTestNode>(new NumberTestNode(2)
                , n => n.Tick
                , n => n.Start
                , n => n.End
                ));*/

            NUnit.Framework.Assert.AreEqual( TICKRESULT.RUNNING , target.Tick(0));
            NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, target.Tick(0));
            NUnit.Framework.Assert.AreEqual(TICKRESULT.SUCCESS, target.Tick(0));
            NUnit.Framework.Assert.AreEqual(TICKRESULT.FAILURE, target.Tick(0));


        }

        [NUnit.Framework.Test()]
        public void TestTick2()
        {
            SequenceNode target = new SequenceNode();
            target.Add(new ActionNode<SequenceTestNode>(()=>new SequenceTestNode(3)
                , n => n.Tick
                , n => n.Start
                , n => n.End
                ));
          

            NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, target.Tick(0));
            NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, target.Tick(0));
            NUnit.Framework.Assert.AreEqual(TICKRESULT.SUCCESS, target.Tick(0));           



        }

        [NUnit.Framework.Test()]
        public void TestTick3()
        {

            var builder = new Builder();
            var node = builder
                .Sequence()
                    .Action((delta) => TICKRESULT.FAILURE)
                    .Action((delta) => TICKRESULT.SUCCESS)
                .End().Build();

            NUnit.Framework.Assert.AreEqual(TICKRESULT.FAILURE, node.Tick(0));
            NUnit.Framework.Assert.AreEqual(TICKRESULT.FAILURE, node.Tick(0));



        }

        [NUnit.Framework.Test()]
        public void TestTick4()
        {

            var builder = new Builder();
            var node = builder
                .Sequence()
                    .Action((delta) => TICKRESULT.SUCCESS)
                    .Action((delta) => TICKRESULT.SUCCESS)
                .End().Build();

            NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, node.Tick(0));
            NUnit.Framework.Assert.AreEqual(TICKRESULT.SUCCESS, node.Tick(0));

            NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, node.Tick(0));
            NUnit.Framework.Assert.AreEqual(TICKRESULT.SUCCESS, node.Tick(0));

        }

        [NUnit.Framework.Test()]
        public void TestTick5()
        {

            var builder = new Builder();
            var node = builder
                .Sequence()
                    .Action((delta) => TICKRESULT.SUCCESS)
                    .Action((delta) => TICKRESULT.FAILURE)
                    .Action((delta) => TICKRESULT.SUCCESS)
                .End().Build();

            NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, node.Tick(0));
            NUnit.Framework.Assert.AreEqual(TICKRESULT.FAILURE, node.Tick(0));

            NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, node.Tick(0));
            NUnit.Framework.Assert.AreEqual(TICKRESULT.FAILURE, node.Tick(0));

        }


        [NUnit.Framework.Test()]
        public void TestTick6()
        {
            bool executee = false;
            var builder = new Builder();
            var node = builder
                .Selector()
                    .Sequence()
                        .Action((delta) => TICKRESULT.SUCCESS)
                        .Action((delta) => TICKRESULT.FAILURE)
                        .Action(
                            (delta) =>
                            {
                                executee = true;
                                return TICKRESULT.SUCCESS;
                            })
                    .End()
                .End().Build();

            NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, node.Tick(0));
            NUnit.Framework.Assert.AreEqual(TICKRESULT.FAILURE, node.Tick(0));

            NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, node.Tick(0));
            NUnit.Framework.Assert.AreEqual(TICKRESULT.FAILURE, node.Tick(0));

            NUnit.Framework.Assert.AreEqual(false, executee);

            

        }

        [NUnit.Framework.Test()]
        public void TestTick7()
        {
            bool executee = false;

            int count = 0;
            var builder = new Builder();
            var node = builder
                .Selector()
                    .Sequence()
                        .Action((delta) => TICKRESULT.SUCCESS)
                        .Action((delta) => TICKRESULT.SUCCESS)                        
                        .Action(
                            (delta) =>
                            {
                                count ++;
                                if(count > 3)
                                    return TICKRESULT.SUCCESS;
                                return TICKRESULT.RUNNING;
                            })
                    .End()
                .End().Build();

            NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, node.Tick(0));
            NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, node.Tick(0));
            NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, node.Tick(0));
            NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, node.Tick(0));
            NUnit.Framework.Assert.AreEqual(TICKRESULT.RUNNING, node.Tick(0));
            NUnit.Framework.Assert.AreEqual(TICKRESULT.SUCCESS, node.Tick(0));
            

            NUnit.Framework.Assert.AreEqual(false, executee);



        }

        private TICKRESULT _Test(float arg)
        {
            throw new NotImplementedException();
        }

        [NUnit.Framework.Test()]
        public void TestTick8()
        {
            int node1_1 = 0;
            int node1_2 = 0;
            int node2_1 = 0;
            var builder = new Builder();
            var node  = builder
                .Sequence()
                    .Action((delta) =>
                    {
                        node1_1++;
                        return TICKRESULT.SUCCESS;                   ;
                    })
                    .Action((delta) =>
                    {
                        node1_2++;
                        return TICKRESULT.SUCCESS; ;
                    })
                    .Sequence()
                        .Action((delta) =>
                        {
                            node2_1++;
                            return TICKRESULT.SUCCESS; ;
                        })
                    .End()
                .End()
                .Build();

            node.Tick(0);
            node.Tick(0);
            node.Tick(0);

            NUnit.Framework.Assert.AreEqual(node1_1, 1);
            NUnit.Framework.Assert.AreEqual(node1_2, 1);
            NUnit.Framework.Assert.AreEqual(node2_1, 1);
        }
    }

    

    public class SequenceTestNode 
    {
        private  int _I;

        public SequenceTestNode(int i)
        {
            _I = i;
        }

        public TICKRESULT Tick(float arg)
        {
            --_I;
            if(_I > 0)
                return  TICKRESULT.RUNNING;

            return TICKRESULT.SUCCESS;
        }

        public void Start()
        {
            
        }

        public void End()
        {
            
        }
    }
}

