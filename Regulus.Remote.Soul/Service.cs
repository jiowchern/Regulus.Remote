using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Remote.Soul
{
    public class Service : IService
    {
        private readonly IBinderProvider _Entry;
        private readonly IProtocol _Protocol;

        readonly System.Collections.Generic.List<User> _Users;

        public Service(IBinderProvider entry, IProtocol protocol)
        {
            _Users = new System.Collections.Generic.List<User>();
            this._Entry = entry;
            this._Protocol = protocol;
        }
        void IService.Join(Network.IStreamable stream,object state)
        {
            User user = new User(stream, _Protocol);
            lock(_Users)
            {
                _Users.Add(user);                
            }
            user.Launch();
            _Entry.AssignBinder(user.Binder, state);
        }

        void IService.Leave(Network.IStreamable stream)
        {
            User user = null;
            lock(_Users)
            {
                user = _Users.FirstOrDefault( u=>u.Stream == stream);                
            }
            if(user != null)
            {
                user.Shutdown();
                lock (_Users)
                    _Users.Remove(user);
            }
                
            
        }

        void IDisposable.Dispose()
        {            
            lock(_Users)
            {
                foreach (var user in _Users)
                {
                    user.Shutdown();
                }
                _Users.Clear();
            }            
        }
    }
}
