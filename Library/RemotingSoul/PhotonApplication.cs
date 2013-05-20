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

namespace Samebest.Remoting.Soul
{
	public abstract class PhotonApplication : Photon.SocketServer.ApplicationBase
	{
		System.Threading.Thread	_FrameworkThread;
		Samebest.Remoting.PhotonExpansion.IPhotonFramework _Framework;
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
			_FrameworkThread.Start();

			Logger.Info("Setup end.");
		}

		void _FrameworkUpdate()
		{
			Logger.Info("_FrameworkUpdate begin.");
			while (_Framework != null && _Framework.Update() == true && Running)
			{
				Queue<SoulProvider> q = null;
				lock (_SynObject)
				{					
					q = _Queue;
					_Queue = new Queue<SoulProvider>();
				}

				if (q != null)
					foreach (var provider in q)
					{
						_Framework.ObtainController(provider);
					}				
				
			}

			Logger.Info("_FrameworkUpdate end.");
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
		System.Collections.Generic.Queue<Samebest.Remoting.Soul.SoulProvider> _Queue = new Queue<SoulProvider>();
		
		protected override Photon.SocketServer.PeerBase CreatePeer(Photon.SocketServer.InitRequest initRequest)
		{
			Logger.Info("CreatePeer begin.");
			var peer = new ServerPeer(initRequest);
			lock (_SynObject)
			{
				Logger.Info("CreatePeer Enqueue.");
				_Queue.Enqueue(new Samebest.Remoting.Soul.SoulProvider(peer));			
			}
			Logger.Info("CreatePeer end.");
			return peer;
		}


		protected abstract Samebest.Remoting.PhotonExpansion.IPhotonFramework _Setup();

	}
}
