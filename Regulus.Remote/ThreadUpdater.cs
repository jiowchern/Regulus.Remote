using Regulus.Utility;
using System.Threading;

namespace Regulus.Remote
{
    public class ThreadUpdater
    {
        private readonly System.Action _Updater;
        
        private CancellationTokenSource _Cancel;         
        public ThreadUpdater(System.Action updater)
        {
            _Updater = updater;            
        }

        void _Update()
        {
            AutoPowerRegulator regulator = new AutoPowerRegulator(new PowerRegulator());

            while (!_Cancel.Token.IsCancellationRequested)
            {
                _Updater();
                regulator.Operate();
            }

        }
        public void Start()
        {

            _Cancel = new CancellationTokenSource();
            System.Threading.Tasks.Task.Factory.StartNew(_Update, _Cancel.Token);//_Task.Start();

        }

        public void Stop()
        {
            _Cancel.Cancel();            
            _Cancel.Dispose();
        }
    }
}
