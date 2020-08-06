using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

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
            var user = new User(stream, _Protocol);
            user.Launch();            
            _Entry.AssignBinder(user.Binder);
            _Users.Add(user);
        }

		public void Leave(Network.IStreamable stream)
        {
            var user = _Users.FirstOrDefault(u => u.Stream == stream);
            if(user != null)
            {                
                _Users.Remove(user);
                user.Shutdown();
            }
        }

        void IDisposable.Dispose()
        {
            foreach (var user in _Users)
            {
                user.Shutdown();
            }
            _Users.Clear();
        }
    }
}
