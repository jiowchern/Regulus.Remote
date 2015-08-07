// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CSFishStage.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the CsFishStage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Common.GPI;

using Random = Regulus.Utility.Random;

namespace VGame.Project.FishHunter.Formula
{
	public class CsFishStage : IFishStage
	{
		private event Action<string> _OnHitExceptionEvent;

		private event Action<HitResponse> _OnHitResponseEvent;

		private readonly long _AccountId;

		private readonly int _FishStage;

		private readonly HitBase _Formula;

		public CsFishStage(long player_id, int stage_id)
		{
			_Formula = new HitTest(Random.Instance);
			_AccountId = player_id;
			_FishStage = stage_id;
		}

		int IFishStage.FishStage
		{
			get { return _FishStage; }
		}

		void IFishStage.Hit(HitRequest request)
		{
			var response = _Formula.Request(request);

			_OnHitResponseEvent.Invoke(response);

			// this._MakeLog(request, response);
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
	}
}