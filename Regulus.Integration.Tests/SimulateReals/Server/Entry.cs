using Regulus.Remote;

namespace Regulus.Integration.Tests
{
    namespace SimulateReals
    {
        namespace Server
        {
            class Entry : Regulus.Remote.IEntry
            {
                readonly System.Collections.Generic.Dictionary<Regulus.Remote.IBinder, User> _Users;
                public Entry()
                {
                    _Users = new System.Collections.Generic.Dictionary<IBinder, User>();
                }
                void IBinderProvider.RegisterClientBinder(IBinder binder)
                {
                    _Users.Add(binder, new User(binder));
                }

                void IBinderProvider.UnregisterClientBinder(IBinder binder)
                {
                    _Users.Remove(binder);
                }
            }
        }
    }
    
}