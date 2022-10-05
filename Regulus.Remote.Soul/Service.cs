using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Remote.Soul
{
    public class Service : IService , AdvanceProviable
    {
        private readonly IBinderProvider _Entry;
        private readonly IProtocol _Protocol;
        private readonly ISerializable _Serializable;
        private readonly IListenable _Listenable;
        private readonly IInternalSerializable _InternalSerializable;
        readonly System.Collections.Generic.List<User> _Users;
        readonly Regulus.Utility.Updater _Updater;
        readonly  Regulus.Remote.ThreadUpdater _ThreadUpdater;

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
            _Updater.AddEvent += (user) => _Entry.AssignBinder((user as User).Binder);
            _Updater.AddEvent += (user) => _JoinEvent.Invoke(user as User);
            _Updater.RemoveEvent += (user) => _LeaveEvent.Invoke(user as User);

            

            _JoinEvent += d => { };
            _LeaveEvent += d => { };

            _ThreadUpdater = new ThreadUpdater(_Drive);
            _ThreadUpdater.Start();
        }

        event Action<Advanceable> _JoinEvent;
        event Action<Advanceable> AdvanceProviable.JoinEvent
        {
            add
            {
                _JoinEvent += value;
            }

            remove
            {
                _JoinEvent -= value;
            }
        }

        event Action<Advanceable> _LeaveEvent;
        event Action<Advanceable> AdvanceProviable.LeaveEvent
        {
            add
            {
                _LeaveEvent += value;
            }

            remove
            {
                _LeaveEvent -= value;
            }
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
            _ThreadUpdater.Stop();
            _Listenable.StreamableEnterEvent -= _Join;
            _Listenable.StreamableLeaveEvent -= _Leave;

            lock (_Users)
            {                
                _Users.Clear();
            }            
        }

        void _Drive()
        {
            _Updater.Working();
        }
    }
}

