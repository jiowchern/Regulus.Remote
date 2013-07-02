using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.Crystal.Standalone
{
	class Core
	{
		Regulus.Remoting.ISoulBinder _Binder;
		public IStorage	Storage {get ; private set;}

		public Regulus.Remoting.ISoulBinder Binder { get { return _Binder; }}
		Samebest.Game.StageMachine<Core> _StageMachine;
		public Core(Regulus.Remoting.ISoulBinder binder)
		{
			_Binder = binder;
			_StageMachine = new Samebest.Game.StageMachine<Core>(this);			
		}

		public void Launch()
		{
			_ToFirst();
		}

		private void _ToFirst()
		{
			_StageMachine.Push( new Regulus.Project.Crystal.Standalone.Stage.First() );
		}

		public void Update()
		{
			_StageMachine.Update();
		}
		public void Shutdown()
		{
			_StageMachine.Termination();
		}
	}
}
