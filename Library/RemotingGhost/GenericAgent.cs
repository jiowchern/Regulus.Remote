using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samebest.Remoting.Ghost
{
	public class GenericAgent<TProvider> where TProvider : Samebest.Remoting.Ghost.Provider , new()
	{
		Samebest.Remoting.Ghost.Agent	_Agent;
		public Samebest.Remoting.Ghost.LinkState LinkState {get; private set;}
		TProvider _Provider;
		public GenericAgent()			
		{
			
		}

		public void Launch(Samebest.Remoting.Ghost.Config config)
		{
			LinkState = new Remoting.Ghost.LinkState();
			_Agent = new Remoting.Ghost.Agent(config);
			_Provider = new TProvider();
			_Agent.Launch(LinkState, _Provider);			
		}

		public bool Update()
		{
			return _Agent.Update();
		}

		public void Shutdown()
		{
			_Agent.Shutdown();
		}

		public TProvider GetProvider()
		{
			return _Provider;
		}
	}
}
