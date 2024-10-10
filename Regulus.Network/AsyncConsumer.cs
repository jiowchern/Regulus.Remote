using Regulus.Remote;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Regulus.Network
{
    public class AsyncConsumer<T> : IDisposable where T : Task
    {
        List<T> _Items;
        readonly List<T> _Items1;
        readonly List<T> _Items2;
        readonly SemaphoreSlim _Semaphore;
        readonly Task _Task;
        readonly CancellationTokenSource _CancellationTokenSource;
        
        public delegate Task ConsumeHandler(T item);
        public event ConsumeHandler ConsumeEvent;

        public AsyncConsumer()
        {
            _Items1 = new List<T>();
            _Items2 = new List<T>();
            _Items = _Items1;
            _Semaphore = new SemaphoreSlim(0);
            _CancellationTokenSource = new CancellationTokenSource();

            _Task = Task.Run(() => _Run(_CancellationTokenSource.Token));
            
        }

        private async void _Run(CancellationToken cancellationToken)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            while (true)
            {
                _Semaphore.Wait(cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }


                System.Collections.Generic.List<T> items ;
                lock  (_Items)
                {
                    items = _Items;
                    _Items = _Items == _Items1 ? _Items2 : _Items1;                    
                }
                //stopwatch.Restart();
                foreach (var item in items)
                {
                    await item.ContinueWith(t => {
                        t.Exception?.Handle(e =>
                        {
                            Regulus.Utility.Log.Instance.WriteInfo($"AsyncConsumer error {e.ToString()}.");
                            return true; 
                        });
                        
                    });                    
                }
                items.Clear();
                //var relayTime = (int)stopwatch.ElapsedMilliseconds;
                //await System.Threading.Tasks.Task.Delay(relayTime+1);
                


            }
        }

        public void Enqueue(T item)
        {
            lock (_Items)
            {
                _Items.Add(item);
            }
            _Semaphore.Release();
        }

        public void Dispose()
        {
            _CancellationTokenSource.Cancel();
            _Semaphore.Release(); // 釋放等待中的執行緒
            _Task.Wait(); // 等待任務完成
            _Semaphore.Dispose();
            _CancellationTokenSource.Dispose();
        }
    }
}