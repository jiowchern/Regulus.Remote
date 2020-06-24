using Regulus.Framework;
using Regulus.Remote;
using Regulus.Utility;
using System;
using System.Net;

namespace Regulus.Application.Client.Test
{
    internal class TestAgent : IAgent
    {

        readonly System.Collections.Generic.Dictionary<Type, IProvider> _Providers;
        long IAgent.Ping => throw new NotImplementedException();

        bool IAgent.Connected => throw new NotImplementedException();

        public TestAgent()
        {
            _Providers = new System.Collections.Generic.Dictionary<Type, IProvider>();
            _Providers.Add(typeof(IType), new TProvider<IType>());


        }

        event Action IAgent.BreakEvent
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event Action IAgent.ConnectEvent
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event Action<string, string> IAgent.ErrorMethodEvent
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        internal void Add(Type base_type , CType cType)
        {
            var provider = _Providers[base_type];
            
            provider.Add(cType);
            provider.Ready(cType.Id);
        }

        event Action<byte[], byte[]> IAgent.ErrorVerifyEvent
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        

        

        void IBootable.Launch()
        {
            throw new NotImplementedException();
        }

        internal void Remove(Type base_type, CType cType)
        {
            var provider = _Providers[base_type];

            
            provider.Remove(cType.Id);
        }

        INotifier<T> INotifierQueryable.QueryNotifier<T>() 
        {

            return _Providers[typeof(T)] as INotifier<T>;
        }

        void IBootable.Shutdown()
        {
            throw new NotImplementedException();
        }

        bool IUpdatable.Update()
        {
            throw new NotImplementedException();
        }
    }
}