using System;

namespace Regulus.Remote.Soul
{
    public class AsyncService : Soul.IService
    {
        
        readonly ThreadUpdater _ThreadUpdater;

        readonly SyncService _SyncService;
        readonly IDisposable _Disposed;
        public AsyncService(SyncService syncService) 
        {
            _SyncService = syncService;
            _Disposed = _SyncService;
            _ThreadUpdater = new ThreadUpdater(_Update);
            _ThreadUpdater.Start();
        }

        private void _Update()
        {

            _SyncService.Update();
        }

        void IDisposable.Dispose()
        {
            _ThreadUpdater.Stop();
            _Disposed.Dispose();
        }
        
    }
}

