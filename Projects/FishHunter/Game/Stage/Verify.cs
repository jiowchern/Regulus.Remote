// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Verify.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the Verify type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Remoting;
using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPI;

#endregion

namespace VGame.Project.FishHunter.Stage
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