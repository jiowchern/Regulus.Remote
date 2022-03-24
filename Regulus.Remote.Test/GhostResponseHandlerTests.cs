﻿using System;
using System.Reflection;
using NSubstitute;

namespace Regulus.Remote.Tests
{
    

    public class GhostResponseHandlerTests
    {
        interface IA
        {
            Regulus.Remote.Property<int> Property1 { get; }
            event System.Action<int> Event1;
        }
        class GhostIA : IA, IGhost
        {
            public GhostIA()
            {                
                _Regulus_Remote_Tests_GhostResponseHandlerTests_IA_Property1 = new Property<int>();
                _Regulus_Remote_Tests_GhostResponseHandlerTests_IA_Event1 = new GhostEventHandler();
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
            
            private readonly GhostEventHandler _Regulus_Remote_Tests_GhostResponseHandlerTests_IA_Event1;
            event Action<int> IA.Event1
            {
                add
                {
                    _Regulus_Remote_Tests_GhostResponseHandlerTests_IA_Event1.Add(value);
                }

                remove
                {
                    _Regulus_Remote_Tests_GhostResponseHandlerTests_IA_Event1.Remove(value);
                }
            }
            
            private readonly Property<int> _Regulus_Remote_Tests_GhostResponseHandlerTests_IA_Property1;
            Property<int> IA.Property1 => _Regulus_Remote_Tests_GhostResponseHandlerTests_IA_Property1;

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
        //GhostIA _GhostIA;
        IA _IA;
        MemberMap _MemberMap;
        [NUnit.Framework.SetUp]
        public void Setup()
        {
            var ghostIA = new GhostIA();
            ISerializable serializable = new Regulus.Remote.Tests.Serializer().Serializable;
            var map = new MemberMap(typeof(IA).GetMethods(), typeof(IA).GetEvents(), typeof(IA).GetProperties(), new System.Tuple<System.Type, System.Func<Regulus.Remote.IProvider>>[] { });
            
            _Serializable = serializable;
            _GhostResponseHandler = new GhostResponseHandler(ghostIA, map , serializable);
            _MemberMap = map;
            _IA = ghostIA;
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
            var handler = _GhostResponseHandler;
            var eventId = _MemberMap.GetEvent(typeof(IA).GetEvents()[0]);
            int value = 0;
            _IA.Event1 += (v) => value = v;

            byte[][] buffers = new byte[][] { _Serializable.Serialize(typeof(int), 10) };
            handler.InvokeEvent(eventId, 1, buffers);
            NUnit.Framework.Assert.AreEqual(10, value);
        }

        [NUnit.Framework.Test]
        public void UpdateSetPropertyTest()
        {
            var handler = _GhostResponseHandler;
            var property = _MemberMap.GetProperty(typeof(IA).GetProperties()[0]);
            handler.UpdateSetProperty(property, _Serializable.Serialize(typeof(int) , 2));
            NUnit.Framework.Assert.AreEqual(2, _IA.Property1.Value);
        }


    }    
}
