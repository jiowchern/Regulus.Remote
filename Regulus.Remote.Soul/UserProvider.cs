using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.Remote.Soul
{



    public class UserProvider : IDisposable
    {
        
        private readonly IProtocol _Protocol;
        private readonly ISerializable _Serializable;
        private readonly IListenable _Listenable;
        private readonly IInternalSerializable _InternalSerializable;
        readonly System.Collections.Generic.List<User> _Users;
        readonly Regulus.Utility.Looper<Network.IStreamable> _Looper;
        public readonly System.Collections.Concurrent.ConcurrentQueue<User> NewUsers;
        public UserProvider(IProtocol protocol, ISerializable serializable , IListenable listenable, Regulus.Remote.IInternalSerializable internal_serializable)
        {
            NewUsers = new System.Collections.Concurrent.ConcurrentQueue<User>();
            _Users = new System.Collections.Generic.List<User>();
            _Looper = new Utility.Looper<Network.IStreamable>();
            _Looper.AddItemEvent += _Join;
            _Looper.RemoveItemEvent -= _Leave;
            this._Protocol = protocol;
            this._Serializable = serializable;
            this._Listenable = listenable;
            _InternalSerializable = internal_serializable;
            _Listenable.StreamableEnterEvent += _Looper.Add;
            _Listenable.StreamableLeaveEvent += _Looper.Remove;

        
        }

        void _Join(Network.IStreamable stream)
        {
            User user = new User(stream, _Protocol , _Serializable, _InternalSerializable);
            
            user.Launch();
            lock (_Users)
            {
                _Users.Add(user);                
            }
            NewUsers.Enqueue(user);
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
                user.Shutdown();
                lock (_Users)
                    _Users.Remove(user);
            }
        }

        void IDisposable.Dispose()
        {
        
            _Listenable.StreamableEnterEvent -= _Join;
            _Listenable.StreamableLeaveEvent -= _Leave;

            lock (_Users)
            {                
                _Users.Clear();
            }            
        }

        public IEnumerable<User> GetUsers()
        {
            _Looper.Update();
            lock(_Users)
            {
                return _Users.ToArray();
            }
                
        }
                
    }
}

