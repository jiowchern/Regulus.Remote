using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Regulus.Network
{
    public class TaskExecutionController : IDisposable
    {
        public interface Collectible
        {
            System.Collections.Concurrent.BlockingCollection<System.Func<Task>> Funcs();
        }
        private readonly CancellationTokenSource _CancellationTokenSource;
        private readonly Task _Task;
        private readonly System.Collections.Generic.List<Collectible> _Collectibles; 

        public TaskExecutionController()
        {
            _CancellationTokenSource = new CancellationTokenSource();
            _Collectibles = new List<Collectible>();
            _Task = Task.Run(() => _Run(_CancellationTokenSource.Token));
        }

        public void Add(Collectible collectible)
        {
            lock(_Collectibles)
            {
                _Collectibles.Add(collectible);
            }
        }

        public void Remove(Collectible collectible)
        {
            lock(_Collectibles)
            {
                _Collectibles.Remove(collectible);
            }
        }

        private async void _Run(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                if (_Collectibles.Count == 0)
                    continue;
                var collectors = new System.Collections.Concurrent.BlockingCollection<System.Func<Task>>[0];
                lock (_Collectibles)
                {
                    collectors = _Collectibles.Select(c => c.Funcs()).ToArray();
                }

                System.Collections.Concurrent.Partitioner.Create(0, collectors.Length, 60)
                        .AsParallel()
                        .ForAll(async (tup) =>
                        {
                            var colls = collectors.Skip(tup.Item1).Take(tup.Item2 - tup.Item1).ToArray();
                            while(true)
                            {
                                if (cancellationToken.IsCancellationRequested)
                                {
                                    break;
                                }
                                var index = System.Collections.Concurrent.BlockingCollection<System.Func<Task>>.TryTakeFromAny(colls, out var func, 100, cancellationToken);
                                if (index >= 0)
                                {
                                    await func();
                                }
                                else
                                { 
                                    break; 
                                }
                            }
                            
                        });

                
                
            }
        }
        void IDisposable.Dispose()
        {
            _CancellationTokenSource.Cancel();            
            _Task.Wait(); // 等待任務完成            
            _CancellationTokenSource.Dispose();
        }
    }
}