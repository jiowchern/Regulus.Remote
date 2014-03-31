using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net;
using log4net.Config;
using LogManager = ExitGames.Logging.LogManager;
using System.IO;

namespace Regulus.Remoting.Soul
{
	public abstract class PhotonApplication : Photon.SocketServer.ApplicationBase
	{
		System.Threading.Thread	_FrameworkThread;
		Regulus.Remoting.PhotonExpansion.IPhotonFramework _Framework;
		protected ILogger Logger{	get;private set;	}
		protected override void Setup()
		{
			
			LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
			GlobalContext.Properties["LogFileName"] = ApplicationName + System.DateTime.Now.ToString("yyyy-MM-dd");
			XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(BinaryPath, "log4net.config")));
			Logger = LogManager.GetLogger(this.GetType().FullName);
            
			Logger.Info("Setup begin.");
			 _Framework = _Setup();
			_Framework.Launch();
			_FrameworkThread = new System.Threading.Thread(_FrameworkUpdate);
            _FrameworkThread.Priority = System.Threading.ThreadPriority.Normal;
			_FrameworkThread.Start();

			Logger.Info("Setup end.");
		}

		void _FrameworkUpdate()
		{
			Logger.Info("_FrameworkUpdate begin.");

            Regulus.Utility.Poller<SoulProvider> providers = new Regulus.Utility.Poller<SoulProvider>();
			while (_Framework != null && _Framework.Update() == true && Running)
			{
                System.Threading.Thread.Sleep(0);
				Queue<SoulProvider> q = null;
				lock (_SynObject)
				{
                    if (_NewProviderQueue.Count > 0)
                    {
                        q = _NewProviderQueue;
                        _NewProviderQueue = new Queue<SoulProvider>();
                    }
					
				}

                if (q != null)
                {
                    foreach (var provider in q)
                    {
                        providers.Add(provider);

                        provider.BreakEvent += () =>
                        {
                            providers.Remove(p => p == provider);
                        };
                        _Framework.ObtainController(provider);
                    }
                }
                _UpdateProvider(providers.UpdateSet());

			}

			Logger.Info("_FrameworkUpdate end.");
		}

        private void _UpdateProvider(SoulProvider[] providers)
        {
            foreach (var provider in providers)
            {
                provider.Update();
            }
        }

		protected override void TearDown()
		{
			Logger.Info("TearDown begin.");

			if (_Framework != null)
				_Framework.Shutdown();
			_Framework = null;
			_FrameworkThread.Join();
			while (_FrameworkThread.IsAlive) ;

			Logger.Info("TearDown end.");
		}	
		object	_SynObject = new object();	
		System.Collections.Generic.Queue<Regulus.Remoting.Soul.SoulProvider> _NewProviderQueue = new Queue<SoulProvider>();
		
		protected override Photon.SocketServer.PeerBase CreatePeer(Photon.SocketServer.InitRequest initRequest)
		{
			
			var peer = new ServerPeer(initRequest);
            
			lock (_SynObject)
			{                
                _NewProviderQueue.Enqueue(new Regulus.Remoting.Soul.SoulProvider(peer, peer));
			}
			
			return peer;
		}

		protected abstract Regulus.Remoting.PhotonExpansion.IPhotonFramework _Setup();

	}
}
