using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regulus.BehaviourTree;

namespace Regulus.BehaviourTree.Tests
{
    /// <summary>此類別包含 SequenceNode 的參數化單元測試</summary>
    [TestClass]
    [PexClass(typeof(SequenceNode))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class SequenceNodeTest
    {

        /// <summary>.ctor() 的測試虛設常式</summary>
        [PexMethod]
        internal SequenceNode ConstructorTest()
        {
            SequenceNode target = new SequenceNode();            
            return target;
            // TODO: 將判斷提示加入 方法 SequenceNodeTest.ConstructorTest()
        }

        /// <summary>Tick(Single) 的測試虛設常式</summary>
        [PexMethod]
        internal TICKRESULT TickTest([PexAssumeUnderTest]SequenceNode target, float delta)
        {
            TICKRESULT result = target.Tick(delta);
            return result;
            // TODO: 將判斷提示加入 方法 SequenceNodeTest.TickTest(SequenceNode, Single)
        }

        [TestMethod]
        public void TestTick1()
        {
            SequenceNode target = new SequenceNode();
            target.Add(new ActionNode<NumberTestNode>(new NumberTestNode(3)
                , n => n.Tick
                , n => n.Start
                , n => n.End
                ));

            /*target.Add(new ActionNode<NumberTestNode>(new NumberTestNode(2)
                , n => n.Tick
                , n => n.Start
                , n => n.End
                ));*/

            Assert.AreEqual( TICKRESULT.RUNNING , target.Tick(0));
            Assert.AreEqual(TICKRESULT.RUNNING, target.Tick(0));
            Assert.AreEqual(TICKRESULT.SUCCESS, target.Tick(0));
            Assert.AreEqual(TICKRESULT.FAILURE, target.Tick(0));


        }

        [TestMethod]
        public void TestTick2()
        {
            SequenceNode target = new SequenceNode();
            target.Add(new ActionNode<SequenceTestNode>(new SequenceTestNode(3)
                , n => n.Tick
                , n => n.Start
                , n => n.End
                ));
          

            Assert.AreEqual(TICKRESULT.RUNNING, target.Tick(0));
            Assert.AreEqual(TICKRESULT.RUNNING, target.Tick(0));
            Assert.AreEqual(TICKRESULT.SUCCESS, target.Tick(0));           



        }

        [TestMethod]
        public void TestTick3()
        {
            SequenceNode target = new SequenceNode();
            target.Add(new ActionNode<SequenceTestNode>(new SequenceTestNode(1)
                , n => n.Tick
                , n => n.Start
                , n => n.End
                ));

            target.Add(new ActionNode<SequenceTestNode>(new SequenceTestNode(2)
                , n => n.Tick
                , n => n.Start
                , n => n.End
                ));

            Assert.AreEqual(TICKRESULT.RUNNING, target.Tick(0));
            Assert.AreEqual(TICKRESULT.SUCCESS, target.Tick(0));            
            Assert.AreEqual(TICKRESULT.SUCCESS, target.Tick(0));



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
