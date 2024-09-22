
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Regulus.Remote.Reactive;

namespace Regulus.Profiles.StandaloneAllFeature.Console
{
    class User
    {
        public readonly Regulus.Remote.Ghost.IAgent Agent;
        public readonly int Id;
        public long Ticks;
        public User(Regulus.Remote.Ghost.IAgent agent, int id)
        {
            Agent = agent;
            Id = id;
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var protocol = Regulus.Profiles.StandaloneAllFeature.Protocols.ProtocolProvider.Create();
            var entry = new Server.Entry();
            var service = Regulus.Remote.Standalone.Provider.CreateService(entry , protocol);

            int range = 100;
            
            var agents = new System.Collections.Generic.List<User>();
            for (int i = 0; i < range; i++)
            {
                var agent = service.Create();
                agents.Add(new User(agent , i + 1));
            }

            
            
            System.Threading.Tasks.Parallel.ForEach(agents, a =>
            {
                var agent = a.Agent;

                var obs = from e in agent.QueryNotifier<Regulus.Profiles.StandaloneAllFeature.Protocols.Featureable>().SupplyEvent()
                          from num in System.Reactive.Linq.Observable.Range(0, range)
                          from v in e.Inc(num).RemoteValue()
                          select v;
                bool enable = true;


                var bufferObs = obs.Buffer(range);
                var stopWatch = new Stopwatch();
                stopWatch.Restart();
                System.Console.WriteLine($"Start {a.Id}/{range}");
                bufferObs.Subscribe(v =>
                {                    
                    stopWatch.Stop();
                    a.Ticks = stopWatch.ElapsedTicks;                    
                    System.Threading.Volatile.Write(ref enable, false);
                    
                });

                /*while(System.Threading.Volatile.Read(ref enable))
                {
                    agent.Update();
                    System.Threading.Tasks.Task.Delay(0).Wait();
                }*/
                System.Threading.SpinWait.SpinUntil(() => {
                    agent.Update();
                    return !System.Threading.Volatile.Read(ref enable);
                });

                var time = new TimeSpan(a.Ticks / range);
                System.Console.WriteLine($"Done {a.Id}/{range} time:{time}");
            });

            var ticks = agents.Sum( u=>u.Ticks );
            
            var average = new TimeSpan(ticks / range / range);
            System.Console.WriteLine($"Average time : {average} ({average.TotalMilliseconds}ms)");
            System.Console.WriteLine($"Total time : {new TimeSpan(ticks)}");

            service.Dispose();
        }
    }
}
