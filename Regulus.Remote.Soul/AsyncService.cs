using System;
using System.Threading.Tasks;

namespace Regulus.Remote.Soul
{
    public class AsyncService : Soul.IService
    {
        
        

        readonly SyncService _SyncService;
        readonly IDisposable _Disposed;

        readonly System.Threading.Tasks.Task _ThreadUpdater;
        volatile bool _Enable;
        public AsyncService(SyncService syncService) 
        {
            _SyncService = syncService;
            _Disposed = _SyncService;
            
            _Enable = true;
            _ThreadUpdater = Task.Factory.StartNew(() => _Update(), TaskCreationOptions.LongRunning);
        }

        private void _Update()
        {
            var ar = new Regulus.Utility.AutoPowerRegulator(new Regulus.Utility.PowerRegulator());
            while(_Enable)
            {
                _SyncService.Update();
                ar.Operate();
            }            
        }

        void IDisposable.Dispose()
        {
            _Enable = false;
            _ThreadUpdater.Wait();
            _Disposed.Dispose();
        }
        
    }
}

