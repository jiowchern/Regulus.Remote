
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

		private readonly Guid _AccountId;

		private readonly int _FishStage;

		private readonly HitBase _Formula;

		public FishStage(Guid account, int stage_id)
		{
			_Formula = new HitTest(Random.Instance);
			_AccountId = account;
			_FishStage = stage_id;
		}

		event Action<string> IFishStage.OnHitExceptionEvent
		{
			add { _OnHitExceptionEvent += value; }
			remove { _OnHitExceptionEvent -= value; }
		}

		private event Action<HitResponse[]> _OnTotalHitResponseEvent;

		event Action<HitResponse[]> IFishStage.OnTotalHitResponseEvent
		{
			add { this._OnTotalHitResponseEvent += value; }
			remove { this._OnTotalHitResponseEvent -= value; }
		}

		Guid IFishStage.AccountId
		{
			get { return _AccountId; }
		}

		int IFishStage.FishStage
		{
			get { return _FishStage; }
		}

		void IFishStage.Hit(HitRequest request)
		{
			var responses = _Formula.TotalRequest(request);

			_OnTotalHitResponseEvent.Invoke(responses);

			_MakeLog(request, responses);
		}

		private void _MakeLog(HitRequest request, HitResponse[] responses)
		{
			var format = "PlayerVisitor:{0}\tStage:{1}\nRequest:{2}\nResponse:{3}";

			var log = string.Format(
				format, 
				_AccountId, 
				_FishStage, 
				request.ShowMembers(" "), 
				responses.ShowMembers(" "));

			Singleton<Log>.Instance.WriteInfo(log);
		}
	}
}
