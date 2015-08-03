// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HitHandler.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the HitHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Extension;
using Regulus.Framework;
using Regulus.Utility;

using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPIs;

#endregion

namespace FormulaUserBot
{
	internal class HitHandler : IUpdatable
	{
		public static double Interval = 1.0f / 20.0;

		private readonly IFishStage _Stage;

		private readonly TimeCounter _TimeCounter;

		private bool _Enable;

		private HitRequest _Request;

		public HitHandler(IFishStage _Stage, HitRequest request)
		{
			this._Stage = _Stage;
			this._Request = request;
			_Enable = true;
			_TimeCounter = new TimeCounter();
		}

		bool IUpdatable.Update()
		{
			return _Enable;
		}

		void IBootable.Launch()
		{
			_Stage.HitResponseEvent += _Response;
			_Stage.Hit(_Request);
			_TimeCounter.Reset();
		}

		void IBootable.Shutdown()
		{
			_Stage.HitResponseEvent -= _Response;
		}

		private void _Response(HitResponse obj)
		{
			if (obj.FishID == _Request.FishID && obj.WepID == _Request.WepID)
			{
				_Enable = false;

				Singleton<Log>.Instance.WriteLine(string.Format("時間{2}:請求{0}\n回應{1}", _Request.ShowMembers(), obj.ShowMembers(), 
					_TimeCounter.Second));
			}
		}
	}
}