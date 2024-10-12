
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Regulus.Remote;
using Regulus.Remote.Reactive;

namespace Regulus.Profiles.StandaloneAllFeature.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var protocol = Regulus.Profiles.StandaloneAllFeature.Protocols.ProtocolProvider.Create();
            var entry = new Server.Entry();
            int range = 10;

            
            

            
            var set = Regulus.Remote.Server.Provider.CreateTcpService( entry, protocol);
            var port = Regulus.Network.Tcp.Tools.GetAvailablePort();
            set.Listener.Bind(port);

            ProcessAgents( range, ()=>{
                var clientSet = Regulus.Remote.Client.Provider.CreateTcpAgent(Regulus.Profiles.StandaloneAllFeature.Protocols.ProtocolProvider.Create());

                var w = clientSet.Connector.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, port)).GetAwaiter();
                var peer = w.GetResult();
                clientSet.Agent.Enable(peer);
                return clientSet.Agent;
            });


            var service = Regulus.Remote.Standalone.Provider.CreateService(entry, protocol);
            ProcessAgents(range, () => {
                lock (service)
                    return service.Create();
            });

            set.Service.Dispose();

        }

        private static void ProcessAgents(int range,Func<Regulus.Remote.Ghost.IAgent> agent_provider)
        {
            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = 10,
            };

            var ticks = 0L;
            for (int i = 1;i<=range;i++)
            {

                System.Threading.Tasks.Parallel.For(0, 10, options, index =>
                {
                    var user = new User(agent_provider(), i * (index + 1));
                    var agent = user.Agent;

                    var obs = from e in agent.QueryNotifier<Regulus.Profiles.StandaloneAllFeature.Protocols.Featureable>().SupplyEvent()
                              from num in System.Reactive.Linq.Observable.Range(0, range)
                              from v in e.Inc(System.Guid.NewGuid().ToString() + System.Guid.NewGuid().ToString()+ System.Guid.NewGuid().ToString()+ System.Guid.NewGuid().ToString()+ System.Guid.NewGuid().ToString()+ System.Guid.NewGuid().ToString()+ System.Guid.NewGuid().ToString()+ System.Guid.NewGuid().ToString()+ System.Guid.NewGuid().ToString()+ System.Guid.NewGuid().ToString()+ System.Guid.NewGuid().ToString()+ System.Guid.NewGuid().ToString()).RemoteValue()
                              select v;
                    bool enable = true;

                    var bufferObs = obs.Buffer(range);
                    var stopWatch = new Stopwatch();
                    stopWatch.Restart();
                    System.Console.WriteLine($"Start {user.Id}/{range * 10}");
                    bufferObs.Subscribe(v =>
                    {
                        stopWatch.Stop();
                        user.Ticks = stopWatch.ElapsedTicks;
                        enable = false;
                    });

                    long sleepCount = 0;
                    while (enable)
                    {
                        agent.Update();
                        var sw = Stopwatch.StartNew();
                        System.Threading.Tasks.Task.Delay(range).Wait();
                        sleepCount += sw.ElapsedTicks;
                    }

                    agent.Disable();
                    user.Ticks = user.Ticks - sleepCount;
                    var time = new TimeSpan(user.Ticks / range);
                    System.Console.WriteLine($"Done {user.Id}/{range} time:{time}");

                    System.Threading.Interlocked.Add(ref ticks, user.Ticks);
                });

            }





            var average = new TimeSpan(ticks / range / range);
            System.Console.WriteLine($"Average time : {average} ({average.TotalMilliseconds}ms)");
            System.Console.WriteLine($"Total time : {new TimeSpan(ticks)}");

            //service.Dispose();

            var chunks = Regulus.Memorys.PoolProvider.Shared.Chunks;
            foreach (var chunk in chunks)
            {
                System.Console.WriteLine($"Remote Chunk : {chunk.BufferSize} {chunk.AvailableCount} {chunk.DefaultAllocationThreshold} {chunk.PageSize}");
            }
        }
    }
}
