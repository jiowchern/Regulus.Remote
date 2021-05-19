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
        readonly Regulus.Utility.Updater _Updater;

        readonly ThreadUpdater _ThreadUpdater;

        public Service(IBinderProvider entry, IProtocol protocol)
        {
            
            _Users = new System.Collections.Generic.List<User>();
            this._Entry = entry;
            this._Protocol = protocol;
            _Updater = new Utility.Updater();
            _Updater.AddEvent += (user) => _Entry.AssignBinder(((User)user).Binder, ((User)user).State);
            _ThreadUpdater = new ThreadUpdater(_Update);
            _ThreadUpdater.Start();
        }

        private void _Update()
        {
            _Updater.Working();
        }

        void IService.Join(Network.IStreamable stream,object state)
        {
            User user = new User(stream, _Protocol , state);
            lock(_Users)
            {
                _Users.Add(user);                
            }
            
            _Updater.Add(user);
            
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
                _Updater.Remove(user);                
                lock (_Users)
                    _Users.Remove(user);
            }
        }

        void IDisposable.Dispose()
        {
            _ThreadUpdater.Stop();
            _Updater.Shutdown();
            lock (_Users)
            {                
                _Users.Clear();
            }            
        }
    }
}
