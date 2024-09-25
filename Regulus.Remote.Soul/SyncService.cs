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
            User newUser;
            while (_UserProvider.NewUsers.TryDequeue(out newUser))
            {
                _Binder.AssignBinder(newUser.Binder);
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

