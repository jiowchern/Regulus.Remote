using Regulus.Utility;
using System;

namespace Regulus.Remote.Sample
{
 /*   public class UpdatableEntry : IBinderProvider, System.IDisposable
    {
        public delegate IUpdatable OnSpawnUpdatable(IBinder binder);
        readonly System.Collections.Concurrent.ConcurrentQueue<IBinder> _AddBinders;
        readonly System.Collections.Concurrent.ConcurrentQueue<IUpdatable> _RemoveBinders;
        readonly Regulus.Utility.Updater _Users;
        private readonly ThreadUpdater _Updater;
        private readonly OnSpawnUpdatable _Proviable;

        public UpdatableEntry(OnSpawnUpdatable proviable)
        {
            _Users = new Utility.Updater();
            _AddBinders = new System.Collections.Concurrent.ConcurrentQueue<IBinder>();
            _RemoveBinders = new System.Collections.Concurrent.ConcurrentQueue<IUpdatable>();

            _Updater = new Regulus.Remote.ThreadUpdater(_Update);
            _Updater.Start();
            this._Proviable = proviable;
        }

        private void _Update()
        {
            IBinder binder;
            while (_AddBinders.TryDequeue(out binder))
            {
                var user = _Proviable(binder);
                binder.BreakEvent += () => { _RemoveBinders.Enqueue(user); };
                _Users.Add(user);
            }
            IUpdatable removeUser;
            while (_RemoveBinders.TryDequeue(out removeUser))
            {
                _Users.Remove(removeUser);
            }
            _Users.Working();
        }

        void IBinderProvider.AssignBinder(IBinder binder)
        {
            _AddBinders.Enqueue(binder);
        }
        void IDisposable.Dispose()
        {
            _Updater.Stop();
            _Users.Shutdown();
        }
    }*/
}

