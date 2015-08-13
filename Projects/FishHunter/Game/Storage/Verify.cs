using Regulus.Remoting;
using Regulus.Utility;


using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.Storage
{
	internal class Verify : IStage
	{
		public event FishHunter.Verify.DoneCallback DoneEvent;

		private readonly ISoulBinder _Binder;

		private readonly FishHunter.Verify _Verify;

		public Verify(ISoulBinder binder, FishHunter.Verify verify)
		{
			_Verify = verify;
			_Binder = binder;
		}

		void IStage.Enter()
		{
			_Verify.OnDoneEvent += DoneEvent;

			_Binder.Bind<IVerify>(_Verify);
		}

		void IStage.Leave()
		{
			_Binder.Unbind<IVerify>(_Verify);
			_Verify.OnDoneEvent -= DoneEvent;
		}

		void IStage.Update()
		{
		}
	}
}
