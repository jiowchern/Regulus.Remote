using System;

namespace Regulus.Remote.Soul
{
    public class SyncService : System.IDisposable
    {
        readonly IBinderProvider _Binder;
        readonly UserProvider _UserProvider;
        readonly IDisposable _UserProviderDisposable;
        


        public SyncService(IBinderProvider binder, UserProvider user_provider)
        {
            _Binder = binder;
            _UserProvider = user_provider;
            _UserProviderDisposable = user_provider;
        }

        public void Update()
        {
            
            while (_UserProvider.UserLifecycleEvents.TryTake(out var userAction))
            {
                if(userAction.State == UserProvider.UserLifecycleState.Join)
                {
                    _Binder.RegisterClientBinder(userAction.User.Binder);
                }
                else if(userAction.State == UserProvider.UserLifecycleState.Leave)
                {
                    _Binder.UnregisterClientBinder(userAction.User.Binder);
                }
                
            }

            var users = _UserProvider.GetUsers();
            foreach (Advanceable user in users)
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

