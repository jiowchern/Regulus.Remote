using NSubstitute;
using NUnit.Framework;
using Regulus.Remote;
using System;

namespace RemotingTest
{


    public class SoulEventTests
    {
        public interface TestType
        {
            event System.Action TestEvent;
        }
        public class GhostTestType : TestType
        {
            public GhostTestType(Regulus.Remote.GhostEventHandler handler)
            {
                _TestEvent = handler;
            }
            readonly Regulus.Remote.GhostEventHandler _TestEvent;
            event Action TestType.TestEvent
            {
                add
                {
                    _TestEvent.Add(value);
                }

                remove
                {
                    _TestEvent.Remove(value);
                }
            }
        }
        [NUnit.Framework.Test()]
        public void GhostEventInvokeTest()
        {
            GhostEventHandler ghostEventHandler = new Regulus.Remote.GhostEventHandler();

            bool invokeEnable = false;
            TestType ghostTestType = new GhostTestType(ghostEventHandler) as TestType;
            ghostTestType.TestEvent += () =>
            {
                invokeEnable = true;
            };
            ghostEventHandler.Invoke(1);


            Assert.AreEqual(true, invokeEnable);

        }
        [NUnit.Framework.Test()]
        public void SoulEventInvokeTest()
        {
            TestType obj = NSubstitute.Substitute.For<TestType>();
            SoulProvider.Soul soul = new SoulProvider.Soul(0, 0, typeof(TestType), obj);

            string eventCatcher = "";
            InvokeEventCallabck callback = (entiry_id, event_id, handler_id, args) =>
            {
                eventCatcher = $"{entiry_id}-{event_id}-{handler_id}";
            };
            GenericEventClosure closure = new GenericEventClosure(1, 1, 1, callback);
            System.Reflection.EventInfo info = typeof(TestType).GetEvent("TestEvent");
            soul.AddEvent(new SoulProvider.Soul.EventHandler(obj, new System.Action(closure.Run), info, 1));

            obj.TestEvent += Raise.Event<Action>();

            Assert.AreEqual("1-1-1", eventCatcher);

            soul.RemoveEvent(info, 1);
        }
    }
}

