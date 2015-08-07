
using System;

using Regulus.Extension;
using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

namespace VGame.Project.FishHunter.ZsFormula
{
	internal class ZsFishStage : IFishStage
	{
		private event Action<HitResponse> _OnHitResponseEvent;

		private readonly long _AccountId;

		private readonly StageData _StageData;

		private readonly ZsHitChecker _HitChecker;

		private Action<string> _OnHitExceptionEvent;

		public ZsFishStage(long account_id, StageData stage_data)
		{
			if (stage_data == null)
			{
				return;
			}

			_HitChecker = new ZsHitChecker(stage_data);
			_AccountId = account_id;

			_StageData = stage_data;
		}

		event Action<string> IFishStage.OnHitExceptionEvent
		{
			add { _OnHitExceptionEvent += value; }
			remove { _OnHitExceptionEvent -= value; }
		}

		event Action<HitResponse> IFishStage.OnHitResponseEvent
		{
			add { _OnHitResponseEvent += value; }
			remove { _OnHitResponseEvent -= value; }
		}

		long IFishStage.AccountId
		{
			get { return _AccountId; }
		}

		int IFishStage.FishStage
		{
			get { return _StageData.StageId; }
		}

		void IFishStage.Hit(HitRequest request)
		{
			var response = _HitChecker.Request(request);
			
			_OnHitResponseEvent.Invoke(response);

			_MakeLog(request, response);
		}

		private void _MakeLog(HitRequest request, HitResponse response)
		{
			var format = "PlayerVisitor:{0}\tStage:{1}\nRequest:{2}\nResponse:{3}";

			var log = string.Format(
				format,
				_AccountId,
				_StageData.StageId,
				request.ShowMembers(" "),
				response.ShowMembers(" "));

			Singleton<Log>.Instance.WriteInfo(log);
		}
	}
}