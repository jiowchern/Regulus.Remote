using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.Crystal.Game
{
	public class Core : Regulus.Game.IFramework
	{
		Regulus.Remoting.ISoulBinder _Binder;
		public IStorage	Storage {get ; private set;}

		public Regulus.Remoting.ISoulBinder Binder { get { return _Binder; }}
		Regulus.Game.StageMachine<Core> _StageMachine;
		public Core(Regulus.Remoting.ISoulBinder binder , IStorage storage)
		{
			Storage = storage;
			_Binder = binder;
			_StageMachine = new Regulus.Game.StageMachine<Core>(this);

			binder.BreakEvent += _OnInactive;
		}
		~Core()
		{
			_Binder.BreakEvent -= _OnInactive;
		}
		void _OnInactive()
		{
			if (InactiveEvent != null)
				InactiveEvent();			
		}

		public void Launch()
		{
			_ToFirst();
		}

		private void _ToFirst()
		{
			_StageMachine.Push( new Regulus.Project.Crystal.Game.Stage.First() );
		}

		public bool Update()
		{
			_StageMachine.Update();
			return true;
		}
		public void Shutdown()
		{
			_StageMachine.Termination();
		}

		public event Action InactiveEvent;
	}
}
