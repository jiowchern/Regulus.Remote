using Regulus.Memorys;
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
        
        readonly System.Collections.Concurrent.ConcurrentDictionary<Network.IStreamable, User> _Users;
        public readonly IReadOnlyDictionary<Network.IStreamable,User> Users;
        
        private readonly IPool _Pool;
        public enum UserLifecycleState
        {
            Join,
            Leave
        }
        public struct UserLifecycleEvent
        {
            public UserLifecycleState State;
            public User User;
        }

        public readonly System.Collections.Concurrent.ConcurrentBag<UserLifecycleEvent> UserLifecycleEvents;
        
        public UserProvider(IProtocol protocol, ISerializable serializable , IListenable listenable, Regulus.Remote.IInternalSerializable internal_serializable , Regulus.Memorys.IPool pool)
        {
            _Pool = pool;

            UserLifecycleEvents = new System.Collections.Concurrent.ConcurrentBag<UserLifecycleEvent>();            
            _Users = new System.Collections.Concurrent.ConcurrentDictionary<Network.IStreamable, User>();
            Users= _Users;


            this._Protocol = protocol;
            this._Serializable = serializable;
            this._Listenable = listenable;
            _InternalSerializable = internal_serializable;
            _Listenable.StreamableEnterEvent += _Join;
            _Listenable.StreamableLeaveEvent += _Leave;

        
        }

        void _Join(Network.IStreamable stream)
        {
            User user = new User(stream, _Protocol , _Serializable, _InternalSerializable, _Pool);
            user.ErrorEvent += () => _Leave(user.Stream);
            user.Launch();
            while(!_Users.TryAdd(user.Stream, user))
            {
                System.Threading.Thread.Sleep(1);
            }
            UserLifecycleEvents.Add(new UserLifecycleEvent { State = UserLifecycleState.Join, User = user });    
        }

        void _Leave(Network.IStreamable stream)
        {
            User user = null;            
            while(!_Users.TryRemove(stream, out user))
            {
                System.Threading.Thread.Sleep(1);
            }
            
            if(user != null)
            {                
                user.Shutdown();
                UserLifecycleEvents.Add(new UserLifecycleEvent { State = UserLifecycleState.Leave, User = user });
            }
        }

        void IDisposable.Dispose()
        {            

            _Listenable.StreamableEnterEvent -= _Join;
            _Listenable.StreamableLeaveEvent -= _Leave;

            _Users.Clear();
        }

        
                
    }
}

