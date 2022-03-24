using System.Reflection;
using NSubstitute;

namespace Regulus.Remote.Tests
{
    

    public class GhostResponseHandlerTests
    {
        interface IA
        {
            Regulus.Remote.Property<int> Property1 { get; }
        }
        class GhostIA : IA, IGhost
        {
            public GhostIA()
            {
                Property1 = new Property<int>();
                _Regulus_Remote_Tests_GhostResponseHandlerTests_IA_Property1 = Property1;
            }

            event CallMethodCallback IGhost.CallMethodEvent
            {
                add
                {
                    throw new System.NotImplementedException();
                }

                remove
                {
                    throw new System.NotImplementedException();
                }
            }

            event EventNotifyCallback IGhost.AddEventEvent
            {
                add
                {
                    throw new System.NotImplementedException();
                }

                remove
                {
                    throw new System.NotImplementedException();
                }
            }

            event EventNotifyCallback IGhost.RemoveEventEvent
            {
                add
                {
                    throw new System.NotImplementedException();
                }

                remove
                {
                    throw new System.NotImplementedException();
                }
            }

            public Property<int> Property1;
            private Property<int> _Regulus_Remote_Tests_GhostResponseHandlerTests_IA_Property1;
            Property<int> IA.Property1 => Property1;

            long IGhost.GetID()
            {
                throw new System.NotImplementedException();
            }

            object IGhost.GetInstance()
            {
                return this;
            }

            bool IGhost.IsReturnType()
            {
                throw new System.NotImplementedException();
            }
        }


        Regulus.Remote.GhostResponseHandler _GhostResponseHandler;
        ISerializable _Serializable;
        GhostIA _GhostIA;
        MemberMap _MemberMap;
        [NUnit.Framework.SetUp]
        public void Setup()
        {
            var ghostIA = new GhostIA();
            ISerializable serializable = new Regulus.Remote.Tests.Serializer().Serializable;
            var map = new MemberMap(typeof(IA).GetMethods(), typeof(IA).GetEvents(), typeof(IA).GetProperties(), new System.Tuple<System.Type, System.Func<Regulus.Remote.IProvider>>[] { });
            
            _Serializable = serializable;
            _GhostResponseHandler = new GhostResponseHandler(ghostIA, map , serializable);
            _GhostIA = ghostIA;
            _MemberMap = map;
        }
        [NUnit.Framework.Test]
        public void BaseTest()
        {
            var ghost = NSubstitute.Substitute.For<IGhost>();
            var handler = new Regulus.Remote.GhostResponseHandler(ghost,null, null);
            NUnit.Framework.Assert.AreEqual(ghost , handler.Base);
        }

        [NUnit.Framework.Test]
        public void InvokeEvent()
        {

        }
        [NUnit.Framework.Test]
        public void UpdateSetPropertyTest()
        {
            var handler = _GhostResponseHandler;
            handler.UpdateSetProperty(1, _Serializable.Serialize(typeof(int) , 2));
            NUnit.Framework.Assert.AreEqual(2, _GhostIA.Property1.Value);
        }


    }    
}
