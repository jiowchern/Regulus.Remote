using System;
using System.Diagnostics;

namespace Regulus.Remote.Soul
{
    public class SyncService : System.IDisposable
    {
        readonly IEntry _Entry;
        readonly UserProvider _UserProvider;
        readonly IDisposable _UserProviderDisposable;
        


        public SyncService(IEntry entry, UserProvider user_provider)
        {
            _Entry = entry;
            _UserProvider = user_provider;
            _UserProviderDisposable = user_provider;
        }

        public void Update()
        {
            
            while (_UserProvider.UserLifecycleEvents.TryTake(out var userAction))
            {
                if(userAction.State == UserProvider.UserLifecycleState.Join)
                {
                    _Entry.RegisterClientBinder(userAction.User.Binder);
                }
                else if(userAction.State == UserProvider.UserLifecycleState.Leave)
                {
                    _Entry.UnregisterClientBinder(userAction.User.Binder);
                }
                
            }

            _Entry.Update();
            
            
            foreach (Advanceable user in _UserProvider.Users.Values)
            {
                user.Advance();
            }
            
        }

        void IDisposable.Dispose()
        {            
            _UserProviderDisposable.Dispose();
        }
    }
}

