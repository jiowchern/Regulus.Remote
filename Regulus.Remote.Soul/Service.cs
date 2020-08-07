using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Remote.Soul
{
    public class Service : IDisposable
    {
        private readonly IBinderProvider _Entry;
        private readonly IProtocol _Protocol;
        readonly List<User> _Users;

        public Service(IBinderProvider entry, IProtocol protocol)
        {
            _Users = new List<User>();
            this._Entry = entry;
            this._Protocol = protocol;
        }
        public void Join(Network.IStreamable stream)
        {
            User user = new User(stream, _Protocol);
            user.Launch();
            _Entry.AssignBinder(user.Binder);
            _Users.Add(user);
        }

        public void Leave(Network.IStreamable stream)
        {
            User user = _Users.FirstOrDefault(u => u.Stream == stream);
            if (user != null)
            {
                _Users.Remove(user);
                user.Shutdown();
            }
        }

        void IDisposable.Dispose()
        {
            foreach (User user in _Users)
            {
                user.Shutdown();
            }
            _Users.Clear();
        }
    }
}
