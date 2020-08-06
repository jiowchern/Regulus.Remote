
using Regulus.Remote;
using Regulus.Utility;
using System;
using System.Net;

namespace Regulus.Application.Client.Test
{
    internal class TestAgent : INotifierQueryable
    {

        readonly System.Collections.Generic.Dictionary<Type, IProvider> _Providers;
        

        public TestAgent()
        {
            _Providers = new System.Collections.Generic.Dictionary<Type, IProvider>();
            _Providers.Add(typeof(IType), new TProvider<IType>());


        }

      

        

        

        internal void Add(Type base_type , CType cType)
        {
            var provider = _Providers[base_type];
            
            provider.Add(cType);
            provider.Ready(cType.Id);
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

        
    }
}