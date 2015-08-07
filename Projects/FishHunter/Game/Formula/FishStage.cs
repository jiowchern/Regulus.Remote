
using System;

using Regulus.Extension;
using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

using Random = Regulus.Utility.Random;

namespace VGame.Project.FishHunter.Formula
{
	internal class FishStage : IFishStage
	{
		private event Action<string> _OnHitExceptionEvent;

		private event Action<HitResponse> _OnHitResponseEvent;

		private readonly long _AccountId;

		private readonly byte _FishStage;

		private readonly HitBase _Formula;

		public FishStage(long account, int stage_id)
		{
			_Formula = new HitTest(Random.Instance);
			_AccountId = account;
			_FishStage = (byte)stage_id;
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
			get { return _FishStage; }
		}

		void IFishStage.Hit(HitRequest request)
		{
			var response = _Formula.Request(request);

			_OnHitResponseEvent.Invoke(response);

			_MakeLog(request, response);
		}

		private void _MakeLog(HitRequest request, HitResponse response)
		{
			var format = "PlayerVisitor:{0}\tStage:{1}\nRequest:{2}\nResponse:{3}";

			var log = string.Format(
				format, 
				_AccountId, 
				_FishStage, 
				request.ShowMembers(" "), 
				response.ShowMembers(" "));

			Singleton<Log>.Instance.WriteInfo(log);
		}
	}
}
