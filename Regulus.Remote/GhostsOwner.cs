using System;
using System.Collections.Generic;

namespace Regulus.Remote
{
    namespace ProviderHelper
    {
        public class GhostsOwner
        {
            private readonly Dictionary<Type, IProvider> _Providers;
            private readonly IProtocol _Protocol;

            public GhostsOwner(IProtocol protocol)
            {
                _Protocol = protocol;
                _Providers = new Dictionary<Type, IProvider>();
            }

            public IProvider QueryProvider(Type type)
            {
                lock (_Providers)
                {
                    if (!_Providers.TryGetValue(type, out var provider))
                    {
                        provider = BuildProvider(type);
                        _Providers.Add(type, provider);
                    }
                    return provider;
                }
            }

            private IProvider BuildProvider(Type type)
            {
                MemberMap map = _Protocol.GetMemberMap();
                return map.CreateProvider(type);
            }

            public INotifier<T> QueryProvider<T>()
            {
                return QueryProvider(typeof(T)) as INotifier<T>;
            }

            public void ClearProviders()
            {
                lock (_Providers)
                {
                    foreach (var provider in _Providers.Values)
                    {
                        provider.ClearGhosts();
                    }
                    _Providers.Clear();
                }
            }

            public void RemoveGhost(long id)
            {
                lock (_Providers)
                {
                    foreach (var provider in _Providers.Values)
                    {
                        provider.Remove(id);
                    }
                }
            }
        }

    }
}
