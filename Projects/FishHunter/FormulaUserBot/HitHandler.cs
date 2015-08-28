
using Regulus.Extension;
using Regulus.Framework;
using Regulus.Utility;


using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

namespace FormulaUserBot
{
	internal class HitHandler : IUpdatable
	{
		private readonly HitRequest _Request;

		private readonly IFishStage _Stage;

		private readonly TimeCounter _TimeCounter;

		private bool _Enable;

		public static double Interval = 1.0f / 20.0;


		public HitHandler(IFishStage _Stage, HitRequest request)
		{
			this._Stage = _Stage;
			_Request = request;
			_Enable = true;
			_TimeCounter = new TimeCounter();
		}

		bool IUpdatable.Update()
		{
			return _Enable;
		}

		void IBootable.Launch()
		{
			_Stage.OnTotalHitResponseEvent += _Stage_OnTotalHitResponseEvent;
			_Stage.Hit(_Request);
			_TimeCounter.Reset();
		}

		void IBootable.Shutdown()
		{
			_Stage.OnTotalHitResponseEvent -= _Stage_OnTotalHitResponseEvent;
		}

		private void _Stage_OnTotalHitResponseEvent(HitResponse[] hit_responses)
		{
			foreach(var hit in hit_responses)
			{
				_Response(hit);
			}
		}

		private void _Response(HitResponse obj)
		{
			foreach(var fishData in _Request.FishDatas)
			{
				if(obj.FishId != fishData.FishId || obj.WepId != _Request.WeaponData.BulletId)
				{
					continue;
				}
				
				_Enable = false;

				Singleton<Log>.Instance.WriteLine(
					string.Format(
						"時間{2}:請求{0}\n回應{1}", 
						_Request.ShowMembers(), 
						obj.ShowMembers(), 
						_TimeCounter.Second));
			}
		}
	}
}
