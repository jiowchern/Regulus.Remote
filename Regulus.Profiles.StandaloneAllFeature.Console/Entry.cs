using Regulus.Profiles.StandaloneAllFeature.Protocols;
using Regulus.Remote;

namespace Regulus.Profiles.StandaloneAllFeature.Server
{
    class Entry : Regulus.Remote.IEntry 
    {
        readonly System.Collections.Generic.List<User> _Users;

        public Entry()
        {
            _Users = new List<User>();
        }

        void IBinderProvider.AssignBinder(IBinder binder)
        {
            binder.BreakEvent += () => {
                _Users.RemoveAll(user => user.Binder == binder);
            };
            
            var user = new User(binder);
            _Users.Add(user);
            
        }

    }
}
