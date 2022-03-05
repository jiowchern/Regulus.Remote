using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Remote.Soul
{
    public class Service : IService
    {
        private readonly IBinderProvider _Entry;
        private readonly IProtocol _Protocol;
        private readonly ISerializable _Serializable;
        private readonly IListenable _Listenable;
        private readonly IInternalSerializable _InternalSerializable;
        readonly System.Collections.Generic.List<User> _Users;
        readonly Regulus.Utility.Updater _Updater;

        readonly ThreadUpdater _ThreadUpdater;

        public Service(IBinderProvider entry, IProtocol protocol, ISerializable serializable , IListenable listenable, Regulus.Remote.IInternalSerializable internal_serializable)
        {
            
            _Users = new System.Collections.Generic.List<User>();
            this._Entry = entry;
            this._Protocol = protocol;
            this._Serializable = serializable;
            this._Listenable = listenable;
            _InternalSerializable = internal_serializable;

            _Listenable.StreamableEnterEvent += _Join;
            _Listenable.StreamableLeaveEvent += _Leave;
            _Updater = new Utility.Updater();
            _Updater.AddEvent += (user) => _Entry.AssignBinder(((User)user).Binder);
            _ThreadUpdater = new ThreadUpdater(_Update);
            _ThreadUpdater.Start();
        }

        private void _Update()
        {
            _Updater.Working();
        }

        void _Join(Network.IStreamable stream)
        {
            User user = new User(stream, _Protocol , _Serializable, _InternalSerializable);
            lock(_Users)
            {
                _Users.Add(user);                
            }
            
            _Updater.Add(user);
            
        }

        void _Leave(Network.IStreamable stream)
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
            _Listenable.StreamableEnterEvent -= _Join;
            _Listenable.StreamableLeaveEvent -= _Leave;

            _ThreadUpdater.Stop();
            _Updater.Shutdown();
            lock (_Users)
            {                
                _Users.Clear();
            }            
        }
    }
}
