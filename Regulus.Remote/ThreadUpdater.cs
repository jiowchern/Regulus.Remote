using Regulus.Utility;
using System.Threading;
using System.Threading.Tasks;

namespace Regulus.Remote
{
    public class ThreadUpdater
    {
        private readonly System.Action _Updater;
        
        private CancellationTokenSource _Cancel;
        private Task _Task;        

        public ThreadUpdater(System.Action updater)
        {
            _Updater = updater;            
        }

        void _Update(CancellationToken token)
        {
            AutoPowerRegulator regulator = new AutoPowerRegulator(new PowerRegulator());
            System.Threading.Thread.Sleep(1000);
            while (!token.IsCancellationRequested)
            {
                _Updater();
                regulator.Operate();
            }

        }
        public void Start()
        {

            _Cancel = new CancellationTokenSource();
            
            _Task = System.Threading.Tasks.Task.Factory.StartNew(()=>_Update(_Cancel.Token), _Cancel.Token);

        }

        public void Stop()
        {
            _Cancel.Cancel();            
            _Cancel.Dispose();

            _Task.Wait();
        }
    }
}
