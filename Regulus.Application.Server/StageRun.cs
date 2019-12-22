using System;
using Regulus.Network;
using Regulus.Remote.Soul;
using Regulus.Utility;


namespace Regulus.Remote.Soul.Console
{
    internal class ParallelUpdate : Launcher<IUpdatable>
    {
        public void Update()
        {
            /*var exceptions = new System.Collections.Concurrent.ConcurrentQueue<Exception>();

			Parallel.ForEach(base._GetObjectSet(), (updater) => 
			{
				try
				{
					_Update(updater);
				}
				catch (Exception e) { exceptions.Enqueue(e); }
			});

			if (exceptions.Count > 0) 
				throw new AggregateException(exceptions);*/
            foreach (var up in _GetObjectSet())
            {
                _Update(up);
            }
        }

        private void _Update(IUpdatable updater)
        {
            var result = false;

            result = updater.Update();

            if (result == false)
            {
                Remove(updater);
            }
        }
    }

    internal class StageRun : IStage
    {
        public event Action ShutdownEvent;

        private readonly Command _Command;

        private readonly Launcher _Launcher;

        private readonly Service _Server;

        private readonly Regulus.Utility.Console.IViewer _View;

        public StageRun(IEntry core, IProtocol protocol, Command command, int port, Regulus.Utility.Console.IViewer viewer, IListenable server)
        {
            _View = viewer;
            _Command = command;

            _Server = new Service(core, port, protocol, server);
            _Server.BreakEvent += _Break;
            _Launcher = new Launcher();
            _Launcher.Push(_Server);
        }

        private void _Break()
        {
            ShutdownEvent();
        }

        void IStage.Enter()
        {
            _Launcher.Launch();
            _Command.Register(
                "Status",
                () =>
                {
                    _View.WriteLine("PeerFPS:" + _Server.PeerFPS);
                    _View.WriteLine("PeerCount:" + _Server.PeerCount);
                    _View.WriteLine("CoreFPS:" + _Server.CoreFPS);

                    _View.WriteLine(string.Format("PeerUsage:{0:0.00%}", _Server.PeerUsage));
                    _View.WriteLine(string.Format("CoreUsage:{0:0.00%}", _Server.CoreUsage));

                    _View.WriteLine(
                        "\nTotalReadBytes:"
                        + string.Format("{0:N0}", Singleton<NetworkMonitor>.Instance.Read.TotalBytes));
                    _View.WriteLine(
                        "TotalWriteBytes:"
                        + string.Format("{0:N0}", Singleton<NetworkMonitor>.Instance.Write.TotalBytes));
                    _View.WriteLine(
                        "\nSecondReadBytes:"
                        + string.Format("{0:N0}", Singleton<NetworkMonitor>.Instance.Read.SecondBytes));
                    _View.WriteLine(
                        "SecondWriteBytes:"
                        + string.Format("{0:N0}", Singleton<NetworkMonitor>.Instance.Write.SecondBytes));
                    _View.WriteLine("\nRequest Queue:" + Peer.TotalRequest);
                    _View.WriteLine("Response Queue:" + Peer.TotalResponse);
                });
            _Command.Register("Shutdown", _ShutdownEvent);
        }

        void IStage.Leave()
        {
            _Launcher.Shutdown();

            _Command.Unregister("Shutdown");
            _Command.Unregister("FPS");
        }

        void IStage.Update()
        {
        }

        private void _ShutdownEvent()
        {
            ShutdownEvent();
        }
    }
}
